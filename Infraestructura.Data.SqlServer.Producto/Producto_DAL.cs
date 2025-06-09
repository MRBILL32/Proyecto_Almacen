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
        public void EliminarProducto(int idProd) 
        {
            using (var cnx = cn.Conectar())
            {
                SqlCommand cmd = new SqlCommand("usp_EliminarProducto", cnx);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@idProd", idProd);

                cnx.Open();
                cmd.ExecuteNonQuery();
            }
        }

        public string ActualizarProducto(Tb_Producto productos)
        {
            string mensaje = "";
            using (var cnx = cn.Conectar())
            {
                try
                {
                    SqlCommand cmd = new SqlCommand("usp_ActualizarProducto", cnx);
                    cmd.CommandType = CommandType.StoredProcedure;

                    cmd.Parameters.AddWithValue("@idProd", productos.IdProd);
                    cmd.Parameters.AddWithValue("@nomProd", productos.NomProd);
                    cmd.Parameters.AddWithValue("@marcaProd", productos.MarcaProd);
                    cmd.Parameters.AddWithValue("@idCate", productos.IdCate);
                    cmd.Parameters.AddWithValue("@precioUnit", productos.PrecioUnit);
                    cmd.Parameters.AddWithValue("@stock", productos.Stock);

                    cnx.Open();
                    int filasAfectadas = cmd.ExecuteNonQuery();
                    mensaje = filasAfectadas > 0 ? "Producto actualizado correctamente." : "No se actualizó ningún producto.";
                }
                catch (SqlException ex)
                {
                    mensaje = "Error al actualizar: " + ex.Message;
                }
            }
            return mensaje;
        }
    }
}
