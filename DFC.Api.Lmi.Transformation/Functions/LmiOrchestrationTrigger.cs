using AutoMapper;
using DFC.Api.Lmi.Transformation.Contracts;
using DFC.Api.Lmi.Transformation.Extensions;
using DFC.Api.Lmi.Transformation.Models;
using DFC.Api.Lmi.Transformation.Models.ClientOptions;
using DFC.Api.Lmi.Transformation.Models.FunctionRequestModels;
using DFC.Api.Lmi.Transformation.Models.JobGroupModels;
using DFC.Api.Lmi.Transformation.Models.LmiImportApiModels;
using DFC.Compui.Cosmos.Contracts;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.DurableTask;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

namespace DFC.Api.Lmi.Transformation.Functions
{
    public class LmiOrchestrationTrigger
    {
        private const string EventTypeForPublished = "published";
        private const string EventTypeForDeleted = "deleted";

        private readonly ILogger<LmiOrchestrationTrigger> logger;
        private readonly IMapper mapper;
        private readonly ILmiImportApiConnector lmiImportApiConnector;
        private readonly IDocumentService<JobGroupModel> jobGroupDocumentService;
        private readonly IEventGridService eventGridService;
        private readonly EventGridClientOptions eventGridClientOptions;

        public LmiOrchestrationTrigger(
            ILogger<LmiOrchestrationTrigger> logger,
            IMapper mapper,
            ILmiImportApiConnector lmiImportApiConnector,
            IDocumentService<JobGroupModel> jobGroupDocumentService,
            IEventGridService eventGridService,
            EventGridClientOptions eventGridClientOptions)
        {
            this.logger = logger;
            this.mapper = mapper;
            this.jobGroupDocumentService = jobGroupDocumentService;
            this.lmiImportApiConnector = lmiImportApiConnector;
            this.jobGroupDocumentService = jobGroupDocumentService;
            this.eventGridService = eventGridService;
            this.eventGridClientOptions = eventGridClientOptions;

            //TODO: ian: need to initialize the telemetry properly
            Activity? activity = null;
            if (Activity.Current == null)
            {
                activity = new Activity(nameof(LmiOrchestrationTrigger)).Start();
                activity.SetParentId(Guid.NewGuid().ToString());
            }
        }

        [FunctionName(nameof(RefreshJobGroupOrchestrator))]
        public async Task<bool> RefreshJobGroupOrchestrator([OrchestrationTrigger] IDurableOrchestrationContext context)
        {
            _ = context ?? throw new ArgumentNullException(nameof(context));

            var socRequest = context.GetInput<SocRequestModel>();
            var upsertResult = await context.CallActivityAsync<HttpStatusCode>(nameof(TransformItemActivity), socRequest.Uri).ConfigureAwait(true);

            if (upsertResult == HttpStatusCode.OK || upsertResult == HttpStatusCode.Created)
            {
                var eventGridPostRequest = new EventGridPostRequestModel
                {
                    ItemId = socRequest.SocId,
                    Api = $"{eventGridClientOptions.ApiEndpoint}/{socRequest.SocId}",
                    DisplayText = $"LMI transformed into job-group from {socRequest.Uri}",
                    EventType = EventTypeForPublished,
                };

                await context.CallActivityAsync(nameof(PostTransformationEventActivity), eventGridPostRequest).ConfigureAwait(true);

                return true;
            }

            return false;
        }

        [FunctionName(nameof(PurgeJobGroupOrchestrator))]
        public async Task PurgeJobGroupOrchestrator([OrchestrationTrigger] IDurableOrchestrationContext context)
        {
            _ = context ?? throw new ArgumentNullException(nameof(context));

            var socRequest = context.GetInput<SocRequestModel>();

            await context.CallActivityAsync(nameof(PurgeSocActivity), socRequest.SocId).ConfigureAwait(true);

            var eventGridPostRequest = new EventGridPostRequestModel
            {
                ItemId = socRequest.SocId,
                Api = $"{eventGridClientOptions.ApiEndpoint}/{socRequest.SocId}",
                DisplayText = $"LMI purged job-group for {socRequest.SocId}",
                EventType = EventTypeForDeleted,
            };

            await context.CallActivityAsync(nameof(PostTransformationEventActivity), eventGridPostRequest).ConfigureAwait(true);
        }

        [FunctionName(nameof(PurgeOrchestrator))]
        public async Task PurgeOrchestrator([OrchestrationTrigger] IDurableOrchestrationContext context)
        {
            _ = context ?? throw new ArgumentNullException(nameof(context));

            var socRequest = context.GetInput<SocRequestModel>();

            await context.CallActivityAsync(nameof(PurgeActivity), null).ConfigureAwait(true);

            var eventGridPostRequest = new EventGridPostRequestModel
            {
                ItemId = context.NewGuid(),
                Api = $"{eventGridClientOptions.ApiEndpoint}",
                DisplayText = "LMI purged all job-group ",
                EventType = EventTypeForDeleted,
            };

            await context.CallActivityAsync(nameof(PostTransformationEventActivity), eventGridPostRequest).ConfigureAwait(true);
        }

