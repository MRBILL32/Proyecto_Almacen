using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Dominio.Core.MainModule.Carrito;

namespace Almacen.Controllers
{
    public class CarritoController : Controller
    {
        CarritoManager carritos = new CarritoManager();

        // Mostrar carrito
        public ActionResult Carrito()
        {
            if (Session["usuario"] == null)
                return RedirectToAction("IniciarSesion", "Usuario");

            dynamic usuario = Session["usuario"];
            int idUser = usuario.IdUsuario;
            int idCarrito = carritos.CrearCarrito(idUser);
            var productos = carritos.ListarCarrito(idCarrito);

            return View(productos);
        }

        // Agregar producto al carrito
        [HttpPost]
        public ActionResult Agregar(int idProd, int cantidad)
        {
            if (Session["usuario"] == null)
                return RedirectToAction("IniciarSesion", "Usuario");

            dynamic usuario = Session["usuario"];
            int idUser = usuario.IdUsuario;

            int idCarrito = carritos.CrearCarrito(idUser);
            string mensaje = carritos.AgregarProductoCarrito(idCarrito, idProd, cantidad);

            TempData["Mensaje"] = mensaje;
            return RedirectToAction("ProductosUsuario", "Producto");
        }

        // Eliminar producto del carrito
        [HttpPost]
        public ActionResult Eliminar(int idProd)
        {
            if (Session["usuario"] == null)
                return RedirectToAction("IniciarSesion", "Usuario");

            dynamic usuario = Session["usuario"];
            int idUser = usuario.IdUsuario;

            int idCarrito = carritos.CrearCarrito(idUser);
            carritos.EliminarProductoCarrito(idCarrito, idProd);

            TempData["Mensaje"] = "Producto eliminado correctamente.";
            return RedirectToAction("Carrito");
        }

        // Vaciar carrito
        [HttpPost] public ActionResult Vaciar()
        {
            if (Session["usuario"] == null)
                return RedirectToAction("IniciarSesion", "Usuario");

            dynamic usuario = Session["usuario"];
            int idUser = usuario.IdUsuario;

            int idCarrito = carritos.CrearCarrito(idUser);
            carritos.VaciarCarrito(idCarrito);

            TempData["Mensaje"] = "Carrito vaciado correctamente.";
            return RedirectToAction("ProductosUsuario", "Producto");
        }

        //Efectuar Compra
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult ConfirmarCompra()
        {
            if (Session["usuario"] == null)
                return RedirectToAction("IniciarSesion", "Usuario");

            dynamic usuario = Session["usuario"];
            int idUser = usuario.IdUsuario;
            int idCarrito = carritos.CrearCarrito(idUser);

            string mensaje = carritos.Efectuarcompra(idCarrito, idUser);

            TempData["Mensaje"] = mensaje;
            return RedirectToAction("ProductosUsuario", "Producto"); // O la vista que muestre el carrito o historial
        }

    }
}