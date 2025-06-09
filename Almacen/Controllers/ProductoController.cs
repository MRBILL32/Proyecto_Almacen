using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Dominio.Core.Entities.Producto;
using Dominio.Core.MainModule.Producto;
using Infraestructura.Data.SqlServer.Producto;

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

        // GET
        public ActionResult InsertarProducto() 
        {
            //cargar las categorias para el dropdown
            ViewBag.Categorias = new SelectList(categorias.ListarCategorias(), "IdCate", "NomCate");
            return View();
        }

        //POST
        [HttpPost]
        public ActionResult InsertarProducto(Tb_Producto producto) 
        {
            if (ModelState.IsValid) 
            {
                //asignar imagen por defecto
                producto.ImagenUrl = "/Content/Fotos/default.jpg";

                //llama al DAL para Agregar el producto
                var productos = new Producto_DAL();
                string mensaje = productos.InsertarProducto(producto);

                TempData["Mensaje"] = mensaje;
                return RedirectToAction("ProductosAdmin");
            }

            //si hay error, recarga las categorias para el dropdown
            ViewBag.Categorias = new SelectList(categorias.ListarCategorias(), "IdCate", "NomCate");
            return View(producto);
        }

        public ActionResult Editar(int id)
        {
            var producto = productos.ListarProductoAdmin().FirstOrDefault(p => p.IdProd == id);
            if (producto == null)
                return HttpNotFound();

            // Lógica para la imagen basada en el nombre del producto
            string nombreImagen = producto.NomProd.ToLower().Replace(" ", "-") + ".jpg";
            string rutaRelativa = "/Content/Fotos/" + nombreImagen;
            string rutaFisica = Server.MapPath(rutaRelativa);

            producto.ImagenUrl = (producto.Stock == 0)
                ? "/Content/Fotos/default.jpg"
                : (System.IO.File.Exists(rutaFisica) ? rutaRelativa : "/Content/Fotos/default.jpg");

            ViewBag.Categorias = new SelectList(
                categorias.ListarCategorias(), "IdCate", "NomCate", producto.IdCate
            );

            return View(producto);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Editar(Tb_Producto producto)
        {
            if (ModelState.IsValid)
            {
                string mensaje = productos.ActualizarProducto(producto);
                TempData["Mensaje"] = mensaje;
                return RedirectToAction("ProductosAdmin");
            }

            // Si hay error, recarga categorías para la vista
              ViewBag.Categorias = new SelectList(categorias.ListarCategorias(), "IdCate", "NomCate");

            return View(producto);
        }


        public ActionResult Buscar(string busqueda)
        {
            var producto = new ProductoManager();
            IEnumerable<Tb_Producto> resultado;

            if (User.IsInRole("Admin"))
            {
                resultado = producto.BuscarParaAdmin(busqueda);
                return View("ProductosAdmin", resultado);
            }
            else
            {
                resultado = producto.BuscarParaUsuario(busqueda);
                return View("ProductosUsuario", resultado); // O el nombre de tu vista de usuario
            }
        }

        [HttpPost]
        public ActionResult Eliminar(int idProd)
        {
            productos.EliminarProducto(idProd);
            return RedirectToAction("ProductosAdmin");
        }



    }
}
