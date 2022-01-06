using DFC.Api.Lmi.Transformation.Models.LmiImportApiModels;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DFC.Api.Lmi.Transformation.Contracts
{
    public interface ILmiImportApiConnector
    {
        Task<IList<SocDatasetSummaryItemModel>?> GetSummaryAsync(Uri uri);

        Task<SocDatasetModel?> GetDetailAsync(Uri uri);
    }
}
