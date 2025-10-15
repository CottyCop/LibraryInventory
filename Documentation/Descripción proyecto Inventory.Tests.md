
Se crea el proyecto **Inventory.Tests** para manejar pruebas locales de la aplicación, de esta manera se evita comprometer la base de datos. Este proyecto se divide en 3 clases:

- *DbContextFixture.cs*, el cual contiene el siguiente código:

``` csharp
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Infra.Data;

namespace Inventory.Tests;

internal static class DbContextFixture
{
    public static (AppDbContext ctx, SqliteConnection conn) CreateContext()
    {
        var conn = new SqliteConnection("DataSource=:memory:");
        conn.Open();

        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseSqlite(conn)
            .Options;

        var ctx = new AppDbContext(options);
        ctx.Database.EnsureCreated();
        return (ctx, conn);
    }
}
```

Se crea una conexión SQLite en memoria (`DataSource=:memory:`) y se abre; mientras esta conexión esté abierta, la base de datos vive en RAM.  
Con esa conexión se construyen las opciones del `AppDbContext`(`UseSqlite(conn)`) y se instancia el **`AppDbContext`**.  
Luego se llama a **`Database.EnsureCreated()`**, que crea el esquema(tablas, claves foráneas, índices) en la BD en memoria según el modelo de EF. Finalmente, se devuelve una tupla `(ctx, conn)` con el contexto y la conexión para usarlos en la prueba.

- *SeedHelper.cs*, el cual contiene el siguiente código:

```csharp
using Domain.Models;
using Infra.Data;

namespace Inventory.Tests;

internal static class SeedHelper
{
    public static void SeedBasic(AppDbContext ctx)
    {
    
        var e1 = new editoriale { id = 1, nombre = "Albatros", sede = "Bogotá" };
        var e2 = new editoriale { id = 2, nombre = "Monteverde", sede = "Medellín" };

        var a1 = new autore { id = 1, nombre = "Ana", apellidos = "Ramírez" };
        var a2 = new autore { id = 2, nombre = "Luis", apellidos = "González" };
        var a3 = new autore { id = 3, nombre = "María", apellidos = "Pérez" };

        var l1001 = new libro
        {
            ISBN = 1001,
            titulo = "Intro C#",
            sipnosis = "Fundamentos",
            n_paginas = "240",
            editoriales = e1
        };
        l1001.autores.Add(a1);
        l1001.autores.Add(a2);   

        var l1002 = new libro
        {
            ISBN = 1002,
            titulo = "ASP.NET Core",
            sipnosis = "Construcción APIs",
            n_paginas = "280",
            editoriales = e2
        };
        l1002.autores.Add(a2);   
        l1002.autores.Add(a3);

        ctx.libros.AddRange(l1001, l1002);
        ctx.SaveChanges();
    }
}

```

`SeedHelper.SeedBasic` crea un conjunto mínimo y estable de datos en la BD de pruebas (SQLite en memoria).  Debido a que el modelo generado por scaffold usa IDs no generados (`ValueGeneratedNever()`), el seed asigna IDs explícitos a `editoriale` y `autore`, y reutiliza las mismas instancias cuando un autor participa en más de un libro.  
Para evitar problemas de asignación doble, el método o lo agrega los libros (`ctx.libros.AddRange(...)`), permitiendo que EF Core propague inserción a editoriales, autores y la tabla puente N:M. Finalmente, `SaveChanges()` materializa todo el grafo en la BD en memoria, dejándola lista para los tests.

- *LibrosControllerTests.cs*, el cual se encarga de hacer las pruebas en memoria.

