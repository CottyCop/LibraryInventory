
Se crea el proyecto **Inventory.Tests** para manejar pruebas locales de la aplicación, de esta manera se evita comprometer la base de datos. Este proyecto se divide en 3 clases:

*DbContextFixture.cs*, el cual contiene el siguiente código:

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