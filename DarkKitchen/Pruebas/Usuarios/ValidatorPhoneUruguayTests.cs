using LogicaDeNegocio;
using LogicaDeNegocio.Shared;

namespace Pruebas.Usuarios;

[TestClass]
public class ValidatorPhoneUruguayTests
{
    [TestMethod]
    public void EsValido_TelefonoNulo_RetornaFalse()
    {
        var validador = new PhoneValidatorUruguay();

        string? telefono = null;
        var resultado = validador.ItsValid(telefono!);

        Assert.IsFalse(resultado);
    }

    [TestMethod]
    public void EsValido_TelefonoVacio_RetornaFalse()
    {
        var validador = new PhoneValidatorUruguay();

        var resultado = validador.ItsValid(string.Empty);

        Assert.IsFalse(resultado);
    }

    [TestMethod]
    public void EsValido_TelefonoConSoloEspacios_RetornaFalse()
    {
        var validador = new PhoneValidatorUruguay();

        var resultado = validador.ItsValid("   ");

        Assert.IsFalse(resultado);
    }

    [TestMethod]
    public void EsValido_TelefonoConLargoMenorA9_RetornaFalse()
    {
        var validador = new PhoneValidatorUruguay();

        var resultado = validador.ItsValid("09912345");

        Assert.IsFalse(resultado);
    }

    [TestMethod]
    public void EsValido_TelefonoConLargoMayorA9_RetornaFalse()
    {
        var validador = new PhoneValidatorUruguay();

        var resultado = validador.ItsValid("0991234567");

        Assert.IsFalse(resultado);
    }

    [TestMethod]
    public void EsValido_TelefonoConLetras_RetornaFalse()
    {
        var validador = new PhoneValidatorUruguay();

        var resultado = validador.ItsValid("09912A456");

        Assert.IsFalse(resultado);
    }

    [TestMethod]
    public void EsValido_TelefonoConCaracterEspecial_RetornaFalse()
    {
        var validador = new PhoneValidatorUruguay();

        var resultado = validador.ItsValid("09912-456");

        Assert.IsFalse(resultado);
    }

    [TestMethod]
    public void EsValido_TelefonoConPrefijoInvalido_RetornaFalse()
    {
        var validador = new PhoneValidatorUruguay();

        var resultado = validador.ItsValid("090123456");

        Assert.IsFalse(resultado);
    }

    [TestMethod]
    public void EsValido_TelefonoFijo_RetornaFalse()
    {
        var validador = new PhoneValidatorUruguay();

        var resultado = validador.ItsValid("29012345");

        Assert.IsFalse(resultado);
    }

    [TestMethod]
    public void EsValido_TelefonoConPrefijo091_RetornaTrue()
    {
        var validador = new PhoneValidatorUruguay();

        var resultado = validador.ItsValid("091123456");

        Assert.IsTrue(resultado);
    }

    [TestMethod]
    public void EsValido_TelefonoConPrefijo099_RetornaTrue()
    {
        var validador = new PhoneValidatorUruguay();

        var resultado = validador.ItsValid("099123456");

        Assert.IsTrue(resultado);
    }

    [TestMethod]
    public void EsValido_TelefonoConPrefijo098_RetornaTrue()
    {
        var validador = new PhoneValidatorUruguay();

        var resultado = validador.ItsValid("098123456");

        Assert.IsTrue(resultado);
    }
}
