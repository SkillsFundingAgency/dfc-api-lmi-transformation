using AutoMapper;
using DFC.Api.Lmi.Transformation.Extensions;
using DFC.Api.Lmi.Transformation.Models.JobGroupModels;
using DFC.Api.Lmi.Transformation.Models.LmiImportApiModels;
using System;
using System.Diagnostics.CodeAnalysis;
using System.Linq;

namespace DFC.Api.Lmi.Transformation.AutoMapperProfiles.ValuerConverters
{
    [ExcludeFromCodeCoverage]
    public class JobGrowthConverter : IValueConverter<SocDatasetModel?, JobGrowthPredictionModel?>
    {
        public JobGrowthPredictionModel? Convert(SocDatasetModel? sourceMember, ResolutionContext context)
        {
            _ = context ?? throw new ArgumentNullException(nameof(context));

            if (sourceMember == null || sourceMember.JobGrowth == null || sourceMember.ReplacementDemand == null)
            {
                return default;
            }

            var predictedEmployment = sourceMember.JobGrowth?.PredictedEmployment;
            var firstYearPredictedEmployment = predictedEmployment?.OrderBy(o => o.Year).FirstOrDefault();
            var lastYearPredictedEmployment = predictedEmployment?.OrderByDescending(o => o.Year).FirstOrDefault();

            if (firstYearPredictedEmployment != null && lastYearPredictedEmployment != null)
            {
                var result = new JobGrowthPredictionModel()
                {
                    StartYearRange = firstYearPredictedEmployment.Year,
                    EndYearRange = lastYearPredictedEmployment.Year,
                    JobsCreated = (lastYearPredictedEmployment.Employment - firstYearPredictedEmployment.Employment).RoundToNearest(100),
                    PercentageGrowth = (lastYearPredictedEmployment.Employment - firstYearPredictedEmployment.Employment) / firstYearPredictedEmployment.Employment * 100,
                };

                if (sourceMember.ReplacementDemand != null)
                {
                    result.Retirements = sourceMember.ReplacementDemand.Rate.RoundToNearest(100);
                    result.PercentageRetirements = sourceMember.ReplacementDemand.Rate / firstYearPredictedEmployment.Employment * 100;
                }

                return result;
            }

            return default;
        }
    }
}
