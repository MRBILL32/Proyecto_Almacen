using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Dominio.Core.Entities.Pedido;

namespace Infraestructura.Data.SqlServer.Pedido
{
    public class Pedido_DAL
    {
        Conexion cn = new Conexion();

        public List<DetallePedido> ObtenerHistorialUsuario(int idUser)
        {
            return EjecutarConsultaHistorial("usp_HistorialPedidosUsuario", new SqlParameter("@idUser", idUser));
        }

        public List<DetallePedido> ObtenerHistorialAdmin()
        {
            return EjecutarConsultaHistorial("usp_HistorialPedidosAdmin");
        }

        private List<DetallePedido> EjecutarConsultaHistorial(string nombreSP, params SqlParameter[] parametros)
        {
            List<DetallePedido> lista = new List<DetallePedido>();

            using (var cnx = cn.Conectar())
            {
                SqlCommand cmd = new SqlCommand(nombreSP, cnx);
                cmd.CommandType = CommandType.StoredProcedure;

                if (parametros != null && parametros.Length > 0)
                {
                    cmd.Parameters.AddRange(parametros);
                }

                try
                {
                    cnx.Open();
                    using (SqlDataReader dr = cmd.ExecuteReader())
                    {
                        while (dr.Read())
                        {
                            lista.Add(MapearDetallePedido(dr));
                        }
                    }
                }
                catch (Exception ex)
                {
                    // Aquí puedes loguear el error o manejarlo según convenga
                    throw new Exception("Error al obtener historial de pedidos: " + ex.Message);
                }
            }

            return lista;
        }

        private DetallePedido MapearDetallePedido(SqlDataReader dr)
        {
            return new DetallePedido
            {
                IdPedido = Convert.ToInt32(dr["idPedido"]),
                Nombres = dr["nombres"].ToString(),
                Apellidos = dr["apellidos"] != DBNull.Value ? dr["apellidos"].ToString() : "",
                NomProd = dr["nomProd"].ToString(),
                Cantidad = Convert.ToInt32(dr["cantidad"]),
                PrecioUnit = Convert.ToDecimal(dr["precioUnit"]),
                Subtotal = Convert.ToDecimal(dr["subtotal"]),
                Fecha = Convert.ToDateTime(dr["fecha"]),
                Total = Convert.ToDecimal(dr["total"]),
                Estado = dr["estado"].ToString()
            };
        }

        public List<DetallePedido> BuscarPedidosPorUsuario(string nombreUsuario)
        {
            List<DetallePedido> lista = new List<DetallePedido>();
            using (var cnx = cn.Conectar())
            {
                SqlCommand cmd = new SqlCommand("usp_BuscarPedidoUsuario", cnx);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@nombreUsuario", nombreUsuario);

                cnx.Open();
                using (SqlDataReader dr = cmd.ExecuteReader())
                {
                    while (dr.Read())
                    {
                        lista.Add(new DetallePedido
                        {
                            IdPedido = Convert.ToInt32(dr["idPedido"]),
                            Nombres = dr["nombres"].ToString(),
                            Apellidos = dr["apellidos"] != DBNull.Value ? dr["apellidos"].ToString() : "",
                            NomProd = dr["nomProd"].ToString(),
                            Cantidad = Convert.ToInt32(dr["cantidad"]),
                            PrecioUnit = Convert.ToDecimal(dr["precioUnit"]),
                            Subtotal = Convert.ToDecimal(dr["subtotal"]),
                            Fecha = Convert.ToDateTime(dr["fecha"]),
                            Total = Convert.ToDecimal(dr["total"]),
                            Estado = dr["estado"].ToString()
                        });
                    }
                }
            }
            return lista;
        }

        public List<DetallePedido> BuscarPedidosPorUsuarioFiltrado(int idUser, string nombreUsuario, string producto)
        {
            List<DetallePedido> lista = new List<DetallePedido>();
            using (var cnx = cn.Conectar())
            {
                SqlCommand cmd = new SqlCommand("usp_BuscarPedidoUsuarioFiltrado", cnx);
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.Parameters.AddWithValue("@idUser", idUser);
                cmd.Parameters.AddWithValue("@nombreUsuario", nombreUsuario ?? string.Empty);
                cmd.Parameters.AddWithValue("@producto", producto ?? string.Empty);

                try
                {
                    cnx.Open();
                    using (SqlDataReader dr = cmd.ExecuteReader())
                    {
                        while (dr.Read())
                        {
                            lista.Add(new DetallePedido
                            {
                                IdPedido = Convert.ToInt32(dr["idPedido"]),
                                Nombres = dr["nombres"].ToString(),
                                Apellidos = dr["apellidos"] != DBNull.Value ? dr["apellidos"].ToString() : "",
                                NomProd = dr["nomProd"].ToString(),
                                Cantidad = Convert.ToInt32(dr["cantidad"]),
                                PrecioUnit = Convert.ToDecimal(dr["precioUnit"]),
                                Subtotal = Convert.ToDecimal(dr["subtotal"]),
                                Fecha = Convert.ToDateTime(dr["fecha"]),
                                Total = Convert.ToDecimal(dr["total"]),
                                Estado = dr["estado"].ToString()
                            });
                        }
                    }
                }
                catch (Exception ex)
                {
                    // Aquí puedes loguear el error o manejarlo según convenga
                    throw new Exception("Error al buscar pedidos filtrados: " + ex.Message);
                }
            }
            return lista;
        }
    }
}
