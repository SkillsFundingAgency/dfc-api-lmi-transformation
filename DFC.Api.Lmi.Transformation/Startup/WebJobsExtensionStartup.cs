using AzureFunctions.Extensions.Swashbuckle;
using DFC.Api.Lmi.Transformation.Connectors;
using DFC.Api.Lmi.Transformation.Contracts;
using DFC.Api.Lmi.Transformation.Extensions;
using DFC.Api.Lmi.Transformation.HttpClientPolicies;
using DFC.Api.Lmi.Transformation.Models.ClientOptions;
using DFC.Api.Lmi.Transformation.Models.JobGroupModels;
using DFC.Api.Lmi.Transformation.Services;
using DFC.Api.Lmi.Transformation.Startup;
using DFC.Compui.Cosmos;
using DFC.Compui.Cosmos.Contracts;
using DFC.Compui.Subscriptions.Pkg.Netstandard.Extensions;
using DFC.Swagger.Standard;
using Microsoft.Azure.Documents.Client;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Diagnostics.CodeAnalysis;
using System.Reflection;

[assembly: WebJobsStartup(typeof(WebJobsExtensionStartup), "Web Jobs Extension Startup")]

namespace DFC.Api.Lmi.Transformation.Startup
{
    [ExcludeFromCodeCoverage]
    public class WebJobsExtensionStartup : IWebJobsStartup
    {
        private const string AppSettingsPolicies = "Policies";
        private const string CosmosDbLmiTransformationConfigAppSettings = "Configuration:CosmosDbConnections:LmiTransformation";

        public void Configure(IWebJobsBuilder builder)
        {
            _ = builder ?? throw new ArgumentNullException(nameof(builder));

            var configuration = new ConfigurationBuilder()
                .SetBasePath(Environment.CurrentDirectory)
                .AddJsonFile("local.settings.json", optional: true, reloadOnChange: true)
                .AddEnvironmentVariables()
                .Build();

            var cosmosDbConnection = configuration.GetSection(CosmosDbLmiTransformationConfigAppSettings).Get<CosmosDbConnection>();
            var cosmosRetryOptions = new RetryOptions { MaxRetryAttemptsOnThrottledRequests = 20, MaxRetryWaitTimeInSeconds = 60 };

            builder.Services.AddSingleton(configuration.GetSection(nameof(EventGridClientOptions)).Get<EventGridClientOptions>() ?? new EventGridClientOptions());

            builder.AddSwashBuckle(Assembly.GetExecutingAssembly());
            builder.Services.AddHttpClient();
            builder.Services.AddApplicationInsightsTelemetry();
            builder.Services.AddAutoMapper(typeof(WebJobsExtensionStartup).Assembly);
            builder.Services.AddDocumentServices<JobGroupModel>(cosmosDbConnection, false, cosmosRetryOptions);
            builder.Services.AddSubscriptionService(configuration);
            builder.Services.AddTransient<ISwaggerDocumentGenerator, SwaggerDocumentGenerator>();
            builder.Services.AddTransient<ILmiWebhookReceiverService, LmiWebhookReceiverService>();
            builder.Services.AddTransient<IEventGridService, EventGridService>();
            builder.Services.AddTransient<IEventGridClientService, EventGridClientService>();
            builder.Services.AddTransient<IApiConnector, ApiConnector>();
            builder.Services.AddTransient<IApiDataConnector, ApiDataConnector>();

            var policyOptions = configuration.GetSection(AppSettingsPolicies).Get<PolicyOptions>() ?? new PolicyOptions();
            var policyRegistry = builder.Services.AddPolicyRegistry();

            builder.Services.AddSingleton(configuration.GetSection(nameof(LmiImportApiClientOptions)).Get<LmiImportApiClientOptions>() ?? new LmiImportApiClientOptions());

            builder.Services
                .AddPolicies(policyRegistry, nameof(LmiImportApiClientOptions), policyOptions)
                .AddHttpClient<ILmiImportApiConnector, LmiImportApiConnector, LmiImportApiClientOptions>(nameof(LmiImportApiClientOptions), nameof(PolicyOptions.HttpRetry), nameof(PolicyOptions.HttpCircuitBreaker));
        }
    }
}