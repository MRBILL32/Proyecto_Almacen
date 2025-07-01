using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dominio.Core.Entities
{
    public class tb_Usuario
    {
        public int IdUsuario { get; set; }

        [Display(Name = "Nombres", Order = 0)]
        [Required(AllowEmptyStrings = false, ErrorMessage = "Ingrese sus nombres reales")]
        public string nombres { get; set; }

        [Display(Name = "Apellidos", Order = 1)]
        [Required(AllowEmptyStrings = false, ErrorMessage = "Ingrese sus apellidos reales")]
        public string apellidos { get; set; }


        [Display(Name = "Nº DNI", Order = 2)]
        [Required(AllowEmptyStrings = false, ErrorMessage = "Ingrese solo números")]
        [RegularExpression(@"^\d{7}$", ErrorMessage = "Debe ingresar exactamente 7 dígitos numéricos")]
        public string dni { get; set; }

        [Display(Name = "ID rol de usuario", Order = 3)]
        public int idRol { get; set; }          // FK a la tabla Rol

        [Display(Name = "Tipo rol de usuario", Order = 4)]
        public string tipoRol { get; set; }     // ← Nombre del rol (ej: "administrador", "usuario")

        [Display(Name = "Nombre de Usuario", Order = 5)]
        [Required(AllowEmptyStrings = false, ErrorMessage = "Ingrese un usuario")]
        public string login { get; set; }

        [Display(Name = "Contraseña de Usuario", Order = 6)]
        [Required(AllowEmptyStrings = false, ErrorMessage = "Ingrese una contraseña")]
        public string password { get; set; }

        [Display(Name = "Correo Electronico", Order = 7)]
        [Required(AllowEmptyStrings = false, ErrorMessage = "Ingrese su correo electrónico")]
        public string correo { get; set; }
    }
}
