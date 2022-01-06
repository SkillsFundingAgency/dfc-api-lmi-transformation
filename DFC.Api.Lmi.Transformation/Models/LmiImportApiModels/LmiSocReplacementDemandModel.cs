using System.Diagnostics.CodeAnalysis;

namespace DFC.Api.Lmi.Transformation.Models.LmiImportApiModels
{
    [ExcludeFromCodeCoverage]
    public class LmiSocReplacementDemandModel
    {
        public string? Measure { get; set; }

        public int StartYear { get; set; }

        public int EndYear { get; set; }

        public decimal Rate { get; set; }
    }
}
