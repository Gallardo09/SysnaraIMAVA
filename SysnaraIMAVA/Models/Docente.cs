using System;
using System.Collections.Generic;

namespace SysnaraIMAVA.Models;

public partial class Docente
{
    public int? Idaño { get; set; }

    public int Idpk { get; set; }

    public string Iddocente { get; set; } = null!;

    public string NombreCompleto { get; set; } = null!;

    public string? Genero { get; set; }

    public string? Telefono { get; set; }

    public string? Celular { get; set; }

    public string? Correo { get; set; }

    public string? Direccion { get; set; }

    public string? Profesion { get; set; }

    public virtual Año? IdañoNavigation { get; set; }
}
