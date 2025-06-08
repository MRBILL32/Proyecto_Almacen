using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dominio.Core.Entities
{
    public class RecuperarPassword
    {
        [Required(ErrorMessage = "Ingrese su correo electrónico")]
        [EmailAddress(ErrorMessage = "Ingrese un correo válido")]
        public string Correo { get; set; }
    }
}
