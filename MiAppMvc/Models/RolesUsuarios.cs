using System.ComponentModel.DataAnnotations;

namespace MiAppMvc.Models
{
    public class RolesUsuarios
    {
        [Key]
        public int RoleId { get; set; }
        public string? RolName { get; set; }
    }
}
