using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;

namespace Infraestructura.Helpers
{
    public class CorreoHelper
    {
        public static void EnviarConfirmacion(string destinatario, string nombreAdmin)
        {
            var remitente = "stivent456@gmail.com";
            var clave = "uvfv wisp duco cayk"; //contraseña de aplicacion

            var mensaje = new MailMessage(remitente, destinatario);
            mensaje.Subject = "Confirmacion de nuevo Administrador";
            mensaje.Body = $"Hola {nombreAdmin}, tu registro como administrador fue exitoso.";

            var smtp = new SmtpClient("smtp.gmail.com", 587);
            smtp.EnableSsl = true;
            smtp.Credentials = new NetworkCredential(remitente, clave);
            smtp.Send(mensaje);
        }
    }
}
