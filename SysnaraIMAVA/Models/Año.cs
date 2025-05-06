using System;
using System.Collections.Generic;

namespace SysnaraIMAVA.Models;

public partial class Año
{
    public int Idaño { get; set; }

    public DateOnly AñoInicio { get; set; }

    public DateOnly AñoFin { get; set; }

    public string? Observacion { get; set; }

    public virtual ICollection<Asignatura> Asignaturas { get; set; } = new List<Asignatura>();

    public virtual ICollection<Docente> Docentes { get; set; } = new List<Docente>();

    public virtual ICollection<Grado> Grados { get; set; } = new List<Grado>();

    public virtual ICollection<Matricula> Matriculas { get; set; } = new List<Matricula>();

    public virtual ICollection<Nota> Nota { get; set; } = new List<Nota>();

    public virtual ICollection<PersonalidadIiip> PersonalidadIiips { get; set; } = new List<PersonalidadIiip>();

    public virtual ICollection<PersonalidadIip> PersonalidadIips { get; set; } = new List<PersonalidadIip>();

    public virtual ICollection<PersonalidadIvp> PersonalidadIvps { get; set; } = new List<PersonalidadIvp>();

    public virtual ICollection<Personalidad> Personalidads { get; set; } = new List<Personalidad>();
}
