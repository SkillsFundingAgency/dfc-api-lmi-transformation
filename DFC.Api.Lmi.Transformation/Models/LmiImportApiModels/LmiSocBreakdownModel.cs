using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace DFC.Api.Lmi.Transformation.Models.LmiImportApiModels
{
    [ExcludeFromCodeCoverage]
    public class LmiSocBreakdownModel
    {
        public string? Note { get; set; }

        public string? Measure { get; set; }

        public List<LmiSocBreakdownYearModel>? PredictedEmployment { get; set; }
    }
}
