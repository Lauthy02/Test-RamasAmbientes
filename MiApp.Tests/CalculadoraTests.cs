using Microsoft.VisualStudio.TestTools.UnitTesting;
using MiApp.App; // Importa el namespace de tu aplicación

namespace MiApp.Tests;

[TestClass]
public class CalculadoraTests
{
    [TestMethod]
    public void Sumar_DosNumeros_RetornaSumaCorrecta()
    {
        // Arrange: Prepara el escenario
        var calculadora = new Calculadora();
        int a = 2;
        int b = 3;
        int esperado = 5;

        // Act: Ejecuta el método a probar
        int resultado = calculadora.Sumar(a, b);

        // Assert: Verifica el resultado
        Assert.AreEqual(esperado, resultado, "Error: 2 + 3 debería ser 5");
    }
}