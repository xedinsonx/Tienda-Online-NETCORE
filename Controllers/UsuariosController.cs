using Microsoft.AspNetCore.Mvc;
using CrudDemoPro.Data;
using CrudDemoPro.Infrastructure;
using CrudDemoPro.Models;
using Microsoft.EntityFrameworkCore;

namespace CrudDemoPro.Controllers;

[ApiController]
[Route("api/[controller]")]
public class UsuariosController : ControllerBase
{
    private readonly AppDbContext _context;
    private readonly IKafkaEventPublisher _eventPublisher;

    public UsuariosController(AppDbContext context, IKafkaEventPublisher eventPublisher)
    {
        _context = context;
        _eventPublisher = eventPublisher;
    }

    // ✅ GET all → devuelve lista (aunque esté vacía)
    [HttpGet]
    public async Task<ActionResult<IEnumerable<Usuario>>> GetAll()
    {
        return await _context.Usuarios.ToListAsync();
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(int id)
    {
        var usuario = await _context.Usuarios.FindAsync(id);
        return usuario == null ? NotFound() : Ok(usuario);
    }

    [HttpPost]
    public async Task<IActionResult> Create(Usuario usuario)
    {
        _context.Usuarios.Add(usuario);
        await _context.SaveChangesAsync();
        await _eventPublisher.PublishAsync("usuario.created", usuario);
        return CreatedAtAction(nameof(GetById), new { id = usuario.Id }, usuario);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Update(int id, Usuario usuario)
    {
        var existing = await _context.Usuarios.FindAsync(id);
        if (existing == null) return NotFound();

        existing.Nombre = usuario.Nombre;
        existing.Email = usuario.Email;
        await _context.SaveChangesAsync();
        await _eventPublisher.PublishAsync("usuario.updated", existing);
        return Ok(existing);
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        var usuario = await _context.Usuarios.FindAsync(id);
        if (usuario == null) return NotFound();

        _context.Usuarios.Remove(usuario);
        await _context.SaveChangesAsync();
        await _eventPublisher.PublishAsync("usuario.deleted", usuario);
        return NoContent();
    }
}
