using DFC.Api.Lmi.Transformation.Functions;
using FakeItEasy;
using Microsoft.AspNetCore.Http;

using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Xunit;

namespace DFC.Api.Lmi.Transformation.UnitTests.Functions
{
    public class HealthPingHttpTriggerTests
    {
        [Fact]
        public void HealthPingHttpTriggerTestsReturnsOk()
        {
            // Arrange
            var context = new DefaultHttpContext();

            // Act
            var result = HealthPingHttpTrigger.Run(context.Request);

            // Assert
            Assert.IsType<OkResult>(result);
        }
    }
}
