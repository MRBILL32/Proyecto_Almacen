using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dominio.Core.Entities.Pedido
{
    public class DetallePedidoClienteDTO
    {
        public string NomProd { get; set; }
        public int Cantidad { get; set; }
        public decimal PrecioUnit { get; set; }
        public decimal Subtotal { get; set; }
        public DateTime Fecha { get; set; }
    }

}
