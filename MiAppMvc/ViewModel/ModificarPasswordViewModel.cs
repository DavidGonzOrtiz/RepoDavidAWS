using System.ComponentModel.DataAnnotations;

namespace MiAppMvc.ViewModel
{
    public class ModificarPasswordViewModel : IValidatableObject
    {
        [DataType(DataType.Password)]
        [Display(Name = "Contraseña actual")]
        public string PasswordActual { get; set; } // NO requerido siempre

        [DataType(DataType.Password)]
        [Display(Name = "Nueva contraseña")]
        [RegularExpression(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[\W_]).{8,}$",
            ErrorMessage = "La nueva contraseña debe tener al menos 8 caracteres, incluyendo mayúscula, minúscula, número y símbolo.")]
        public string PasswordNueva { get; set; } // NO requerido siempre

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            // Si quiere cambiar la contraseña, tiene que ingresar la actual sí o sí
            if (!string.IsNullOrEmpty(PasswordNueva) && string.IsNullOrEmpty(PasswordActual))
            {
                yield return new ValidationResult(
                    "Debe ingresar la contraseña actual para cambiar la contraseña.",
                    new[] { nameof(PasswordActual) });
            }
        }
    }
}
