namespace S3ImageTrigger.Models;

public class ImageObject
{
    public string FullName { get; set; }
    public string Extension
    {
        get
        {
            return FullName.Split('.').Last();
        }
    }
    public MemoryStream Content { get; set; }
}
