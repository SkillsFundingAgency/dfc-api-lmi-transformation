﻿using DFC.Api.Lmi.Transformation.Functions;
using FakeItEasy;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Internal;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Xunit;

namespace DFC.Api.Lmi.Transformation.UnitTests.Functions
{
    public class HealthPingHttpTriggerTests
    {
        private readonly ILogger logger = A.Fake<ILogger>();

        [Fact]
        public void HealthPingHttpTriggerTestsReturnsOk()
        {
            // Arrange

            // Act
            var result = HealthPingHttpTrigger.Run(new DefaultHttpRequest(new DefaultHttpContext()), logger);

            // Assert
            Assert.IsType<OkResult>(result);
        }
    }
}
