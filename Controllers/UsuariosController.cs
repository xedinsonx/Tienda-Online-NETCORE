using Microsoft.AspNetCore.Mvc;
using CrudDemoPro.Data;
using CrudDemoPro.Models;
using Microsoft.EntityFrameworkCore;

namespace CrudDemoPro.Controllers;

[ApiController]
[Route("api/[controller]")]
public class UsuariosController : ControllerBase
{
    private readonly AppDbContext _context;

    public UsuariosController(AppDbContext context)
    {
        _context = context;
    }

    // ✅ GET all → devuelve lista (aunque esté vacía)
    [HttpGet]
    public async Task<ActionResult<IEnumerable<Usuario>>> GetAll()
    {
        return await _context.Usuarios.ToListAsync();
    }

    [HttpGet("{id}")]
    public IActionResult GetById(int id)
    {
        var usuario = _context.Usuarios.Find(id);
        return usuario == null ? NotFound() : Ok(usuario);
    }

    [HttpPost]
    public IActionResult Create(Usuario usuario)
    {
        _context.Usuarios.Add(usuario);
        _context.SaveChanges();
        return CreatedAtAction(nameof(GetById), new { id = usuario.Id }, usuario);
    }

    [HttpPut("{id}")]
    public IActionResult Update(int id, Usuario usuario)
    {
        var existing = _context.Usuarios.Find(id);
        if (existing == null) return NotFound();

        existing.Nombre = usuario.Nombre;
        existing.Email = usuario.Email;
        _context.SaveChanges();
        return Ok(existing);
    }

    [HttpDelete("{id}")]
    public IActionResult Delete(int id)
    {
        var usuario = _context.Usuarios.Find(id);
        if (usuario == null) return NotFound();

        _context.Usuarios.Remove(usuario);
        _context.SaveChanges();
        return NoContent();
    }
}
