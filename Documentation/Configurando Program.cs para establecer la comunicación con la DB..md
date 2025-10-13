
Como la conexión ya fue creada, ahora es necesario registrar el `Dbcontext` para poder indicar como abrir esa "puerta" de la base de datos (DB) y así poder hacer una consulta. En este sentido, tenemos que añadir las siguientes referencias al archivo *Program.cs* que habita en el proyecto *WebApplicaiton1*.

```csharp
using Infra.Data;     
using Microsoft.EntityFrameworkCore; 
```

De esta manera, se indica donde esta el `Dbcontext` y el paquete necesario para utilizarlo. Ahora, solo resta que se *Program.cs* pueda leer la conexión, por lo que se añade las siguientes líneas en el `var builder`

```csharp
var connectionString = builder.Configuration.GetConnectionString("Default");

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(connectionString));
```

En este sentido, se esta obteniendo al conexión de *appsetting.json*, y se esta usando **AddDbContext** para crear la configuración del del contenedor (DI) para cuando se haga un llamado al constructor de la *AppDbContext*, se instancie este contexto, y se puede establecer la conexión con la DB.