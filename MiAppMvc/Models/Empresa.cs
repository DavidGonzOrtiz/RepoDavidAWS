using System.ComponentModel.DataAnnotations;

namespace MiAppMvc.Models
{
    
    public class Empresa
    {
        [Key]
        public Guid EmpresaId { get; set; }

        [Required]
        public string Nombre { get; set; }

        [Required]
        [EmailAddress]
        public string UserName { get; set; }

        [Required]
        [RegularExpression(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[\W_]).{8,}$", ErrorMessage = "La contraseña debe tener al menos 8 caracteres, incluyendo una mayúscula, una minúscula, un número y un carácter especial.")]
        public string Password { get; set; }

        // Relación uno a muchos
        public ICollection<Eventos> Eventos { get; set; } = new List<Eventos>();

        // Relación con el modelo de RolesUsuarios
        public int RoleId { get; set; }
        public RolesUsuarios? Role { get; set; }        
    }
}
