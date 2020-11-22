using System.Collections.Generic;
using System.Threading.Tasks;

namespace KeyRotationSample.BlobAccess
{
    public interface IBlobService
    {
        Task<List<string>> ListBlobs(string containerName);
    }
}
