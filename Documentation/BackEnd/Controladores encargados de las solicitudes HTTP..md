
Ahora, solo resta construir las solicitudes HTTP mediante los controladores.  Empezaremos por la encargada de obtener todos los libros en inventario.    

```csharp
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
```

En donde se brinda  la opción de consultar el inventario por titulo o editorial;  No es común que hayan varios libros con el mismo nombre, pero no imposible. También, se hizo uso de **Skip** y **Take** para limitar la cantidad de resultados y tener un mayor rendimiento en las consultas.

Para consultar un libro en especifico, es mas sencillo, dado que solo hay que suministrar el identificador único ISBN.

```csharp
[HttpGet("LibrosPorISBN{isbn:int}")]
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
```

