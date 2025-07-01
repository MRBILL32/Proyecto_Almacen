using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Dominio.Core.Entities.Producto;
using Infraestructura.Data.SqlServer.Producto;

namespace Dominio.Core.MainModule.Producto
{
   public class CategoriaManager
    {
        Categoria_DAL categoria = new Categoria_DAL();

        public IEnumerable<tb_Categoria> ListarCategorias()
        {
            return categoria.ListarCategorias();
        }
    }
}
