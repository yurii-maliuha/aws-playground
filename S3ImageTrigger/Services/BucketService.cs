using Amazon.S3;
using Amazon.S3.Model;
using static Amazon.Lambda.S3Events.S3Event;

namespace S3ImageTrigger.Services;

public class BucketService : IBucketService
{
    public IAmazonS3 _s3Client { get; set; }
    public BucketService(IAmazonS3 s3Client)
    {
        _s3Client = s3Client;
    }

    public async Task<Stream> GetImageStream(S3Entity s3Entity)
    {
        var (bucketName, objectName) = (s3Entity.Bucket.Name, s3Entity.Object.Key);
        var request = new GetObjectRequest
        {
            BucketName = bucketName,
            Key = objectName
        };

        using (GetObjectResponse response = await _s3Client.GetObjectAsync(request))
        {
            var ms = new MemoryStream();
            await response.ResponseStream.CopyToAsync(ms);
            ms.Seek(0, SeekOrigin.Begin);
            return ms;
        }

    }
}
