using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Domain.Models;

public partial class editoriale
{
    [Key]
    public int id { get; set; }

    [StringLength(50)]
    [Unicode(false)]
    public string nombre { get; set; } = null!;

    [StringLength(50)]
    [Unicode(false)]
    public string sede { get; set; } = null!;

    [InverseProperty("editoriales")]
    public virtual ICollection<libro> libros { get; set; } = new List<libro>();
}
