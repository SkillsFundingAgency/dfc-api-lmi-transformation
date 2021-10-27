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
    public class ReplacementDemandConverter : IValueConverter<IList<IBaseContentItemModel>?, ReplacementDemandModel?>
    {
        public ReplacementDemandModel? Convert(IList<IBaseContentItemModel>? sourceMember, ResolutionContext context)
        {
            _ = context ?? throw new ArgumentNullException(nameof(context));

            if (sourceMember == null || !sourceMember.Any())
            {
                return default;
            }

            var replacementDemandList = new List<LmiSocReplacementDemand>();
            var predictedList = new List<PredictedModel>();

            foreach (var item in sourceMember)
            {
                switch (item.ContentType)
                {
                    case nameof(LmiSocReplacementDemand):
                        if (item is LmiSocReplacementDemand contentItem)
                        {
                            replacementDemandList.Add(context.Mapper.Map<LmiSocReplacementDemand>(contentItem));
                        }

                        break;

                    case nameof(LmiSocPredicted):
                        if (item is LmiSocPredicted lmiSocPredicted)
                        {
                            predictedList.Add(context.Mapper.Map<PredictedModel>(lmiSocPredicted));
                        }

                        break;
                }
            }

            var lmiSocReplacementDemand = replacementDemandList?.OrderBy(o => o.StartYear).FirstOrDefault();
            var firstYearPredictedEmployment = predictedList.FirstOrDefault()?.PredictedEmployment?.OrderBy(o => o.Year).FirstOrDefault();

            if (lmiSocReplacementDemand != null && firstYearPredictedEmployment != null)
            {
                var result = new ReplacementDemandModel
                {
                    StartYearRange = lmiSocReplacementDemand.StartYear,
                    EndYearRange = lmiSocReplacementDemand.EndYear,
                    Rate = lmiSocReplacementDemand.Rate,
                    PercentageGrowth = lmiSocReplacementDemand.Rate / firstYearPredictedEmployment.Employment * 100,
                };

                return result;
            }

            return default;
        }
    }
}
