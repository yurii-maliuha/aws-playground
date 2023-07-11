using static Amazon.Lambda.S3Events.S3Event;

namespace S3ImageTrigger.Services;

public interface IBucketService
{
    Task<Stream> GetImageStream(S3Entity s3Entity);
}
