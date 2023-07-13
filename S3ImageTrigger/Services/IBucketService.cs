using S3ImageTrigger.Models;
using static Amazon.Lambda.S3Events.S3Event;

namespace S3ImageTrigger.Services;

public interface IBucketService
{
    Task<ImageObject> GetImageStream(S3Entity s3Entity);
    Task SaveImage(ImageObject imageObject, string bucketName);
}
