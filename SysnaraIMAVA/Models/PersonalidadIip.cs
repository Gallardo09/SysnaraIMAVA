using System;
using System.Collections.Generic;

namespace SysnaraIMAVA.Models;

public partial class PersonalidadIip
{
    public int? Idaño { get; set; }

    public int? Idest { get; set; }

    public string Idestudiante { get; set; } = null!;

    public string NombreEstudiante { get; set; } = null!;

    public string? Genero { get; set; }

    public string? Idgrado { get; set; }

    public string Grado { get; set; } = null!;

    public string? Seccion { get; set; }

    public string? Jornada { get; set; }

    public int IdpersonalidadIi { get; set; }

    public string? Puntualidad { get; set; }

    public string? OrdenYPresentacion { get; set; }

    public string? Moralidad { get; set; }

    public string? EspirituDeTrabajo { get; set; }

    public string? Sociabilidad { get; set; }

    public int? Inasistencias { get; set; }

    public virtual Año? IdañoNavigation { get; set; }

    public virtual Matricula? IdestNavigation { get; set; }

    public virtual Grado? IdgradoNavigation { get; set; }
}
