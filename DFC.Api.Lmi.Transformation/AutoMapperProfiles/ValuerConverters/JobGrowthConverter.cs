using AutoMapper;
using DFC.Api.Lmi.Transformation.Models.ContentApiModels;
using DFC.Api.Lmi.Transformation.Models.JobGroupModels;
using DFC.Content.Pkg.Netcore.Data.Contracts;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;

namespace DFC.Api.Lmi.Transformation.AutoMapperProfiles.ValuerConverters
{
    [ExcludeFromCodeCoverage]
    public class JobGrowthConverter : IValueConverter<IList<IBaseContentItemModel>?, JobGrowthPredictionModel?>
    {
        public JobGrowthPredictionModel? Convert(IList<IBaseContentItemModel>? sourceMember, ResolutionContext context)
        {
            _ = context ?? throw new ArgumentNullException(nameof(context));

            if (sourceMember == null || !sourceMember.Any())
            {
                return default;
            }

            var predictedList = new List<PredictedModel>();
            var replacementDemandList = new List<LmiSocReplacementDemand>();

            foreach (var item in sourceMember)
            {
                switch (item.ContentType)
                {
                    case nameof(LmiSocPredicted):
                        if (item is LmiSocPredicted lmiSocPredicted)
                        {
                            predictedList.Add(context.Mapper.Map<PredictedModel>(lmiSocPredicted));
                        }

                        break;

                    case nameof(LmiSocReplacementDemand):
                        if (item is LmiSocReplacementDemand contentItem)
                        {
                            replacementDemandList.Add(context.Mapper.Map<LmiSocReplacementDemand>(contentItem));
                        }

                        break;

                }
            }

            var predictedEmployment = predictedList.FirstOrDefault()?.PredictedEmployment;
            var firstYearPredictedEmployment = predictedEmployment?.OrderBy(o => o.Year).FirstOrDefault();
            var lastYearPredictedEmployment = predictedEmployment?.OrderByDescending(o => o.Year).FirstOrDefault();
            var lmiSocReplacementDemand = replacementDemandList?.OrderBy(o => o.StartYear).FirstOrDefault();

            if (firstYearPredictedEmployment != null && lastYearPredictedEmployment != null)
            {
                var result = new JobGrowthPredictionModel()
                {
                    StartYearRange = firstYearPredictedEmployment.Year,
                    EndYearRange = lastYearPredictedEmployment.Year,
                    JobsCreated = lastYearPredictedEmployment.Employment - firstYearPredictedEmployment.Employment,
                    PercentageGrowth = (lastYearPredictedEmployment.Employment - firstYearPredictedEmployment.Employment) / firstYearPredictedEmployment.Employment * 100,
                };

                if (lmiSocReplacementDemand != null)
                {
                    result.Retirements = lmiSocReplacementDemand.Rate;
                    result.PercentageRetirements = lmiSocReplacementDemand.Rate / firstYearPredictedEmployment.Employment * 100;
                }

                return result;
            }

            return default;
        }
    }
}
