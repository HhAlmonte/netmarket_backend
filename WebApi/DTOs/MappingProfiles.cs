using AutoMapper;
using Core.Entities;
using Core.Entities.OrdenCompra;
using WebApi.DTO;

namespace WebApi.DTOs
{
    public class MappingProfiles : Profile
    {
        public MappingProfiles()
        {
            CreateMap<Producto, ProductoDto>()
                .ForMember(p => p.Categoria, x => x.MapFrom(a => a.Categoria.Nombre))
                .ForMember(p => p.Marca, x => x.MapFrom(a => a.Marca.Nombre));

            CreateMap<Core.Entities.Direccion, DireccionDto>().ReverseMap();
            CreateMap<Usuario, UsuarioDto>().ReverseMap();

            CreateMap<DireccionDto, Core.Entities.OrdenCompra.Direccion>().ReverseMap();

            CreateMap<OrdenCompras, OrdenCompraReponseDto>()
                .ForMember(o => o.TipoEnvio, x => x.MapFrom(y => y.TipoEnvio.Nombre))
                .ForMember(o => o.TipoEnvioPrecio, x => x.MapFrom(y => y.TipoEnvio.Precio));

            CreateMap<OrdenItem, OrdenItemResponseDto>()
                .ForMember(o => o.ProductoId, x => x.MapFrom(y => y.ItemOrdenado.ProductoItemId))
                .ForMember(o => o.ProductoNombre, x => x.MapFrom(y => y.ItemOrdenado.ProductoName))
                .ForMember(o => o.ProductoImagen, x => x.MapFrom(y => y.ItemOrdenado.ImagenUrl));

        }
    }
}
