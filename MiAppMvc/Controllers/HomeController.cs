using System.Diagnostics;
using System.Security.Claims;
using MiAppMvc.Data;
using MiAppMvc.Models;
using MiAppMvc.ViewModel;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace MiAppMvc.Controllers
{
    public class HomeController : Controller
    {
        private readonly DbContextEventos _dbContext;

        public HomeController(DbContextEventos dbContext)
        {
            _dbContext = dbContext;
        }

        public IActionResult Index()
        {
            return View();
        }

        // VER LISTADO DE EVENTOS
        public IActionResult VerEventos()
        {

            ViewBag.UsuarioId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            var queryEventos = _dbContext.Eventos
                .Include(e => e.Empresa)
                .ToList();

            return View(queryEventos);
        }

        // INICIO DE SESIÓN
        [HttpGet]
        public IActionResult InicioSesion()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> InicioSesion(EmpresaUsuario viewModel, string tipoLogin)
        {
            if (tipoLogin == "formEmpresas")
            {
                var queryEmpresas = await _dbContext.Empresas.
                    FirstOrDefaultAsync(e => e.UserName == viewModel.Empresa!.UserName
                    && e.Password == viewModel.Empresa!.Password);

                if (queryEmpresas == null)
                {
                    ViewData["ERROR"] = "Usuario o Contraseña incorrectos";
                    return View("InicioSesion", viewModel);
                }

                var claims = new List<Claim>
                {
                    new Claim(ClaimTypes.Email, queryEmpresas.UserName),
                    new Claim(ClaimTypes.Name, queryEmpresas.Nombre!),
                    new Claim(ClaimTypes.NameIdentifier, queryEmpresas.EmpresaId.ToString()),
                    new Claim("RolId", queryEmpresas.RoleId.ToString())
                };

                var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);

                var authProperties = new AuthenticationProperties
                {
                    IsPersistent = true,
                };

                await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme,
                    new ClaimsPrincipal(claimsIdentity), authProperties);
                return RedirectToAction("Index");
            }

            if (tipoLogin == "formUsuarios")
            {
                var queryUsuarios = await _dbContext.Usuarios.
                    FirstOrDefaultAsync(u => u.UserName == viewModel.Usuarios!.UserName
                    && u.Password == viewModel.Usuarios!.Password);

                if (queryUsuarios == null)
                {
                    ViewData["ERROR"] = "Usuario o Contraseña incorrectos";
                    return View("InicioSesion", viewModel);
                }

                var claims = new List<Claim>
                {
                    new Claim(ClaimTypes.Email, queryUsuarios.UserName),
                    new Claim(ClaimTypes.Name, queryUsuarios.FirstName),
                    new Claim(ClaimTypes.NameIdentifier, queryUsuarios.UsuarioId.ToString()),
                    new Claim("RolId", queryUsuarios.RoleId.ToString())
                };

                var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);

                var authProperties = new AuthenticationProperties
                {
                    IsPersistent = true,
                };

                await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme,
                    new ClaimsPrincipal(claimsIdentity), authProperties);
                return RedirectToAction("Index");
            }

            return View("Index");
        }

        // CERRAR SESIÓN
        [HttpPost]
        public async Task<IActionResult> CerrarSesion()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return RedirectToAction("Index");
        }

        // REGISTRO
        [HttpGet]
        public IActionResult RegistroWeb()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> RegistroWeb(EmpresaUsuario viewModel, string tipoRegistro)
        {
            if (tipoRegistro == "registroEmpresas")
            {
                if (ModelState.IsValid)
                {
                    var queryEmpresas = await _dbContext.Empresas.
                        FirstOrDefaultAsync(e => e.UserName == viewModel.Empresa!.UserName
                        && e.Nombre == viewModel.Empresa!.Nombre);

                    if (queryEmpresas == null)
                    {
                        var nuevaEmpresa = new Empresa
                        {
                            EmpresaId = new Guid(),
                            Nombre = viewModel.Empresa!.Nombre,
                            UserName = viewModel.Empresa!.UserName,
                            Password = viewModel.Empresa!.Password,
                            RoleId = 3
                        };

                        _dbContext.Empresas.Add(nuevaEmpresa);
                        await _dbContext.SaveChangesAsync();

                        TempData["EXITO"] = "Empresa registrada con éxito";
                        return View("InicioSesion");
                    }
                    else
                    {
                        TempData["ERROR"] = "Empresa ya registrada";
                        return View("RegistroWeb", viewModel);
                    }
                }
            }
            else if (tipoRegistro == "registroUsuarios")
            {
                if (ModelState.IsValid)
                {
                    var queryUsuarios = await _dbContext.Usuarios.
                        FirstOrDefaultAsync(u => u.UserName == viewModel.Usuarios!.UserName);

                    if (queryUsuarios == null)
                    {
                        var nuevoUsuario = new Usuarios
                        {
                            UsuarioId = new Guid(),
                            UserName = viewModel.Usuarios!.UserName,
                            Password = viewModel.Usuarios!.Password,
                            RoleId = 1,
                            FirstName = viewModel.Usuarios!.FirstName,
                            SecondName = viewModel.Usuarios!.SecondName
                        };

                        _dbContext.Usuarios.Add(nuevoUsuario);
                        await _dbContext.SaveChangesAsync();

                        TempData["EXITO"] = "Usuario registrado con éxito";
                        return View("InicioSesion");
                    }
                    else
                    {
                        TempData["ERROR"] = "Usuario ya registrado";
                        return View("RegistroWeb", viewModel);
                    }
                }
            }

            TempData["ERROR"] = "Error al registrar en la base de datos";
            return View("RegistroWeb", viewModel);
        }
    }
}
