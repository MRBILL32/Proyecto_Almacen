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
            mensaje.Body = $"Hola {nombreAdmin}, tu registro como administrador fue exitoso, ya puede iniciar sesion como administrador(a) bienvenido(a) al equipo.";

            var smtp = new SmtpClient("smtp.gmail.com", 587);
            smtp.EnableSsl = true;
            smtp.Credentials = new NetworkCredential(remitente, clave);
            smtp.Send(mensaje);
        }

        public static void EnviarRechazo(string destinatario, string nombreAdmin)
        {
            var remitente = "stivent456@gmail.com";
            var clave = "uvfv wisp duco cayk"; //contraseña de aplicacion

            var mensaje = new MailMessage(remitente, destinatario);
            mensaje.Subject = "Rechazo de nuevo Administrador";
            mensaje.Body = $"Hola {nombreAdmin}, tu registro como administrador fue Rechazado, si sospecha que hubo un error le pedimos que se comunique con soporte gracias.";

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
            mensaje.Body = $"Hola {nombreDestino}, Su correo electronico esta siendo revisado por un administrador en breves recibira un correo de confirmacion.";

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
        <p>Ahi un nuevo usuario solicitando registrarse como administrador:</p>
        <ul>
            <li><strong>Nombre:</strong> {nombreSolicitante}</li>
            <li><strong>Login:</strong> {login}</li>
            <li><strong>Correo:</strong> {correoSolicitante}</li>
        </ul>
        <p>Por favor, toma una decisión sobre esta solicitud:</p>
        <a href='{urlDecidir}' style='background-color: #007bff; color: white; padding: 10px 15px; text-decoration:none; border-radius:5px;'>Tomar decisión</a>
            <br />
    ";

            var smtp = new SmtpClient("smtp.gmail.com", 587);
            smtp.EnableSsl = true;
            smtp.Credentials = new NetworkCredential(remitente, clave);
            smtp.Send(mensaje);
        }
        public static void EnviarRecuperacion(string destinatario, string urlRecuperar, string nombreUsuario = "")
        {
            // Correo del remitente 
            var remitente = "stivent456@gmail.com";
            var clave = "uvfv wisp duco cayk"; // Contraseña de aplicación

            // Mensaje enviado al destinatario
            var mensaje = new MailMessage(remitente, destinatario);
            mensaje.Subject = "Recuperación de Contraseña";

            // Saludo personalizado si tienes el nombre
            string saludo = string.IsNullOrEmpty(nombreUsuario) ? "Hola," : $"Hola {nombreUsuario},";

            mensaje.Body = $@" {saludo}<br />
        Recibimos una solicitud para recuperar su cuenta. Si fue usted, por favor continúe con el procedimiento haciendo clic en el siguiente botón.<br /><br />

        <a href='{urlRecuperar}' style='background-color: #007bff; color: white; padding: 10px 15px; text-decoration:none; border-radius:5px;'>Restablecer contraseña</a>
        <br /><br />

        Si no fue usted, ignore este mensaje o comuníquese con soporte.
    ";
            mensaje.IsBodyHtml = true; // <--- Esto es importante para que el HTML se vea bien

            var smtp = new SmtpClient("smtp.gmail.com", 587);
            smtp.EnableSsl = true;
            smtp.Credentials = new NetworkCredential(remitente, clave);
            smtp.Send(mensaje);
        }
    }
}
