namespace S3ImageTrigger.Services;

public class ImageService : IImageService
{
    public void BuildThumbnailImage(Stream imgStream, string imageName)
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
}
