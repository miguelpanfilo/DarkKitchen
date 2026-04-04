using Dominio;
using Dominio.Excepciones;

namespace Pruebas.Usuarios;

[TestClass]
public class EmailTests
{
    [TestMethod]
    public void MakeEmail_ValidEmail_ReturnsEmail()
    {
        var email = Email.MakeEmail("miguel@gmail.com");

        Assert.IsNotNull(email);
        Assert.AreEqual("miguel@gmail.com", email.Value);
    }

    [TestMethod]
    public void MakeEmail_EmailWithSpacesAndUppercase_ReturnsNormalizedEmail()
    {
        var email = Email.MakeEmail("  MIGUEL@GMAIL.COM  ");

        Assert.IsNotNull(email);
        Assert.AreEqual("miguel@gmail.com", email.Value);
    }

    [TestMethod]
    public void MakeEmail_EmptyEmail_ThrowsExceptionWithCorrectMessage()
    {
        var ex = Assert.ThrowsException<UsuarioExcepcion>(() =>
            Email.MakeEmail(string.Empty)
        );

        Assert.AreEqual("Email cannot be empty", ex.Message);
    }

    [TestMethod]
    public void MakeEmail_NullEmail_ThrowsExceptionWithCorrectMessage()
    {
        string? email = null;

        var ex = Assert.ThrowsException<UsuarioExcepcion>(() =>
            Email.MakeEmail(email)
        );

        Assert.AreEqual("Email cannot be empty", ex.Message);
    }

    [TestMethod]
    public void MakeEmail_EmailWithoutAtSymbol_ThrowsExceptionWithCorrectMessage()
    {
        var ex = Assert.ThrowsException<UsuarioExcepcion>(() =>
            Email.MakeEmail("miguelgmail.com")
        );

        Assert.AreEqual("Email format is not valid", ex.Message);
    }

    [TestMethod]
    public void MakeEmail_EmailWithTwoAtSymbols_ThrowsExceptionWithCorrectMessage()
    {
        var ex = Assert.ThrowsException<UsuarioExcepcion>(() =>
            Email.MakeEmail("miguel@@gmail.com")
        );

        Assert.AreEqual("Email format is not valid", ex.Message);
    }

    [TestMethod]
    public void MakeEmail_EmailWithoutLocalPart_ThrowsExceptionWithCorrectMessage()
    {
        var ex = Assert.ThrowsException<UsuarioExcepcion>(() =>
            Email.MakeEmail("@gmail.com")
        );

        Assert.AreEqual("Email format is not valid", ex.Message);
    }

    [TestMethod]
    public void MakeEmail_EmailWithoutDomain_ThrowsExceptionWithCorrectMessage()
    {
        var ex = Assert.ThrowsException<UsuarioExcepcion>(() =>
            Email.MakeEmail("miguel@")
        );

        Assert.AreEqual("Email format is not valid", ex.Message);
    }

    [TestMethod]
    public void MakeEmail_EmailWithoutDotInDomain_ThrowsExceptionWithCorrectMessage()
    {
        var ex = Assert.ThrowsException<UsuarioExcepcion>(() =>
            Email.MakeEmail("miguel@gmail")
        );

        Assert.AreEqual("Email format is not valid", ex.Message);
    }

    [TestMethod]
    public void MakeEmail_EmailWithDomainStartingWithDot_ThrowsExceptionWithCorrectMessage()
    {
        var ex = Assert.ThrowsException<UsuarioExcepcion>(() =>
            Email.MakeEmail("miguel@.gmail.com")
        );

        Assert.AreEqual("Email format is not valid", ex.Message);
    }

    [TestMethod]
    public void MakeEmail_EmailWithDomainEndingWithDot_ThrowsExceptionWithCorrectMessage()
    {
        var ex = Assert.ThrowsException<UsuarioExcepcion>(() =>
            Email.MakeEmail("miguel@gmail.com.")
        );

        Assert.AreEqual("Email format is not valid", ex.Message);
    }

    [TestMethod]
    public void MakeEmail_EmailWithDoubleDotInDomain_ThrowsExceptionWithCorrectMessage()
    {
        var ex = Assert.ThrowsException<UsuarioExcepcion>(() =>
            Email.MakeEmail("miguel@gmail..com")
        );

        Assert.AreEqual("Email format is not valid", ex.Message);
    }

    [TestMethod]
    public void MakeEmail_EmailWithDoubleDotInLocalPart_ThrowsExceptionWithCorrectMessage()
    {
        var ex = Assert.ThrowsException<UsuarioExcepcion>(() =>
            Email.MakeEmail("mi..guel@gmail.com")
        );

        Assert.AreEqual("Email format is not valid", ex.Message);
    }

    [TestMethod]
    public void IsTheSame_SameEmailValue_ReturnsTrue()
    {
        var email1 = Email.MakeEmail("miguel@gmail.com");
        var email2 = Email.MakeEmail("MIGUEL@GMAIL.COM");

        var result = email1.IsTheSame(email2);

        Assert.IsTrue(result);
    }

    [TestMethod]
    public void IsTheSame_DifferentEmailValue_ReturnsFalse()
    {
        var email1 = Email.MakeEmail("miguel@gmail.com");
        var email2 = Email.MakeEmail("otro@gmail.com");

        var result = email1.IsTheSame(email2);

        Assert.IsFalse(result);
    }

    [TestMethod]
    public void ToString_ReturnsValue()
    {
        var email = Email.MakeEmail("miguel@gmail.com");

        var result = email.ToString();

        Assert.AreEqual("miguel@gmail.com", result);
    }
}
