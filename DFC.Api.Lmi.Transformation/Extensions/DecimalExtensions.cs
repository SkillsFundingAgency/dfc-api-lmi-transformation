using System;

namespace DFC.Api.Lmi.Transformation.Extensions
{
    public static class DecimalExtensions
    {
        public static int RoundToNearest(this decimal input, int nearest)
        {
            decimal result = Math.Round(input / nearest, 0, MidpointRounding.AwayFromZero) * nearest;
            return (int)result;
        }
    }
}
