namespace S3ImageTrigger.Services;

public interface IImageService
{
    void BuildThumbnailImage(Stream imgStream, string imageName);
}
