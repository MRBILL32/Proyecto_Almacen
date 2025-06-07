using System;
using System.Collections.Generic;
using System.Linq;
using System.Configuration;
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

        public static void EnviarAviso(string destinatario, string nombreDestino)
        {
            //correo del remitente 
            var remitente = "stivent456@gmail.com";
            var clave = "uvfv wisp duco cayk"; //contraseña de aplicacion

            //mensaje enviado al destinatario
            var mensaje = new MailMessage(remitente, destinatario);
            mensaje.Subject = "Correo en Revision";
            mensaje.Body = $"Hola {nombreDestino}, Su correo electronico esta siendo revisado para su posterior aprobacion.";

            var smtp = new SmtpClient("smtp.gmail.com", 587);
            smtp.EnableSsl = true;
            smtp.Credentials = new NetworkCredential(remitente, clave);
            smtp.Send(mensaje);
        }

        public static void EnviarSolicitudRevision(string destinatarioAdmin, string nombreSolicitante, string login, string correoSolicitante, string urlDecidir)
        {
            var remitente = "stivent456@gmail.com";
            var clave = "uvfv wisp duco cayk";

            var mensaje = new MailMessage(remitente, destinatarioAdmin);
            mensaje.Subject = "Nueva solicitud de administrador";
            mensaje.IsBodyHtml = true;

            mensaje.Body = $@"
        <p>Hola Administrador,</p>
        <p>Nuevo usuario solicitando permiso de administrador:</p>
        <ul>
            <li><strong>Nombre:</strong> {nombreSolicitante}</li>
            <li><strong>Login:</strong> {login}</li>
            <li><strong>Correo:</strong> {correoSolicitante}</li>
        </ul>
        <p>Por favor, toma una decisión sobre esta solicitud:</p>
        <a href='{urlDecidir}' style='background-color: #007bff; color: white; padding: 10px 15px; text-decoration:none; border-radius:5px;'>Tomar decisión</a>
    ";

            var smtp = new SmtpClient("smtp.gmail.com", 587);
            smtp.EnableSsl = true;
            smtp.Credentials = new NetworkCredential(remitente, clave);
            smtp.Send(mensaje);
        }
    }
}
