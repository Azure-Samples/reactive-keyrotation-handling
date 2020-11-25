using Azure.Storage.Blobs;

namespace KeyRotationSample.BlobAccess
{
    public class BlobStorageService
    {
        public BlobServiceClient blobServiceClient;

        public BlobStorageService(string connection)
        {
            blobServiceClient = new BlobServiceClient(connection);
        }

        public void RefreshBlobServiceClient(string connection)
        {
            blobServiceClient = new BlobServiceClient(connection);
        }
    }
}
