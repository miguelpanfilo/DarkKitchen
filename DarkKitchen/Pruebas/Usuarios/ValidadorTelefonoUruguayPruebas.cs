using LogicaDeNegocio;

namespace Pruebas.Usuarios;

[TestClass]
public class ValidadorTelefonoUruguayPruebas
{
    [TestMethod]
    public void EsValido_TelefonoNulo_RetornaFalse()
    {
        var validador = new ValidadorTelefonoUruguay();

        string? telefono = null;
        var resultado = validador.EsValido(telefono!);

        Assert.IsFalse(resultado);
    }

    [TestMethod]
    public void EsValido_TelefonoVacio_RetornaFalse()
    {
        var validador = new ValidadorTelefonoUruguay();

        var resultado = validador.EsValido(string.Empty);

        Assert.IsFalse(resultado);
    }

    [TestMethod]
    public void EsValido_TelefonoConSoloEspacios_RetornaFalse()
    {
        var validador = new ValidadorTelefonoUruguay();

        var resultado = validador.EsValido("   ");

        Assert.IsFalse(resultado);
    }

    [TestMethod]
    public void EsValido_TelefonoConLargoMenorA9_RetornaFalse()
    {
        var validador = new ValidadorTelefonoUruguay();

        var resultado = validador.EsValido("09912345");

        Assert.IsFalse(resultado);
    }

    [TestMethod]
    public void EsValido_TelefonoConLargoMayorA9_RetornaFalse()
    {
        var validador = new ValidadorTelefonoUruguay();

        var resultado = validador.EsValido("0991234567");

        Assert.IsFalse(resultado);
    }

    [TestMethod]
    public void EsValido_TelefonoConLetras_RetornaFalse()
    {
        var validador = new ValidadorTelefonoUruguay();

        var resultado = validador.EsValido("09912A456");

        Assert.IsFalse(resultado);
    }

    [TestMethod]
    public void EsValido_TelefonoConCaracterEspecial_RetornaFalse()
    {
        var validador = new ValidadorTelefonoUruguay();

        var resultado = validador.EsValido("09912-456");

        Assert.IsFalse(resultado);
    }

    [TestMethod]
    public void EsValido_TelefonoConPrefijoInvalido_RetornaFalse()
    {
        var validador = new ValidadorTelefonoUruguay();

        var resultado = validador.EsValido("090123456");

        Assert.IsFalse(resultado);
    }

    [TestMethod]
    public void EsValido_TelefonoFijo_RetornaFalse()
    {
        var validador = new ValidadorTelefonoUruguay();

        var resultado = validador.EsValido("29012345");

        Assert.IsFalse(resultado);
    }

    [TestMethod]
    public void EsValido_TelefonoConPrefijo091_RetornaTrue()
    {
        var validador = new ValidadorTelefonoUruguay();

        var resultado = validador.EsValido("091123456");

        Assert.IsTrue(resultado);
    }

    [TestMethod]
    public void EsValido_TelefonoConPrefijo099_RetornaTrue()
    {
        var validador = new ValidadorTelefonoUruguay();

        var resultado = validador.EsValido("099123456");

        Assert.IsTrue(resultado);
    }

    [TestMethod]
    public void EsValido_TelefonoConPrefijo098_RetornaTrue()
    {
        var validador = new ValidadorTelefonoUruguay();

        var resultado = validador.EsValido("098123456");

        Assert.IsTrue(resultado);
    }
}
