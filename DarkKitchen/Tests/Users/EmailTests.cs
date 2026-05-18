using Domain;

namespace Tests.Users;

[TestClass]
public class EmailTests
{
    [TestMethod]
    public void MakeEmail_ValidEmail_ReturnsEmail()
    {
        var result = Email.TryMakeEmail("miguel@gmail.com");

        Assert.IsTrue(result.Success);
        Assert.IsNotNull(result.Value);
        Assert.AreEqual("miguel@gmail.com", result.Value!.Value);
    }

    [TestMethod]
    public void MakeEmail_EmailWithSpacesAndUppercase_ReturnsNormalizedEmail()
    {
        var result = Email.TryMakeEmail("  MIGUEL@GMAIL.COM  ");

        Assert.IsTrue(result.Success);
        Assert.IsNotNull(result.Value);
        Assert.AreEqual("miguel@gmail.com", result.Value!.Value);
    }

    [TestMethod]
    public void MakeEmail_EmptyEmail_ThrowsExceptionWithCorrectMessage()
    {
        var result = Email.TryMakeEmail(string.Empty);

        Assert.IsFalse(result.Success);
        Assert.AreEqual("Email no puede estar vacío", result.ErrorMessage);
    }

    [TestMethod]
    public void MakeEmail_NullEmail_ThrowsExceptionWithCorrectMessage()
    {
        string? email = null;

        var result = Email.TryMakeEmail(email);

        Assert.IsFalse(result.Success);
        Assert.AreEqual("Email no puede estar vacío", result.ErrorMessage);
    }

    [TestMethod]
    public void MakeEmail_EmailWithoutAtSymbol_ThrowsExceptionWithCorrectMessage()
    {
        var result = Email.TryMakeEmail("miguelgmail.com");

        Assert.IsFalse(result.Success);
        Assert.AreEqual("Formato de Email inválido", result.ErrorMessage);
    }

    [TestMethod]
    public void MakeEmail_EmailWithTwoAtSymbols_ThrowsExceptionWithCorrectMessage()
    {
        var result = Email.TryMakeEmail("miguel@@gmail.com");

        Assert.IsFalse(result.Success);
        Assert.AreEqual("Formato de Email inválido", result.ErrorMessage);
    }

    [TestMethod]
    public void MakeEmail_EmailWithoutLocalPart_ThrowsExceptionWithCorrectMessage()
    {
        var result = Email.TryMakeEmail("@gmail.com");

        Assert.IsFalse(result.Success);
        Assert.AreEqual("Formato de Email inválido", result.ErrorMessage);
    }

    [TestMethod]
    public void MakeEmail_EmailWithoutDomain_ThrowsExceptionWithCorrectMessage()
    {
        var result = Email.TryMakeEmail("miguel@");

        Assert.IsFalse(result.Success);
        Assert.AreEqual("Formato de Email inválido", result.ErrorMessage);
    }

    [TestMethod]
    public void MakeEmail_EmailWithoutDotInDomain_ThrowsExceptionWithCorrectMessage()
    {
        var result = Email.TryMakeEmail("miguel@gmail");

        Assert.IsFalse(result.Success);
        Assert.AreEqual("Formato de Email inválido", result.ErrorMessage);
    }

    [TestMethod]
    public void MakeEmail_EmailWithDomainStartingWithDot_ThrowsExceptionWithCorrectMessage()
    {
        var result = Email.TryMakeEmail("miguel@.gmail.com");

        Assert.IsFalse(result.Success);
        Assert.AreEqual("Formato de Email inválido", result.ErrorMessage);
    }

    [TestMethod]
    public void MakeEmail_EmailWithDomainEndingWithDot_ThrowsExceptionWithCorrectMessage()
    {
        var result = Email.TryMakeEmail("miguel@gmail.com.");

        Assert.IsFalse(result.Success);
        Assert.AreEqual("Formato de Email inválido", result.ErrorMessage);
    }

    [TestMethod]
    public void MakeEmail_EmailWithDoubleDotInDomain_ThrowsExceptionWithCorrectMessage()
    {
        var result = Email.TryMakeEmail("miguel@gmail..com");

        Assert.IsFalse(result.Success);
        Assert.AreEqual("Formato de Email inválido", result.ErrorMessage);
    }

    [TestMethod]
    public void MakeEmail_EmailWithDoubleDotInLocalPart_ThrowsExceptionWithCorrectMessage()
    {
        var result = Email.TryMakeEmail("mi..guel@gmail.com");

        Assert.IsFalse(result.Success);
        Assert.AreEqual("Formato de Email inválido", result.ErrorMessage);
    }

    [TestMethod]
    public void IsTheSame_SameEmailValue_ReturnsTrue()
    {
        var email1Result = Email.TryMakeEmail("miguel@gmail.com");
        var email2Result = Email.TryMakeEmail("MIGUEL@GMAIL.COM");

        Assert.IsTrue(email1Result.Success);
        Assert.IsTrue(email2Result.Success);

        var result = email1Result.Value!.IsTheSame(email2Result.Value!);

        Assert.IsTrue(result);
    }

    [TestMethod]
    public void IsTheSame_DifferentEmailValue_ReturnsFalse()
    {
        var email1Result = Email.TryMakeEmail("miguel@gmail.com");
        var email2Result = Email.TryMakeEmail("otro@gmail.com");

        Assert.IsTrue(email1Result.Success);
        Assert.IsTrue(email2Result.Success);

        var result = email1Result.Value!.IsTheSame(email2Result.Value!);

        Assert.IsFalse(result);
    }

    [TestMethod]
    public void ToString_ReturnsValue()
    {
        var result = Email.TryMakeEmail("miguel@gmail.com");

        Assert.IsTrue(result.Success);
        Assert.IsNotNull(result.Value);

        var text = result.Value!.ToString();

        Assert.AreEqual("miguel@gmail.com", text);
    }
}
