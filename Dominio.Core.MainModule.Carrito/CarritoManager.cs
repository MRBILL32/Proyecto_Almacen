using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Infraestructura.Data.SqlServer.Carrito;
using Dominio.Core.Entities.Carrito;

namespace Dominio.Core.MainModule.Carrito
{
    public class CarritoManager
    {
        Carrito_DAL carrito = new Carrito_DAL();

        public int CrearCarrito(int idUser) 
        {
            return carrito.CrearCarrito(idUser);
        }

        public string AgregarProductoCarrito(int idCarrito, int idProd, int cantidad) 
        {
            return carrito.AgregarProductoCarrito(idCarrito, idProd, cantidad);
        }

        public List<Tb_DetalleCarrito> ListarCarrito(int idCarrito) 
        {
            return carrito.ListarCarrito(idCarrito);
        }

        public void EliminarProductoCarrito(int idCarrito, int idProd) 
        {
            carrito.EliminarProductoCarrito(idCarrito, idProd);
        }

        public void VaciarCarrito(int idCarrito)
        {
            carrito.VaciarCarrito(idCarrito);
        }

        public string Efectuarcompra(int idCarrito, int idUser) 
        {
            return carrito.EfectuarCompra(idCarrito, idUser);
        }

    }
}
