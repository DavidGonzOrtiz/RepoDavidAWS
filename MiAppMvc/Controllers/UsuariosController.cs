using MiAppMvc.Data;
using MiAppMvc.Models;
using MiAppMvc.ViewModel;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using System.Threading.Tasks;

namespace MiAppMvc.Controllers
{
    public class UsuariosController : Controller
    {
        private readonly DbContextEventos _dbContextUsuarios;

        public UsuariosController(DbContextEventos dbContextUsuarios)
        {
            _dbContextUsuarios = dbContextUsuarios;
        }
        public async Task<IActionResult> Index()
        {   
            var UsuarioIdstr = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (string.IsNullOrEmpty(UsuarioIdstr) || !Guid.TryParse(UsuarioIdstr, out Guid UsuarioId))
            {
                return RedirectToAction("InicioSesion", "Home");
            }

            var usuarios = await _dbContextUsuarios.Usuarios
                .Where(u => u.UsuarioId == UsuarioId)
                .Include(u => u.EventosUsuarios!)
                    .ThenInclude(eu => eu.Evento)
                        .ThenInclude(e => e.Empresa).FirstOrDefaultAsync();
            var viewModel = new EmpresaUsuario
            {
                NombreUsuario = usuarios?.UserName ?? string.Empty,
                Empresa = null, // si no necesitas una empresa específica, puede quedar nulo
                Usuarios = usuarios,
                ListaEventosUsuarios = usuarios?.EventosUsuarios?.ToList(),
                ListaEventos = usuarios?.EventosUsuarios?
                    .Select(eu => eu.Evento)
                    .Where(e => e != null)
                    .ToList() ?? new List<Eventos>()
            };

            return View(viewModel);
        }

        // EDITAR PERFIL USUARIO
        [HttpGet]
        public async Task<IActionResult> EditarPerfil()
        {
            var usuarioIdStr = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(usuarioIdStr) || !Guid.TryParse(usuarioIdStr, out Guid usuarioId))
                return RedirectToAction("InicioSesion", "Home");

            var usuario = await _dbContextUsuarios.Usuarios.FindAsync(usuarioId);
            if (usuario == null) return NotFound();

            var model = new EditarPerfilViewModel
            {
                UserName = usuario.UserName,
                FirstName = usuario.FirstName,
                SecondName = usuario.SecondName,
            };

            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> EditarPerfil(EditarPerfilViewModel viewModel)
        {
            var usuarioIdStr = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (string.IsNullOrEmpty(usuarioIdStr) || !Guid.TryParse(usuarioIdStr, out Guid usuarioId))
                return RedirectToAction("InicioSesion", "Home");

            var usuario = await _dbContextUsuarios.Usuarios.FindAsync(usuarioId);

            if (usuario == null) return NotFound();

            if (!ModelState.IsValid)
                return View(viewModel);

            usuario.FirstName = viewModel.FirstName;
            usuario.SecondName = viewModel.SecondName;
            usuario.UserName = viewModel.UserName;

            await _dbContextUsuarios.SaveChangesAsync();

            TempData["EXITO"] = "Datos modificados correctamente";
            return View(viewModel);
        }

        // MODIFICAR CONTRASEÑA
        [HttpGet]
        public async Task<IActionResult> ModificarPassword()
        {
            var usuarioIdStr = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(usuarioIdStr) || !Guid.TryParse(usuarioIdStr, out Guid usuarioId))
                return RedirectToAction("InicioSesion", "Home");

            var usuario = await _dbContextUsuarios.Usuarios.FindAsync(usuarioId);
            if (usuario == null) return NotFound();

            var model = new ModificarPasswordViewModel
            {
            };

            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> ModificarPassword(ModificarPasswordViewModel viewModel)
        {
            var usuarioIdStr = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(usuarioIdStr) || !Guid.TryParse(usuarioIdStr, out Guid usuarioId))
                return RedirectToAction("InicioSesion", "Home");

            var usuario = await _dbContextUsuarios.Usuarios.FindAsync(usuarioId);
            if (usuario == null) return NotFound();

            if (ModelState.IsValid)
            {
                if (usuario.Password != viewModel.PasswordActual)
                {
                    TempData["ERROR"] = "La contraseña actual es incorrecta";
                    return View(viewModel);
                }

                if (usuario.Password == viewModel.PasswordNueva)
                {
                    TempData["ERROR"] = "La contraseña actual no puede ser similar a la nueva";
                    return View(viewModel);
                }

                if (!string.IsNullOrEmpty(viewModel.PasswordNueva))
                {
                    usuario.Password = viewModel.PasswordNueva;
                }

                _dbContextUsuarios.Update(usuario);
                await _dbContextUsuarios.SaveChangesAsync();

                TempData["EXITO"] = "Contraseña modificada correctamente";
                return View("EditarPerfil", new EditarPerfilViewModel {
                    UserName = usuario.UserName,
                    FirstName = usuario.FirstName,
                    SecondName = usuario.SecondName
                });
            }

            TempData["ERROR"] = "Error al modificar la contraseña";
            return View(viewModel);
        }

        // INSCRIPCIÓN EVENTO
        public async Task<IActionResult> InscripcionEvento(Guid eventoId, Guid usuarioId)
        {
            var queryComprobarInscripciones = await _dbContextUsuarios.EventosUsuarios
                .FirstOrDefaultAsync(EU => EU.UsuarioId == usuarioId && EU.EventoId == eventoId);

            if (queryComprobarInscripciones != null)
            {
                TempData["ERROR"] = "Ya estás inscrito en este evento";
                return RedirectToAction("VerEventos", "Home");
            }
            var inscripcion = new EventosUsuarios
            {
                EventoId = eventoId,
                UsuarioId = usuarioId
            };

            _dbContextUsuarios.EventosUsuarios.Add(inscripcion);
            _dbContextUsuarios.SaveChanges();

            TempData["EXITO"] = "Te has inscrito correctamente al evento";
            return RedirectToAction("VerEventos", "Home");
        }
    }
}
