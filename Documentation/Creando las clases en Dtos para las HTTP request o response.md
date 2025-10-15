Como solo se esta exigiendo que una biblioteca haga manejo de su inventario, se crearan dos solicitudes Get: 

- Una para obtener los libros en la biblioteca
- Una para obtener todo los detalles de un libro en especifico.

Lo primero es crear los `Dtos` para establecer esa información que se va a obtener de la DB. Estos fueron creados en `WebApplication1.Dtos` de la siguiente manera.

```csharp
namespace WebApplication1.Dtos;

public record LibroDetalleDto(
    int ISBN,
    string Titulo,
    string Sipnosis,
    string NumeroPaginas,
    string Editorial,
    List<string> Autores
);
```

Este es el encargado de obtener todos los detalles de ese libro en especifico identificado con su *ISBN*

```csharp
namespace WebApplication1.Dtos;

public record LibroListItemDto(
    int ISBN,
    string Titulo,
    string Editorial
);
```

Y este el encargado para mostrar todos los libros en inventario.
De esta manera, al generar una solicitud HTTP, se harán usos de estas clases presentes en la carpeta `Dtos` para mapear esa información a la entidad correspondiente en `Domain.Models`, la cual será tratada de manera adecuada por EF y hacer las consultas correspondiente en la DB.

