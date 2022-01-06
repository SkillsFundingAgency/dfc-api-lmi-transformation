﻿using Newtonsoft.Json;
using System.Diagnostics.CodeAnalysis;

namespace DFC.Api.Lmi.Transformation.Models.JobGroupModels
{
    [ExcludeFromCodeCoverage]
    public class BreakdownYearValueModel
    {
        public int Code { get; set; }

        public string? Name { get; set; }

        public string? Note { get; set; }

        public decimal Employment { get; set; }
    }
}
