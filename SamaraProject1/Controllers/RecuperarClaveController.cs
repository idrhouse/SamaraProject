using Microsoft.AspNetCore.Mvc;
using SamaraProject1.Models;
using SamaraProject1.Servicios.Contrato;
using Microsoft.Extensions.Configuration;
using System;
using System.Threading.Tasks;
using MimeKit;
using MailKit.Net.Smtp;
using SamaraProject1.Recursos;
using Microsoft.Extensions.Logging;

namespace SamaraProject1.Controllers
{
    public class RecuperarClaveController : Controller
    {
        private readonly IAdministradorService _administradorService;
        private readonly IConfiguration _configuration;
        private readonly ILogger<RecuperarClaveController> _logger;

        public RecuperarClaveController(IAdministradorService administradorService, IConfiguration configuration, ILogger<RecuperarClaveController> logger)
        {
            _administradorService = administradorService;
            _configuration = configuration;
            _logger = logger;
        }

        [HttpGet]
        public IActionResult RecuperarClave()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> RecuperarClave(RecuperarClaveModel modelo)
        {
            if (!ModelState.IsValid)
            {
                return View(modelo);
            }

            try
            {
                var administrador = await _administradorService.GetAdministradorPorCorreo(modelo.Correo);
                if (administrador == null)
                {
                    ModelState.AddModelError("", "Correo no encontrado.");
                    return View(modelo);
                }

                // Generar token y preparar correo
                var token = Guid.NewGuid().ToString();
                administrador.TokenRecuperacion = token;
                administrador.TokenExpiracion = DateTime.UtcNow.AddDays(1);

                var updateResult = await _administradorService.ActualizarAdministrador(administrador);
                if (!updateResult)
                {
                    throw new Exception("No se pudo actualizar el token de recuperación.");
                }

                var enlace = Url.Action("RestablecerClave", "RecuperarClave", new { token }, Request.Scheme);
                await EnviarCorreoRecuperacion(modelo.Correo, enlace);

                // Mensaje de éxito
                TempData["SuccessMessage"] = "Se ha enviado un enlace de recuperación a tu correo electrónico.";
                return RedirectToAction("RecuperarClave");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error en la recuperación de contraseña: {Mensaje}", ex.Message);
                TempData["ErrorMessage"] = "Ocurrió un error al procesar la solicitud. Por favor, inténtalo de nuevo.";
                return RedirectToAction("RecuperarClave");
            }
        }



        [HttpGet]
        public IActionResult RestablecerClave(string token)
        {
            if (string.IsNullOrEmpty(token))
            {
                return RedirectToAction("IniciarSesion", "Inicio");
            }

            var modelo = new RestablecerClaveModel { Token = token };
            return View(modelo);
        }

        [HttpPost]
        public async Task<IActionResult> RestablecerClave(RestablecerClaveModel modelo)
        {
            if (!ModelState.IsValid)
            {
                return View(modelo);
            }

            try
            {
                var administrador = await _administradorService.GetAdministradorPorToken(modelo.Token);
                if (administrador == null)
                {
                    ModelState.AddModelError("", "Token inválido o expirado.");
                    return View(modelo);
                }

                // Verificar si el token ha expirado
                if (administrador.TokenExpiracion < DateTime.Now)
                {
                    ModelState.AddModelError("", "El token ha expirado.");
                    return View(modelo);
                }

                if (modelo.NuevaClave.Length < 8)
                {
                    ModelState.AddModelError("NuevaClave", "La nueva contraseña debe tener al menos 8 caracteres.");
                    return View(modelo);
                }

                administrador.Clave = Utilidades.Cifrar(modelo.NuevaClave);
                administrador.TokenRecuperacion = null;  // Limpiar el token
                administrador.TokenExpiracion = null;   // Limpiar la fecha de expiración

                var updateResult = await _administradorService.ActualizarAdministrador(administrador);

                if (!updateResult)
                {
                    throw new Exception("No se pudo actualizar la contraseña.");
                }

                TempData["SuccessMessage"] = "Contraseña actualizada correctamente.";
                return RedirectToAction("IniciarSesion", "Inicio");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error en el proceso de restablecimiento de contraseña para el token: {Token}", modelo.Token);
                ModelState.AddModelError("", "Ocurrió un error al restablecer la contraseña. Por favor, inténtelo de nuevo más tarde.");
                return View(modelo);
            }
        }


        private async Task EnviarCorreoRecuperacion(string correoDestino, string enlaceRecuperacion)
        {
            try
            {
                var smtpSettings = _configuration.GetSection("SmtpSettings").Get<SmtpSettings>();

                var message = new MimeMessage();
                message.From.Add(new MailboxAddress(smtpSettings.SenderName, smtpSettings.SenderEmail));
                message.To.Add(new MailboxAddress("", correoDestino));
                message.Subject = "Recuperación de Contraseña";
                message.Body = new TextPart("html")
                {
                    Text = $@"
            <h2>Recuperación de Contraseña</h2>
            <p>Hola,</p>
            <p>Puedes restablecer tu contraseña utilizando el siguiente enlace:</p>
            <p><a href='{enlaceRecuperacion}'>Restablecer Contraseña</a></p>
            <p>Si no solicitaste este cambio, por favor ignora este correo.</p>
            <p>Saludos,<br>Equipo de Soporte</p>"
                };

                using var client = new SmtpClient();

                // Ignorar errores de certificado solo si es necesario
                client.ServerCertificateValidationCallback = (sender, certificate, chain, sslPolicyErrors) => true;

                await client.ConnectAsync(smtpSettings.Server, smtpSettings.Port, MailKit.Security.SecureSocketOptions.StartTls);
                await client.AuthenticateAsync(smtpSettings.Username, smtpSettings.Password);
                await client.SendAsync(message);
                await client.DisconnectAsync(true);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al enviar correo de recuperación a: {CorreoDestino}", correoDestino);
                throw;
            }
        }

    }
}