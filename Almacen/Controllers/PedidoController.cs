using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Dominio.Core.Entities.Pedido;
using Dominio.Core.MainModule.Pedido;

namespace Almacen.Controllers
{
    public class PedidoController : Controller
    {

        PedidoManager pedido = new PedidoManager();

        public ActionResult HistorialPedidos()
        {
            if (Session["usuario"] == null)
                return RedirectToAction("IniciarSesion", "Usuario");

            dynamic usuario = Session["usuario"];
            int idUser = usuario.IdUsuario;
            int idRol = Convert.ToInt32(Session["idRol"]);

            List<DetallePedido> pedidos;

            if (idRol == 1) // Admin
            {
                pedidos = pedido.ListarHistorialAdmin();
            }
            else // Comprador
            {
                pedidos = pedido.ListarHistorialUsuario(idUser);
            }

            return View(pedidos);
        }

        public ActionResult BuscarPedidos(string nombreUsuario, string producto)
        {
            if (Session["usuario"] == null)
                return RedirectToAction("IniciarSesion", "Usuario");

            dynamic usuario = Session["usuario"];
            int idUser = usuario.IdUsuario;
            int idRol = Convert.ToInt32(Session["idRol"]);

            List<DetallePedido> pedidos;

            if (idRol == 1) // Admin
            {
                pedidos = pedido.BuscarPedidosPorUsuario(nombreUsuario ?? string.Empty);
            }
            else // Usuario normal
            {
                pedidos = pedido.BuscarPedidosPorUsuarioFiltrado(idUser, nombreUsuario ?? string.Empty, producto ?? string.Empty);
            }

            return View("HistorialPedidos", pedidos);
        }
    }
}