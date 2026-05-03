using Domain;
using Domain.Exceptions;

namespace Tests.Products;

[TestClass]
public class ProductTest
{
    private static Product CreateValidProduct()
    {
        return Product.MakeProduct(
            "COD123",
            "Milanesa al pan",
            "Milanesa al pan completa con fritas",
            "Minutas clásicas",
            "Sandwiches",
            100,
            ValidPhotos());
    }

    private static List<ProductPhoto> ValidPhotos(int count = 1)
    {
        var photos = new List<ProductPhoto>();

        for (var i = 0; i < count; i++)
        {
            photos.Add(new ProductPhoto($"foto{i}.jpg"));
        }

        return photos;
    }

    [TestMethod]
    public void GivenValidData_WhenCreatingProduct_ThenProductIsCreated()
    {
        var product = Product.MakeProduct(
            "COD123",
            "Milanesa al pan",
            "Milanesa al pan completa",
            "Minutas clásicas",
            "Sandwiches",
            100,
            ValidPhotos());

        Assert.IsNotNull(product);
        Assert.AreEqual("COD123", product.Code);
        Assert.AreEqual("Milanesa al pan", product.Name);
        Assert.AreEqual("Milanesa al pan completa", product.Description);
        Assert.AreEqual("Minutas clásicas", product.CommercialLine);
        Assert.AreEqual("Sandwiches", product.Category);
        Assert.AreEqual(100m, product.Price);
        Assert.IsTrue(product.IsActive);
    }

    [TestMethod]
    public void GivenEmptyName_WhenCreatingProduct_ThenThrowsProductExceptionWithMessage()
    {
        var ex = Assert.ThrowsException<ProductException>(() =>
            Product.MakeProduct(
                "COD123",
                " ",
                "Milanesa al pan completa",
                "Minutas clásicas",
                "Sandwiches",
                100,
                ValidPhotos(0)));

        Assert.AreEqual("El nombre no puede estar vacío.", ex.Message);
    }

    [TestMethod]
    public void GivenInvalidCode_WhenCreatingProduct_ThenThrowsProductExceptionWithMessage()
    {
        var ex = Assert.ThrowsException<ProductException>(() =>
            Product.MakeProduct(
                " ",
                "Milanesa al pan",
                "Milanesa al pan completa",
                "Minutas clásicas",
                "Sandwiches",
                100,
                ValidPhotos(0)));

        Assert.AreEqual("El código no puede estar vacío.", ex.Message);
    }

    [TestMethod]
    public void GivenEmptyDescription_WhenCreatingProduct_ThenThrowsProductExceptionWithMessage()
    {
        var ex = Assert.ThrowsException<ProductException>(() =>
            Product.MakeProduct(
                "COD123",
                "Milanesa al pan",
                " ",
                "Minutas clásicas",
                "Sandwiches",
                100,
                ValidPhotos(0)));

        Assert.AreEqual("La descripción no puede estar vacío.", ex.Message);
    }

    [TestMethod]
    public void GivenEmptyCommercialLine_WhenCreatingProduct_ThenThrowsProductExceptionWithMessage()
    {
        var ex = Assert.ThrowsException<ProductException>(() =>
            Product.MakeProduct(
                "COD123",
                "Milanesa al pan",
                "Milanesa al pan completa",
                " ",
                "Sandwiches",
                100,
                ValidPhotos(0)));

        Assert.AreEqual("Línea comercial no puede estar vacío.", ex.Message);
    }

    [TestMethod]
    public void GivenEmptyCategory_WhenCreatingProduct_ThenThrowsProductExceptionWithMessage()
    {
        var ex = Assert.ThrowsException<ProductException>(() =>
            Product.MakeProduct(
                "COD123",
                "Milanesa al pan",
                "Milanesa al pan completa",
                "Minutas clásicas",
                " ",
                100,
                ValidPhotos(0)));

        Assert.AreEqual("La categoría no puede estar vacío.", ex.Message);
    }

