using Core.Entities;
using Core.Entities.OrdenCompra;
using Core.Interface;
using Core.Specifications;

namespace BussinessLogic.Logic
{
    public class OrdenComprasService : IOrdenCompraService
    { 
        private readonly ICarritoCompraRepository _carritoCompraRepository;
        private readonly IUnitOfWork _unitOfWork;

        public OrdenComprasService(ICarritoCompraRepository carritoCompraRepository, IUnitOfWork unitOfWork)
        {
            _carritoCompraRepository = carritoCompraRepository;
            _unitOfWork = unitOfWork;
        }

        public async Task<OrdenCompras> AddOrdenCompraAsync(string compradorEmail, int tipoEnvio, string carritoId, Core.Entities.OrdenCompra.Direccion direccion)
        {
            var carritoCompra = await _carritoCompraRepository.GetCarritoCompraAsync(carritoId);

            var items = new List<OrdenItem>();

            foreach (var item in carritoCompra.Items)
            {
                var productoItem = await _unitOfWork.Repository<Producto>().GetByIdAsync(item.Id);
                var itemOrdenado = new ProductoItemOrdenado(productoItem.Id, productoItem.Nombre, productoItem.Imagen);
                var ordenItem = new OrdenItem(itemOrdenado, productoItem.Precio, item.Cantidad);
                items.Add(ordenItem);
            }

            var tipoEnvioEntity = await _unitOfWork.Repository<TipoEnvio>().GetByIdAsync(tipoEnvio);

            var subtotal = items.Sum(item => item.Precio * item.Cantidad);

            var ordenCompra = new OrdenCompras(compradorEmail, direccion, tipoEnvioEntity, items, subtotal);

            _unitOfWork.Repository<OrdenCompras>().AddEntity(ordenCompra);
            var resultado = await _unitOfWork.Complete();

            if(resultado <= 0)
            {
                return null;
            }

            await _carritoCompraRepository.DeleteCarritoCompraAsync(carritoId);

            return ordenCompra;
        }

        public async Task<IReadOnlyList<OrdenCompras>> GetOrdenComprasByUserEmailAsync(string email)
        {
            var spec = new OrdenCompraWithItemsSpecification(email);

            return await _unitOfWork.Repository<OrdenCompras>().GetAllWithSpec(spec);
        }

        public async Task<OrdenCompras> GetOrdenComprasByUserIdAsync(int id, string email)
        {
            var spec = new OrdenCompraWithItemsSpecification(id, email);

            return await _unitOfWork.Repository<OrdenCompras>().GetByIdWithSpec(spec);
        }

        public async Task<IReadOnlyList<TipoEnvio>> GetTiposEnvios()
        {
            return await _unitOfWork.Repository<TipoEnvio>().GetAllAsync();
        }
    }
}
