namespace Contract.Products;

public class PhotoDTO
{
    public string FileName { get; set; }
    public string Extension { get; set; }
    public int SizeInBytes { get; set; }
    public string Base64 { get; set; }

    public PhotoDTO(string fileName, string extension, int sizeInBytes, string base64)
    {
        FileName = fileName;
        Extension = extension;
        SizeInBytes = sizeInBytes;
        Base64 = base64;
    }
}