    [TestMethod]
    public void GivenShortCode_WhenCreatingProduct_ThenThrowsProductExceptionWithMessage()
    {
        var code = new string('A', 4);

        var ex = Assert.ThrowsException<ProductException>(() =>
            Product.MakeProduct(
                code,
                "Milanesa al pan",
                "Milanesa al pan completa",
                "Minutas clásicas",
                "Sandwiches",
                100,
                ValidPhotos(0)));

        Assert.AreEqual("El código debe tener entre 5 y 20 caracteres.", ex.Message);
    }

    [TestMethod]
    public void GivenLongCode_WhenCreatingProduct_ThenThrowsProductExceptionWithMessage()
    {
        var code = new string('A', 21);

        var ex = Assert.ThrowsException<ProductException>(() =>
            Product.MakeProduct(
                code,
                "Nombre válido",
                "Descripción suficientemente larga",
                "Minutas clásicas",
                "Sandwiches",
                100,
                ValidPhotos(0)));

        Assert.AreEqual("El código debe tener entre 5 y 20 caracteres.", ex.Message);
    }

    [TestMethod]
    public void GivenShortName_WhenCreatingProduct_ThenThrowsProductExceptionWithMessage()
    {
        var name = new string('A', 9);

        var ex = Assert.ThrowsException<ProductException>(() =>
            Product.MakeProduct(
                "COD123",
                name,
                "Descripción suficientemente larga",
                "Minutas clásicas",
                "Sandwiches",
                100,
                ValidPhotos(0)));

        Assert.AreEqual("El nombre debe tener entre 10 y 50 caracteres.", ex.Message);
    }

    [TestMethod]
    public void GivenLongName_WhenCreatingProduct_ThenThrowsProductExceptionWithMessage()
    {
        var name = new string('A', 51);

        var ex = Assert.ThrowsException<ProductException>(() =>
            Product.MakeProduct(
                "COD123",
                name,
                "Descripción suficientemente larga",
                "Minutas clásicas",
                "Sandwiches",
                100,
                ValidPhotos(0)));

        Assert.AreEqual("El nombre debe tener entre 10 y 50 caracteres.", ex.Message);
    }

    [TestMethod]
    public void GivenShortDescription_WhenCreatingProduct_ThenThrowsProductExceptionWithMessage()
    {
        var description = new string('A', 19);

        var ex = Assert.ThrowsException<ProductException>(() =>
            Product.MakeProduct(
                "COD123",
                "Nombre válido",
                description,
                "Minutas clásicas",
                "Sandwiches",
                100,
                ValidPhotos(0)));

        Assert.AreEqual("La descripción debe tener entre 20 y 500 caracteres.", ex.Message);
    }

    [TestMethod]
    public void GivenLongDescription_WhenCreatingProduct_ThenThrowsProductExceptionWithMessage()
    {
        var description = new string('A', 501);

        var ex = Assert.ThrowsException<ProductException>(() =>
            Product.MakeProduct(
                "COD123",
                "Nombre válido",
                description,
                "Minutas clásicas",
                "Sandwiches",
                100,
                ValidPhotos(0)));

        Assert.AreEqual("La descripción debe tener entre 20 y 500 caracteres.", ex.Message);
    }

    [TestMethod]
    public void GivenValidData_WhenCreatingProduct_ThenProductIsActiveByDefault()
    {
        var product = Product.MakeProduct(
            "ABC12",
            "Milanesa completa",
            "Milanesa completa con guarnicion y bebida incluida",
            "Minutas clasicas",
            "Fritos",
            100,
            ValidPhotos());

        Assert.IsTrue(product.IsActive);
    }

    [TestMethod]
    public void Deactivate_WhenCalled_ProductBecomesInactive()
    {
        var product = Product.MakeProduct(
            "ABC12",
            "Milanesa completa",
            "Milanesa completa con guarnicion y bebida incluida",
            "Minutas clasicas",
            "Fritos",
            100,
            ValidPhotos());

        product.Deactivate();

        Assert.IsFalse(product.IsActive);
    }

