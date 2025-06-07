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
        [HttpPost]
        public ActionResult IniciarSesion(IniciarSesion model)
        {
            if (!ModelState.IsValid)
                return View(model);

            // Aquí no se hashea la contraseña
            var usuario = Usuarios.IniciarSesion(model.login, model.password);

            if (usuario == null)
            {
                ModelState.AddModelError("", "Usuario o contraseña incorrectos.");
                return View(model);
            }

            // Autenticación correcta: guarda datos en sesión
            Session["usuario"] = usuario;

            // Redirige a la acción Registrar del UsuarioController
            TempData["Exito"] = "Has iniciado sesión correctamente.";
            return RedirectToAction("Registrar", "Usuario");
        }

        public ActionResult EnEspera()
        {
            return View();
        }

        // Usuario/Decidir
        [HttpGet]
        public ActionResult Decidir(int idUsuario)
        {
            var usuario = Usuarios.ObtenerPorId(idUsuario);
            if (usuario == null)
                return HttpNotFound();

            return View(usuario);
        }

        [HttpPost]
        public ActionResult Decidir(int IdUsuario, string decision)
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
                CorreoHelper.EnviarAviso(usuario.correo, usuario.nombres);
                ViewBag.Mensaje = "El usuario ha sido rechazado.";
                ViewBag.EsExito = false;
            }

            return View("Confirmacion");
        }
    }
}
