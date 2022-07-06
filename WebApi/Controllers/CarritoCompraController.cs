using Core.Entities;
using Core.Interface;
using Microsoft.AspNetCore.Mvc;

namespace WebApi.Controllers
{
    public class CarritoCompraController : BaseApiController
    {
        private readonly ICarritoCompraRepository _carritoCompra;

        public CarritoCompraController(ICarritoCompraRepository carritoCompra)
        {
            _carritoCompra = carritoCompra;
        }

        [HttpGet]
        public async Task<ActionResult<CarritoCompra>> GetCarritoCompraById(string id)
        {
            var carrito = await _carritoCompra.GetCarritoCompraAsync(id);

            return Ok(carrito ?? new CarritoCompra(id));
        }

        [HttpPost]
        public async Task<ActionResult<CarritoCompra>> UpdateCarritoCompra(CarritoCompra carritoParametro)
        {
            var carritoActualizado = await _carritoCompra.UpdateCarritoCompraAsync(carritoParametro);
            return Ok(carritoActualizado);
        }

        [HttpDelete]
        public async Task DeleteCarritoCompra(string id)
        {
            await _carritoCompra.DeleteCarritoCompraAsync(id);
        }
    }
}
