using Amazon.Lambda.S3Events;
using static Amazon.Lambda.S3Events.S3Event;

namespace S3ImageTrigger.Services;

public interface IBucketService
{
    Task<Stream> GetImageStream(S3Entity s3Entity);
    Task SaveImage(S3Event.S3Entity s3Entity, Stream imgStream);
}
