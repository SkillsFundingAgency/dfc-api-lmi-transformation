﻿using DFC.Api.Lmi.Transformation.Models;
using DFC.Api.Lmi.Transformation.Models.ClientOptions;
using System.Threading.Tasks;

namespace DFC.Api.Lmi.Transformation.Contracts
{
    public interface IEventGridService
    {
        Task SendEventAsync(EventGridEventData? eventGridEventData, string? subject, string? eventType);

        bool IsValidEventGridClientOptions(EventGridClientOptions? eventGridClientOptions);
    }
}
