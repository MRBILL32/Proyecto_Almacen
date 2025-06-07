using System;
using Infraestructura.Helpers;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Infraestructura.Data.SqlServer;
using Dominio.Core.Entities;

namespace Dominio.Core.MainModule
{
    public class UsuarioManager
    {
        Usuario_DAL usuario = new Usuario_DAL();
        public string Registrar(tb_Usuario reg) 
        {
            return usuario.Registrar(reg);
        }

        public tb_Usuario IniciarSesion(string login, string password)
        {
            return usuario.IniciarSesion(login, password);
        }
    }
}
