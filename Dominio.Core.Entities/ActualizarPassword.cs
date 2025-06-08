using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dominio.Core.Entities
{
    public class ActualizarPassword
    {
        [Required(ErrorMessage = "Ingrese la nueva contraseña")]
        [DataType(DataType.Password)]
        public string NuevaPassword { get; set; }

        [Required(ErrorMessage = "Confirme la nueva contraseña")]
        [DataType(DataType.Password)]
        [Compare("NuevaPassword", ErrorMessage = "Las contraseñas no coinciden")]
        public string ConfirmarPassword { get; set; }
    }
}
