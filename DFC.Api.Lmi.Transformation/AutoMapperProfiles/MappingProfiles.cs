using AutoMapper;
using DFC.Api.Lmi.Transformation.AutoMapperProfiles.ValuerConverters;
using DFC.Api.Lmi.Transformation.Models.JobGroupModels;
using DFC.Api.Lmi.Transformation.Models.LmiImportApiModels;
using System.Diagnostics.CodeAnalysis;

namespace DFC.Api.Lmi.Transformation.AutoMapperProfiles
{
    [ExcludeFromCodeCoverage]
    public class MappingProfiles : Profile
    {
        public MappingProfiles()
        {
            CreateMap<JobGroupModel, JobGroupSummaryItemModel>();

            CreateMap<SocDatasetModel, JobGroupModel>()
                .ForMember(d => d.TransformedDate, s => s.Ignore())
                .ForMember(d => d.JobGrowth, opt => opt.ConvertUsing(new JobGrowthConverter(), a => a))
                .ForMember(d => d.QualificationLevel, opt => opt.ConvertUsing(new QualificationLevelConverter(), a => a.QualificationLevel))
                .ForMember(d => d.EmploymentByRegion, opt => opt.ConvertUsing(new EmploymentByRegionConverter(), a => a.EmploymentByRegion))
                .ForMember(d => d.TopIndustriesInJobGroup, opt => opt.ConvertUsing(new TopIndustriesInJobGroupConverter(), a => a.TopIndustriesInJobGroup));

            CreateMap<LmiSocJobProfileModel, JobProfileModel>();

            CreateMap<LmiSocBreakdownModel, BreakdownModel>()
               .ForMember(d => d.PredictedEmployment, opt => opt.ConvertUsing(new BreakdownYearListConverter(), a => a.PredictedEmployment));

            CreateMap<LmiSocBreakdownYearModel, BreakdownYearModel>()
                .ForMember(d => d.Breakdown, opt => opt.ConvertUsing(new BreakdownYearValuesConverter(), a => a.Breakdown));

            CreateMap<LmiSocBreakdownYearValueModel, BreakdownYearValueModel>();
        }
    }
}
