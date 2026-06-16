using Contract;
using DataAccess.Repositories;

namespace Tests.Storages;

[TestClass]
public sealed class StorageProductPhotoTest
{
    private string _tempDir = null!;

    [TestInitialize]
    public void Setup()
    {
        _tempDir = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString());
    }

    [TestCleanup]
    public void Cleanup()
    {
        if (Directory.Exists(_tempDir))
        {
            Directory.Delete(_tempDir, true);
        }
    }

    [TestMethod]
    public void GivenValidPhoto_WhenSaving_ThenCreatesFile()
    {
        Directory.CreateDirectory(_tempDir);
        var storage = new ProductPhotoStorage(_tempDir);
        var base64 = Convert.ToBase64String(new byte[] { 1, 2, 3 });
        var photo = new PhotoDTO("foto", ".jpg", 3, base64);

        var url = storage.Save(photo.Base64, photo.Extension);

        var fullPath = Path.Combine(_tempDir, Path.GetFileName(url));
        Assert.IsTrue(File.Exists(fullPath));
    }

    [TestMethod]
    public void GivenExistingFile_WhenDeleting_ThenRemovesFile()
    {
        Directory.CreateDirectory(_tempDir);
        var filePath = Path.Combine(_tempDir, "foto.jpg");
        File.WriteAllBytes(filePath, new byte[] { 1, 2, 3 });
        var storage = new ProductPhotoStorage(_tempDir);

        storage.Delete(filePath);

        Assert.IsFalse(File.Exists(filePath));
    }

    [TestMethod]
    public void GivenNonExistingDirectory_WhenSavingPhoto_ThenCreatesDirectoryAndSavesFile()
    {
        var storage = new ProductPhotoStorage(_tempDir);
        var base64 = Convert.ToBase64String(new byte[] { 1, 2, 3 });
        var photo = new PhotoDTO("foto", ".jpg", 3, base64);

        var url = storage.Save(photo.Base64, photo.Extension);

        var fullPath = Path.Combine(_tempDir, Path.GetFileName(url));
        Assert.IsTrue(Directory.Exists(_tempDir));
        Assert.IsTrue(File.Exists(fullPath));
    }
}
