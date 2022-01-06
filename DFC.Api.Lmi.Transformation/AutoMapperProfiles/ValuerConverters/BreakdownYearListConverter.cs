using AutoMapper;
using DFC.Api.Lmi.Transformation.Models.JobGroupModels;
using DFC.Api.Lmi.Transformation.Models.LmiImportApiModels;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;

namespace DFC.Api.Lmi.Transformation.AutoMapperProfiles.ValuerConverters
{
    [ExcludeFromCodeCoverage]
    public class BreakdownYearListConverter : IValueConverter<IList<LmiSocBreakdownYearModel>?, List<BreakdownYearModel>?>
    {
        public List<BreakdownYearModel>? Convert(IList<LmiSocBreakdownYearModel>? sourceMember, ResolutionContext context)
        {
            _ = context ?? throw new ArgumentNullException(nameof(context));

            if (sourceMember == null || !sourceMember.Any())
            {
                return default;
            }

            var results = new List<BreakdownYearModel>();

            foreach (var item in sourceMember)
            {
                results.Add(context.Mapper.Map<BreakdownYearModel>(item));
            }

            results = results.OrderBy(o => o.Year).Take(1).ToList();

            return results;
        }
    }
}
