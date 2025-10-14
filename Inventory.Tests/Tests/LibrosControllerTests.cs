using System;
// LibrosControllerTests.cs
// Inventory.Tests/Tests/LibrosControllerTests.cs

using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;

using Infra.Data;                          // AppDbContext
using Microsoft.AspNetCore.Mvc;            // OkObjectResult, NotFoundResult
using Microsoft.Data.Sqlite;               // SqliteConnection
using Microsoft.EntityFrameworkCore;       // UseSqlite, EnsureCreated, AsNoTracking
using NUnit.Framework;                     // NUnit

using WebApplication1.Controllers;         // LibrosController
using WebApplication1.Dtos;                // LibroListItemDto, LibroDetalleDto
using Domain.Models;                       // editoriale, autore, libro

namespace Inventory.Tests
{
    // ---------- Fixture: DbContext con SQLite InMemory ----------
    

    // ---------- Seed: datos de prueba ----------
   

    // ---------- Shim solo para TESTS: forma del objeto anónimo del listado ----------
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

            // El detalle retorna un LibroDetalleDto concreto -> casteo directo
            var dto = (LibroDetalleDto)ok.Value!;

            Assert.That(dto.ISBN, Is.EqualTo(1001));               // OJO: propiedad es "ISBN" (mayúsculas)
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

