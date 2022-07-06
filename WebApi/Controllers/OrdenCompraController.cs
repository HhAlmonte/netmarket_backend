using AutoMapper;
using Core.Entities.OrdenCompra;
using Core.Interface;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using WebApi.DTOs;
using WebApi.Errors;

namespace WebApi.Controllers
{
    public class OrdenCompraController : BaseApiController
    {
        private readonly IOrdenCompraService _ordenCompraService;
        private readonly IMapper _mapper;

        public OrdenCompraController(IOrdenCompraService ordenCompra, IMapper mapper)
        {
            _ordenCompraService = ordenCompra;
            _mapper = mapper;
        }

        [Authorize]
        [HttpPost]
        public async Task<ActionResult<OrdenCompraReponseDto>> AddOrdenCompra(OrdenCompraDto ordenCompraDto)
        {
            var email = HttpContext.User?.Claims?.FirstOrDefault(x => x.Type == ClaimTypes.Email)?.Value;

            var direccion = _mapper.Map<DireccionDto, Direccion>(ordenCompraDto.DireccionEnvio);

            var ordenCompra = await _ordenCompraService.AddOrdenCompraAsync(email, ordenCompraDto.TipoEnvio, ordenCompraDto.CarritoCompraId, direccion);

            if(ordenCompra == null)
            {
                return BadRequest(new CodeErrorResponse(400, "Errores creando la orden de compra"));
            }

            return Ok(_mapper.Map<OrdenCompras, OrdenCompraReponseDto>(ordenCompra));
        }

        [Authorize]
        [HttpGet]
        public async Task<ActionResult<IReadOnlyList<OrdenCompraReponseDto>>> GetOrdenCompras()
        {
            var email = HttpContext.User?.Claims?.FirstOrDefault(x => x.Type == ClaimTypes.Email)?.Value;
            var ordenCompras = await _ordenCompraService.GetOrdenComprasByUserEmailAsync(email);

            return Ok(_mapper.Map<IReadOnlyList<OrdenCompras>, IReadOnlyList<OrdenCompraReponseDto>>(ordenCompras));
        }

        [Authorize]
        [HttpGet("{id}")]
        public async Task<ActionResult<OrdenCompraReponseDto>> GetOrdenById(int id)
        {
            var email = HttpContext.User?.Claims?.FirstOrDefault(x => x.Type == ClaimTypes.Email)?.Value;

            var ordenCompra = await _ordenCompraService.GetOrdenComprasByUserIdAsync(id, email);

            if(ordenCompra == null)
            {
                return NotFound(new CodeErrorResponse(404, "No se encontró la orden de compra."));
            }

            return _mapper.Map<OrdenCompras, OrdenCompraReponseDto>(ordenCompra);
        }

        [HttpGet("tipoEnvio")]
        public async Task<ActionResult<IReadOnlyList<TipoEnvio>>> GetTipoEnvios()
        {
            return Ok(await _ordenCompraService.GetTiposEnvios());
        }
    }
}
