using Azure;
using Azure.Security.KeyVault.Secrets;
using KeyRotationSample.BlobAccess;
using KeyRotationSample.DataAccess;
using Microsoft.Azure.Cosmos;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Polly;
using Polly.Retry;
using System;
using System.Net;
using System.Threading;

namespace KeyRotationSample.KeyRotation
{
    public class KeyRotationHelper : IKeyRotation
    {
        private readonly ICosmosDbService cosmosDbService;
        private readonly BlobStorageService blobStorageService;
        private readonly SecretClient client;
        private readonly IConfiguration configuration;
        private readonly ILogger logger;
        private static SemaphoreSlim semaphoreSlim = new SemaphoreSlim(1);

        public AsyncRetryPolicy RetryCosmosPolicy { get; private set; }
        public AsyncRetryPolicy RetryBlobPolicy { get; private set; }

        public KeyRotationHelper(ICosmosDbService cosmosDbService, BlobStorageService blobStorageService, SecretClient client, IConfiguration configuration, ILogger<KeyRotationHelper> logger)
        {
            this.cosmosDbService = cosmosDbService;
            this.blobStorageService = blobStorageService;
            this.client = client;
            this.configuration = configuration;
            this.logger = logger;
            this.RetryCosmosPolicy = this.GetCosmosRetryPolicy();
            this.RetryBlobPolicy = GetBlobRetryPolicy();
        }

        /// <summary>
        /// This method creates a Polly retry policy when there is cosmos exception with code Unauthorized.
        /// </summary>
        /// <returns>The AsyncRetryPolicy</returns>
        private AsyncRetryPolicy GetCosmosRetryPolicy()
        {
            return Policy.Handle<CosmosException>(e => e.StatusCode == HttpStatusCode.Unauthorized)
                .RetryAsync(1, async (exception, retryCount) =>
                {
                    try
                    {
                        await semaphoreSlim.WaitAsync().ConfigureAwait(false);
                        logger.LogInformation("Read the cosmos key from KeyVault.");

                        // Get the latest cosmos key.
                        var cosmosKeySecret = await client.GetSecretAsync(Constants.CosmosKey).ConfigureAwait(false);

                        logger.LogInformation("Refresh cosmos connection with upadated secret.");
                        cosmosDbService.Reconnect(new Uri(configuration[Constants.CosmosUrl]), cosmosKeySecret.Value.Value, configuration[Constants.CosmosDatabase], configuration[Constants.CosmosCollection]);
                    }
                    finally
                    {
                        // release the semaphore
                        semaphoreSlim.Release();
                    }
                });
        }

        // <summary>
        /// This method creates a Polly retry policy when there is blob exception with code Unauthorized.
        /// </summary>
        /// <returns>The AsyncRetryPolicy</returns>
        private AsyncRetryPolicy GetBlobRetryPolicy()
        {
            return Policy.Handle<RequestFailedException>(e => (e.Status == (int)HttpStatusCode.Unauthorized || e.Status == (int)HttpStatusCode.Forbidden))
                .RetryAsync(1, async (exception, retryCount) =>
                {
                    try
                    {
                        await semaphoreSlim.WaitAsync().ConfigureAwait(false);
                        logger.LogInformation("Read the blob connection from KeyVault.");

                        // Get the latest blob connection key.
                        var blobConnectionSecret = await client.GetSecretAsync(Constants.BlobConnection).ConfigureAwait(false);


                        logger.LogInformation("Refresh blob storage connection with upadated secret.");
                        blobStorageService.RefreshBlobServiceClient(blobConnectionSecret.Value.Value);
                    }
                    finally
                    {
                        // release the semaphore
                        semaphoreSlim.Release();
                    }
                });
        }
    }
}