using Almacenamiento;
using Contratos;

namespace Pruebas.Storages;

[TestClass]
public class StorageProductPhotoTest
{
    [TestMethod]
    public void GivenValidPhoto_WhenSaving_ThenCreatesFile()
    {
        var tempDir = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString());
        Directory.CreateDirectory(tempDir);

        try
        {
            var storage = new ProductPhotoStorage(tempDir);
            var base64 = Convert.ToBase64String(new byte[] { 1, 2, 3 });

            var photo = new PhotoDTO("foto", ".jpg", 3, base64);

            var path = storage.Save(photo);

            Assert.IsTrue(File.Exists(path));
        }
        finally
        {
            Directory.Delete(tempDir, true);
        }
    }
}
