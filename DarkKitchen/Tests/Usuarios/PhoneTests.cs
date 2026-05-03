using Domain;

namespace Pruebas.Usuarios;

[TestClass]
public class PhoneTests
{
    [TestMethod]
    public void MakePhone_ValidPhone_ReturnsPhone()
    {
        var result = Phone.TryMakePhone("099123456");

        Assert.IsTrue(result.Success);
        Assert.IsNotNull(result.Value);
        Assert.AreEqual("099123456", result.Value!.Value);
    }

    [TestMethod]
    public void MakePhone_ValidPhoneWithSpaces_NormalizesCorrectly()
    {
        var result = Phone.TryMakePhone(" 099123456 ");

        Assert.IsTrue(result.Success);
        Assert.IsNotNull(result.Value);
        Assert.AreEqual("099123456", result.Value!.Value);
    }

    [TestMethod]
    public void MakePhone_NullPhone_ThrowsExceptionWithCorrectMessage()
    {
        var phone = string.Empty;

        var result = Phone.TryMakePhone(phone);

        Assert.IsFalse(result.Success);
        Assert.AreEqual("Telefono no puede estar vacío", result.ErrorMessage);
    }

    [TestMethod]
    public void MakePhone_EmptyPhone_ThrowsExceptionWithCorrectMessage()
    {
        var result = Phone.TryMakePhone(string.Empty);

        Assert.IsFalse(result.Success);
        Assert.AreEqual("Telefono no puede estar vacío", result.ErrorMessage);
    }

    [TestMethod]
    public void MakePhone_PhoneWithLetters_ThrowsExceptionWithCorrectMessage()
    {
        var result = Phone.TryMakePhone("09912a456");

        Assert.IsFalse(result.Success);
        Assert.AreEqual("Telefono solo puede contener números", result.ErrorMessage);
    }

    [TestMethod]
    public void MakePhone_PhoneWithSymbols_ThrowsExceptionWithCorrectMessage()
    {
        var result = Phone.TryMakePhone("09912-456");

        Assert.IsFalse(result.Success);
        Assert.AreEqual("Telefono solo puede contener números", result.ErrorMessage);
    }

    [TestMethod]
    public void ToString_ReturnsValue()
    {
        var result = Phone.TryMakePhone("099123456");

        Assert.IsTrue(result.Success);
        Assert.IsNotNull(result.Value);

        var text = result.Value!.ToString();

        Assert.AreEqual("099123456", text);
    }
}
