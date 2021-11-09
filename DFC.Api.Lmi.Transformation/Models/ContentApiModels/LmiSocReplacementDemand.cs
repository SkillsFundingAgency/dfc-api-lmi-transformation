using DFC.Content.Pkg.Netcore.Data.Models;
using Newtonsoft.Json;
using System.Diagnostics.CodeAnalysis;

namespace DFC.Api.Lmi.Transformation.Models.ContentApiModels
{
    [ExcludeFromCodeCoverage]
    public class LmiSocReplacementDemand : BaseContentItemModel
    {
        public int Soc { get; set; }

        [JsonProperty(PropertyName = "skos__prefLabel")]
        public string? Measure { get; set; }

        public int StartYear { get; set; }

        public int EndYear { get; set; }

        public decimal Rate { get; set; }
    }
}
