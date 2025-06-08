using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Dominio.Core.MainModule.Producto;

namespace Almacen.Controllers
{
    public class ProductoController : Controller
    {
        ProductoManager productos = new ProductoManager();
        CategoriaManager categorias = new CategoriaManager();

        public ActionResult ProductosAdmin(int numeropagina = 1)
        {
            if (Session["idRol"] == null || Convert.ToInt32(Session["idRol"]) != 1)
                return RedirectToAction("IniciarSesion", "Usuario");

            int registrosPorPagina = 12;
            int totalPaginas;
            var productosPagina = productos.ListarProductosAdminPaginado(numeropagina, registrosPorPagina, out totalPaginas);

            // Validar que la página esté en rango
            if (numeropagina < 1) numeropagina = 1;
            if (numeropagina > totalPaginas) numeropagina = totalPaginas;

            ViewBag.NumeroPagina = numeropagina;
            ViewBag.TotalPaginas = totalPaginas;

            return View("ProductosAdmin", productosPagina);
        }
        public ActionResult ProductosUsuario(int numeropagina = 1)
        {
            if (Session["idRol"] == null || Convert.ToInt32(Session["idRol"]) != 2)
                return RedirectToAction("IniciarSesion", "Usuario");

            int registrosPorPagina = 12;
            int totalPaginas;
            var productosPagina = productos.ListarProductosPaginado(numeropagina, registrosPorPagina, out totalPaginas);

            // Validar que la página esté en rango
            if (numeropagina < 1) numeropagina = 1;
            if (numeropagina > totalPaginas) numeropagina = totalPaginas;

            ViewBag.NumeroPagina = numeropagina;
            ViewBag.TotalPaginas = totalPaginas;

            return View("ProductosUsuario", productosPagina);
        }
    }
}
