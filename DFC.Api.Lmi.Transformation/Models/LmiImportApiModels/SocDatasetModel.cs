using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace DFC.Api.Lmi.Transformation.Models.LmiImportApiModels
{
    [ExcludeFromCodeCoverage]
    public class SocDatasetModel
    {
        public Guid Id { get; set; }

        public int Soc { get; set; }

        public DateTime? CachedDate { get; set; }

        public string? Title { get; set; }

        public string? Description { get; set; }

        public string? Qualifications { get; set; }

        public string? Tasks { get; set; }

        public List<string>? AdditionalTitles { get; set; }

        public List<LmiSocJobProfileModel>? JobProfiles { get; set; }

        public LmiSocPredictedModel? JobGrowth { get; set; }

        public LmiSocReplacementDemandModel? ReplacementDemand { get; set; }

        public LmiSocBreakdownModel? QualificationLevel { get; set; }

        public LmiSocBreakdownModel? EmploymentByRegion { get; set; }

        public LmiSocBreakdownModel? TopIndustriesInJobGroup { get; set; }
    }
}
