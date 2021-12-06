﻿using System;
using System.Net.Http;
using System.Threading.Tasks;

namespace DFC.Api.Lmi.Transformation.Contracts
{
    public interface IApiConnector
    {
        Task<string?> GetAsync(HttpClient? httpClient, Uri url, string acceptHeader);
    }
}
