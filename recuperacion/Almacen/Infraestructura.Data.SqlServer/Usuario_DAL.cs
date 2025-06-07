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

        public string Registrar(tb_Usuario reg)
        {
            string mensaje = "";
            cnx = cn.Conectar();
            cnx.Open();

            try
            {
                SqlCommand cmd = new SqlCommand("usp_CrearUser", cnx);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@nombres", reg.nombres);
                cmd.Parameters.AddWithValue("@apellidos", reg.apellidos);
                cmd.Parameters.AddWithValue("@dni", reg.dni);
                cmd.Parameters.AddWithValue("@idrol", reg.idRol);
                cmd.Parameters.AddWithValue("@login", reg.login);
                cmd.Parameters.AddWithValue("@password", reg.password);
                cmd.Parameters.AddWithValue("@correo", reg.correo);
                cmd.Parameters.AddWithValue("@estado", reg.correo);

                int a = cmd.ExecuteNonQuery();
                if (a > 0)
                    mensaje = "Usuario registrado correctamente.";
                else
                    mensaje = "No se registró el usuario.";
            }
            catch (SqlException ex)
            {
                mensaje = "Error al registrar el usuario. Intenta nuevamente.";
                mensaje = ex.Message;
            }
            finally
            {
                cnx.Close();
            }

            return mensaje;
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
            catch (SqlException ex)
            {
                // Capturamos el error del procedimiento almacenado para mostrarlo en la UI
                throw new Exception("Inicio de sesión fallido: " + ex.Message);
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
