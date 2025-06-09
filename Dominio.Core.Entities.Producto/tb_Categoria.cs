using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dominio.Core.Entities.Producto
{
    public class tb_Categoria
    {

        [Display(Name = "Categoría")]
        public int IdCate { get; set; }

        public string nomCate { get; set; }

    }
}
