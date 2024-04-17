using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using JWTAuth.WebApi.Interface;
using JWTAuth.WebApi.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace JWTAuth.WebApi.Controllers
{
    //[Authorize]
    [Route("api/ventas")]
    [ApiController]
    public class VentasController : ControllerBase
    {
        private readonly IAVentas _IVenta;
        private readonly DatabaseContext _context;

        public VentasController(IAVentas IVenta, DatabaseContext context)
        {
            _IVenta = IVenta;
            _context = context;
        }

        [Route("showAll")]
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Ventas>>> Get()
        {
            return await Task.FromResult(_IVenta.GetVentasDetails());
        }

        [Route("add")]
        [HttpPost]
        public async Task<ActionResult<LoginUserModelo>> Post2(Ventas venta)
        {
            LoginUserModelo respuesta = new();
            _IVenta.AddVentas(venta);
            respuesta.Estatus = "Añadida exitosamente";
            return await Task.FromResult(respuesta);
        }

        [Route("add2")]
        [HttpPost]
        public async Task<ActionResult<LoginUserModelo>> GetUserñ(User id)
        {
            LoginUserModelo respuesta = new();
            var user1 = await GetUser(id.Email);

            if (user1 != null)
            {
                respuesta.Estatus = user1.Email;
                return await Task.FromResult(respuesta);
            }
            else
            {
                respuesta.Estatus = "Datos incorrectos";
                return await Task.FromResult(respuesta);
            }
        }

        private async Task<User> GetUser(string email)
        {
            return await _context.Users.FirstOrDefaultAsync(u => u.Email == email);
        }
    }
}
