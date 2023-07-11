namespace S3ImageTrigger.Services;

public interface IImageService
{
    void BuildThumbnailImageTest(Stream imgStream, string imageName);
    Task<Stream> BuildThumbnailImage(Stream imgStream);
}
