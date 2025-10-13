using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Domain.Models;

public partial class autore
{
    [Key]
    public int id { get; set; }

    [StringLength(50)]
    [Unicode(false)]
    public string nombre { get; set; } = null!;

    [StringLength(50)]
    public string apellidos { get; set; } = null!;

    [ForeignKey("autores_id")]
    [InverseProperty("autores")]
    public virtual ICollection<libro> libros_ISBNs { get; set; } = new List<libro>();
}
