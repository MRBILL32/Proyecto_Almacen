using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using Dominio.Core.Entities.Producto;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Infraestructura.Data.SqlServer;
using System.Data;

namespace Infraestructura.Data.SqlServer.Producto
{
    public class Producto_DAL
    {
        Conexion cn = new Conexion();

        // Listado producto usuario
        public IEnumerable<Tb_Producto> ListarProductos()
        {
            List<Tb_Producto> temporal = new List<Tb_Producto>();
            using (var cnx = cn.Conectar())
            {
                SqlCommand cmd = new SqlCommand("usp_listarProductos", cnx);
                cmd.CommandType = CommandType.StoredProcedure;

                cnx.Open();
                using (SqlDataReader dr = cmd.ExecuteReader())
                {
                    while (dr.Read())
                    {
                        Tb_Producto reg = new Tb_Producto()
                        {
                            IdProd = Convert.ToInt32(dr["idProd"]),
                            NomProd = dr["nomProd"].ToString(),
                            MarcaProd = dr["marcaProd"].ToString(),
                            NomCate = dr["nomCate"].ToString(),
                            PrecioUnit = Convert.ToDecimal(dr["precioUnit"]),
                            Stock = Convert.ToInt16(dr["stock"])
                        };
                        temporal.Add(reg);
                    }
                }
            }
            return temporal;
        }

        // Listar producto Admin
        public IEnumerable<Tb_Producto> ListaProducAdmin()
        {
            List<Tb_Producto> temporal = new List<Tb_Producto>();
            using (var cnx = cn.Conectar())
            {
                SqlCommand cmd = new SqlCommand("usp_ListaProducAdmin", cnx);
                cmd.CommandType = CommandType.StoredProcedure;

                cnx.Open();
                using (SqlDataReader dr = cmd.ExecuteReader())
                {
                    while (dr.Read())
                    {
                        Tb_Producto reg = new Tb_Producto()
                        {
                            IdProd = Convert.ToInt32(dr["idProd"]),
                            NomProd = dr["nomProd"].ToString(),
                            MarcaProd = dr["marcaProd"].ToString(),
                            IdCate = Convert.ToInt32(dr["idCate"]),
                            NomCate = dr["nomCate"].ToString(),
                            PrecioUnit = Convert.ToDecimal(dr["precioUnit"]),
                            Stock = Convert.ToInt16(dr["stock"]),
                            Activo = Convert.ToBoolean(dr["activo"])
                        };
                        temporal.Add(reg);
                    }
                }
            }
            return temporal;
        }

     
    }
}
