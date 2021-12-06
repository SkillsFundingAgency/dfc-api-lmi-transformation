using DFC.Api.Lmi.Transformation.Contracts;
using DFC.Api.Lmi.Transformation.Models.LmiImportApiModels;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;

namespace DFC.Api.Lmi.Transformation.Connectors
{
    public class LmiImportApiConnector : ILmiImportApiConnector
    {
        private readonly ILogger<LmiImportApiConnector> logger;
        private readonly HttpClient httpClient;
        private readonly IApiDataConnector apiDataConnector;

        public LmiImportApiConnector(
            ILogger<LmiImportApiConnector> logger,
            HttpClient httpClient,
            IApiDataConnector apiDataConnector)
        {
            this.logger = logger;
            this.httpClient = httpClient;
            this.apiDataConnector = apiDataConnector;
        }

        public async Task<IList<SocDatasetSummaryItemModel>?> GetSummaryAsync(Uri uri)
        {
            logger.LogInformation($"Retrieving summaries from LMI Import API: {uri}");

            var socDatasetSummaries = await apiDataConnector.GetAsync<IList<SocDatasetSummaryItemModel>>(httpClient, uri).ConfigureAwait(false);

            return socDatasetSummaries;
        }

        public Task<SocDatasetModel?> GetDetailAsync(Uri uri)
        {
            logger.LogInformation($"Retrieving details from LMI Import API: {uri}");

            return apiDataConnector.GetAsync<SocDatasetModel>(httpClient, uri);
        }
    }
}
