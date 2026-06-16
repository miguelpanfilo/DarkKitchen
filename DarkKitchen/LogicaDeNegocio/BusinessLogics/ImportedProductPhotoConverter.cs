using BusinessLogicsAbstractions;
using Contract;

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
        var resolvedPath = Path.IsPathRooted(path)
            ? path
            : Path.Combine(AppContext.BaseDirectory, path);
        try
        {
            return File.ReadAllBytes(resolvedPath);
        }
        catch(FileNotFoundException)
        {
            throw new ValidationException(
                $"La imagen '{path}' no existe");
        }
    }
}
