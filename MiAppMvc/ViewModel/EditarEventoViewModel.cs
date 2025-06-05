using MiAppMvc.Models;
using System.ComponentModel.DataAnnotations;

namespace MiAppMvc.ViewModel
{
    public class EditarEventoViewModel
    {
        public Guid EventoId { get; set; }

        [Required]
        [StringLength(10)]
        public string EventoName { get; set; }

        [Required]
        public DateTime FechaInicio { get; set; }

        [Required]
        public DateTime FechaFin { get; set; }

        [Required]
        [StringLength(30)]
        public string Description { get; set; }
    }

}
