using System.ComponentModel.DataAnnotations;

namespace BibliotecaAPI.Validations
{
    public class PrimeraLetraMayusculaAttribute: ValidationAttribute
    {
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            if ( value is null || string.IsNullOrEmpty ( value.ToString() ) ) 
                return ValidationResult.Success!;

            var primera_letra = value.ToString()![0].ToString();

            if ( primera_letra != primera_letra.ToUpper() )
                return new ValidationResult ( "La primera letra debe ser mayúscula" );

            return ValidationResult.Success!;
        }
    }
}
