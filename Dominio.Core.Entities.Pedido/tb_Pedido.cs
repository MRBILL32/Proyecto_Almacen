using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dominio.Core.Entities.Pedido
{
    public class tb_Pedido
    {
        public int idPedido { get; set; }

        public int IdUser { get; set; }
        [Display(Name = "Nombres")]
        public string Nombres { get; set; }
        [Display(Name = "Apellidos")]
        public string Apellidos { get; set; }

        [Display(Name = "DNI")]
        public string Dni { get; set; }
        [Display(Name = "Correo Electronico")]
        public string Correo { get; set; }
        [Display(Name = "Fecha")]
        public DateTime Fecha { get; set; }
        [Display(Name = "Total")]
        public decimal Total { get; set; }
        [Display(Name = "Estado")]
        public string Estado { get; set; }
    }
}
