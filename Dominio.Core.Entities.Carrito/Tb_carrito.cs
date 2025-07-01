using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dominio.Core.Entities.Carrito
{
    public class Tb_carrito
    {
        public int idCarrito { get; set; }

        public int idUser { get; set; }

        public DateTime FechaCreacion { get; set; }
    }
}
