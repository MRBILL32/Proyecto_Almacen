using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Dominio.Core.Entities.Producto;
using Infraestructura.Data.SqlServer.Producto;

namespace Dominio.Core.MainModule.Producto
{
    public class ProductoManager
    {
        Producto_DAL producto = new Producto_DAL();

        public IEnumerable<Tb_Producto> ListarProductos()
        {
            return producto.ListarProductos();
        }

        public List<Tb_Producto> ListarProductosPaginado(int pagina, int tamanoPagina, out int totalPaginas)
        {
            var todosLosProductos = ListarProductos().ToList();
            int totalRegistros = todosLosProductos.Count;
            totalPaginas = (int)Math.Ceiling((double)totalRegistros / tamanoPagina);

            var productosPagina = todosLosProductos
                .Skip((pagina - 1) * tamanoPagina)
                .Take(tamanoPagina)
                .ToList();

            // Asignar imagen a cada producto
            foreach (var prod in productosPagina)
            {
                // Convierte el nombre a minúsculas y reemplaza espacios por guiones
                string nombreImagen = prod.NomProd.ToLower().Replace(" ", "-") + ".jpg";
                string ruta = $"/Content/Fotos/{nombreImagen}";
                string rutaFisica = System.Web.HttpContext.Current.Server.MapPath(ruta);

                if (System.IO.File.Exists(rutaFisica))
                    prod.ImagenUrl = ruta;
                else
                    prod.ImagenUrl = "/Content/Fotos/default.jpg";
            }

            return productosPagina;
        }

        public IEnumerable<Tb_Producto> ListarProductoAdmin()
        {
            return producto.ListaProducAdmin();
        }

        public List<Tb_Producto> ListarProductosAdminPaginado(int pagina, int tamanoPagina, out int totalPaginas)
        {
            var todosLosProductos = ListarProductoAdmin().ToList();
            int totalRegistros = todosLosProductos.Count;
            totalPaginas = (int)Math.Ceiling((double)totalRegistros / tamanoPagina);

            var productosPagina = todosLosProductos
                .Skip((pagina - 1) * tamanoPagina)
                .Take(tamanoPagina)
                .ToList();

            // Asignar imagen a cada producto (opcional, si usas imágenes)
            foreach (var prod in productosPagina)
            {
                // Convierte el nombre a minúsculas y reemplaza espacios por guiones
                string nombreImagen = prod.NomProd.ToLower().Replace(" ", "-") + ".jpg";
                string ruta = $"/Content/Fotos/{nombreImagen}";
                string rutaFisica = System.Web.HttpContext.Current.Server.MapPath(ruta);

                if (System.IO.File.Exists(rutaFisica))
                    prod.ImagenUrl = ruta;
                else
                    prod.ImagenUrl = "/Content/Fotos/default.jpg";
            }

            return productosPagina;
        }


        public void EliminarProducto(int idProd) 
        {
            producto.EliminarProducto(idProd);
        }

        public string ActualizarProducto(Tb_Producto productos)
        {
            return producto.ActualizarProducto(productos);
        }

    }
}
