using S3ImageTrigger.Models;

namespace S3ImageTrigger.Services;

public interface IImageService
{
    Task<ImageObject> BuildThumbnailImage(ImageObject image);
}
