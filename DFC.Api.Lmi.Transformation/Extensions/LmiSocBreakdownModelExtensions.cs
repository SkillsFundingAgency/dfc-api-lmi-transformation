using DFC.Api.Lmi.Transformation.Models.LmiImportApiModels;

namespace DFC.Api.Lmi.Transformation.Extensions
{
    public static class LmiSocBreakdownModelExtensions
    {
        public static void SetMeasures(this LmiSocBreakdownModel? lmiSocBreakdownModel)
        {
            lmiSocBreakdownModel?.PredictedEmployment?.ForEach(f => f.Breakdown?.ForEach(f2 => f2.Measure = lmiSocBreakdownModel.Measure));
        }
    }
}