        [FunctionName(nameof(RefreshOrchestrator))]
        [Timeout("04:00:00")]
        public async Task RefreshOrchestrator([OrchestrationTrigger] IDurableOrchestrationContext context)
        {
            _ = context ?? throw new ArgumentNullException(nameof(context));

            var socRequest = context.GetInput<SocRequestModel>();
            var summaryItems = await context.CallActivityAsync<IList<SocDatasetSummaryItemModel>?>(nameof(GetSummaryItemsActivity), socRequest.Uri).ConfigureAwait(true);

            if (summaryItems != null && summaryItems.Any())
            {
                await context.CallActivityAsync(nameof(PurgeActivity), null).ConfigureAwait(true);

                logger.LogInformation($"Transforming {summaryItems.Count} LMI data items");

                var parallelTasks = new List<Task<HttpStatusCode>>();

                foreach (var summaryItem in summaryItems.OrderBy(o => o.Soc))
                {
                    var uri = new Uri($"{socRequest.Uri}/{summaryItem.Id}", UriKind.Absolute);
                    parallelTasks.Add(context.CallActivityAsync<HttpStatusCode>(nameof(TransformItemActivity), uri));
                }

                await Task.WhenAll(parallelTasks).ConfigureAwait(true);

                var eventGridPostRequest = new EventGridPostRequestModel
                {
                    ItemId = context.NewGuid(),
                    Api = $"{eventGridClientOptions.ApiEndpoint}",
                    DisplayText = $"LMI transformed all job-groups from {socRequest.Uri}",
                    EventType = EventTypeForPublished,
                };

                await context.CallActivityAsync(nameof(PostTransformationEventActivity), eventGridPostRequest).ConfigureAwait(true);

                int transformedToJobGroupCount = parallelTasks.Count(t => t.Result == HttpStatusCode.OK || t.Result == HttpStatusCode.Created);

                logger.LogInformation($"Transformed to Job-group {transformedToJobGroupCount} of {summaryItems.Count} SOCs");
            }
            else
            {
                logger.LogWarning("No data available LMI Import data - no data transformed");
            }
        }

        [FunctionName(nameof(GetSummaryItemsActivity))]
        public Task<IList<SocDatasetSummaryItemModel>?> GetSummaryItemsActivity([ActivityTrigger] Uri uri)
        {
            logger.LogInformation($"Getting LMI SOC dataset summaries from {uri}");

            return lmiImportApiConnector.GetSummaryAsync(uri);
        }

        [FunctionName(nameof(PurgeActivity))]
        public Task<bool> PurgeActivity([ActivityTrigger] string? name)
        {
            logger.LogInformation("Deleting all SOC datasets");

            return jobGroupDocumentService.PurgeAsync();
        }

        [FunctionName(nameof(PurgeSocActivity))]
        public Task<bool> PurgeSocActivity([ActivityTrigger] Guid socId)
        {
            logger.LogInformation($"Deleting SOC datasets item: {socId}");

            return jobGroupDocumentService.DeleteAsync(socId);
        }

        [FunctionName(nameof(TransformItemActivity))]
        [Timeout("01:00:00")]
        public async Task<HttpStatusCode> TransformItemActivity([ActivityTrigger] Uri uri)
        {
            logger.LogInformation($"Loading SOC dataset item: {uri}");
            var lmiSoc = await lmiImportApiConnector.GetDetailAsync(uri).ConfigureAwait(false);

            if (lmiSoc != null)
            {
                lmiSoc.QualificationLevel.SetMeasures();
                lmiSoc.EmploymentByRegion.SetMeasures();
                lmiSoc.TopIndustriesInJobGroup.SetMeasures();

                logger.LogInformation($"Transforming SOC dataset item for {uri} to cache");
                var jobGroup = mapper.Map<JobGroupModel>(lmiSoc);

                if (jobGroup != null)
                {
                    var existingJobGroup = await jobGroupDocumentService.GetAsync(w => w.Soc == jobGroup.Soc, jobGroup.Soc.ToString(CultureInfo.InvariantCulture)).ConfigureAwait(false);
                    if (existingJobGroup != null)
                    {
                        jobGroup.Etag = existingJobGroup.Etag;
                    }

                    logger.LogInformation($"Upserting SOC datasets item: {jobGroup.Soc}");
                    return await jobGroupDocumentService.UpsertAsync(jobGroup).ConfigureAwait(false);
                }
            }

            return HttpStatusCode.BadRequest;
        }

        [FunctionName(nameof(PostTransformationEventActivity))]
        public Task PostTransformationEventActivity([ActivityTrigger] EventGridPostRequestModel? eventGridPostRequest)
        {
            _ = eventGridPostRequest ?? throw new ArgumentNullException(nameof(eventGridPostRequest));

            logger.LogInformation($"Posting to event grid for: {eventGridPostRequest.DisplayText}: {eventGridPostRequest.EventType}");

            var eventGridEventData = new EventGridEventData
            {
                ItemId = $"{eventGridPostRequest.ItemId}",
                Api = eventGridPostRequest.Api,
                DisplayText = eventGridPostRequest.DisplayText,
                VersionId = Guid.NewGuid().ToString(),
                Author = eventGridClientOptions.SubjectPrefix,
            };

            return eventGridService.SendEventAsync(eventGridEventData, eventGridClientOptions.SubjectPrefix, eventGridPostRequest.EventType);
        }
    }
}
