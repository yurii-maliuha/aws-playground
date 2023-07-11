using SixLabors.ImageSharp.Formats.Jpeg;

namespace S3ImageTrigger.Services;

public class ImageService : IImageService
{
    public void BuildThumbnailImageTest(Stream imgStream, string imageName)
    {
        //var thumbnailImgStream = new MemoryStream();
        using (var image = Image.Load(imgStream))
        {
            // Create B&W thumbnail
            image.Mutate(ctx => ctx.Resize(new ResizeOptions()
            {
                Mode = ResizeMode.Min,
                Size = new Size(200, 200)
            }));
            //image.Save(thumbnailImgStream, new JpegEncoder());
            //thumbnailImgStream.Seek(0, SeekOrigin.Begin);

            var thumbnailImgName = string.Join("-thumbnail.", imageName.Split('.'));
            image.Save(thumbnailImgName);
        }
    }

    public async Task<Stream> BuildThumbnailImage(Stream imgStream)
    {
        var thumbnailImgStream = new MemoryStream();
        using (var image = Image.Load(imgStream))
        {
            // Create B&W thumbnail
            image.Mutate(ctx => ctx.Resize(new ResizeOptions()
            {
                Mode = ResizeMode.Min,
                Size = new Size(200, 200)
            }));

            await image.SaveAsync(thumbnailImgStream, new JpegEncoder());
            thumbnailImgStream.Seek(0, SeekOrigin.Begin);

            return thumbnailImgStream;
        }
    }
}
