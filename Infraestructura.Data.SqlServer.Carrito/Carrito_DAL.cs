using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Dominio.Core.Entities.Carrito;

namespace Infraestructura.Data.SqlServer.Carrito
{
    public class Carrito_DAL
    {
        Conexion cn = new Conexion();

        //crear carrito para usuario
        public int CrearCarrito(int idUser)
        {
            int idCarrito = 0;
            using (var cnx = cn.Conectar())
            {
                SqlCommand cmd = new SqlCommand("usp_CrearCarrito", cnx);
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.Parameters.AddWithValue("@idUser", idUser);

                SqlParameter outputId = new SqlParameter("@idCarrito", SqlDbType.Int)
                {
                    Direction = ParameterDirection.Output
                };
                cmd.Parameters.Add(outputId);

                cnx.Open();
                cmd.ExecuteNonQuery();

                idCarrito = (int)outputId.Value;
            }
            return idCarrito;
        }

        //agregar productos al carrito
        public string AgregarProductoCarrito(int idCarrito, int idProd, int cantidad)
        {
            using (var cnx = cn.Conectar())
            {
                SqlCommand cmd = new SqlCommand("usp_AgregarProductoCarrito", cnx);
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.Parameters.AddWithValue("@idCarrito", idCarrito);
                cmd.Parameters.AddWithValue("@idProd", idProd);
                cmd.Parameters.AddWithValue("@cantidad", cantidad);

                try
                {
                    cnx.Open();
                    cmd.ExecuteNonQuery();
                    return "Producto agregado al carrito.";
                }
                catch (SqlException ex)
                {
                    return "Error: " + ex.Message;
                }
            }
        }

        //listar carrito (vista)
        public List<Tb_DetalleCarrito> ListarCarrito(int idCarrito)
        {
            List<Tb_DetalleCarrito> lista = new List<Tb_DetalleCarrito>();
            using (var cnx = cn.Conectar())
            {
                SqlCommand cmd = new SqlCommand("usp_ListarCarrito", cnx);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@idCarrito", idCarrito);

                cnx.Open();
                using (SqlDataReader dr = cmd.ExecuteReader())
                {
                    while (dr.Read())
                    {
                        Tb_DetalleCarrito detalle = new Tb_DetalleCarrito()
                        {
                            IdDetalleCarrito = Convert.ToInt32(dr["idDetalleCarrito"]),
                            IdProd = Convert.ToInt32(dr["idProd"]),
                            NomProd = dr["nomProd"].ToString(),
                            MarcaProd = dr["marcaProd"].ToString(),
                            Cantidad = Convert.ToInt32(dr["cantidad"]),
                            PrecioUnit = Convert.ToDecimal(dr["precioUnit"]),
                            // Subtotal es propiedad calculada, no se asigna directamente
                        };
                        lista.Add(detalle);
                    }
                }
            }
            return lista;
        }

        //eliminar productos(individualmente)
        public void EliminarProductoCarrito(int idCarrito, int idProd)
        {
            using (var cnx = cn.Conectar())
            {
                SqlCommand cmd = new SqlCommand("usp_EliminarProductoCarrito", cnx);
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.Parameters.AddWithValue("@idCarrito", idCarrito);
                cmd.Parameters.AddWithValue("@idProd", idProd);

                cnx.Open();
                cmd.ExecuteNonQuery();
            }
        }

        //vaciar carrito 
        public void VaciarCarrito(int idCarrito)
        {
            using (var cnx = cn.Conectar())
            {
                SqlCommand cmd = new SqlCommand("usp_VaciarCarrito", cnx);
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.Parameters.AddWithValue("@idCarrito", idCarrito);

                cnx.Open();
                cmd.ExecuteNonQuery();
            }
        }

        //Efectuar Compra
        public string EfectuarCompra(int idCarrito, int idUser)
        {
            using (var cnx = cn.Conectar())
            {
                SqlCommand cmd = new SqlCommand("usp_EfectuarCompra", cnx);
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.Parameters.AddWithValue("@idCarrito", idCarrito);
                cmd.Parameters.AddWithValue("@idUser", idUser);

                try
                {
                    cnx.Open();
                    cmd.ExecuteNonQuery();
                    return "Compra realizada con éxito.";
                }
                catch (Exception ex)
                {
                    return "Error al realizar la compra: " + ex.Message;
                }
            }
        }


    }
}