    [TestMethod]
    public void Activate_WhenCalled_ProductBecomesActive()
    {
        var product = Product.MakeProduct(
            "ABC12",
            "Milanesa completa",
            "Milanesa completa con guarnicion y bebida incluida",
            "Minutas clasicas",
            "Fritos",
            100,
            ValidPhotos());

        product.Deactivate();
        product.Activate();

        Assert.IsTrue(product.IsActive);
    }

    [DataTestMethod]
    [DataRow(0)]
    [DataRow(-1)]
    public void GivenInvalidPrice_WhenCreatingProduct_ThenThrowsProductExceptionWithMessage(int price)
    {
        var ex = Assert.ThrowsException<ProductException>(() =>
            Product.MakeProduct(
                "COD123",
                "Milanesa al pan",
                "Milanesa al pan completa",
                "Minutas clásicas",
                "Sandwiches",
                price,
                ValidPhotos(0)));

        Assert.AreEqual("El precio debe ser mayor a 0.", ex.Message);
    }

    [DataTestMethod]
    [DataRow(1)]
    [DataRow(3)]
    public void GivenValidNumberOfPhotos_WhenCreatingProduct_ThenProductIsCreated(int photoCount)
    {
        var product = Product.MakeProduct(
            "COD123",
            "Milanesa al pan",
            "Milanesa al pan completa con fritas",
            "Minutas clásicas",
            "Sandwiches",
            100,
            ValidPhotos(photoCount));

        Assert.IsNotNull(product);
        Assert.AreEqual(photoCount, product.Photos.Count);
    }

    [DataTestMethod]
    [DataRow(0)]
    [DataRow(4)]
    public void GivenInvalidNumberOfPhotos_WhenCreatingProduct_ThenThrowsProductException(int photoCount)
    {
        var exception = Assert.ThrowsException<ProductException>(() =>
            Product.MakeProduct(
                "COD123",
                "Milanesa al pan",
                "Milanesa al pan completa con fritas",
                "Minutas clásicas",
                "Sandwiches",
                100,
                ValidPhotos(photoCount)));

        Assert.AreEqual("El producto debe tener entre 1 y 3 fotos.", exception.Message);
    }

    [TestMethod]
    public void GivenNullPhotos_WhenCreatingProduct_ThenThrowsProductException()
    {
        var exception = Assert.ThrowsException<ProductException>(() =>
            Product.MakeProduct(
                "COD123",
                "Milanesa al pan",
                "Milanesa al pan completa con fritas",
                "Minutas clásicas",
                "Sandwiches",
                100,
                null!));

        Assert.AreEqual("El producto debe tener entre 1 y 3 fotos.", exception.Message);
    }

    [TestMethod]
    public void GivenValidName_WhenUpdatingName_ThenNameIsUpdated()
    {
        var product = CreateValidProduct();

        var newName = "Chivito completo";

        product.UpdateName(newName);

        Assert.AreEqual(newName, product.Name);
    }

    [TestMethod]
    public void GivenEmptyName_WhenUpdatingName_ThenThrowsProductExceptionWithMessage()
    {
        var product = CreateValidProduct();

        var ex = Assert.ThrowsException<ProductException>(() => product.UpdateName(string.Empty));

        Assert.AreEqual("El nombre no puede estar vacío.", ex.Message);
    }

    [DataTestMethod]
    [DataRow("corto")]
    [DataRow("este nombre es extremadamente largo y supera los cincuenta caracteres permitidos")]
    public void GivenInvalidLengthName_WhenUpdatingName_ThenThrowsProductExceptionWithMessage(string invalidName)
    {
        var product = CreateValidProduct();

        var ex = Assert.ThrowsException<ProductException>(() => product.UpdateName(invalidName));

        Assert.AreEqual("El nombre debe tener entre 10 y 50 caracteres.", ex.Message);
    }

