﻿using AutoMapper;
using Core.Entities;
using Core.Interface;
using Core.Specifications;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WebApi.DTO;
using WebApi.DTOs;
using WebApi.Errors;

namespace WebApi.Controllers
{
    public class ProductoController : BaseApiController
    {
        private readonly IGenericRepository<Producto> _productoRepository;
        private readonly IMapper _mapper;

        public ProductoController(IGenericRepository<Producto> productoRepository, IMapper mapper)
        {
            _productoRepository = productoRepository;
            _mapper = mapper;
        }

        [HttpGet]
        public async Task<ActionResult<Pagination<ProductoDto>>> GetProductos([FromQuery] ProductoSpecificationParams productoParams)
        {
            var spec = new ProductWithCategoriaAndMarcaSpecification(productoParams);

            var productos = await _productoRepository.GetAllWithSpec(spec);

            var specCount = new ProductoForCountingSpecification(productoParams);

            var totalProductos = await _productoRepository.CountAsync(specCount);

            var rounded = Math.Ceiling(Convert.ToDecimal(totalProductos) / Convert.ToDecimal(productoParams.PageSize));
            var totalPages = Convert.ToInt32(rounded);

            var data = _mapper.Map<IReadOnlyList<Producto>, IReadOnlyList<ProductoDto>>(productos);

            return Ok(
                new Pagination<ProductoDto>
                {
                    Count = totalProductos,
                    Data = data,
                    PageCount = totalPages,
                    PageIndex = productoParams.PageIndex,
                    PageSize = productoParams.PageSize
                }
                );
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<ProductoDto>> GetProducto(int id)
        {
            var spec = new ProductWithCategoriaAndMarcaSpecification(id);
            var productos = await _productoRepository.GetByIdWithSpec(spec);

            if (productos == null)
            {
                return NotFound(new CodeErrorResponse(404));
            }

            return _mapper.Map<Producto, ProductoDto>(productos);
        }

        [Authorize(Roles = "ADMIN")]
        [HttpPost]
        public async Task<ActionResult<Producto>> Post(Producto producto)
        {
            var resultado = await _productoRepository.Add(producto);

            if(resultado == 0)
            {
                throw new Exception("No se insertó el producto");
            }

            return Ok(producto);
        }

        [Authorize(Roles = "ADMIN")]
        [HttpPut("{id}")]
        public async Task<ActionResult<Producto>> Put(int id, Producto producto)
        {
            producto.Id = id;
            var resultado = await _productoRepository.Update(producto);

            if(resultado == 0)
            {
                throw new Exception("No se pudo actualizar el producto");
            }

            return Ok(producto);
        }
    }
}
