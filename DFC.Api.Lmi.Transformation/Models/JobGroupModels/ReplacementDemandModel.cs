using System.Diagnostics.CodeAnalysis;

namespace DFC.Api.Lmi.Transformation.Models.JobGroupModels
{
    [ExcludeFromCodeCoverage]
    public class ReplacementDemandModel
    {
        public int StartYearRange { get; set; }

        public int EndYearRange { get; set; }

        public decimal Rate { get; set; }

        public decimal PercentageGrowth { get; set; }
    }
}
