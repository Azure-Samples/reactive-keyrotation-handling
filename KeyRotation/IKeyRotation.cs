using Polly.Retry;

namespace KeyRotationSample.KeyRotation
{
    public interface IKeyRotation
    {
        AsyncRetryPolicy RetryCosmosPolicy { get; }
        AsyncRetryPolicy RetryBlobPolicy { get; }
    }
}
