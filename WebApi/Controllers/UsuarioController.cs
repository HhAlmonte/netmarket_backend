﻿using AutoMapper;
using Core.Entities;
using Core.Interface;
using Core.Specifications;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using WebApi.DTOs;
using WebApi.Errors;
using WebApi.Extensions;

namespace WebApi.Controllers
{
    public class UsuarioController : BaseApiController
    {
        private readonly UserManager<Usuario> _userManager;
        private readonly SignInManager<Usuario> _signInManager;
        private readonly ITokenService _tokenService;
        private readonly IMapper _mapper;
        private readonly IPasswordHasher<Usuario> _passwordHasher;
        private readonly IGenericSeguridadRepository<Usuario> _seguridadRepository;
        private readonly RoleManager<IdentityRole> _roleManager;
        public UsuarioController(UserManager<Usuario> userManager,
                                 SignInManager<Usuario> signInManager,
                                 ITokenService tokenService,
                                 IMapper mapper,
                                 IPasswordHasher<Usuario> passwordHasher,
                                 IGenericSeguridadRepository<Usuario> seguridadRepository,
                                 RoleManager<IdentityRole> roleManager
            )
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _tokenService = tokenService;
            _mapper = mapper;
            _passwordHasher = passwordHasher;
            _seguridadRepository = seguridadRepository;
            _roleManager = roleManager;
        }


        [HttpPost("login")]
        public async Task<ActionResult<UsuarioDto>> Login(LoginDto loginDto)
        {
            var usuario = await _userManager.FindByEmailAsync(loginDto.Email);

            if (User == null)
            {
                return Unauthorized(new CodeErrorResponse(401));
            }

            var resultado = await _signInManager.CheckPasswordSignInAsync(usuario, loginDto.Password, false);

            if (!resultado.Succeeded)
            {
                return Unauthorized(new CodeErrorResponse(401));
            }

            var roles = await _userManager.GetRolesAsync(usuario);

            return new UsuarioDto
            {
                Id = usuario.Id,
                Email = usuario.Email,
                Username = usuario.UserName,
                Token = _tokenService.CreateToken(usuario, roles),
                Nombre = usuario.Nombre,
                Apellido = usuario.Apellido,
                Imagen = usuario.Imagen,
                Admin = roles.Contains("ADMIN") ? true : false,
            };
        }

        [HttpPost("registrar")]
        public async Task<ActionResult<UsuarioDto>> Registrar(RegistrarDto registrarDto)
        {

            var usuario = new Usuario
            { 
                Email = registrarDto.Email,
                UserName = registrarDto.UserName,
                Nombre = registrarDto.Nombre,
                Apellido = registrarDto.Apellido
            };

            var resultado = await _userManager.CreateAsync(usuario, registrarDto.Password);
            
            if (!resultado.Succeeded)
            {
                return BadRequest(new CodeErrorResponse(400));
            }

            return new UsuarioDto
            { 
                Id = usuario.Id,
                Nombre = usuario.Nombre,
                Apellido = usuario.Apellido,
                Token = _tokenService.CreateToken(usuario, null),
                Email = usuario.Email,
                Username = usuario.UserName,
                Admin = false
            };

        }

        [Authorize]
        [HttpPut("actualizar/{id}")]
        public async Task<ActionResult<UsuarioDto>> Actualizar(string id, RegistrarDto registrarDto)
        {
            var usuario = await _userManager.FindByIdAsync(id);

            if (usuario == null)
            {
                return NotFound(new CodeErrorResponse(404, "El Usuario no existe"));
            }

            usuario.Nombre = registrarDto.Nombre;
            usuario.Apellido = registrarDto.Apellido;
            usuario.Imagen = registrarDto.Imagen;

            if (!string.IsNullOrEmpty(registrarDto.Password))
            {
                usuario.PasswordHash = _passwordHasher.HashPassword(usuario, registrarDto.Password);
            }

            var resultado = await _userManager.UpdateAsync(usuario);

            if (!resultado.Succeeded)
            {
                return BadRequest(new CodeErrorResponse(400, "No se pudo actualizar el usuario"));
            }

            var roles = await _userManager.GetRolesAsync(usuario);

            return new UsuarioDto
            {
                Id = usuario.Id,
                Nombre = usuario.Nombre,
                Apellido = usuario.Apellido,
                Email = usuario.Email,
                Username = usuario.UserName,
                Token = _tokenService.CreateToken(usuario, roles),
                Imagen = usuario.Imagen,
                Admin = roles.Contains("ADMIN") ? true : false
            };
        }

