using Dominio;
using Dominio.Excepciones;

namespace Pruebas.Usuarios;

[TestClass]
public class PhoneTests
{
    [TestMethod]
    public void MakePhone_ValidPhone_ReturnsPhone()
    {
        var phone = Phone.MakePhone("099123456");

        Assert.IsNotNull(phone);
        Assert.AreEqual("099123456", phone.Value);
    }

    [TestMethod]
    public void MakePhone_ValidPhoneWithSpaces_NormalizesCorrectly()
    {
        var phone = Phone.MakePhone(" 099123456 ");

        Assert.AreEqual("099123456", phone.Value);
    }

    [TestMethod]
    public void MakePhone_NullPhone_ThrowsExceptionWithCorrectMessage()
    {
        string? phone = null;

        var ex = Assert.ThrowsException<UserException>(() =>
            Phone.MakePhone(phone!));

        Assert.AreEqual("Phone cannot be empty", ex.Message);
    }

    [TestMethod]
    public void MakePhone_EmptyPhone_ThrowsExceptionWithCorrectMessage()
    {
        var ex = Assert.ThrowsException<UserException>(() =>
            Phone.MakePhone(string.Empty));

        Assert.AreEqual("Phone cannot be empty", ex.Message);
    }

    [TestMethod]
    public void MakePhone_PhoneWithLetters_ThrowsExceptionWithCorrectMessage()
    {
        var ex = Assert.ThrowsException<UserException>(() =>
            Phone.MakePhone("09912a456"));

        Assert.AreEqual("Phone must contain only digits", ex.Message);
    }

    [TestMethod]
    public void MakePhone_PhoneWithSymbols_ThrowsExceptionWithCorrectMessage()
    {
        var ex = Assert.ThrowsException<UserException>(() =>
            Phone.MakePhone("09912-456"));

        Assert.AreEqual("Phone must contain only digits", ex.Message);
    }

    [TestMethod]
    public void ToString_ReturnsValue()
    {
        var phone = Phone.MakePhone("099123456");

        var result = phone.ToString();

        Assert.AreEqual("099123456", result);
    }
}
