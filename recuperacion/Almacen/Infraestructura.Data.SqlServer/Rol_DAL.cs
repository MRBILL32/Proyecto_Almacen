using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Dominio.Core.Entities;

namespace Infraestructura.Data.SqlServer
{
    public class Rol_DAL
    {
        Conexion cn = new Conexion();
        SqlConnection cnx;

        public List<tb_Rol> Rol() 
        {
            List<tb_Rol> temporal = new List<tb_Rol>();
            cnx = cn.Conectar();

            SqlCommand cmd = new SqlCommand("usp_listarRol", cnx);
            cmd.CommandType = CommandType.StoredProcedure;

            cnx.Open();
            SqlDataReader dr = cmd.ExecuteReader();
            while (dr.Read()) 
            {
                tb_Rol reg = new tb_Rol()
                {
                    idRol = dr.GetInt32(0),
                    tipoRol = dr.GetString(1)
                };
                temporal.Add(reg);
            }
            dr.Close();
            cnx.Close();
            return temporal;
        }
    }
}
