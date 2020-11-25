using Microsoft.Azure.Cosmos;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace KeyRotationSample.DataAccess
{
    public class CosmosDbService : ICosmosDbService
    {
        public CosmosClient Client;
        public Container Container;
        public CosmosDbService(Uri cosmosUrl, string cosmosKey, string cosmosDatabase, string cosmosCollection)
        {
            Client = GetCosmosClient(cosmosUrl, cosmosKey);
            Container = Client.GetContainer(cosmosDatabase, cosmosCollection);
        }
        public async Task<IEnumerable<string>> GetSampleDataAsync()
        {
            string sqlQuery = "select value c.title from c";
            
            List<string> results = new List<string>();
            FeedIterator<string> query = Container.GetItemQueryIterator<string>(sqlQuery, requestOptions: new QueryRequestOptions { MaxItemCount = 1000, ConsistencyLevel = ConsistencyLevel.Session });

            while (query.HasMoreResults)
            {
                foreach (string doc in await query.ReadNextAsync().ConfigureAwait(false))
                {
                    results.Add(doc);
                }
            }

            return results;
        }

        public void Reconnect(Uri cosmosUrl, string cosmosKey, string cosmosDatabase, string cosmosCollection)
        {
            Client = GetCosmosClient(cosmosUrl, cosmosKey);
            Container = Client.GetContainer(cosmosDatabase, cosmosCollection);
        }



        private CosmosClient GetCosmosClient(Uri cosmosUrl, string cosmosKey)
        {
            //Include validations for the required parameters.

            //Cosmosclient options
            var cosmosClientOptions = new CosmosClientOptions { RequestTimeout = TimeSpan.FromSeconds(60), MaxRetryAttemptsOnRateLimitedRequests = 10, MaxRetryWaitTimeOnRateLimitedRequests = TimeSpan.FromSeconds(60) };
            // Create new cosmos client.
            CosmosClient cosmosClient = new CosmosClient(cosmosUrl.AbsoluteUri, cosmosKey, cosmosClientOptions);
            

            return cosmosClient;
        }
    }
}
