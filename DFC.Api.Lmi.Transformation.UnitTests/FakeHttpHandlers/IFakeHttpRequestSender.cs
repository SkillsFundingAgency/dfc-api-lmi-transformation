using System.Net.Http;

namespace DFC.Api.Lmi.Transformation.UnitTests.FakeHttpHandlers
{
    public interface IFakeHttpRequestSender
    {
        HttpResponseMessage Send(HttpRequestMessage request);
    }
}
