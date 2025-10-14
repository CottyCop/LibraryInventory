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
        ctx.Database.EnsureCreated(); // crea tablas/relaciones según tus entidades
        return (ctx, conn);
    }
}
