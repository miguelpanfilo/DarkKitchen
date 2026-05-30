using BusinessLogics;
using BusinessLogicsAbstractions.Exceptions;
using FluentAssertions;

namespace Tests.imports;

[TestClass]
public class ImportedProductPhotoConverterTest
{
    [TestMethod]
    public void Convert_WhenPhotoPathExists_ShouldReturnPhotoDto()
    {
        var converter = new ImportedProductPhotoConverter();

        var path = Path.GetTempFileName();
        File.WriteAllBytes(path, [1, 2, 3]);

        var photos = converter.Convert([path]);

        photos.Should().HaveCount(1);
        photos[0].FileName.Should().Be(Path.GetFileName(path));
        photos[0].Extension.Should().Be(Path.GetExtension(path));
        photos[0].SizeInBytes.Should().Be(3);
        photos[0].Base64.Should().Be(Convert.ToBase64String([1, 2, 3]));

        File.Delete(path);
    }

    [TestMethod]
    public void Convert_WhenPhotoPathDoesNotExist_ShouldThrowValidationException()
    {
        var converter = new ImportedProductPhotoConverter();

        var photoPaths = new List<string>
        {
            "C:\\archivo-que-no-existe.jpg"
        };

        var act = () => converter.Convert(photoPaths);

        act.Should().Throw<ValidationException>()
            .WithMessage("*no existe*");
    }
}
