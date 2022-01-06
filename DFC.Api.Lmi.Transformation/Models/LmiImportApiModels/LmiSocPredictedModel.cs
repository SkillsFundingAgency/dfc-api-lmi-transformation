using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace DFC.Api.Lmi.Transformation.Models.LmiImportApiModels
{
    [ExcludeFromCodeCoverage]
    public class LmiSocPredictedModel
    {
        public string? Measure { get; set; }

        public List<LmiSocPredictedYearModel>? PredictedEmployment { get; set; }
    }
}