```csharp
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;

using Infra.Data;                          // AppDbContext
using Microsoft.AspNetCore.Mvc;            // OkObjectResult, NotFoundResult
using Microsoft.Data.Sqlite;               // SqliteConnection
using Microsoft.EntityFrameworkCore;       // UseSqlite, EnsureCreated, AsNoTracking
using NUnit.Framework;                     // NUnit

using WebApplication1.Controllers;         // LibrosController
using WebApplication1.Dtos;                //LibroListItemDto, LibroDetalleDto
using Domain.Models;                       
namespace Inventory.Tests
{
    
    public record PagedResultShim<T>(int Total, int Page, int PageSize, System.Collections.Generic.List<T> Items);

    [TestFixture]
    public class LibrosControllerTests
    {
        private AppDbContext _ctx = null!;
        private SqliteConnection _conn = null!;
        private LibrosController _controller = null!;

        [SetUp]
        public void SetUp()
        {
            var tuple = DbContextFixture.CreateContext();
            _ctx = tuple.ctx;
            _conn = tuple.conn;

            SeedHelper.SeedBasic(_ctx);
            _controller = new LibrosController(_ctx);
        }

        [TearDown]
        public void TearDown()
        {
            _ctx?.Dispose();
            _conn?.Dispose(); // cierra la BD en memoria
        }

        [Test]
        public async Task TodosLibros_SinFiltros_DevuelvePaginadoOk()
        {
            var result = await _controller.Get(search: null, editorialId: null, page: 1, pageSize: 1);

            Assert.That(result, Is.TypeOf<OkObjectResult>());
            var ok = (OkObjectResult)result;

            // El controlador retorna un tipo anónimo -> lo pasamos por JSON a un shim público
            var json = JsonSerializer.Serialize(ok.Value);
            var page = JsonSerializer.Deserialize<PagedResultShim<LibroListItemDto>>(
                json,
                new JsonSerializerOptions { PropertyNameCaseInsensitive = true }
            )!;

            Assert.That(page.Total, Is.EqualTo(2));     // sembramos 2 libros
            Assert.That(page.Page, Is.EqualTo(1));
            Assert.That(page.PageSize, Is.EqualTo(1));
            Assert.That(page.Items.Count, Is.EqualTo(1));
        }

        [Test]
        public async Task LibrosPorISBN_Existente_DevuelveDetalle()
        {
            var result = await _controller.GetByIsbn(1001);

            Assert.That(result, Is.TypeOf<OkObjectResult>());
            var ok = (OkObjectResult)result;

            var dto = (LibroDetalleDto)ok.Value!;

            Assert.That(dto.ISBN, Is.EqualTo(1001));               
            Assert.That(dto.Titulo.ToLower(), Does.Contain("c#"));
            Assert.That(dto.Autores, Has.Some.Contains("Ana"));
            Assert.That(dto.Autores, Has.Some.Contains("Luis"));
        }

        [Test]
        public async Task LibrosPorISBN_Inexistente_Devuelve404()
        {
            var result = await _controller.GetByIsbn(9999);
            Assert.That(result, Is.TypeOf<NotFoundResult>());
        }

        [Test]
        public async Task TodosLibros_FiltraPorEditorial()
        {
            var e1 = await _ctx.editoriales.AsNoTracking().FirstAsync(e => e.nombre == "Albatros");

            var result = await _controller.Get(search: null, editorialId: e1.id, page: 1, pageSize: 10);

            var ok = (OkObjectResult)result;

            var json = JsonSerializer.Serialize(ok.Value);
            var page = JsonSerializer.Deserialize<PagedResultShim<LibroListItemDto>>(
                json,
                new JsonSerializerOptions { PropertyNameCaseInsensitive = true }
            )!;

            Assert.That(page.Items.Count, Is.EqualTo(1)); // solo el 1001 pertenece a Albatros
        }
    }
}
```

Se encarga de realizar las pruebas de los servicios creados en *LibrosController.cs*.  Esto lo logra al hacer uso de las dos clases anteriormente creadas para generar la conexión e instanciar los datos en esa conexión. Se crea un record publico para poder manejar el contenido del método **Get(...)** en donde se obtienen todos los libros de la DB; este método devuelve un cuerpo de un objeto anónimo, por lo que es necesario este record publico `PagedResultShim` para poder acceder a los campos correspondientes. Esta compuesto de 4 tareas:

 - **TodosLibros_SinFiltros_DevuelvePaginadoOk()**: en el cual se comprueba el éxito al solicitar los libros sin ningún parámetro de búsqueda.
 - **LibrosPorISBN_Existente_DevuelveDetalle()**: Se comprueba el éxito del método **GetByISBN**, donde no es necesario hacer uso del record publico *PagedResultShim* dado que el contenido del `IActionResult` es un Dto, no un anónimo como en el caso del método **Get**
 - **LibrosPorISBN_Inexistente_Devuelve404()**: Comprueba el caso en el cual el cliente haga búsqueda de un libro que no este en el inventario.
 - **TodosLibros_FiltraPorEditorial()**: Comprueba el éxito de la búsqueda de los libros vinculados a Id de una editorial.