using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Dominio.Core.Entities;
using Dominio.Core.MainModule;
using Infraestructura.Helpers;

namespace Almacen.Controllers
{
    public class UsuarioController : Controller
    {
        UsuarioManager Usuarios = new UsuarioManager();
        RolManager Roles = new RolManager();


        // GET: Usuario/Registrar
        public ActionResult Registrar()
        {
            ViewBag.roles = new SelectList(Roles.Rol(), "idRol", "tipoRol");
            return View(new tb_Usuario());
        }

        // POST: Registro
        [HttpPost]
        public ActionResult Registrar(tb_Usuario reg)
        {
            if (!ModelState.IsValid)
            {
                ViewBag.roles = new SelectList(Roles.Rol(), "idRol", "tipoRol", reg.idRol);
                return View(reg);
            }

            if (reg.idRol == 1)
                reg.estado = "Pendiente";
            else
                reg.estado = "Aprobado";

            var resultado = Usuarios.Registrar(reg);

            if (!string.IsNullOrEmpty(resultado.Mensaje) && resultado.Mensaje.Contains("Usuario registrado correctamente."))
            {
                if (reg.estado == "Pendiente")
                {
                    // AQUÍ va el código del punto 3:
                    string urlDecidir = Url.Action("Decidir", "Usuario", new { idUsuario = resultado.IdUsuario }, Request.Url.Scheme);
                    CorreoHelper.EnviarSolicitudRevision("stivent456@gmail.com", reg.nombres, reg.login, reg.correo, urlDecidir);

                    CorreoHelper.EnviarAviso(reg.correo, reg.nombres);
                    return RedirectToAction("EnEspera", "Usuario");
                }
                else
                {
                    TempData["Exito"] = "Cuenta creada correctamente.";
                    return RedirectToAction("IniciarSesion", "Usuario");
                }
            }

            ViewBag.mensaje = resultado.Mensaje;
            ViewBag.roles = new SelectList(Roles.Rol(), "idRol", "tipoRol", reg.idRol);
            return View(reg);
        }
        
        // GET: IniciarSesion
        public ActionResult IniciarSesion()
        {
            return View(new IniciarSesion());
        }

        // POST: IniciarSesion
        [HttpPost] public ActionResult IniciarSesion(IniciarSesion model)
        {
            if (!ModelState.IsValid)
                return View(model);

            try
            {
                var usuario = Usuarios.IniciarSesion(model.login, model.password);

                if (usuario == null)
                {
                    ModelState.AddModelError("", "Usuario o contraseña incorrectos.");
                    return View(model);
                }

                // Autenticación correcta: guarda datos en sesión
                Session["usuario"] = usuario;
                TempData["Exito"] = "Has iniciado sesión correctamente.";
                return RedirectToAction("Registrar", "Usuario");
            }
            catch (Exception ex)
            {
                // Si el mensaje es el de cuenta pendiente, muestra el mensaje especial
                if (ex.Message.Contains("Estado Pendiente."))
                {
                    TempData["MensajePendiente"] = "Su cuenta está pendiente de revisión. Si tarda mucho, comuníquese con soporte.";
                    return RedirectToAction("EnEspera", "Usuario");
                }
                else
                {
                    ModelState.AddModelError("", "Usuario o contraseña incorrectos.");
                    return View(model);
                }
            }
        }

        // Usuario/Decidir
        public ActionResult Decidir(int idUsuario)
        {
            var usuario = Usuarios.ObtenerPorId(idUsuario);
            if (usuario == null)
                return HttpNotFound();

            return View(usuario);
        }

        [HttpPost] public ActionResult Decidir(int IdUsuario, string decision)
        {
            var usuario = Usuarios.ObtenerPorId(IdUsuario); // ← Primero obtén los datos

            Usuarios.CambiarEstado(IdUsuario, decision); // ← Luego cambia el estado (o elimina)

            if (decision == "Aprobado")
            {
                CorreoHelper.EnviarConfirmacion(usuario.correo, usuario.nombres);
                ViewBag.Mensaje = "¡Usuario aprobado correctamente!";
                ViewBag.EsExito = true;
            }
            else
            {
                CorreoHelper.EnviarRechazo(usuario.correo, usuario.nombres);
                ViewBag.Mensaje = "El usuario ha sido rechazado.";
                ViewBag.EsExito = false;
            }

            return View("Confirmacion");
        }

        public ActionResult EnEspera()
        {

            return View();
        }

        [HttpGet]
        public ActionResult RecuperarPassword()
        {
            return View();
        }

        [HttpPost]
        public ActionResult RecuperarPassword(RecuperarPassword model)
        {
            if (!ModelState.IsValid)
                return View(model);

            var usuario = Usuarios.BuscarCorreo(model.Correo);

            TempData["Exito"] = "Si el correo ingresado está registrado, recibirá un mensaje con instrucciones para recuperar su contraseña.";

            if (usuario != null)
            {
                try
                {
                    string urlRecuperar = Url.Action("ActualizarPassword", "Usuario", new { id = usuario.IdUsuario }, Request.Url.Scheme);
                    CorreoHelper.EnviarRecuperacion(usuario.correo, urlRecuperar, usuario.nombres);
                }
                catch (Exception ex)
                {
                    ViewBag.Error = "No se pudo enviar el correo: " + ex.Message;
                    return View(model);
                }
            }

            return RedirectToAction("RecuperarPassword", "Usuario");
        }


        // GET: Usuario/ActualizarPassword
        [HttpGet]
        public ActionResult ActualizarPassword(int? id)
        {
            if (id == null)
                return RedirectToAction("RecuperarPassword", "Usuario");

            TempData["idUserRecuperar"] = id.Value;
            return View();
        }

        // POST: Usuario/ActualizarPassword
        [HttpPost]
        public ActionResult ActualizarPassword(string nuevaPassword)
        {
            if (TempData["idUserRecuperar"] == null)
                return RedirectToAction("ActualizarPassword");

            int idUser = (int)TempData["idUserRecuperar"];

            if (string.IsNullOrWhiteSpace(nuevaPassword))
            {
                ViewBag.Error = "Debe ingresar una nueva contraseña.";
                return View();
            }

            Usuarios.ActualizarPassword(idUser, nuevaPassword);

            TempData["Exito"] = "¡Contraseña actualizada correctamente!";
            return RedirectToAction("IniciarSesion", "Usuario");
        }



    }
}
