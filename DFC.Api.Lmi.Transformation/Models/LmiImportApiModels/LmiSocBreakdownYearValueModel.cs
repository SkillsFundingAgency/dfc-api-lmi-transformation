using System.Diagnostics.CodeAnalysis;
using System.Text.Json.Serialization;

namespace DFC.Api.Lmi.Transformation.Models.LmiImportApiModels
{
    [ExcludeFromCodeCoverage]
    public class LmiSocBreakdownYearValueModel
    {
        [JsonIgnore]
        public string? Measure { get; set; }

        public int Code { get; set; }

        public string? Note { get; set; }

        public string? Name { get; set; }

        public decimal Employment { get; set; }
    }
}
