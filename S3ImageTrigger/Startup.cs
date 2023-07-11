using Amazon.S3;
using Microsoft.Extensions.DependencyInjection;
using S3ImageTrigger.Services;

namespace S3ImageTrigger
{
    public static class Startup
    {
        public static ServiceProvider ServiceProvider { get; private set; }

        public static void ConfigureServices()
        {
            var sericeCollection = new ServiceCollection();

            sericeCollection.AddTransient<IAmazonS3, AmazonS3Client>();
            sericeCollection.AddTransient<IBucketService, BucketService>();
            sericeCollection.AddTransient<IImageService, ImageService>();

            ServiceProvider = sericeCollection.BuildServiceProvider();
        }
    }
}