    [TestMethod]
    public void GivenValidCode_WhenUpdatingCode_ThenUpdatesProductCode()
    {
        var product = CreateValidProduct();
        var newCode = "COD999";

        product.UpdateCode(newCode);

        Assert.AreEqual(newCode, product.Code);
    }

    [DataTestMethod]
    [DataRow("", "El código no puede estar vacío.")]
    [DataRow("abc", "El código debe tener entre 5 y 20 caracteres.")]
    [DataRow("este-codigo-es-demasiado-largo-para-ser-valido", "El código debe tener entre 5 y 20 caracteres.")]
    public void GivenInvalidCode_WhenUpdatingCode_ThenThrowsProductExceptionWithExpectedMessage(
        string invalidCode,
        string expectedMessage)
    {
        var product = CreateValidProduct();

        var exception = Assert.ThrowsException<ProductException>(() => product.UpdateCode(invalidCode));

        Assert.AreEqual(expectedMessage, exception.Message);
    }

    [TestMethod]
    public void GivenValidDescription_WhenUpdatingDescription_ThenUpdatesProductDescription()
    {
        var product = CreateValidProduct();
        var newDescription = "Esta es una descripción válida con más de veinte caracteres.";

        product.UpdateDescription(newDescription);

        Assert.AreEqual(newDescription, product.Description);
    }

    [DataTestMethod]
    [DataRow("", "La descripción no puede estar vacío.")]
    [DataRow("corta", "La descripción debe tener entre 20 y 500 caracteres.")]
    [DataRow("Esta descripción es extremadamente larga y fue creada únicamente para superar el máximo de quinientos " +
             "caracteres permitidos por la validación del dominio. Sigue creciendo con texto de relleno para " +
             "asegurarnos de que realmente rompa la regla establecida en la entidad Product y " +
             "así verificar correctamente que el método " +
             "UpdateDescription lance la excepción esperada cuando el contenido excede ampliamente el " +
             "límite definido. Agrego todavía más texto para no quedar corto, manteniendo la misma idea y " +
             "forzando un caso claramente inválido dentro del test.", "La descripción debe tener entre 20 y 500 caracteres.")]
    public void GivenInvalidDescription_WhenUpdatingDescription_ThenThrowsProductExceptionWithExpectedMessage(
        string invalidDescription,
        string expectedMessage)
    {
        var product = CreateValidProduct();

        var exception = Assert.ThrowsException<ProductException>(
            () => product.UpdateDescription(invalidDescription));

        Assert.AreEqual(expectedMessage, exception.Message);
    }

    [TestMethod]
    public void GivenValidPrice_WhenUpdatingPrice_ThenUpdatesProductPrice()
    {
        var product = CreateValidProduct();
        var newPrice = 200;

        product.UpdatePrice(newPrice);

        Assert.AreEqual(newPrice, product.Price);
    }

    [DataTestMethod]
    [DataRow(0.0)]
    [DataRow(-10.0)]
    public void GivenInvalidPrice_WhenUpdatingPrice_ThenThrowsProductException(double invalidPrice)
    {
        var product = CreateValidProduct();

        var exception = Assert.ThrowsException<ProductException>(
            () => product.UpdatePrice((decimal)invalidPrice));

        Assert.AreEqual("El precio debe ser mayor a 0.", exception.Message);
    }

    [TestMethod]
    public void GivenValidPhotos_WhenUpdatingPhotos_ThenUpdatesProductPhotos()
    {
        var product = CreateValidProduct();
        var newPhotos = new List<ProductPhoto>
        {
            new ProductPhoto("photo-1.jpg"),
            new ProductPhoto("photo-2.jpg")
        };

        product.UpdatePhotos(newPhotos);

        Assert.AreEqual(2, product.Photos.Count);
        Assert.AreEqual("photo-1.jpg", product.Photos[0].Path);
        Assert.AreEqual("photo-2.jpg", product.Photos[1].Path);
    }

