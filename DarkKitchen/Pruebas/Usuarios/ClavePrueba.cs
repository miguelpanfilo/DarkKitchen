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

        bool esValida = Clave.Validar(clave);

        Assert.IsFalse(esValida);
    }

    [TestMethod]
    public void Validar_ClaveVacia_RetornaFalso()
    {
        string clave = string.Empty;

        bool esValida = Clave.Validar(clave);

        Assert.IsFalse(esValida);
    }

    [TestMethod]
    public void Validar_ClaveConSoloEspacios_RetornaFalso()
    {
        string clave = "   ";

        bool esValida = Clave.Validar(clave);

        Assert.IsFalse(esValida);
    }

    [TestMethod]
    public void Validar_ClaveMenorA15_RetornaFalso()
    {
        string clave = "Passw!rdAbc9Xy";

        bool esValida = Clave.Validar(clave);

        Assert.IsFalse(esValida);
    }

    [TestMethod]
    public void Validar_ClaveMayorA25_RetornaFalso()
    {
        string clave = "Password!Abc9XyzKlmNoPqR";

        bool esValida = Clave.Validar(clave);

        Assert.IsFalse(esValida);
    }

    [TestMethod]
    public void Validar_ClaveSinMayuscula_RetornaFalso()
    {
        string clave = "password!abc9xyz";

        bool esValida = Clave.Validar(clave);

        Assert.IsFalse(esValida);
    }

    [TestMethod]
    public void Validar_ClaveSinMinuscula_RetornaFalso()
    {
        string clave = "PASSWORD!ABC9XYZ";

        bool esValida = Clave.Validar(clave);

        Assert.IsFalse(esValida);
    }

    [TestMethod]
    public void Validar_ClaveSinNumero_RetornaFalso()
    {
        string clave = "Password!AbcXyz";

        bool esValida = Clave.Validar(clave);

        Assert.IsFalse(esValida);
    }

    [TestMethod]
    public void Validar_ClaveSinSimbolo_RetornaFalso()
    {
        string clave = "PasswordAbc9Xyz";

        bool esValida = Clave.Validar(clave);

        Assert.IsFalse(esValida);
    }

    [TestMethod]
    public void Validar_ClaveConSecuenciaDeNumeros_RetornaFalso()
    {
        string clave = "Password!Ab12Xy";

        bool esValida = Clave.Validar(clave);

        Assert.IsFalse(esValida);
    }

    [TestMethod]
    public void Validar_ClaveDe15Caracteres_RetornaTrue()
    {
        string clave = "Password!Abc9Xy";

        bool esValida = Clave.Validar(clave);

        Assert.IsTrue(esValida);
    }

    [TestMethod]
    public void Validar_ClaveDe25Caracteres_RetornaTrue()
    {
        string clave = "Password!Abc9XyzKlmNoPq";

        bool esValida = Clave.Validar(clave);

        Assert.IsTrue(esValida);
    }

    [TestMethod]
    public void Validar_ClaveCorrecta_RetornaTrue()
    {
        string clave = "Password@2Ok6Abc";

        bool esValida = Clave.Validar(clave);

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
        string clave = "abc";

        Assert.ThrowsException<UsuarioExcepcion>(() => Clave.Crear(clave));
    }

    [TestMethod]
    public void Crear_ClaveValida_RetornaObjetoClave()
    {
        string clave = "Password@2Ok6Abc";

        Clave resultado = Clave.Crear(clave);

        Assert.IsNotNull(resultado);
        Assert.AreEqual(clave, resultado.Valor);
    }
}
