using AutoMapper;
using DFC.Api.Lmi.Transformation.Models.JobGroupModels;
using DFC.Api.Lmi.Transformation.Models.LmiImportApiModels;
using System;
using System.Diagnostics.CodeAnalysis;
using System.Linq;

namespace DFC.Api.Lmi.Transformation.AutoMapperProfiles.ValuerConverters
{
    [ExcludeFromCodeCoverage]
    public class QualificationLevelConverter : IValueConverter<LmiSocBreakdownModel?, QualificationLevelModel?>
    {
        public QualificationLevelModel? Convert(LmiSocBreakdownModel? sourceMember, ResolutionContext context)
        {
            _ = context ?? throw new ArgumentNullException(nameof(context));

            if (sourceMember == null)
            {
                return default;
            }

            var breakdownModel = context.Mapper.Map<BreakdownModel>(sourceMember);
            var predictedEmployment = breakdownModel?.PredictedEmployment;
            if (predictedEmployment != null)
            {
                var firstYearResult = predictedEmployment.OrderBy(o => o.Year).FirstOrDefault();

                if (firstYearResult != null)
                {
                    var maxEmploymentBreakdown = firstYearResult.Breakdown?.OrderByDescending(o => o.Employment).FirstOrDefault();

                    if (maxEmploymentBreakdown != null)
                    {
                        var result = new QualificationLevelModel()
                        {
                            Year = firstYearResult.Year,
                            Code = maxEmploymentBreakdown.Code,
                            Name = maxEmploymentBreakdown.Name,
                            Note = maxEmploymentBreakdown.Note,
                            Employment = maxEmploymentBreakdown.Employment,
                        };

                        return result;
                    }
                }
            }

            return default;
        }
    }
}
