using AutoMapper;
using DFC.Api.Lmi.Transformation.Common;
using DFC.Api.Lmi.Transformation.Models.JobGroupModels;
using DFC.Api.Lmi.Transformation.Models.LmiImportApiModels;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;

namespace DFC.Api.Lmi.Transformation.AutoMapperProfiles.ValuerConverters
{
    [ExcludeFromCodeCoverage]
    public class BreakdownYearValuesConverter : IValueConverter<IList<LmiSocBreakdownYearValueModel>?, List<BreakdownYearValueModel>?>
    {
        public List<BreakdownYearValueModel>? Convert(IList<LmiSocBreakdownYearValueModel>? sourceMember, ResolutionContext context)
        {
            _ = context ?? throw new ArgumentNullException(nameof(context));

            if (sourceMember == null || !sourceMember.Any())
            {
                return default;
            }

            var results = new List<BreakdownYearValueModel>();
            var measure = sourceMember.First().Measure;

            foreach (var item in sourceMember)
            {
                var model = context.Mapper.Map<BreakdownYearValueModel>(item);

                switch (measure)
                {
                    case Constants.MeasureForRegion:
                        var excludeRegions = new[] { Constants.RegionCodeForWales, Constants.RegionCodeForScotland, Constants.RegionCodeForNorthernIreland };

                        if (!excludeRegions.Contains(model.Code))
                        {
                            results.Add(model);
                        }

                        break;
                    default:
                        results.Add(model);
                        break;
                }
            }

            if (results.Any() && measure != null)
            {
                switch (measure)
                {
                    case Constants.MeasureForRegion:
                        results = results.OrderBy(o => o.Name).ToList();
                        break;
                    case Constants.MeasureForIndustry:
                        results = results.OrderByDescending(o => o.Employment).Take(10).ToList();
                        break;
                }
            }

            return results;
        }
    }
}
