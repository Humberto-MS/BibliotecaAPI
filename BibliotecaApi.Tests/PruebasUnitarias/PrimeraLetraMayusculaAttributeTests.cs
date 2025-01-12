using BibliotecaAPI.Validations;
using System.ComponentModel.DataAnnotations;

namespace BibliotecaApi.Tests.PruebasUnitarias {

    [TestClass]
    public sealed class PrimeraLetraMayusculaAttributeTests {

        [TestMethod]
        public void PrimeraLetraMinuscula_DevuelveError() {
            var primera_letra_mayuscula = new PrimeraLetraMayusculaAttribute();
            var valor = "humberto";
            var val_context = new ValidationContext ( new { Nombre = valor } );
            var resultado = primera_letra_mayuscula.GetValidationResult ( valor, val_context );
            Assert.AreEqual ( "La primera letra debe ser mayúscula", resultado.ErrorMessage );
        }

        [TestMethod]
        public void ValorNulo_NoDevuelveError () {
            var primera_letra_mayuscula = new PrimeraLetraMayusculaAttribute ();
            string valor = null;
            var val_context = new ValidationContext ( new { Nombre = valor } );
            var resultado = primera_letra_mayuscula.GetValidationResult ( valor, val_context );
            Assert.IsNull ( resultado );
        }

        [TestMethod]
        public void ValorConPrimeraLetraMayuscula_NoDevuelveError () {
            var primera_letra_mayuscula = new PrimeraLetraMayusculaAttribute ();
            string valor = "Humberto";
            var val_context = new ValidationContext ( new { Nombre = valor } );
            var resultado = primera_letra_mayuscula.GetValidationResult ( valor, val_context );
            Assert.IsNull ( resultado );
        }
    }
}
