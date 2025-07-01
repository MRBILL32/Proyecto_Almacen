using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infraestructura.Data.SqlServer
{
    public class Conexion
    {
        SqlConnection cn;

        public SqlConnection Conectar() 
        {
            cn = new SqlConnection(ConfigurationManager.ConnectionStrings["conex"].ConnectionString);
            
            return cn;
        }
    }
}
