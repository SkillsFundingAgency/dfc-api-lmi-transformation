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
    public class TopIndustriesInJobGroupConverter : IValueConverter<LmiSocBreakdownModel?, List<BreakdownModel>?>
    {
        public List<BreakdownModel>? Convert(LmiSocBreakdownModel? sourceMember, ResolutionContext context)
        {
            _ = context ?? throw new ArgumentNullException(nameof(context));

            if (sourceMember == null || !sourceMember.PredictedEmployment.Any())
            {
                return default;
            }

            return new List<BreakdownModel> { context.Mapper.Map<BreakdownModel>(sourceMember) };
        }
    }
}
