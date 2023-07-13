using S3ImageTrigger.Models;
using SixLabors.ImageSharp.Formats;
using SixLabors.ImageSharp.Formats.Jpeg;
using SixLabors.ImageSharp.Formats.Png;

namespace S3ImageTrigger.Services;

public class ImageService : IImageService
{
    private const int THUMBNAIL_WIDTH = 200;
    private const int THUMBNAIL_HEIGHT = 200;
    private readonly Dictionary<string, ImageEncoder> _encoderMapper = new Dictionary<string, ImageEncoder>
    {
        { "jpg", new JpegEncoder() },
        { "png", new PngEncoder() }
    };

    public async Task<ImageObject> BuildThumbnailImage(ImageObject imageObject)
    {
        var thumbnailImgStream = new MemoryStream();
        using (var image = Image.Load(imageObject.Content))
        {
            image.Mutate(ctx => ctx.Resize(new ResizeOptions()
            {
                Mode = ResizeMode.Min,
                Size = new Size(THUMBNAIL_WIDTH, THUMBNAIL_HEIGHT)
            }));

            if (!_encoderMapper.Keys.Contains(imageObject.Extension))
            {
                throw new ArgumentOutOfRangeException($"Image extension {imageObject.Extension} is not supported");
            }

            await image.SaveAsync(thumbnailImgStream, _encoderMapper[imageObject.Extension]);
            thumbnailImgStream.Seek(0, SeekOrigin.Begin);

            return new ImageObject()
            {
                FullName = string.Join("-thumbnail.", imageObject.FullName.Split('.')),
                Content = thumbnailImgStream
            };
        }
    }
}
