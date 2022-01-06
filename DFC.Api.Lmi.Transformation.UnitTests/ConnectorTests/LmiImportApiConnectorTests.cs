using DFC.Api.Lmi.Transformation.Connectors;
using DFC.Api.Lmi.Transformation.Contracts;
using DFC.Api.Lmi.Transformation.Models.LmiImportApiModels;
using FakeItEasy;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Xunit;

namespace DFC.Api.Lmi.Transformation.UnitTests.ConnectorTests
{
    [Trait("Category", "LMI Import API connector Unit Tests")]
    public class LmiImportApiConnectorTests
    {
        private readonly ILogger<LmiImportApiConnector> fakeLogger = A.Fake<ILogger<LmiImportApiConnector>>();
        private readonly HttpClient httpClient = new HttpClient();
        private readonly IApiDataConnector fakeApiDataConnector = A.Fake<IApiDataConnector>();
        private readonly ILmiImportApiConnector lmiImportApiConnector;

        public LmiImportApiConnectorTests()
        {
            lmiImportApiConnector = new LmiImportApiConnector(fakeLogger, httpClient, fakeApiDataConnector);
        }

        [Fact]
        public async Task LmiApiConnectorApiConnectorGetSummaryReturnsSuccess()
        {
            // arrange
            var expectedResults = new List<SocDatasetSummaryItemModel>
            {
                new SocDatasetSummaryItemModel
                {
                    Id = Guid.NewGuid(),
                    Soc = 1234,
                    Title = "A title",
                },
            };

            A.CallTo(() => fakeApiDataConnector.GetAsync<IList<SocDatasetSummaryItemModel>>(A<HttpClient>.Ignored, A<Uri>.Ignored)).Returns(expectedResults);

            // act
            var results = await lmiImportApiConnector.GetSummaryAsync(new Uri("https://somewhere.com", UriKind.Absolute)).ConfigureAwait(false);

            // assert
            A.CallTo(() => fakeApiDataConnector.GetAsync<IList<SocDatasetSummaryItemModel>>(A<HttpClient>.Ignored, A<Uri>.Ignored)).MustHaveHappenedOnceExactly();
            Assert.NotNull(results);
            Assert.Equal(expectedResults.Count, results!.Count);
            Assert.Equal(expectedResults.First().Id, results.First().Id);
            Assert.Equal(expectedResults.First().Title, results.First().Title);
            Assert.Equal(expectedResults.First().Soc, results.First().Soc);
        }

        [Fact]
        public async Task LmiApiConnectorGetDetailReturnsSuccess()
        {
            // arrange
            var expectedResult = new SocDatasetModel
            {
                Id = Guid.NewGuid(),
                Soc = 1234,
            };

            A.CallTo(() => fakeApiDataConnector.GetAsync<SocDatasetModel>(A<HttpClient>.Ignored, A<Uri>.Ignored)).Returns(expectedResult);

            // act
            var result = await lmiImportApiConnector.GetDetailAsync(new Uri("https://somewhere.com", UriKind.Absolute)).ConfigureAwait(false);

            // assert
            A.CallTo(() => fakeApiDataConnector.GetAsync<SocDatasetModel>(A<HttpClient>.Ignored, A<Uri>.Ignored)).MustHaveHappenedOnceExactly();
            Assert.NotNull(result);
            Assert.Equal(expectedResult.Id, result!.Id);
            Assert.Equal(expectedResult.Soc, result.Soc);
        }

        [Fact]
        public async Task LmiApiConnectorGetDetailReturnsNullForNoData()
        {
            // arrange
            A.CallTo(() => fakeApiDataConnector.GetAsync<SocDatasetModel>(A<HttpClient>.Ignored, A<Uri>.Ignored)).Returns(default(SocDatasetModel));

            // act
            var result = await lmiImportApiConnector.GetDetailAsync(new Uri("https://somewhere.com", UriKind.Absolute)).ConfigureAwait(false);

            // assert
            A.CallTo(() => fakeApiDataConnector.GetAsync<SocDatasetModel>(A<HttpClient>.Ignored, A<Uri>.Ignored)).MustHaveHappenedOnceExactly();
            Assert.Null(result);
        }
    }
}
