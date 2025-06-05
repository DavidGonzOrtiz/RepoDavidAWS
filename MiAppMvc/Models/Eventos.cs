using System.ComponentModel.DataAnnotations;

namespace MiAppMvc.Models
{
    public enum Categories
    {
        Informatica = 0,
        Deportes = 1,
        Videojuegos = 2,
        Lectura = 3,
        Ciencias = 4
    }
    public class Eventos
    {
        [Key]
        public Guid EventoId { get; set; } = Guid.NewGuid();

        [Required]
        [MaxLength(10)]
        public string EventoName { get; set; }

        [Required]
        public DateTime FechaInicio { get; set; }

        [Required]
        public DateTime FechaFin { get; set; }

        [Required]
        [MaxLength(30)]
        public string Description { get; set; }

        // Relación muchos a muchos con EventosUsuarios
        public ICollection<EventosUsuarios>? EventosUsuarios { get; set; }

        // Foreign key
        public Guid EmpresaId { get; set; }

        // Navegación inversa
        public Empresa? Empresa { get; set; }

        public Categories Categories { get; set; }
    }
}
