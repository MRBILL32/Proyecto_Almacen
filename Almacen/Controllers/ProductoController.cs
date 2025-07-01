using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Dominio.Core.Entities.Producto;
using Dominio.Core.MainModule.Carrito;
using Dominio.Core.MainModule.Producto;
using Infraestructura.Data.SqlServer.Producto;

namespace Almacen.Controllers
{
    public class ProductoController : Controller
    {
        ProductoManager productos = new ProductoManager();
        CategoriaManager categorias = new CategoriaManager();
        CarritoManager carrito = new CarritoManager();

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

        public ActionResult Buscar(string busqueda, int numeropagina = 1)
        {
            var producto = new ProductoManager();
            int registrosPorPagina = 12;
            int totalPaginas;

            if (numeropagina < 1) numeropagina = 1;

            bool busquedaIgnorada = false;

            // Evitar búsqueda por ID numérico
            if (int.TryParse(busqueda, out int _))
            {
                busqueda = ""; // O cualquier valor que no afecte la búsqueda
                busquedaIgnorada = true;
            }

            // Usar Session para determinar rol
            int? idRol = Session["idRol"] as int?;
            IEnumerable<Tb_Producto> productosPagina;

            if (idRol == 1) // Asumiendo 1 es Admin
            {
                productosPagina = producto.BuscarParaAdmin(busqueda, numeropagina, registrosPorPagina, out totalPaginas);
            }
            else
            {
                productosPagina = producto.BuscarParaUsuario(busqueda, numeropagina, registrosPorPagina, out totalPaginas);
            }

            if (totalPaginas == 0) totalPaginas = 1;
            if (numeropagina > totalPaginas) numeropagina = totalPaginas;

            ViewBag.NumeroPagina = numeropagina;
            ViewBag.TotalPaginas = totalPaginas;
            ViewBag.Busqueda = busqueda;

            if (!productosPagina.Any())
                ViewBag.Mensaje = "Artículo no encontrado";

            if (busquedaIgnorada)
                ViewBag.Alerta = "La búsqueda por número ID no está permitida y fue ignorada.";

            if (idRol == 1)
                return View("ProductosAdmin", productosPagina);
            else
                return View("ProductosUsuario", productosPagina);
        }
        
        [HttpPost]
        public ActionResult Eliminar(int idProd)
        {
            productos.EliminarProducto(idProd);
            return RedirectToAction("ProductosAdmin");
        }

        //agregar producto al carrito
        public ActionResult Comprar(int id)
        {
            var producto = productos.BuscarProductoPorId(id); // actualizado

            if (producto == null)
            {
                return HttpNotFound("Producto no encontrado");
            }

            return View("Comprar", producto);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Comprar(int idProd, int cantidad)
        {
            if (Session["usuario"] == null)
                return RedirectToAction("IniciarSesion", "Usuario");

            dynamic usuario = Session["usuario"];
            int idUser = usuario.idUser;

            int idCarrito = carrito.CrearCarrito(idUser);
            string mensaje = carrito.AgregarProductoCarrito(idCarrito, idProd, cantidad);

            TempData["Mensaje"] = mensaje;

            // Redirige a la misma página o a la lista de productos para seguir comprando
            return RedirectToAction("ProductosUsuario");
        }


    }
}
