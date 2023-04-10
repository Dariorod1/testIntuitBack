using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using testIntuit.Models;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Text.RegularExpressions;
using System.Globalization;
using testIntuit.Utils;

namespace intuiTest.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ClienteController : ControllerBase
    {

        public readonly IntuiTestContext _dbcontext;

        public ClienteController(IntuiTestContext _context)
        {
            _dbcontext = _context;
        }
        [HttpGet]
        [Route("clients")]
        public async Task<IActionResult> GetAll()
        {
            try
            {
                var clientes = await _dbcontext.Clientes.ToListAsync();
                return Ok(clientes);
            }
            catch (Exception ex)
            {
                return BadRequest(new { mensaje = "Ocurrió un error al intentar recuperar la lista de clientes" });
            }
        }
        [HttpGet]
        [Route("clients/{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            try
            {
                var cliente = await _dbcontext.Clientes.FindAsync(id);
                if (cliente == null)
                {
                    return NotFound(new { mensaje = "No se encontró el cliente con ID {id}" });
                }

                return Ok(cliente);
            }
            catch (Exception ex)
            {
                return BadRequest(new { mensaje = "Ocurrió un error al intentar recuperar el cliente con ID {id}" });
            }
        }

        [HttpGet("search")]
        public async Task<IActionResult> Search(string nombre)
        {
            if (string.IsNullOrEmpty(nombre))
            {
                return BadRequest("El parámetro de búsqueda no puede estar vacío.");
            }

            try
            {
                var clientes = await _dbcontext.Clientes
                    .Where(c => c.Nombres.Contains(nombre) || c.Apellidos.Contains(nombre))
                    .ToListAsync();

                return Ok(clientes);
            }
            catch (Exception ex)
            {
                return BadRequest($"Error al realizar la búsqueda: {ex.Message}");
            }
        }


        [HttpPost("addClient")]
        public async Task<IActionResult> addClient([FromBody] Cliente cliente)
        {
       
            if (!Validations.ValidaDatos(cliente.Nombres, cliente.Apellidos, cliente.Cuit, cliente.Telefono, cliente.Email))
            {
                return BadRequest("Los campos 'Nombres', 'Apellidos', 'CUIT', 'Teléfono celular' y 'Email' son obligatorios.");
            }

            //Validar EMAIL
            if (!Validations.ValidateEmail(cliente.Email))
            {
                return BadRequest("El email no tiene un formato válido");
            }
            //Vlidamos el CUIT
            if (!Validations.ValidateCUIT(cliente.Cuit))
            {
                return BadRequest("El CUIT no tiene un formato válido");
            }

            // Validar que el email y el cuit no existan en la base de datos
            if (await _dbcontext.Clientes.AnyAsync(c => c.Email == cliente.Email))
            {
                return BadRequest("El email ya está registrado.");
            }

            if (await _dbcontext.Clientes.AnyAsync(c => c.Cuit == cliente.Cuit))
            {
                return BadRequest("El CUIT ya está registrado.");
            }

            try
            {
                // Agregamos el cliente a la base de datos
                _dbcontext.Clientes.Add(cliente);
                await _dbcontext.SaveChangesAsync();

                return Ok(cliente);
            }
            catch (Exception ex)
            {
                // Si algo falla, devolvemos un status 500 (Internal Server Error)
                return StatusCode(500, $"Ocurrió un error al agregar el cliente: {ex.Message}");
            }
        }

        [HttpPut("updateClient/{id}")]
        public async Task<IActionResult> updateClient(int id, [FromBody] Cliente clienteAct)
        {
            // Buscamos el cliente por id
            var cliente = await _dbcontext.Clientes.FindAsync(id);

            if (cliente == null)
            {
                return NotFound("El cliente con el Id {id}"); // Devolvemos un error 404 si no encontramos el cliente
            }


            if (!Validations.ValidaDatos(clienteAct.Nombres, clienteAct.Apellidos, clienteAct.Cuit, clienteAct.Telefono, clienteAct.Email))
            {
                return BadRequest("Los campos 'Nombres', 'Apellidos', 'CUIT', 'Teléfono celular' y 'Email' son obligatorios.");
            }

            //Validar EMAIL
            if (!Validations.ValidateEmail(clienteAct.Email))
            {
                return BadRequest("El email no tiene un formato válido");
            }
            //Vlidamos el CUIT
            if (!Validations.ValidateCUIT(clienteAct.Cuit))
            {
                return BadRequest("El CUIT no tiene un formato válido");
            }
            // Actualizamos los datos del cliente con los nuevos datos
            cliente.Nombres = clienteAct.Nombres;
            cliente.Apellidos = clienteAct.Apellidos;
            cliente.FechaNac = clienteAct.FechaNac;
            cliente.Domicilio = clienteAct.Domicilio;
            cliente.Telefono = clienteAct.Telefono;
            cliente.Email = clienteAct.Email;
            cliente.Cuit = clienteAct.Cuit;

            try
            {
                await _dbcontext.SaveChangesAsync(); // Guardamos los cambios en la base de datos
                return Ok(cliente); // Devolvemos el cliente actualizado
            }
            catch (DbUpdateException)
            {
                // Si ocurre un error al guardar los cambios en la base de datos, devolvemos un error 500
                return StatusCode(StatusCodes.Status500InternalServerError, "Error al actualizar el cliente");
            }
        }

        [HttpDelete("delete/{id}")]
        public async Task<IActionResult> deleteClient(int id)
        {
            try
            {
                var cliente = await _dbcontext.Clientes.FindAsync(id);

                if (cliente == null)
                {
                    return NotFound();
                }

                _dbcontext.Clientes.Remove(cliente);
                await _dbcontext.SaveChangesAsync();

                return Ok(new { mensaje = "El cliente fue borrado correctamente", cliente });
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

    }
}
