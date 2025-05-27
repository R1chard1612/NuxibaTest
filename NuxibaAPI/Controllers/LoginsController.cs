using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NuxibaAPI.Data;
using NuxibaAPI.DTOs;
using NuxibaAPI.Models;
using System.Runtime.CompilerServices;

namespace NuxibaAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class LoginsController : ControllerBase
    {
        private readonly NuxibaContext _context; 

        public LoginsController(NuxibaContext context)
        {
            _context = context;
        }

        //GET: api/logins
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Login>>> GetLogins()
        {
            return await _context.Logins.ToListAsync();
        }

        [HttpPost]
        public async Task<ActionResult<Login>> PostLogin(CreateLoginDto loginDto)
        {
            // Primero se debe validar si existe o no el usuario en ccUsers
            var userExist = await _context.Set<User>().AnyAsync(u => u.User_id == loginDto.User_id);

            if (!userExist)
            {
                return BadRequest($"El usuario {loginDto.User_id} no existe, validar...");
            }

            // Validamos si el ultimo registro del user es un logout
            var lastMovement = await _context.Logins
                .Where(l => l.User_id == loginDto.User_id)
                .OrderByDescending(l => l.Fecha)
                .FirstOrDefaultAsync();

            if (loginDto.TipoMov == 1 && lastMovement?.TipoMov == 1)
            {
                return BadRequest("Ya estas logeado, no hay un logout registado");
            }

            if(loginDto.TipoMov == 0 && (lastMovement?.TipoMov == null || lastMovement.TipoMov == 0))
            {
                return BadRequest("La sesion ya esta cerrada, no hay un login previo");
            }

            // Para evitar errores de fecha el servidor generara la fecha al momento
            //if (login.Fecha > DateTime.UtcNow.AddHours(-6))
            //{
            //    return BadRequest("La fecha no puede ser mayor a la actual...");
            //}

            var login = new Login
            {
                User_id = loginDto.User_id,
                Extension = loginDto.Extension,
                TipoMov = loginDto.TipoMov,
                Fecha = DateTime.UtcNow.AddHours(-6)
            };

            // Si se llega aqui es que se superaron las validaciones, procedemos a guardar el registro
            _context.Logins.Add(login);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetLogins), new { id = login.User_id }, login);
        }

        // Permite actualizar un registro de login/logout (Por defecto borrara el ultimo registro asociado al userId proporcionado)
        [HttpPut("{userId}")]
        public async Task<IActionResult> PutLogin(int userId, UpdateLoginDto loginDto)
        {
            // Recuperamos el ultimo login/logout para ese userId
            var login = await _context.Logins
                .Where(l => l.User_id == userId)
                .OrderByDescending(l => l.Fecha)
                .FirstOrDefaultAsync();

            if(login == null)
            {
                return NotFound($"No se encontro nigun registro para el userId -> {userId}");
            }

            // Aplicamos los cambios
            login.TipoMov = loginDto.TipoMov;
            login.Fecha = DateTime.UtcNow.AddHours(-6);

            await _context.SaveChangesAsync();

            return NoContent();
        }

        //Elimina un registro de login/logout configurado para borrar el ultimo registro asociado al userId proporcionado
        [HttpDelete("{userId}")]
        public async Task<IActionResult> DeleteLogin(int userId)
        {
            // Recuperamos el ultimo login/logout para ese userId
            var login = await _context.Logins
                .Where(l => l.User_id == userId)
                .OrderByDescending(l => l.Fecha)
                .FirstOrDefaultAsync();

            if (login == null)
            {
                return NotFound($"No hay registros asociados para el userId proporcionado -> {userId}");
            }

            _context.Logins.Remove(login);
            await _context.SaveChangesAsync();

            return NoContent();
        }

    }
}
