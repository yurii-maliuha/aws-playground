using Amazon.Lambda.S3Events;
using Amazon.S3;
using Amazon.S3.Model;

namespace S3ImageTrigger.Services;

public class BucketService : IBucketService
{
    public IAmazonS3 _s3Client { get; set; }
    public BucketService(IAmazonS3 s3Client)
    {
        _s3Client = s3Client;
    }

    public async Task<Stream> GetImageStream(S3Event.S3Entity s3Entity)
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

    public async Task SaveImage(S3Event.S3Entity s3Entity, Stream imgStream)
    {
        var (bucketName, imageName) = (s3Entity.Bucket.Name, s3Entity.Object.Key);
        var thumbnailImgName = string.Join("-thumbnail.", imageName.Split('.'));
        await _s3Client.PutObjectAsync(new PutObjectRequest
        {
            BucketName = $"{bucketName}-thumbnails",
            Key = thumbnailImgName,
            InputStream = imgStream
        });
    }
}
