using MiAppMvc.Data;
using MiAppMvc.Models;
using MiAppMvc.ViewModel;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using System.Threading.Tasks;

namespace MiAppMvc.Controllers
{
    public class EmpresaController : Controller
    {
        private readonly DbContextEventos _dbContextEventos;

        public EmpresaController(DbContextEventos dbContextEventos)
        {
            _dbContextEventos = dbContextEventos;
        }
        public async Task<IActionResult> Index()
        {
            var empresaIdStr = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (string.IsNullOrEmpty(empresaIdStr) || !Guid.TryParse(empresaIdStr, out Guid empresaId))
            {
                return RedirectToAction("InicioSesion", "Home");
            }

            // Obtener eventos y usuarios relacionados
            var eventos = await _dbContextEventos.Eventos
                .Where(e => e.EmpresaId == empresaId)
                .Include(e => e.EventosUsuarios!)
                    .ThenInclude(eu => eu.Usuario)
                    .ToListAsync();

            // Crear el ViewModel
            var viewModel = new EmpresaUsuario
            {
                NombreUsuario = User.Identity!.Name!,
                ListaEventos = eventos,
                ListaEventosUsuarios = eventos
                    .SelectMany(e => e.EventosUsuarios!)
                    .ToList()
            };

            return View(viewModel);
        }

        // AGREGAR EVENTO
        [HttpGet]
        public IActionResult AgregarEvento()
        {
            ViewBag.CategoriesList = Enum.GetValues(typeof(Categories))
                .Cast<Categories>()
                .Select(c => new SelectListItem
                {
                    Text = c.ToString(),
                    Value = ((int)c).ToString()
                }).ToList();

            return View();
        }

        [HttpPost]
        public async Task<IActionResult> AgregarEvento(Eventos evento)
        {
            if (ModelState.IsValid)
            {
                var empresaIdStr = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

                if (!Guid.TryParse(empresaIdStr, out Guid empresaId))
                {
                    return RedirectToAction("InicioSesion", "Home");
                }

                var nuevoEvento = new Eventos
                {
                    EventoId = Guid.NewGuid(),
                    EventoName = evento.EventoName,
                    Description = evento.Description,
                    EmpresaId = empresaId,
                    FechaInicio = evento.FechaInicio,
                    FechaFin = evento.FechaFin,
                    Categories = evento.Categories
                };

                _dbContextEventos.Eventos.Add(nuevoEvento);
                await _dbContextEventos.SaveChangesAsync();

                TempData["EXITO"] = "Evento creado correctamente";
                return View("AgregarEvento");
            }

            TempData["ERROR"] = "El evento no se pudo crear";
            return View("AgregarEvento");
        }

        // CANCELAR EVENTO
        [HttpPost]
        public async Task<IActionResult> BorrarEvento(Guid eventoId)
        {
            var queryEventos = await _dbContextEventos.Eventos.Include(e => e.EventosUsuarios)
                .FirstOrDefaultAsync(e => e.EventoId == eventoId);

            if (queryEventos == null)
            {
                TempData["ERROR"] = "Evento no encontrado";
                return RedirectToAction("Index");
            }

            _dbContextEventos.EventosUsuarios.RemoveRange(queryEventos.EventosUsuarios!);

            _dbContextEventos.Eventos.Remove(queryEventos);
            await _dbContextEventos.SaveChangesAsync();

            TempData["EXITO"] = "Evento eliminado correctamente";
            return RedirectToAction("Index");
        }

        // EDITAR EVENTO
        [HttpGet]
        public async Task<IActionResult> EditarEvento(Guid eventoId)
        {
            var evento = await _dbContextEventos.Eventos
                .FirstOrDefaultAsync(e => e.EventoId == eventoId);

            if (evento == null)
            {
                TempData["ERROR"] = "Evento no encontrado";
                return RedirectToAction("Index");
            }

            var viewModel = new EditarEventoViewModel
            {
                EventoId = evento.EventoId,
                EventoName = evento.EventoName,
                FechaInicio = evento.FechaInicio,
                FechaFin = evento.FechaFin,
                Description = evento.Description
            };

            return View(viewModel);
        }

        [HttpPost]
        public async Task<IActionResult> EditarEvento(EditarEventoViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var evento = await _dbContextEventos.Eventos.FirstOrDefaultAsync(e => e.EventoId == model.EventoId);

            if (evento == null)
            {
                TempData["ERROR"] = "Evento no encontrado";
                return RedirectToAction("Index");
            }

            evento.EventoName = model.EventoName;
            evento.FechaInicio = model.FechaInicio;
            evento.FechaFin = model.FechaFin;
            evento.Description = model.Description;

            await _dbContextEventos.SaveChangesAsync();

            TempData["EXITO"] = "Evento actualizado correctamente";
            return RedirectToAction("Index");
        }
    }
}
