using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Dominio.Core.Entities;

namespace Infraestructura.Data.SqlServer
{
    public class Usuario_DAL
    {

        Conexion cn = new Conexion();
        SqlConnection cnx;

        public ResultadoRegistro Registrar(tb_Usuario reg)
        {
            ResultadoRegistro resultado = new ResultadoRegistro();
            cnx = cn.Conectar();
            cnx.Open();

            try
            {
                SqlCommand cmd = new SqlCommand("usp_CrearUser", cnx);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@nombres", reg.nombres);
                cmd.Parameters.AddWithValue("@apellidos", reg.apellidos);
                cmd.Parameters.AddWithValue("@dni", reg.dni);
                cmd.Parameters.AddWithValue("@idRol", reg.idRol);
                cmd.Parameters.AddWithValue("@login", reg.login);
                cmd.Parameters.AddWithValue("@password", reg.password);
                cmd.Parameters.AddWithValue("@correo", reg.correo);
                cmd.Parameters.AddWithValue("@estado", reg.estado);

                object result = cmd.ExecuteScalar();
                if (result != null && int.TryParse(result.ToString(), out int nuevoId))
                {
                    resultado.IdUsuario = nuevoId;
                    resultado.Mensaje = "Usuario registrado correctamente.";
                }
                else
                {
                    resultado.IdUsuario = 0;
                    resultado.Mensaje = "No se registró el usuario.";
                }
            }
            catch (SqlException ex)
            {
                resultado.IdUsuario = 0;
                resultado.Mensaje = "Error: " + ex.Message;
            }
            finally
            {
                cnx.Close();
            }

            return resultado;
        }

        public tb_Usuario IniciarSesion(string login, string password)
        {
            tb_Usuario usuario = null;
            cnx = cn.Conectar();

            try
            {
                cnx.Open();
                SqlCommand cmd = new SqlCommand("usp_InicioSesion", cnx);
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.Parameters.AddWithValue("@login", login);
                cmd.Parameters.AddWithValue("@password", password);

                SqlDataReader reader = cmd.ExecuteReader();

                if (reader.Read())
                {
                    usuario = new tb_Usuario()
                    {
                        IdUsuario = Convert.ToInt32(reader["idUser"]),
                        nombres = reader["nombres"].ToString(),
                        apellidos = reader["apellidos"].ToString(),
                        tipoRol = reader["tipoRol"].ToString(),
                        idRol = Convert.ToInt32(reader["idRol"]),
                        login = reader["login"].ToString(),
                        correo = reader["correo"].ToString()
                    };
                }

                reader.Close();
            }
            catch (SqlException)
            {
                // no es necesario escribir algo aqui

            }
            finally
            {
                if (cnx.State == ConnectionState.Open)
                    cnx.Close();
            }

            return usuario;
        }

        public void CambiarEstado(int idUsuario, string nuevoEstado)
        {
            System.Diagnostics.Debug.WriteLine($"DAL: Cambiando estado de usuario {idUsuario} a {nuevoEstado}");
            cnx = cn.Conectar();

            try
            {
                cnx.Open();
                SqlCommand cmd = new SqlCommand("usp_CambiarEstado", cnx);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@idUsuario", idUsuario);
                cmd.Parameters.AddWithValue("@nuevoEstado", nuevoEstado);

                int rows = cmd.ExecuteNonQuery();
                System.Diagnostics.Debug.WriteLine($"DAL: Fila afectada: {rows}");
            }
            catch (SqlException ex)
            {
                System.Diagnostics.Debug.WriteLine("DAL: Error al cambiar de estado: " + ex.Message);
                throw new Exception("Error al cambiar de estado: " + ex.Message);
            }
            finally
            {
                if (cnx.State == ConnectionState.Open)
                    cnx.Close();
            }
        }

        public tb_Usuario ObtenerPorId(int idUsuario)
        {
            tb_Usuario usuario = null;
            cnx = cn.Conectar();

            try
            {
                cnx.Open();
                SqlCommand cmd = new SqlCommand("usp_ObtenerUsuarioPorId", cnx);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@idUsuario", idUsuario);

                SqlDataReader reader = cmd.ExecuteReader();

                if (reader.Read())
                {
                    usuario = new tb_Usuario()
                    {
                        IdUsuario = Convert.ToInt32(reader["idUser"]),
                        nombres = reader["nombres"].ToString(),
                        apellidos = reader["apellidos"].ToString(),
                        dni = reader["dni"].ToString(),
                        tipoRol = reader["tipoRol"].ToString(),
                        idRol = Convert.ToInt32(reader["idRol"]),
                        login = reader["login"].ToString(),
                        correo = reader["correo"].ToString(),
                        estado = reader["estado"].ToString()
                    };
                }

                reader.Close();
            }
            catch (SqlException ex)
            {
                throw new Exception("Error al obtener usuario por ID: " + ex.Message);
            }
            finally
            {
                if (cnx.State == ConnectionState.Open)
                    cnx.Close();
            }

            return usuario;
        }

    }
}
