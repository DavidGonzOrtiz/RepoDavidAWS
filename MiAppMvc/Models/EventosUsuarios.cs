using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MiAppMvc.Models
{
    public class EventosUsuarios
    {
        [Key]
        [Column(Order = 1)]
        public Guid UsuarioId { get; set; }
        public Usuarios Usuario { get; set; }

        [Key]
        [Column(Order = 2)]
        public Guid EventoId { get; set; }
        public Eventos Evento { get; set; }
    }
}
