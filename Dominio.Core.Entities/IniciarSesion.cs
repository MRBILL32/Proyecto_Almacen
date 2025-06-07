using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dominio.Core.Entities
{

    public class IniciarSesion
    {
        [Display(Name ="Nombre de Usuario")]
        [Required(ErrorMessage = "Ingrese un usuario")]
        public string login { get; set; }

        [Display(Name = "Contraseña")]
        [Required(ErrorMessage = "Ingrese una contraseña")]
        [DataType(DataType.Password)]
        public string password { get; set; }
    }
}
