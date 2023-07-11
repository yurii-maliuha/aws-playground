using Amazon.Lambda.Core;
using Amazon.Lambda.S3Events;
using Amazon.S3;
using Amazon.S3.Model;
using Microsoft.Extensions.DependencyInjection;
using S3ImageTrigger.Services;
using SixLabors.ImageSharp.Formats.Jpeg;

// Assembly attribute to enable the Lambda function's JSON input to be converted into a .NET class.
[assembly: LambdaSerializer(typeof(Amazon.Lambda.Serialization.SystemTextJson.DefaultLambdaJsonSerializer))]

namespace S3ImageTrigger
{
    public class Function
    {
        IAmazonS3 S3Client { get; set; }

        private readonly IBucketService _bucketService;
        private readonly IImageService _imageService;

        /// <summary>
        /// Default constructor. This constructor is used by Lambda to construct the instance. When invoked in a Lambda environment
        /// the AWS credentials will come from the IAM role associated with the function and the AWS region will be set to the
        /// region the Lambda function is executed in.
        /// </summary>
        public Function() : this(null, null)
        {
            S3Client = new AmazonS3Client();
        }

        /// <summary>
        /// Constructs an instance with a preconfigured S3 client. This can be used for testing outside of the Lambda environment.
        /// </summary>
        /// <param name="s3Client"></param>
        public Function(IBucketService bucketService = null, IImageService imageService = null)
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
                    //var (bucketName, objectName) = (s3Event.Bucket.Name, s3Event.Object.Key);
                    //await DownloadObjectFromBucketAsync(new AmazonS3Client(), bucketName, objectName);

                    context.Logger.LogInformation($"Downloading image from S3 [{s3Event.Bucket.Name}, {s3Event.Object.Key}]");
                    using (var imageStream = await _bucketService.GetImageStream(s3Event))
                    {
                        //_imageService.BuildThumbnailImageTest(imageStream, s3Event.Object.Key);
                        using (var thumbnailImgStream = await _imageService.BuildThumbnailImage(imageStream))
                        {
                            context.Logger.LogInformation("Image thumbnail is created");
                            await _bucketService.SaveImage(s3Event, thumbnailImgStream);
                            context.Logger.LogInformation("Image thumbnail is uploaded to S3 bucket");
                        }
                    }
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

        public static async Task DownloadObjectFromBucketAsync(
           IAmazonS3 client,
           string bucketName,
           string objectName)
        {
            // Create a GetObject request
            var request = new GetObjectRequest
            {
                BucketName = bucketName,
                Key = objectName
            };

            try
            {
                string responseBody = "";
                Stream imageStream = new MemoryStream();
                using (GetObjectResponse response = await client.GetObjectAsync(request))
                using (Stream responseStream = response.ResponseStream)
                {
                    using (var image = Image.Load(responseStream))
                    {
                        // Create B&W thumbnail
                        image.Mutate(ctx => ctx.Resize(new ResizeOptions()
                        {
                            Mode = ResizeMode.Min,
                            Size = new Size(200, 200)
                        }));
                        image.Save(imageStream, new JpegEncoder());
                        imageStream.Seek(0, SeekOrigin.Begin);

                        image.Save("result3.jpg");
                    }
                }

            }
            catch (AmazonS3Exception ex)
            {
                Console.WriteLine($"Error saving {objectName}: {ex.Message}");
            }
        }
    }
}