    [TestMethod]
    public void GivenNullPhotos_WhenUpdatingPhotos_ThenThrowsProductException()
    {
        var product = CreateValidProduct();

        var exception = Assert.ThrowsException<ProductException>(
            () => product.UpdatePhotos(null!));

        Assert.AreEqual("El producto debe tener entre 1 y 3 fotos.", exception.Message);
    }

    [TestMethod]
    public void GivenEmptyPhotos_WhenUpdatingPhotos_ThenThrowsProductException()
    {
        var product = CreateValidProduct();
        var photos = new List<ProductPhoto>();

        var exception = Assert.ThrowsException<ProductException>(
            () => product.UpdatePhotos(photos));

        Assert.AreEqual("El producto debe tener entre 1 y 3 fotos.", exception.Message);
    }

    [TestMethod]
    public void GivenMoreThanThreePhotos_WhenUpdatingPhotos_ThenThrowsProductException()
    {
        var product = CreateValidProduct();
        var photos = new List<ProductPhoto>
        {
            new ProductPhoto("photo-1.jpg"),
            new ProductPhoto("photo-2.jpg"),
            new ProductPhoto("photo-3.jpg"),
            new ProductPhoto("photo-4.jpg")
        };

        var exception = Assert.ThrowsException<ProductException>(
            () => product.UpdatePhotos(photos));

        Assert.AreEqual("El producto debe tener entre 1 y 3 fotos.", exception.Message);
    }

    [TestMethod]
    public void GivenValidCommercialLine_WhenUpdatingCommercialLine_ThenUpdatesProductCommercialLine()
    {
        var product = CreateValidProduct();
        var newCommercialLine = "Nueva línea comercial";

        product.UpdateCommercialLine(newCommercialLine);

        Assert.AreEqual(newCommercialLine, product.CommercialLine);
    }

    [DataTestMethod]
    [DataRow("")]
    [DataRow("   ")]
    public void GivenInvalidCommercialLine_WhenUpdatingCommercialLine_ThenThrowsProductException(string invalidCommercialLine)
    {
        var product = CreateValidProduct();

        var exception = Assert.ThrowsException<ProductException>(
            () => product.UpdateCommercialLine(invalidCommercialLine));

        Assert.AreEqual("Línea comercial no puede estar vacío.", exception.Message);
    }

    [TestMethod]
    public void GivenValidCategory_WhenUpdatingCategory_ThenUpdatesProductCategory()
    {
        var product = CreateValidProduct();
        var newCategory = "Nueva categoría";

        product.UpdateCategory(newCategory);

        Assert.AreEqual(newCategory, product.Category);
    }

    [DataTestMethod]
    [DataRow("")]
    [DataRow("   ")]
    public void GivenInvalidCategory_WhenUpdatingCategory_ThenThrowsProductException(string invalidCategory)
    {
        var product = CreateValidProduct();

        var exception = Assert.ThrowsException<ProductException>(
            () => product.UpdateCategory(invalidCategory));

        Assert.AreEqual("La categoría no puede estar vacío.", exception.Message);
    }

    [TestMethod]
    public void GivenNonDeletedProduct_WhenDeleting_ThenMarksItAsDeleted()
    {
        var photos = new List<ProductPhoto>
        {
            new ProductPhoto("photo1.jpg")
        };

        var product = Product.MakeProduct(
            "COD123",
            "Producto válido",
            "Descripción válida del producto",
            "Línea comercial",
            "Categoría",
            100,
            photos);

        product.Delete();

        Assert.IsTrue(product.IsDeleted);
    }

    [TestMethod]
    public void GivenDeletedProduct_WhenDeleting_ThenThrowsProductException()
    {
        var photos = new List<ProductPhoto>
        {
            new ProductPhoto("photo1.jpg")
        };

        var product = Product.MakeProduct(
            "COD123",
            "Producto válido",
            "Descripción válida del producto",
            "Línea comercial",
            "Categoría",
            100,
            photos);

        product.Delete();

        var ex = Assert.ThrowsException<ProductException>(() => product.Delete());

        Assert.AreEqual("El producto ya está eliminado", ex.Message);
    }
}
