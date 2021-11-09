using DFC.Api.Lmi.Transformation.Extensions;
using Xunit;

namespace DFC.Api.Lmi.Transformation.UnitTests.Extenstions
{
    [Trait("Category", "DecimalExtensions - decimal extensions Unit Tests")]
    public class DecimalExtensionsTests
    {
        [Theory]
        [InlineData(1234, 10, 1230)]
        [InlineData(1235, 10, 1240)]
        [InlineData(1234, 100, 1200)]
        [InlineData(1253, 100, 1300)]
        [InlineData(1234432, 1000, 1234000)]
        [InlineData(1234567, 1000, 1235000)]
        public void RoundToNearestReturnsExpected(decimal value, int rounding, int expectedResult)
        {
            // Arrange

            // Act
            var result = value.RoundToNearest(rounding);

            // Assert
            Assert.Equal(expectedResult, result);
        }
    }
}
