using Contract.Products;
using DataAccess.Repositories;

namespace Tests.Storages;

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

            var path = storage.Save(photo.Base64, photo.Extension);

            Assert.IsTrue(File.Exists(path));
        }
        finally
        {
            Directory.Delete(tempDir, true);
        }
    }

    [TestMethod]
    public void GivenExistingFile_WhenDeleting_ThenRemovesFile()
    {
        var tempDir = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString());
        Directory.CreateDirectory(tempDir);

        try
        {
            var filePath = Path.Combine(tempDir, "foto.jpg");
            File.WriteAllBytes(filePath, new byte[] { 1, 2, 3 });

            var storage = new ProductPhotoStorage(tempDir);

            storage.Delete(filePath);

            Assert.IsFalse(File.Exists(filePath));
        }
        finally
        {
            Directory.Delete(tempDir, true);
        }
    }

    [TestMethod]
    public void GivenNonExistingDirectory_WhenSavingPhoto_ThenCreatesDirectoryAndSavesFile()
    {
        var tempDir = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString());

        try
        {
            var storage = new ProductPhotoStorage(tempDir);
            var base64 = Convert.ToBase64String(new byte[] { 1, 2, 3 });

            var photo = new PhotoDTO("foto", ".jpg", 3, base64);

            var path = storage.Save(photo.Base64, photo.Extension);

            Assert.IsTrue(Directory.Exists(tempDir));
            Assert.IsTrue(File.Exists(path));
        }
        finally
        {
            if (Directory.Exists(tempDir))
            {
                Directory.Delete(tempDir, true);
            }
        }
    }
}
