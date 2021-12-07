using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace DFC.Api.Lmi.Transformation.Models.LmiImportApiModels
{
    [ExcludeFromCodeCoverage]
    public class LmiSocBreakdownYearModel
    {
        public int Year { get; set; }

        public List<LmiSocBreakdownYearValueModel>? Breakdown { get; set; }
    }
}
