using System;
using System.Diagnostics.CodeAnalysis;

namespace DFC.Api.Lmi.Transformation.Models.FunctionRequestModels
{
    [ExcludeFromCodeCoverage]
    public class SocRequestModel
    {
        public Guid? SocId { get; set; }

        public Uri? Uri { get; set; }
    }
}
