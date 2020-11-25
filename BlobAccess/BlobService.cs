using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace KeyRotationSample.BlobAccess
{
    public class BlobService : IBlobService
    {
        private readonly BlobStorageService blobStorageService;

        public BlobService(BlobStorageService blobStorageService)
        {
            this.blobStorageService = blobStorageService;
        }
        public async Task<List<string>> ListBlobs(string containerName)
        {
            var containerClient = blobStorageService.blobServiceClient.GetBlobContainerClient(containerName);
            List<string> items = new List<string>();
            await foreach (var blobItem in containerClient.GetBlobsAsync())
            {
                items.Add(blobItem.Name);
            }

            return items;
        }
    }
}
