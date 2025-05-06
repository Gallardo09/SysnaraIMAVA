using System;
using System.Collections.Generic;

namespace SysnaraIMAVA.Models;

public partial class Grado
{
    public int? Idaño { get; set; }

    public string Idgrado { get; set; } = null!;

    public string Grado1 { get; set; } = null!;

    public string? Seccion { get; set; }

    public string? Jornada { get; set; }

    public string? NivelDescripcion { get; set; }

    public virtual ICollection<Asignatura> Asignaturas { get; set; } = new List<Asignatura>();

    public virtual Año? IdañoNavigation { get; set; }

    public virtual ICollection<Matricula> Matriculas { get; set; } = new List<Matricula>();

    public virtual ICollection<Nota> Nota { get; set; } = new List<Nota>();

    public virtual ICollection<PersonalidadIiip> PersonalidadIiips { get; set; } = new List<PersonalidadIiip>();

    public virtual ICollection<PersonalidadIip> PersonalidadIips { get; set; } = new List<PersonalidadIip>();

    public virtual ICollection<PersonalidadIvp> PersonalidadIvps { get; set; } = new List<PersonalidadIvp>();

    public virtual ICollection<Personalidad> Personalidads { get; set; } = new List<Personalidad>();
}
