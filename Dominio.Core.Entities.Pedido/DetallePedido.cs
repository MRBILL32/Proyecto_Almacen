using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dominio.Core.Entities.Pedido
{
    public class DetallePedido
    {
        public int IdDetalle { get; set; }
        public int IdPedido { get; set; }

        [Display(Name = "Nombres")]
        public string Nombres { get; set; }

        public string Apellidos { get; set; }

        public int IdProd { get; set; }

        [Display(Name = "Nombre Producto")]
        public string NomProd { get; set; }

        [Display(Name = "Cantidad")]
        public int Cantidad { get; set; }

        [Display(Name = "Precio Unitario")]
        public decimal PrecioUnit { get; set; }

        [Display(Name = "Sub-Total")]
        public decimal Subtotal { get; set; }

        [Display(Name = "Fecha")]
        public DateTime Fecha { get; set; }

        [Display(Name = "Total")]
        public decimal Total { get; set; }

        [Display(Name = "Estado")]
        public string Estado { get; set; }
    }
}
