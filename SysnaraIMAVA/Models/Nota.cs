using System;
using System.Collections.Generic;

namespace SysnaraIMAVA.Models;

public partial class Nota
{
    public int? Idaño { get; set; }

    public int? Idest { get; set; }

    public string Idestudiante { get; set; } = null!;

    public string NombreEstudiante { get; set; } = null!;

    public string? Genero { get; set; }

    public string? Idasignatura { get; set; }

    public string? Asignatura { get; set; }

    public string? Idgrado { get; set; }

    public string Grado { get; set; } = null!;

    public string? Seccion { get; set; }

    public string? Jornada { get; set; }

    public int Idnota { get; set; }

    public int? Iparcial { get; set; }

    public int? Iiparcial { get; set; }

    public int? Iiiparcial { get; set; }

    public int? Ivparcial { get; set; }

    public int? Isemestre { get; set; }

    public int? Iisemestre { get; set; }

    public int? RecuperacionI { get; set; }

    public int? RecuperacionIi { get; set; }

    public int? PromedioFinal { get; set; }

    public int? IndicePromocion { get; set; }

    public virtual Asignatura? IdasignaturaNavigation { get; set; }

    public virtual Año? IdañoNavigation { get; set; }

    public virtual Matricula? IdestNavigation { get; set; }

    public virtual Grado? IdgradoNavigation { get; set; }
}
