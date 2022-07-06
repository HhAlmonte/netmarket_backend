using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Entities.OrdenCompra
{
    public class ProductoItemOrdenado
    {
        public ProductoItemOrdenado() { }

        public ProductoItemOrdenado(int productoItemId, string productoName, string imagenUrl)
        {
            ProductoItemId = productoItemId;
            ProductoName = productoName;
            ImagenUrl = imagenUrl;
        }

        public int ProductoItemId { get; set; }
        public string ProductoName { get; set; }
        public string ImagenUrl { get; set; }
    }
}
