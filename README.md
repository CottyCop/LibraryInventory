# Inventario de Libros – ASP.NET Core + EF Core

API para listar libros y ver detalle por ISBN, basada en el MER (Libros, Autores, Editoriales) y con relación N:M mediante tabla puente.

## Arquitectura

- **WebApplication1**: API ASP.NET Core (controladores, DTOs, Swagger).
- **Domain**: Entidades EF (clases que representan tablas y navegaciones).
- **Infra.Data**: `AppDbContext` y configuración EF. 
- **Documentation**: Documentación sobre el proceso de creacion de la Appi; la creacion de la DB, de las entidades,
controladores, y el contexto de la DB (exportada desde Obsidian).
- **Inventory.Tests**: Proyecto para pruebas locales de la aplicación para no comprometer la base de datos con algun error.
- 

## Requisitos

- .NET SDK (8.0 o el que uses en el proyecto)
- SQL Server (Developer/LocalDB)

