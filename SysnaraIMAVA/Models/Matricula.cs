using System;
using System.Collections.Generic;

namespace SysnaraIMAVA.Models;

public partial class Matricula
{
    public int? Idaño { get; set; }

    public int? Idimvencargado { get; set; }

    public string Idpadre { get; set; } = null!;

    public string NombrePadre { get; set; } = null!;

    public string? Parentesco { get; set; }

    public int Idest { get; set; }

    public string? Idimv { get; set; }

    public string Idestudiante { get; set; } = null!;

    public string? Nacionalidad { get; set; }

    public string NombreEstudiante { get; set; } = null!;

    public DateOnly FechaNacimiento { get; set; }

    public string? LugarNacimiento { get; set; }

    public string? Genero { get; set; }

    public string? TipoSangre { get; set; }

    public string? Alergia { get; set; }

    public string? Edad { get; set; }

    public string? TelefonoEstudiante { get; set; }

    public string? CelularEstudiante { get; set; }

    public string? Correo { get; set; }

    public string? Direccion { get; set; }

    public string? Estado { get; set; }

    public string? Idgrado { get; set; }

    public string Grado { get; set; } = null!;

    public string? Seccion { get; set; }

    public string? Jornada { get; set; }

    public string? Proviene { get; set; }

    public string? Beca { get; set; }

    public string? RepiteAño { get; set; }

    public string? Observacion { get; set; }

    public string? NivelDescripcion { get; set; }

    public string? TelefonoPadre { get; set; }

    public string? CelPadre { get; set; }

    public string? DireccionPadre { get; set; }

    public string? VacunasCovid { get; set; }

    public string? EstadoMatricula { get; set; }

    public DateOnly? FechaIngreso { get; set; }

    public virtual Año? IdañoNavigation { get; set; }

    public virtual Grado? IdgradoNavigation { get; set; }

    public virtual Padre? IdimvencargadoNavigation { get; set; }

    public virtual ICollection<Nota> Nota { get; set; } = new List<Nota>();

    public virtual ICollection<PersonalidadIiip> PersonalidadIiips { get; set; } = new List<PersonalidadIiip>();

    public virtual ICollection<PersonalidadIip> PersonalidadIips { get; set; } = new List<PersonalidadIip>();

    public virtual ICollection<PersonalidadIvp> PersonalidadIvps { get; set; } = new List<PersonalidadIvp>();

    public virtual ICollection<Personalidad> Personalidads { get; set; } = new List<Personalidad>();
}
