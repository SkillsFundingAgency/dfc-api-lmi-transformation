using System.Diagnostics.CodeAnalysis;

namespace DFC.Api.Lmi.Transformation.Models.LmiImportApiModels
{
    [ExcludeFromCodeCoverage]
    public class LmiSocPredictedYearModel
    {
        public int Year { get; set; }

        public decimal Employment { get; set; }
    }
}
