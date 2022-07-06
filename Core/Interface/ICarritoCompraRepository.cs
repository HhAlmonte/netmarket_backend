using Core.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Interface
{
    public interface ICarritoCompraRepository
    {
        Task<CarritoCompra> GetCarritoCompraAsync(string CarritoId);
        Task<CarritoCompra> UpdateCarritoCompraAsync(CarritoCompra carritoCompra);
        Task<bool> DeleteCarritoCompraAsync(string carritoId);
    }
}
