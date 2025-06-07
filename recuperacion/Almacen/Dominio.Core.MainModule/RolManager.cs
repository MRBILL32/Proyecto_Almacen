using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Dominio.Core.Entities;
using Infraestructura.Data.SqlServer;

namespace Dominio.Core.MainModule
{
    public class RolManager
    {
        Rol_DAL roles = new Rol_DAL();

        public List<tb_Rol> Rol() 
        {
            return roles.Rol();
        }
    }
}
