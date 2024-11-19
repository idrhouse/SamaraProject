using Microsoft.AspNetCore.Mvc;
using SamaraProject1.Models;
using Microsoft.EntityFrameworkCore;
using SamaraProject1.Recursos;
using Microsoft.AspNetCore.Authorization;
using SamaraProject1.Servicios.Contrato;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;
using System;

namespace SamaraProject1.Controllers
{
    [Authorize]
    public class AdministradorController : Controller
    {
        private readonly IAdministradorService _administradorService;
        private readonly ILogger<AdministradorController> _logger;

        public AdministradorController(IAdministradorService administradorService, ILogger<AdministradorController> logger)
        {
            _administradorService = administradorService;
            _logger = logger;
        }

        [HttpGet]
        public async Task<IActionResult> Lista()
        {
            try
            {
                var lista = await _administradorService.GetAllAdministradores();
                return View(lista);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener la lista de administradores");
                return View("Error");
            }
        }

        [HttpGet]
        public IActionResult Crear()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Crear(Administrador administrador)
        {
            if (!ModelState.IsValid)
            {
                return View(administrador);
            }

            try
            {
                // Validación de correo único
                if (await _administradorService.ExisteAdministradorConCorreo(administrador.Correo))
                {
                    ModelState.AddModelError("Correo", "Ya existe un administrador con ese correo.");
                    return View(administrador);
                }

                // Validación de contraseña mínima de 8 caracteres
                if (administrador.Clave != null && administrador.Clave.Length < 8)
                {
                    ModelState.AddModelError("Clave", "La contraseña debe tener al menos 8 caracteres.");
                    return View(administrador);
                }

                // Validación de confirmación de contraseña
                if (administrador.Clave != administrador.ConfirmarClave)
                {
                    ModelState.AddModelError("ConfirmarClave", "Las contraseñas no coinciden.");
                    return View(administrador);
                }

                administrador.Clave = Utilidades.Cifrar(administrador.Clave);
                await _administradorService.SaveAdministrador(administrador);
                return RedirectToAction(nameof(Lista));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al crear nuevo administrador");
                ModelState.AddModelError("", "Ocurrió un error al guardar el administrador.");
                return View(administrador);
            }
        }

        [HttpGet]
        public async Task<IActionResult> Editar(int id)
        {
            try
            {
                var administrador = await _administradorService.GetAdministradorPorId(id);
                if (administrador == null)
                {
                    return NotFound();
                }
                return View(administrador);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener administrador para editar");
                return View("Error");
            }
        }

        [HttpPost]
        public async Task<IActionResult> Editar(Administrador administrador)
        {
            if (!ModelState.IsValid)
            {
                return View(administrador);
            }

            try
            {
                // Validación de correo único (exceptuando el administrador actual)
                if (await _administradorService.ExisteAdministradorConCorreo(administrador.Correo, administrador.IdAdministrador))
                {
                    ModelState.AddModelError("Correo", "Ya existe un administrador con ese correo.");
                    return View(administrador);
                }

                // Validación de contraseña mínima de 8 caracteres
                if (!string.IsNullOrEmpty(administrador.Clave) && administrador.Clave.Length < 8)
                {
                    ModelState.AddModelError("Clave", "La contraseña debe tener al menos 8 caracteres.");
                    return View(administrador);
                }

                // Validación de confirmación de contraseña
                if (administrador.Clave != administrador.ConfirmarClave)
                {
                    ModelState.AddModelError("ConfirmarClave", "Las contraseñas no coinciden.");
                    return View(administrador);
                }

                if (!string.IsNullOrEmpty(administrador.Clave))
                {
                    administrador.Clave = Utilidades.Cifrar(administrador.Clave);
                }

                await _administradorService.EditarAdministrador(administrador);
                return RedirectToAction(nameof(Lista));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al editar administrador");
                ModelState.AddModelError("", "Ocurrió un error al editar el administrador.");
                return View(administrador);
            }
        }

        [HttpGet]
        public async Task<IActionResult> Eliminar(int id)
        {
            try
            {
                var administrador = await _administradorService.GetAdministradorPorId(id);
                if (administrador == null)
                {
                    return NotFound();
                }
                return View(administrador);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener administrador para eliminar");
                return View("Error");
            }
        }

        [HttpPost, ActionName("Eliminar")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EliminarConfirmado(int id)
        {
            try
            {
                await _administradorService.EliminarAdministrador(id);
                return RedirectToAction(nameof(Lista));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al eliminar administrador");
                return View("Error");
            }
        }
    }
}