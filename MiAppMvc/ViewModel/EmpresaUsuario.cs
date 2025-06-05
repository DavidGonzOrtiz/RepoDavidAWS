using MiAppMvc.Models;

namespace MiAppMvc.ViewModel
{
    public class EmpresaUsuario
    {
        public string NombreUsuario { get; set; } = string.Empty;
        public Empresa? Empresa { get; set; }
        public Usuarios? Usuarios { get; set; }
        public RolesUsuarios? RolesUsuarios { get; set; }
        public List<Eventos>? ListaEventos { get; set; }
        public List<EventosUsuarios>? ListaEventosUsuarios { get; set; }

    }
}
