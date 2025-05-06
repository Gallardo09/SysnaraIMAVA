using System;
using System.Collections.Generic;

namespace SysnaraIMAVA.Models;

public partial class Asignatura
{
    public int? Idaño { get; set; }

    public string? Idgrado { get; set; }

    public string Grado { get; set; } = null!;

    public string? Seccion { get; set; }

    public string? Jornada { get; set; }

    public string? NivelDescripcion { get; set; }

    public string Idasignatura { get; set; } = null!;

    public string? Asignatura1 { get; set; }

    public string? Periodo { get; set; }

    public virtual Año? IdañoNavigation { get; set; }

    public virtual Grado? IdgradoNavigation { get; set; }

    public virtual ICollection<Nota> Nota { get; set; } = new List<Nota>();
}
