using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dominio.Core.Entities.Producto
{
    public class Tb_Producto
    {
        [Display(Name = "ID Producto", Order = 0)]
        public int IdProd { get; set; } // auto Generado

        [Display(Name = "Producto", Order = 1)]
        [Required(AllowEmptyStrings = false, ErrorMessage = "Ingrese el Producto")]
        public string NomProd { get; set; }

        [Display(Name = "Marca", Order = 2)]
        [Required(AllowEmptyStrings = false, ErrorMessage = "Ingrese la Marca")]
        public string MarcaProd { get; set; }

        public int IdCate { get; set; }

        [Display(Name = "Categoria", Order = 3)]
        public string NomCate { get; set; }

        [Display(Name = "Precio Unidad", Order = 4)]
        [Required(AllowEmptyStrings = false, ErrorMessage = "Ingrese un Precio por unidad")]
        public decimal PrecioUnit { get; set; }

        [Display(Name = "Stock Disponible", Order = 5)]
        [Required(AllowEmptyStrings = false, ErrorMessage = "Ingrese una Cantidad")]
        public int Stock { get; set; }

        public bool Activo { get; set; } // BIT en SQL Server solo para Administrador

        public string ImagenUrl { get; set; } // imagenes
    }
}
