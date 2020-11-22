using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace KeyRotationSample.DataAccess
{
    public interface ICosmosDbService
    {
        Task<IEnumerable<string>> GetSampleDataAsync();
        void Reconnect(Uri cosmosUrl, string cosmosKey, string cosmosDatabase, string cosmosCollection);
    }
}
