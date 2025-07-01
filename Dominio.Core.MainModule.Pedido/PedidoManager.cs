using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Dominio.Core.Entities.Pedido;
using Infraestructura.Data.SqlServer.Pedido;

namespace Dominio.Core.MainModule.Pedido
{

    public class PedidoManager
    {
        Pedido_DAL pedidos = new Pedido_DAL();

        public List<DetallePedidoClienteDTO> ListarHistorialCliente(int idUser)
        {
            return pedidos.ObtenerHistorialCliente(idUser);
        }

        public List<DetallePedido> ListarHistorialAdmin()
        {
            return pedidos.ObtenerHistorialAdmin();
        }

        public List<DetallePedido> BuscarPedidosPorUsuario(string nombreUsuario)
        {
            return pedidos.BuscarPedidosPorUsuario(nombreUsuario);
        }

        public List<DetallePedido> BuscarPedidosPorUsuarioFiltrado(int idUser, string nombreUsuario, string producto)
        {
            return pedidos.BuscarPedidosPorUsuarioFiltrado(idUser, nombreUsuario, producto);
        }

    }
}
