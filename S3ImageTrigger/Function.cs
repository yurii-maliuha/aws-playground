using Amazon.Lambda.Core;
using Amazon.Lambda.S3Events;
using Microsoft.Extensions.DependencyInjection;
using S3ImageTrigger.Services;

// Assembly attribute to enable the Lambda function's JSON input to be converted into a .NET class.
[assembly: LambdaSerializer(typeof(Amazon.Lambda.Serialization.SystemTextJson.DefaultLambdaJsonSerializer))]

namespace S3ImageTrigger
{
    public class Function
    {
        private readonly IBucketService _bucketService;
        private readonly IImageService _imageService;

        public Function() : this(null, null)
        {

        }

        public Function(IBucketService? bucketService, IImageService? imageService)
        {
            Startup.ConfigureServices();

            _bucketService = bucketService ?? Startup.ServiceProvider.GetRequiredService<IBucketService>();
            _imageService = imageService ?? Startup.ServiceProvider.GetRequiredService<IImageService>();
        }

        public async Task FunctionHandler(S3Event evnt, ILambdaContext context)
        {
            var eventRecords = evnt.Records ?? new List<S3Event.S3EventNotificationRecord>();
            foreach (var record in eventRecords)
            {
                var s3Event = record.S3;
                if (s3Event == null)
                {
                    continue;
                }

                if (_bucketService is null)
                {
                    throw new ArgumentNullException("Bucket service should be defined");
                }

                try
                {
                    context.Logger.LogInformation($"Downloading image from S3 [{s3Event.Bucket.Name}, {s3Event.Object.Key}]");
                    var image = await _bucketService.GetImageStream(s3Event);
                    var thumbnailImg = await _imageService.BuildThumbnailImage(image);

                    context.Logger.LogInformation("Image thumbnail is created");
                    await _bucketService.SaveImage(thumbnailImg, $"{s3Event.Bucket.Name}-thumbnails");
                    context.Logger.LogInformation($"Image {thumbnailImg.FullName} is uploaded to S3 bucket");
                }
                catch (Exception e)
                {
                    context.Logger.LogError($"Error getting object {s3Event.Object.Key} from bucket {s3Event.Bucket.Name}. Make sure they exist and your bucket is in the same region as this function.");
                    context.Logger.LogError(e.Message);
                    context.Logger.LogError(e.StackTrace);
                    throw;
                }
            }
        }
    }
}