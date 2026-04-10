using Dominio;

namespace Pruebas.Usuarios;

[TestClass]
public class PasswordTests
{
    [TestMethod]
    public void Validate_NullPassword_ReturnsFalse()
    {
        string? password = null;

        var isValid = Password.Validate(password);

        Assert.IsFalse(isValid);
    }

    [TestMethod]
    public void Validate_EmptyPassword_ReturnsFalse()
    {
        var password = string.Empty;

        var isValid = Password.Validate(password);

        Assert.IsFalse(isValid);
    }

    [TestMethod]
    public void Validate_PasswordWithOnlySpaces_ReturnsFalse()
    {
        var password = "   ";

        var isValid = Password.Validate(password);

        Assert.IsFalse(isValid);
    }

    [TestMethod]
    public void Validate_PasswordShorterThan15_ReturnsFalse()
    {
        var password = "Passw!rdAbc9Xy";
        var isValid = Password.Validate(password);

        Assert.IsFalse(isValid);
    }

    [TestMethod]
    public void Validate_PasswordLongerThan25_ReturnsFalse()
    {
        var password = "Password!Abc9XyzKlmNoPqRst";

        var isValid = Password.Validate(password);

        Assert.IsFalse(isValid);
    }

    [TestMethod]
    public void Validate_PasswordWithoutUppercase_ReturnsFalse()
    {
        var password = "password!abc9xyz";

        var isValid = Password.Validate(password);

        Assert.IsFalse(isValid);
    }

    [TestMethod]
    public void Validate_PasswordWithoutLowercase_ReturnsFalse()
    {
        var password = "PASSWORD!ABC9XYZ";

        var isValid = Password.Validate(password);

        Assert.IsFalse(isValid);
    }

    [TestMethod]
    public void Validate_PasswordWithoutNumber_ReturnsFalse()
    {
        var password = "Password!AbcXyz";

        var isValid = Password.Validate(password);

        Assert.IsFalse(isValid);
    }

    [TestMethod]
    public void Validate_PasswordWithoutSymbol_ReturnsFalse()
    {
        var password = "PasswordAbc9Xyz";

        var isValid = Password.Validate(password);

        Assert.IsFalse(isValid);
    }

    [TestMethod]
    public void Validate_PasswordWithNumberSequence_ReturnsFalse()
    {
        var password = "Password!Ab12Xy";

        var isValid = Password.Validate(password);

        Assert.IsFalse(isValid);
    }

    [TestMethod]
    public void Validate_PasswordWith15Characters_ReturnsTrue()
    {
        var password = "Password!Abc9Xy";

        var isValid = Password.Validate(password);

        Assert.IsTrue(isValid);
    }

    [TestMethod]
    public void Validate_PasswordWith25Characters_ReturnsTrue()
    {
        var password = "Password!Abc9XyzKlmNoPqR";

        var isValid = Password.Validate(password);

        Assert.IsTrue(isValid);
    }

    [TestMethod]
    public void Validate_ValidPassword_ReturnsTrue()
    {
        var password = "Password@2Ok6Abc";

        var isValid = Password.Validate(password);

        Assert.IsTrue(isValid);
    }

    [TestMethod]
    public void Create_NullPassword_ThrowsUserException()
    {
        string? password = null;

        var result = Password.TryMakePassword(password);

        Assert.IsFalse(result.Success);
        Assert.AreEqual("Password format is not valid", result.ErrorMessage);
    }

    [TestMethod]
    public void Create_InvalidPassword_ThrowsUserException()
    {
        var password = "abc";

        var result = Password.TryMakePassword(password);

        Assert.IsFalse(result.Success);
        Assert.AreEqual("Password format is not valid", result.ErrorMessage);
    }

    [TestMethod]
    public void Create_ValidPassword_GeneratesNonEmptyHash()
    {
        var password = "Password@2Ok6Abc";

        var result = Password.TryMakePassword(password);

        Assert.IsTrue(result.Success);
        Assert.IsNotNull(result.Value);
        Assert.IsFalse(string.IsNullOrWhiteSpace(result.Value!.GetHash()));
    }

    [TestMethod]
    public void Create_SamePassword_GeneratesSameHash()
    {
        var password = "Password@2Ok6Abc";

        var password1 = Password.TryMakePassword(password);
        var password2 = Password.TryMakePassword(password);

        Assert.IsTrue(password1.Success);
        Assert.IsTrue(password2.Success);
        Assert.AreEqual(password1.Value!.GetHash(), password2.Value!.GetHash());
    }

    [TestMethod]
    public void Create_DifferentPassword_GeneratesDifferentHash()
    {
        var password1 = "Password@2Ok6Abc";
        var password2 = "Password@3Ok7Abc";

        var result1 = Password.TryMakePassword(password1);
        var result2 = Password.TryMakePassword(password2);

        Assert.IsTrue(result1.Success);
        Assert.IsTrue(result2.Success);
        Assert.AreNotEqual(result1.Value!.GetHash(), result2.Value!.GetHash());
    }

    [TestMethod]
    public void Verify_CorrectPassword_ReturnsTrue()
    {
        var passwordText = "Password@2Ok6Abc";
        var passwordResult = Password.TryMakePassword(passwordText);

        Assert.IsTrue(passwordResult.Success);

        var isCorrect = passwordResult.Value!.VerifyHash(passwordText);

        Assert.IsTrue(isCorrect);
    }

    [TestMethod]
    public void Verify_IncorrectPassword_ReturnsFalse()
    {
        var passwordText = "Password@2Ok6Abc";
        var passwordResult = Password.TryMakePassword(passwordText);

        Assert.IsTrue(passwordResult.Success);

        var isCorrect = passwordResult.Value!.VerifyHash("Password@3Ok7Abc");

        Assert.IsFalse(isCorrect);
    }
}
