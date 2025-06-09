using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Dominio.Core.Entities.Producto;

namespace Infraestructura.Data.SqlServer.Producto
{
    public class Categoria_DAL
    {
        Conexion cn = new Conexion();

        public IEnumerable<tb_Categoria> ListarCategorias()
        {
            List<tb_Categoria> categorias = new List<tb_Categoria>();
            using (var cnx = cn.Conectar())
            {
                SqlCommand cmd = new SqlCommand("SELECT idCate, nomCate FROM schProductos.Categoria", cnx);
                cnx.Open();
                using (SqlDataReader dr = cmd.ExecuteReader())
                {
                    while (dr.Read())
                    {
                        tb_Categoria cat = new tb_Categoria()
                        {
                            IdCate = Convert.ToInt32(dr["idCate"]),
                            nomCate = dr["nomCate"].ToString()
                        };
                        categorias.Add(cat);
                    }
                }
            }
            return categorias;
        }
    }
}
