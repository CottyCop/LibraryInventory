using Domain.Models;
using Infra.Data;

namespace Inventory.Tests;

internal static class SeedHelper
{
    public static void SeedBasic(AppDbContext ctx)
    {
        // IDs explícitos (por mapeo ValueGeneratedNever en scaffold)
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
        l1001.autores.Add(a2);   // MISMA instancia

        var l1002 = new libro
        {
            ISBN = 1002,
            titulo = "ASP.NET Core",
            sipnosis = "Construcción APIs",
            n_paginas = "280",
            editoriales = e2
        };
        l1002.autores.Add(a2);   // MISMA instancia (no clones)
        l1002.autores.Add(a3);

        // 👇 Solo agregamos libros; EF insertará todo el grafo y la N:M
        ctx.libros.AddRange(l1001, l1002);
        ctx.SaveChanges();
    }
}
