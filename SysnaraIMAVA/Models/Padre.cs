using System;
using System.Collections.Generic;

namespace SysnaraIMAVA.Models;

public partial class Padre
{
    public int Idimvencargado { get; set; }

    public string Idpadre { get; set; } = null!;

    public string NombrePadre { get; set; } = null!;

    public string? Parentesco { get; set; }

    public string? Profesion { get; set; }

    public string? Genero { get; set; }

    public string? TelefonoPadre { get; set; }

    public string? CelPadre { get; set; }

    public string? Correo { get; set; }

    public string? DireccionPadre { get; set; }

    public string? Observacion { get; set; }

    public int? Idaño { get; set; }

    public virtual Año? IdañoNavigation { get; set; }

    public virtual ICollection<Matricula> Matriculas { get; set; } = new List<Matricula>();
}
