using Dominio;
using Dominio.Excepciones;

namespace Pruebas.Usuarios;

[TestClass]
public class ClavePrueba
{
    [TestMethod]
    public void Validar_ClaveNula_RetornaFalso()
    {
        string? clave = null;

        var esValida = Clave.Validar(clave);

        Assert.IsFalse(esValida);
    }

    [TestMethod]
    public void Validar_ClaveVacia_RetornaFalso()
    {
        var clave = string.Empty;

        var esValida = Clave.Validar(clave);

        Assert.IsFalse(esValida);
    }

    [TestMethod]
    public void Validar_ClaveConSoloEspacios_RetornaFalso()
    {
        var clave = "   ";

        var esValida = Clave.Validar(clave);

        Assert.IsFalse(esValida);
    }

    [TestMethod]
    public void Validar_ClaveMenorA15_RetornaFalso()
    {
        var clave = "Passw!rdAbc9Xy";
        var esValida = Clave.Validar(clave);

        Assert.IsFalse(esValida);
    }

    [TestMethod]
    public void Validar_ClaveMayorA25_RetornaFalso()
    {
        var clave = "Password!Abc9XyzKlmNoPqRst";

        var esValida = Clave.Validar(clave);

        Assert.IsFalse(esValida);
    }

    [TestMethod]
    public void Validar_ClaveSinMayuscula_RetornaFalso()
    {
        var clave = "password!abc9xyz";

        var esValida = Clave.Validar(clave);

        Assert.IsFalse(esValida);
    }

    [TestMethod]
    public void Validar_ClaveSinMinuscula_RetornaFalso()
    {
        var clave = "PASSWORD!ABC9XYZ";

        var esValida = Clave.Validar(clave);

        Assert.IsFalse(esValida);
    }

    [TestMethod]
    public void Validar_ClaveSinNumero_RetornaFalso()
    {
        var clave = "Password!AbcXyz";

        var esValida = Clave.Validar(clave);

        Assert.IsFalse(esValida);
    }

    [TestMethod]
    public void Validar_ClaveSinSimbolo_RetornaFalso()
    {
        var clave = "PasswordAbc9Xyz";

        var esValida = Clave.Validar(clave);

        Assert.IsFalse(esValida);
    }

    [TestMethod]
    public void Validar_ClaveConSecuenciaDeNumeros_RetornaFalso()
    {
        var clave = "Password!Ab12Xy";

        var esValida = Clave.Validar(clave);

        Assert.IsFalse(esValida);
    }

    [TestMethod]
    public void Validar_ClaveDe15Caracteres_RetornaTrue()
    {
        var clave = "Password!Abc9Xy";

        var esValida = Clave.Validar(clave);

        Assert.IsTrue(esValida);
    }

    [TestMethod]
    public void Validar_ClaveDe25Caracteres_RetornaTrue()
    {
        var clave = "Password!Abc9XyzKlmNoPqR";

        var esValida = Clave.Validar(clave);

        Assert.IsTrue(esValida);
    }

    [TestMethod]
    public void Validar_ClaveCorrecta_RetornaTrue()
    {
        var clave = "Password@2Ok6Abc";

        var esValida = Clave.Validar(clave);

        Assert.IsTrue(esValida);
    }

    [TestMethod]
    public void Crear_ClaveNula_LanzaUsuarioExcepcion()
    {
        string? clave = null;

        Assert.ThrowsException<UsuarioExcepcion>(() => Clave.Crear(clave));
    }

    [TestMethod]
    public void Crear_ClaveInvalida_LanzaUsuarioExcepcion()
    {
        var clave = "abc";

        Assert.ThrowsException<UsuarioExcepcion>(() => Clave.Crear(clave));
    }

    [TestMethod]
    public void Crear_ClaveValida_GeneraHashNoVacio()
    {
        var clave = "Password@2Ok6Abc";

        Clave resultado = Clave.Crear(clave);

        Assert.IsNotNull(resultado);
        Assert.IsFalse(string.IsNullOrWhiteSpace(resultado.ObtenerHash()));
    }

    [TestMethod]
    public void Crear_MismaClave_GeneraMismoHash()
    {
        var clave = "Password@2Ok6Abc";

        Clave clave1 = Clave.Crear(clave);
        Clave clave2 = Clave.Crear(clave);

        Assert.AreEqual(clave1.ObtenerHash(), clave2.ObtenerHash());
    }

    [TestMethod]
    public void Crear_DistintaClave_GeneraHashDistinto()
    {
        var clave1 = "Password@2Ok6Abc";
        var clave2 = "Password@3Ok7Abc";

        Clave resultado1 = Clave.Crear(clave1);
        Clave resultado2 = Clave.Crear(clave2);

        Assert.AreNotEqual(resultado1.ObtenerHash(), resultado2.ObtenerHash());
    }

    [TestMethod]
    public void Verificar_ClaveCorrecta_RetornaTrue()
    {
        var claveTexto = "Password@2Ok6Abc";
        Clave clave = Clave.Crear(claveTexto);

        var esCorrecta = clave.Verificar(claveTexto);

        Assert.IsTrue(esCorrecta);
    }

    [TestMethod]
    public void Verificar_ClaveIncorrecta_RetornaFalse()
    {
        var claveTexto = "Password@2Ok6Abc";
        Clave clave = Clave.Crear(claveTexto);

        var esCorrecta = clave.Verificar("Password@3Ok7Abc");

        Assert.IsFalse(esCorrecta);
    }
}
