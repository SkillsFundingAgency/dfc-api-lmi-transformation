using System;
using System.Diagnostics.CodeAnalysis;

namespace DFC.Api.Lmi.Transformation.Models.ClientOptions
{
    [ExcludeFromCodeCoverage]
    public abstract class ClientOptionsModel
    {
        public Uri? BaseAddress { get; set; }

        public TimeSpan Timeout { get; set; } = new TimeSpan(0, 0, 30);

        public string? ApiKey { get; set; }
    }
}
