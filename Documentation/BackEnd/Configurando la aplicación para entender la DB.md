
Como ya se creo la base de datos, hay que hacer uso de ciertos paquetes para traer la información de la DB a la aplicación:

- `Microsoft.EntityFrameworkCore` → motor de EF (DbContext, DbSet, LINQ→SQL).
    
- `Microsoft.EntityFrameworkCore.SqlServer` → necesario para entender la BD SQL Server.
    
- `Microsoft.EntityFrameworkCore.Tools` →  Comandos para generar migraciones de datos. 
    
- `Microsoft.EntityFrameworkCore.Design` → soporte de diseño para Tools.

Antes de poder usar estos paquetes, hay que crear la conexión con la DB, para lo cual modificamos el archivo `appsettings.json` ubicado en `WebApplication1/appsettings.json` para establecer las credenciales para conectar con la base de datos. Para mayor seguridad, se usa UserSecrets para esconder la Direccion de conexion mostrada en el repo. 

``` bash
cd WebApplication1
dotnet user-secrets init
dotnet user-secrets set "ConnectionStrings:Default" "Server=...;Database=....;Trusted_Connection=...;TrustServerCertificate=..."
```
En donde aqui se asegura la conexión a la DB.

``` json
  "ConnectionStrings": {
    "Default": "USE-USER-SECRETS-OR-ENV"
  }
```
Y de esta manera, la conexión queda asegurada.

Una vez descargados los paquetes tanto en *Infra.Data* y *WebApplication1*,  y establecida la conexión, procedemos a ejecutar el siguiente comando en la **consola de manejo de paquetes.** Esto consiste en dar el contexto de la base de datos a EF de tal forma que se integre de manera efectiva con los archivos en la solución.

``` powershell
Scaffold-DbContext "Server=localhost;Database=PruebaTecnica1;Trusted_Connection=True;TrustServerCertificate=True" Microsoft.EntityFrameworkCore.SqlServer -Project Infra.Data -StartupProject WebApplication1 -Context AppDbContext -OutputDir Models -ContextDir . -UseDatabaseNames -DataAnnotations -NoOnConfiguring
```

En donde `Scaffold-DbContext` le ordena a EF(Entity Framework) que lea el esquema de la BD y genere código C#.

`"Server=localhost;Database=PruebaTecnica1;Trusted_Connection=True;TrustServerCertificate=True"` se encarga de indicar que DB inspeccionar 

`Microsoft.EntityFrameworkCore.SqlServer` le dice a Tools qué proveedor usar para entender SQL Server (tipos, identidad, comportamientos).

`Project Infra.Data` indica el destino de los archivos generados

`StartupProject WebApplication1` le dice a EF qué proyecto arrancar para resolver servicios de diseño y ensamblaje.


 `Context AppDbContext` Nombra  la clase `DbContext` a generar; es aquí en donde se da el contexto de la base de datos.

 `OutputDir Models` crea la carpeta relativa a *Infra.Data* donde quedarán las entidades, que no son mas que los modelos que EF usara para leer y escribir en SQl. En otras palabras, para interpretar las tablas en la base de datos.
 
`ContextDir.`  crea la carpeta para el **`AppDbContext`**. Con `.` lo deja en la raíz del proyecto *Infra.Data*

 `UseDatabaseNames` respeta exactamente los nombres de tablas/columnas 

`DataAnnotations` agrega atributos en las clases (`[Key]`, `[MaxLength]`, `[ForeignKey]`, etc.) para que el modelo sea legible. Los atributos se interpretan por `Microsoft.EntityFrameworkCore` en tiempo de ejecución.

`NoOnConfiguring`  Evita que el contexto generado incluya un método `OnConfiguring` con la cadena de conexión embebida. Dado que la conexión esta en `appsettings.json`.

Una ves se haya ejecutado el código, se debe de haber creado una carpeta 
`Models` con cada clase representando cada tabla de la DB y sus restricciones correspondientes, tales como las PK, FK, y sus tipos de datos. Por cuestiones de arquitectura, movemos la carpeta `Models` a el proyecto Domain, por lo que tenemos que cambiar el `namespace` en cada clase y la referencia en el `AppDbContext` de tal manera que quede using **Domain.Models**. 