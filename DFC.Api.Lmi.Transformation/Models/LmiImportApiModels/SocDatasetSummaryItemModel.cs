using System;
using System.Diagnostics.CodeAnalysis;

namespace DFC.Api.Lmi.Transformation.Models.LmiImportApiModels
{
    [ExcludeFromCodeCoverage]
    public class SocDatasetSummaryItemModel
    {
        public Guid? Id { get; set; }

        public int Soc { get; set; }

        public string? Title { get; set; }
    }
}
