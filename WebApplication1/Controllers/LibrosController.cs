using Domain.Models;              
using Infra.Data;                   
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore; 
using WebApplication1.Dtos;
using System.Linq;


namespace WebApplication1.Controllers;

[ApiController]
[Route("Inventario/[controller]")]
public class LibrosController : ControllerBase
{
    private readonly AppDbContext _ctx;

    public LibrosController(AppDbContext ctx)
    {
        _ctx = ctx; // DI te inyecta el DbContext registrado en Program.cs
    }

   
    [HttpGet("TodosLibros")]
    public async Task<IActionResult> Get([FromQuery] string? search, [FromQuery] int? editorialId,
                                     [FromQuery] int page = 1, [FromQuery] int pageSize = 20)
    {
        // IQueryable: ejecución diferida (EF Core)
        var q = _ctx.libros.AsNoTracking(); // Microsoft.EntityFrameworkCore

        if (!string.IsNullOrWhiteSpace(search))
            q = q.Where(l => l.titulo.Contains(search));             // LINQ

        if (editorialId.HasValue)
            q = q.Where(l => l.editoriales_id == editorialId.Value); // LINQ

        var total = await q.CountAsync();                            // ejecuta COUNT (EF Core)

        var items = await q.OrderBy(l => l.ISBN)                     // orden estable
                           .Skip((page - 1) * pageSize)
                           .Take(pageSize)
                           // Proyección directa a DTO (trae solo columnas necesarias)
                           .Select(l => new LibroListItemDto(
                               l.ISBN,
                               l.titulo,
                               l.editoriales.nombre                  // navegación 1:N
                           ))
                           .ToListAsync();                           // ejecuta SELECT (EF Core)

        return Ok(new { total, page, pageSize, items });
    }

    [HttpGet("LibrosPorISBN/{isbn:int}")]
    public async Task<IActionResult> GetByIsbn(int isbn)
    {
        // Proyección directa a DTO de detalle (sin materializar entidades completas)
        var dto = await _ctx.libros.AsNoTracking()
            .Where(l => l.ISBN == isbn)
            .Select(l => new LibroDetalleDto(
                l.ISBN,
                l.titulo,
                l.sipnosis,
                l.n_paginas,
                l.editoriales.nombre,
                l.autores.Select(a => a.nombre + " " + a.apellidos).ToList()
            ))
            .FirstOrDefaultAsync(); // ejecuta SELECT (EF Core)

        if (dto is null) return NotFound();

        return Ok(dto);
    }

}