        [Authorize(Roles = "ADMIN")]
        [HttpGet("pagination")]
        public async Task<ActionResult<UsuarioDto>> GetUsuarios([FromQuery] UsuarioSpecificationsParams usuarioParams)
        {
            var spec = new UsuarioSpecification(usuarioParams);
            var usuarios = await _seguridadRepository.GetAllWithSpec(spec);

            var specCount = new UsuarioForCountingSpecification(usuarioParams);
            var totalUsuarios = await _seguridadRepository.CountAsync(specCount);

            var rounded = Math.Ceiling( Convert.ToDecimal(totalUsuarios) / Convert.ToDecimal(usuarioParams.PageSize));
            var totalPages = Convert.ToInt32(rounded);

            var data = _mapper.Map<IReadOnlyList<Usuario>, IReadOnlyList<UsuarioDto>>(usuarios);

            return Ok(
                new Pagination<UsuarioDto>
                {
                    Count = totalUsuarios,
                    Data = data,
                    PageCount = totalPages,
                    PageIndex = usuarioParams.PageIndex,
                    PageSize = usuarioParams.PageSize
                }
                );
        }

        [Authorize(Roles = "ADMIN")]
        [HttpPut("role/{id}")]
        public async Task<ActionResult<UsuarioDto>> UpdateRole(string id, RoleDto roleParam)
        {
            var role = await _roleManager.FindByNameAsync(roleParam.Nombre);

            if(role == null)
            {
                return NotFound(new CodeErrorResponse(404, "El role no exite"));
            }

            var usuario = await _userManager.FindByIdAsync(id);
            if(usuario == null)
            {
                return NotFound(new CodeErrorResponse(404, "El usuario no existe"));
            }

            var usuarioDto = _mapper.Map<Usuario, UsuarioDto>(usuario);

            if (roleParam.Status)
            {
                var resultado = await _userManager.AddToRoleAsync(usuario, roleParam.Nombre);

                if (resultado.Succeeded)
                {
                    usuarioDto.Admin = true;
                }

                if (resultado.Errors.Any())
                {
                    if(resultado.Errors.Where(x => x.Code == "UserAlreadyInRole").Any())
                    {
                        usuarioDto.Admin = true;
                    }
                }
            }
            else
            {
                var resultado = await _userManager.RemoveFromRoleAsync(usuario, roleParam.Nombre);
                if (resultado.Succeeded)
                {
                    usuarioDto.Admin = false;
                }
            }

            if (usuarioDto.Admin)
            {
                var roles = new List<string>();
                roles.Add("ADMIN");
                usuarioDto.Token = _tokenService.CreateToken(usuario, roles);
            }
            else
            {
                usuarioDto.Token = _tokenService.CreateToken(usuario, null);
            }
            return usuarioDto;
        }

        [Authorize(Roles = "ADMIN")]
        [HttpGet("account/{id}")]
        public async Task<ActionResult<UsuarioDto>> GetUsuarioBy(string id)
        {
            var usuario = await _userManager.FindByIdAsync(id);
            if(usuario == null)
            {
                return NotFound(new CodeErrorResponse(404, "Usuario no encontrado"));
            }

            var roles = await _userManager.GetRolesAsync(usuario);
            return new UsuarioDto
            {
                Id = usuario.Id,
                Nombre = usuario.Nombre,
                Apellido = usuario.Apellido,
                Email = usuario.Email,
                Username = usuario.UserName,
                Imagen = usuario.Imagen,
                Admin = roles.Contains("ADMIN") ? true : false,
            };
        }

        [Authorize]
        [HttpGet]
        public async Task<ActionResult<UsuarioDto>> GetUsuario()
        {
            var usuario = await _userManager.BuscarUsuarioAsync(HttpContext.User);
            var roles = await _userManager.GetRolesAsync(usuario);

            return new UsuarioDto
            {   
                Id = usuario.Id,
                Nombre = usuario.Nombre,
                Apellido = usuario.Apellido,
                Email = usuario.Email,
                Username = usuario.UserName,
                Imagen = usuario.Imagen,
                Token = _tokenService.CreateToken(usuario, roles),
                Admin = roles.Contains("ADMIN") ? true : false,
            };
        }

        [HttpGet("emailValido")]
        public async Task<ActionResult<bool>> ValidarEmail([FromQuery] string email)
        {
            var usuario = await _userManager.FindByEmailAsync(email);

            if (usuario == null)
            {
                return false;
            }

            return true;
        }

        [Authorize]
        [HttpGet("direccion")]
        public async Task<ActionResult<DireccionDto>> GetDireccion()
        {
            var usuario = await _userManager.BuscarUsuarioConDireccionAsync(HttpContext.User);

            return _mapper.Map<Direccion, DireccionDto>(usuario.Direccion);

        }

        [Authorize]
        [HttpPut("direccion")]
        public async Task<ActionResult<DireccionDto>> UpdateDireccion(DireccionDto direccion)
        {
            var usuario = await _userManager.BuscarUsuarioConDireccionAsync(HttpContext.User);

            usuario.Direccion = _mapper.Map<DireccionDto, Direccion>(direccion);

            var resultado = await _userManager.UpdateAsync(usuario);

            if (resultado.Succeeded)
            {
                return Ok(_mapper.Map<Direccion, DireccionDto>(usuario.Direccion));
            }

            return BadRequest("No se pudo actualizar la dirección del usuario");
        }
    }
}