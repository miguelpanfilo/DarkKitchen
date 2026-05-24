using BusinessLogicsAbstractions.Exceptions;
using BusinessLogicsAbstractions.Imports;
using Contract.Products;

namespace BusinessLogics;

public class ImportedProductPhotoConverter : IImportedProductPhotoConverter
{
    public List<PhotoDTO> Convert(List<string> photoPaths)
    {
        return photoPaths
            .Select(ConvertPathToPhotoDto)
            .ToList();
    }

    private static PhotoDTO ConvertPathToPhotoDto(string path)
    {
        var bytes = ReadPhotoBytes(path);

        return new PhotoDTO(
            Path.GetFileName(path),
            Path.GetExtension(path),
            bytes.Length,
            System.Convert.ToBase64String(bytes));
    }

    private static byte[] ReadPhotoBytes(string path)
    {
        try
        {
            return File.ReadAllBytes(path);
        }
        catch(FileNotFoundException)
        {
            throw new ValidationException(
                $"La imagen '{path}' no existe");
        }
    }
}
