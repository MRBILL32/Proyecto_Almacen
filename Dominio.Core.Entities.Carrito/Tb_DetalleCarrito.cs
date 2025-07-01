using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dominio.Core.Entities.Carrito
{
    public class Tb_DetalleCarrito
    {
        public int IdDetalleCarrito { get; set; }
        public int IdCarrito { get; set; }
        public int IdProd { get; set; }
        public string NomProd { get; set; }
        public string MarcaProd { get; set; }
        public int Cantidad { get; set; }
        public decimal PrecioUnit { get; set; }
        public decimal Subtotal => Cantidad * PrecioUnit;

    }
}
