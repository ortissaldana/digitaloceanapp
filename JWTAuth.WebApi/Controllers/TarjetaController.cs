using JWTAuth.WebApi.Interface;
using JWTAuth.WebApi.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Threading.Tasks;

namespace JWTAuth.WebApi.Controllers
{
    //[Authorize]
    [Route("api/tarjeta")]
    [ApiController]
    public class TarjetaController : ControllerBase
    {
        private readonly ITarjetas _ITarjeta;

        public TarjetaController(ITarjetas ITarjeta)
        {
            _ITarjeta = ITarjeta;
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Tarjetas>> Get(int id)
        {
            var tarjeta = await Task.FromResult(_ITarjeta.GetTarjetasDetails(id));
            if (tarjeta == null)
            {
                return NotFound();
            }
            return tarjeta;
        }
    }
}
