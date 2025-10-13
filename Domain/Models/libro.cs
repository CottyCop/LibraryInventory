using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Domain.Models;

public partial class libro
{
    [Key]
    public int ISBN { get; set; }

    public int editoriales_id { get; set; }

    [StringLength(50)]
    [Unicode(false)]
    public string titulo { get; set; } = null!;

    [Column(TypeName = "text")]
    public string sipnosis { get; set; } = null!;

    [StringLength(50)]
    [Unicode(false)]
    public string n_paginas { get; set; } = null!;

    [ForeignKey("editoriales_id")]
    [InverseProperty("libros")]
    public virtual editoriale editoriales { get; set; } = null!;

    [ForeignKey("libros_ISBN")]
    [InverseProperty("libros_ISBNs")]
    public virtual ICollection<autore> autores { get; set; } = new List<autore>();
}
