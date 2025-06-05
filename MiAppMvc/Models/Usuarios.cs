using System.ComponentModel.DataAnnotations;

namespace MiAppMvc.Models
{
    public class Usuarios
    {
        [Key]
        public Guid UsuarioId { get; set; } = Guid.NewGuid();

        [Required]
        [EmailAddress]
        public string UserName { get; set; }

        [Required]
        [RegularExpression(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[\W_]).{8,}$", ErrorMessage = "La contraseña debe tener al menos 8 caracteres, incluyendo una mayúscula, una minúscula, un número y un carácter especial.")]
        public string Password { get; set; }

        [Required]
        [MaxLength(10)]
        public string FirstName { get; set; }

        [Required]
        [MaxLength(20)]
        public string SecondName { get; set; }

        // Relación muchos a muchos con EventosUsuarios
        public ICollection<EventosUsuarios>? EventosUsuarios { get; set; }

        // Relación con el modelo de RolesUsuarios
        public int RoleId { get; set; }
        public RolesUsuarios? Role { get; set; }
    }
}
