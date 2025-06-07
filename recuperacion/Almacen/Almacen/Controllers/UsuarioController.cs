using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Dominio.Core.Entities;
using Dominio.Core.MainModule;

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

            // Se guarda la contraseña tal cual sin hashear ni modificar
            string mensaje = Usuarios.Registrar(reg);
            if (mensaje == "Usuario registrado correctamente.")
            {
                TempData["Exito"] = "Cuenta creada correctamente.";
                return RedirectToAction("IniciarSesion", "Usuario");
            }

            ViewBag.mensaje = mensaje;
            ViewBag.roles = new SelectList(Roles.Rol(), "idRol", "tipoRol", reg.idRol);
            return View(reg);
        }

        // GET: IniciarSesion
        public ActionResult IniciarSesion()
        {
            return View(new IniciarSesion());
        }

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
    }
}
