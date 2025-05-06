using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace SysnaraIMAVA.Models;

public partial class DbimavaContext : DbContext
{
    public DbimavaContext()
    {
    }

    public DbimavaContext(DbContextOptions<DbimavaContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Asignatura> Asignaturas { get; set; }

    public virtual DbSet<Año> Años { get; set; }

    public virtual DbSet<Docente> Docentes { get; set; }

    public virtual DbSet<Grado> Grados { get; set; }

    public virtual DbSet<Matricula> Matriculas { get; set; }

    public virtual DbSet<Nota> Notas { get; set; }

    public virtual DbSet<Padre> Padres { get; set; }

    public virtual DbSet<Personalidad> Personalidads { get; set; }

    public virtual DbSet<PersonalidadIiip> PersonalidadIiips { get; set; }

    public virtual DbSet<PersonalidadIip> PersonalidadIips { get; set; }

    public virtual DbSet<PersonalidadIvp> PersonalidadIvps { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see https://go.microsoft.com/fwlink/?LinkId=723263.
        => optionsBuilder.UseSqlServer("Data Source=.\\SQLExpress;Initial Catalog=DBIMAVA;Integrated Security=True;TrustServerCertificate=True");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Asignatura>(entity =>
        {
            entity.HasKey(e => e.Idasignatura).HasName("PK__ASIGNATU__2E8CCF3588862EF8");

            entity.ToTable("ASIGNATURA");

            entity.Property(e => e.Idasignatura)
                .HasMaxLength(30)
                .IsUnicode(false)
                .HasColumnName("IDAsignatura");
            entity.Property(e => e.Asignatura1)
                .HasMaxLength(100)
                .IsUnicode(false)
                .HasColumnName("Asignatura");
            entity.Property(e => e.Grado)
                .HasMaxLength(15)
                .IsUnicode(false);
            entity.Property(e => e.Idaño).HasColumnName("IDAño");
            entity.Property(e => e.Idgrado)
                .HasMaxLength(20)
                .IsUnicode(false)
                .HasColumnName("IDGrado");
            entity.Property(e => e.Jornada)
                .HasMaxLength(15)
                .IsUnicode(false);
            entity.Property(e => e.NivelDescripcion)
                .HasMaxLength(100)
                .IsUnicode(false)
                .HasColumnName("Nivel_Descripcion");
            entity.Property(e => e.Periodo)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.Seccion)
                .HasMaxLength(15)
                .IsUnicode(false);

            entity.HasOne(d => d.IdañoNavigation).WithMany(p => p.Asignaturas)
                .HasForeignKey(d => d.Idaño)
                .HasConstraintName("FK__ASIGNATUR__IDAño__173876EA");

            entity.HasOne(d => d.IdgradoNavigation).WithMany(p => p.Asignaturas)
                .HasForeignKey(d => d.Idgrado)
                .HasConstraintName("FK__ASIGNATUR__IDGra__182C9B23");
        });

        modelBuilder.Entity<Año>(entity =>
        {
            entity.HasKey(e => e.Idaño).HasName("PK__AÑO__931DA792FF217A53");

            entity.ToTable("AÑO");

            entity.Property(e => e.Idaño)
                .ValueGeneratedNever()
                .HasColumnName("IDAño");
            entity.Property(e => e.AñoFin).HasColumnName("Año_Fin");
            entity.Property(e => e.AñoInicio).HasColumnName("Año_Inicio");
            entity.Property(e => e.Observacion)
                .HasMaxLength(50)
                .IsUnicode(false);
        });

        modelBuilder.Entity<Docente>(entity =>
        {
            entity.HasKey(e => e.Idpk).HasName("PK__DOCENTE__B87C5B048DD81A3B");

            entity.ToTable("DOCENTE");

            entity.Property(e => e.Idpk).HasColumnName("IDPK");
            entity.Property(e => e.Celular)
                .HasMaxLength(10)
                .IsUnicode(false);
            entity.Property(e => e.Correo)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.Direccion)
                .HasMaxLength(100)
                .IsUnicode(false);
            entity.Property(e => e.Genero)
                .HasMaxLength(15)
                .IsUnicode(false);
            entity.Property(e => e.Idaño).HasColumnName("IDAño");
            entity.Property(e => e.Iddocente)
                .HasMaxLength(15)
                .IsUnicode(false)
                .HasColumnName("IDDocente");
            entity.Property(e => e.NombreCompleto)
                .HasMaxLength(100)
                .IsUnicode(false)
                .HasColumnName("Nombre_Completo");
            entity.Property(e => e.Profesion)
                .HasMaxLength(100)
                .IsUnicode(false);
            entity.Property(e => e.Telefono)
                .HasMaxLength(10)
                .IsUnicode(false);

            entity.HasOne(d => d.IdañoNavigation).WithMany(p => p.Docentes)
                .HasForeignKey(d => d.Idaño)
                .HasConstraintName("FK__DOCENTE__IDAño__1B0907CE");
        });

        modelBuilder.Entity<Grado>(entity =>
        {
            entity.HasKey(e => e.Idgrado).HasName("PK__GRADO__CEDFC9F716ED42D1");

            entity.ToTable("GRADO");

            entity.Property(e => e.Idgrado)
                .HasMaxLength(20)
                .IsUnicode(false)
                .HasColumnName("IDGrado");
            entity.Property(e => e.Grado1)
                .HasMaxLength(15)
                .IsUnicode(false)
                .HasColumnName("Grado");
            entity.Property(e => e.Idaño).HasColumnName("IDAño");
            entity.Property(e => e.Jornada)
                .HasMaxLength(15)
                .IsUnicode(false);
            entity.Property(e => e.NivelDescripcion)
                .HasMaxLength(100)
                .IsUnicode(false)
                .HasColumnName("Nivel_Descripcion");
            entity.Property(e => e.Seccion)
                .HasMaxLength(15)
                .IsUnicode(false);

            entity.HasOne(d => d.IdañoNavigation).WithMany(p => p.Grados)
                .HasForeignKey(d => d.Idaño)
                .HasConstraintName("FK__GRADO__IDAño__145C0A3F");
        });

        modelBuilder.Entity<Matricula>(entity =>
        {
            entity.HasKey(e => e.Idest).HasName("PK__MATRICUL__9229877DEEAAA7C2");

            entity.ToTable("MATRICULA");

            entity.Property(e => e.Idest).HasColumnName("IDEst");
            entity.Property(e => e.Alergia)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.Beca)
                .HasMaxLength(20)
                .IsUnicode(false);
            entity.Property(e => e.CelPadre)
                .HasMaxLength(10)
                .IsUnicode(false)
                .HasColumnName("Cel_Padre");
            entity.Property(e => e.CelularEstudiante)
                .HasMaxLength(10)
                .IsUnicode(false)
                .HasColumnName("Celular_Estudiante");
            entity.Property(e => e.Correo)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.Direccion)
                .HasMaxLength(100)
                .IsUnicode(false);
            entity.Property(e => e.DireccionPadre)
                .HasMaxLength(100)
                .IsUnicode(false)
                .HasColumnName("Direccion_Padre");
            entity.Property(e => e.Edad)
                .HasMaxLength(2)
                .IsUnicode(false);
            entity.Property(e => e.Estado)
                .HasMaxLength(10)
                .IsUnicode(false);
            entity.Property(e => e.EstadoMatricula)
                .HasMaxLength(20)
                .IsUnicode(false)
                .HasColumnName("Estado_Matricula");
            entity.Property(e => e.FechaIngreso).HasColumnName("Fecha_Ingreso");
            entity.Property(e => e.FechaNacimiento).HasColumnName("Fecha_Nacimiento");
            entity.Property(e => e.Genero)
                .HasMaxLength(15)
                .IsUnicode(false);
            entity.Property(e => e.Grado)
                .HasMaxLength(15)
                .IsUnicode(false);
            entity.Property(e => e.Idaño).HasColumnName("IDAño");
            entity.Property(e => e.Idestudiante)
                .HasMaxLength(15)
                .IsUnicode(false)
                .HasColumnName("IDEstudiante");
            entity.Property(e => e.Idgrado)
                .HasMaxLength(20)
                .IsUnicode(false)
                .HasColumnName("IDGrado");
            entity.Property(e => e.Idimv)
                .HasMaxLength(8)
                .IsUnicode(false)
                .HasComputedColumnSql("('IMV-'+right('00-'+CONVERT([varchar](15),[IDEst]),(4)))", false)
                .HasColumnName("IDIMV");
            entity.Property(e => e.Idimvencargado).HasColumnName("IDIMVEncargado");
            entity.Property(e => e.Idpadre)
                .HasMaxLength(15)
                .IsUnicode(false)
                .HasColumnName("IDPadre");
            entity.Property(e => e.Jornada)
                .HasMaxLength(15)
                .IsUnicode(false);
            entity.Property(e => e.LugarNacimiento)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("Lugar_Nacimiento");
            entity.Property(e => e.Nacionalidad)
                .HasMaxLength(20)
                .IsUnicode(false);
            entity.Property(e => e.NivelDescripcion)
                .HasMaxLength(100)
                .IsUnicode(false)
                .HasColumnName("Nivel_Descripcion");
            entity.Property(e => e.NombreEstudiante)
                .HasMaxLength(70)
                .IsUnicode(false)
                .HasColumnName("Nombre_Estudiante");
            entity.Property(e => e.NombrePadre)
                .HasMaxLength(70)
                .IsUnicode(false)
                .HasColumnName("Nombre_Padre");
            entity.Property(e => e.Observacion)
                .HasMaxLength(100)
                .IsUnicode(false);
            entity.Property(e => e.Parentesco)
                .HasMaxLength(20)
                .IsUnicode(false);
            entity.Property(e => e.Proviene)
                .HasMaxLength(150)
                .IsUnicode(false);
            entity.Property(e => e.RepiteAño)
                .HasMaxLength(4)
                .IsUnicode(false)
                .HasColumnName("Repite_Año");
            entity.Property(e => e.Seccion)
                .HasMaxLength(15)
                .IsUnicode(false);
            entity.Property(e => e.TelefonoEstudiante)
                .HasMaxLength(10)
                .IsUnicode(false)
                .HasColumnName("Telefono_Estudiante");
            entity.Property(e => e.TelefonoPadre)
                .HasMaxLength(10)
                .IsUnicode(false)
                .HasColumnName("Telefono_Padre");
            entity.Property(e => e.TipoSangre)
                .HasMaxLength(10)
                .IsUnicode(false)
                .HasColumnName("Tipo_Sangre");
            entity.Property(e => e.VacunasCovid)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("Vacunas_Covid");

            entity.HasOne(d => d.IdañoNavigation).WithMany(p => p.Matriculas)
                .HasForeignKey(d => d.Idaño)
                .HasConstraintName("FK__MATRICULA__IDAño__1DE57479");

            entity.HasOne(d => d.IdgradoNavigation).WithMany(p => p.Matriculas)
                .HasForeignKey(d => d.Idgrado)
                .HasConstraintName("FK__MATRICULA__IDGra__1FCDBCEB");

            entity.HasOne(d => d.IdimvencargadoNavigation).WithMany(p => p.Matriculas)
                .HasForeignKey(d => d.Idimvencargado)
                .HasConstraintName("FK__MATRICULA__IDIMV__1ED998B2");
        });

        modelBuilder.Entity<Nota>(entity =>
        {
            entity.HasKey(e => e.Idnota).HasName("PK__NOTAS__E5F1D2FB7E062901");

            entity.ToTable("NOTAS");

            entity.Property(e => e.Idnota).HasColumnName("IDNota");
            entity.Property(e => e.Asignatura)
                .HasMaxLength(100)
                .IsUnicode(false);
            entity.Property(e => e.Genero)
                .HasMaxLength(15)
                .IsUnicode(false);
            entity.Property(e => e.Grado)
                .HasMaxLength(15)
                .IsUnicode(false);
            entity.Property(e => e.Idasignatura)
                .HasMaxLength(30)
                .IsUnicode(false)
                .HasColumnName("IDAsignatura");
            entity.Property(e => e.Idaño).HasColumnName("IDAño");
            entity.Property(e => e.Idest).HasColumnName("IDEst");
            entity.Property(e => e.Idestudiante)
                .HasMaxLength(15)
                .IsUnicode(false)
                .HasColumnName("IDEstudiante");
            entity.Property(e => e.Idgrado)
                .HasMaxLength(20)
                .IsUnicode(false)
                .HasColumnName("IDGrado");
            entity.Property(e => e.Iiiparcial).HasColumnName("IIIParcial");
            entity.Property(e => e.Iiparcial).HasColumnName("IIParcial");
            entity.Property(e => e.Iisemestre).HasColumnName("IISemestre");
            entity.Property(e => e.IndicePromocion).HasColumnName("Indice_Promocion");
            entity.Property(e => e.Iparcial).HasColumnName("IParcial");
            entity.Property(e => e.Isemestre).HasColumnName("ISemestre");
            entity.Property(e => e.Ivparcial).HasColumnName("IVParcial");
            entity.Property(e => e.Jornada)
                .HasMaxLength(15)
                .IsUnicode(false);
            entity.Property(e => e.NombreEstudiante)
                .HasMaxLength(70)
                .IsUnicode(false)
                .HasColumnName("Nombre_Estudiante");
            entity.Property(e => e.RecuperacionIi).HasColumnName("RecuperacionII");
            entity.Property(e => e.Seccion)
                .HasMaxLength(15)
                .IsUnicode(false);

            entity.HasOne(d => d.IdasignaturaNavigation).WithMany(p => p.Nota)
                .HasForeignKey(d => d.Idasignatura)
                .HasConstraintName("FK__NOTAS__IDAsignat__24927208");

            entity.HasOne(d => d.IdañoNavigation).WithMany(p => p.Nota)
                .HasForeignKey(d => d.Idaño)
                .HasConstraintName("FK__NOTAS__IDAño__22AA2996");

            entity.HasOne(d => d.IdestNavigation).WithMany(p => p.Nota)
                .HasForeignKey(d => d.Idest)
                .HasConstraintName("FK__NOTAS__IDEst__239E4DCF");

            entity.HasOne(d => d.IdgradoNavigation).WithMany(p => p.Nota)
                .HasForeignKey(d => d.Idgrado)
                .HasConstraintName("FK__NOTAS__IDGrado__25869641");
        });

        modelBuilder.Entity<Padre>(entity =>
        {
            entity.HasKey(e => e.Idimvencargado).HasName("PK__PADRE__314287D72FD2CDCC");

            entity.ToTable("PADRE");

            entity.Property(e => e.Idimvencargado).HasColumnName("IDIMVEncargado");
            entity.Property(e => e.CelPadre)
                .HasMaxLength(10)
                .IsUnicode(false)
                .HasColumnName("Cel_Padre");
            entity.Property(e => e.Correo)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.DireccionPadre)
                .HasMaxLength(100)
                .IsUnicode(false)
                .HasColumnName("Direccion_Padre");
            entity.Property(e => e.Genero)
                .HasMaxLength(15)
                .IsUnicode(false);
            entity.Property(e => e.Idpadre)
                .HasMaxLength(15)
                .IsUnicode(false)
                .HasColumnName("IDPadre");
            entity.Property(e => e.NombrePadre)
                .HasMaxLength(70)
                .IsUnicode(false)
                .HasColumnName("Nombre_Padre");
            entity.Property(e => e.Observacion)
                .HasMaxLength(100)
                .IsUnicode(false);
            entity.Property(e => e.Parentesco)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.Profesion)
                .HasMaxLength(100)
                .IsUnicode(false);
            entity.Property(e => e.TelefonoPadre)
                .HasMaxLength(10)
                .IsUnicode(false)
                .HasColumnName("Telefono_Padre");
        });

        modelBuilder.Entity<Personalidad>(entity =>
        {
            entity.HasKey(e => e.Idpersonalidad).HasName("PK__PERSONAL__4F6DCA43E1A39212");

            entity.ToTable("PERSONALIDAD");

            entity.Property(e => e.Idpersonalidad).HasColumnName("IDPersonalidad");
            entity.Property(e => e.EspirituDeTrabajo)
                .HasMaxLength(20)
                .IsUnicode(false)
                .HasColumnName("Espiritu_de_Trabajo");
            entity.Property(e => e.Genero)
                .HasMaxLength(15)
                .IsUnicode(false);
            entity.Property(e => e.Grado)
                .HasMaxLength(15)
                .IsUnicode(false);
            entity.Property(e => e.Idaño).HasColumnName("IDAño");
            entity.Property(e => e.Idest).HasColumnName("IDEst");
            entity.Property(e => e.Idestudiante)
                .HasMaxLength(15)
                .IsUnicode(false)
                .HasColumnName("IDEstudiante");
            entity.Property(e => e.Idgrado)
                .HasMaxLength(20)
                .IsUnicode(false)
                .HasColumnName("IDGrado");
            entity.Property(e => e.Jornada)
                .HasMaxLength(15)
                .IsUnicode(false);
            entity.Property(e => e.Moralidad)
                .HasMaxLength(20)
                .IsUnicode(false);
            entity.Property(e => e.NombreEstudiante)
                .HasMaxLength(70)
                .IsUnicode(false)
                .HasColumnName("Nombre_Estudiante");
            entity.Property(e => e.OrdenYPresentacion)
                .HasMaxLength(20)
                .IsUnicode(false)
                .HasColumnName("Orden_y_Presentacion");
            entity.Property(e => e.Puntualidad)
                .HasMaxLength(20)
                .IsUnicode(false);
            entity.Property(e => e.Seccion)
                .HasMaxLength(15)
                .IsUnicode(false);
            entity.Property(e => e.Sociabilidad)
                .HasMaxLength(20)
                .IsUnicode(false);

            entity.HasOne(d => d.IdañoNavigation).WithMany(p => p.Personalidads)
                .HasForeignKey(d => d.Idaño)
                .HasConstraintName("FK__PERSONALI__IDAño__286302EC");

            entity.HasOne(d => d.IdestNavigation).WithMany(p => p.Personalidads)
                .HasForeignKey(d => d.Idest)
                .HasConstraintName("FK__PERSONALI__IDEst__29572725");

            entity.HasOne(d => d.IdgradoNavigation).WithMany(p => p.Personalidads)
                .HasForeignKey(d => d.Idgrado)
                .HasConstraintName("FK__PERSONALI__IDGra__2A4B4B5E");
        });

        modelBuilder.Entity<PersonalidadIiip>(entity =>
        {
            entity.HasKey(e => e.IdpersonalidadIii).HasName("PK__PERSONAL__3A29E7D78AA5BB93");

            entity.ToTable("PERSONALIDAD_IIIP");

            entity.Property(e => e.IdpersonalidadIii).HasColumnName("IDPersonalidadIII");
            entity.Property(e => e.EspirituDeTrabajo)
                .HasMaxLength(20)
                .IsUnicode(false)
                .HasColumnName("Espiritu_de_Trabajo");
            entity.Property(e => e.Genero)
                .HasMaxLength(15)
                .IsUnicode(false);
            entity.Property(e => e.Grado)
                .HasMaxLength(15)
                .IsUnicode(false);
            entity.Property(e => e.Idaño).HasColumnName("IDAño");
            entity.Property(e => e.Idest).HasColumnName("IDEst");
            entity.Property(e => e.Idestudiante)
                .HasMaxLength(15)
                .IsUnicode(false)
                .HasColumnName("IDEstudiante");
            entity.Property(e => e.Idgrado)
                .HasMaxLength(20)
                .IsUnicode(false)
                .HasColumnName("IDGrado");
            entity.Property(e => e.Jornada)
                .HasMaxLength(15)
                .IsUnicode(false);
            entity.Property(e => e.Moralidad)
                .HasMaxLength(20)
                .IsUnicode(false);
            entity.Property(e => e.NombreEstudiante)
                .HasMaxLength(70)
                .IsUnicode(false)
                .HasColumnName("Nombre_Estudiante");
            entity.Property(e => e.OrdenYPresentacion)
                .HasMaxLength(20)
                .IsUnicode(false)
                .HasColumnName("Orden_y_Presentacion");
            entity.Property(e => e.Puntualidad)
                .HasMaxLength(20)
                .IsUnicode(false);
            entity.Property(e => e.Seccion)
                .HasMaxLength(15)
                .IsUnicode(false);
            entity.Property(e => e.Sociabilidad)
                .HasMaxLength(20)
                .IsUnicode(false);

            entity.HasOne(d => d.IdañoNavigation).WithMany(p => p.PersonalidadIiips)
                .HasForeignKey(d => d.Idaño)
                .HasConstraintName("FK__PERSONALI__IDAño__74AE54BC");

            entity.HasOne(d => d.IdestNavigation).WithMany(p => p.PersonalidadIiips)
                .HasForeignKey(d => d.Idest)
                .HasConstraintName("FK__PERSONALI__IDEst__75A278F5");

            entity.HasOne(d => d.IdgradoNavigation).WithMany(p => p.PersonalidadIiips)
                .HasForeignKey(d => d.Idgrado)
                .HasConstraintName("FK__PERSONALI__IDGra__76969D2E");
        });

        modelBuilder.Entity<PersonalidadIip>(entity =>
        {
            entity.HasKey(e => e.IdpersonalidadIi).HasName("PK__PERSONAL__5106C2A59188C189");

            entity.ToTable("PERSONALIDAD_IIP");

            entity.Property(e => e.IdpersonalidadIi).HasColumnName("IDPersonalidadII");
            entity.Property(e => e.EspirituDeTrabajo)
                .HasMaxLength(20)
                .IsUnicode(false)
                .HasColumnName("Espiritu_de_Trabajo");
            entity.Property(e => e.Genero)
                .HasMaxLength(15)
                .IsUnicode(false);
            entity.Property(e => e.Grado)
                .HasMaxLength(15)
                .IsUnicode(false);
            entity.Property(e => e.Idaño).HasColumnName("IDAño");
            entity.Property(e => e.Idest).HasColumnName("IDEst");
            entity.Property(e => e.Idestudiante)
                .HasMaxLength(15)
                .IsUnicode(false)
                .HasColumnName("IDEstudiante");
            entity.Property(e => e.Idgrado)
                .HasMaxLength(20)
                .IsUnicode(false)
                .HasColumnName("IDGrado");
            entity.Property(e => e.Jornada)
                .HasMaxLength(15)
                .IsUnicode(false);
            entity.Property(e => e.Moralidad)
                .HasMaxLength(20)
                .IsUnicode(false);
            entity.Property(e => e.NombreEstudiante)
                .HasMaxLength(70)
                .IsUnicode(false)
                .HasColumnName("Nombre_Estudiante");
            entity.Property(e => e.OrdenYPresentacion)
                .HasMaxLength(20)
                .IsUnicode(false)
                .HasColumnName("Orden_y_Presentacion");
            entity.Property(e => e.Puntualidad)
                .HasMaxLength(20)
                .IsUnicode(false);
            entity.Property(e => e.Seccion)
                .HasMaxLength(15)
                .IsUnicode(false);
            entity.Property(e => e.Sociabilidad)
                .HasMaxLength(20)
                .IsUnicode(false);

            entity.HasOne(d => d.IdañoNavigation).WithMany(p => p.PersonalidadIips)
                .HasForeignKey(d => d.Idaño)
                .HasConstraintName("FK__PERSONALI__IDAño__6FE99F9F");

            entity.HasOne(d => d.IdestNavigation).WithMany(p => p.PersonalidadIips)
                .HasForeignKey(d => d.Idest)
                .HasConstraintName("FK__PERSONALI__IDEst__70DDC3D8");

            entity.HasOne(d => d.IdgradoNavigation).WithMany(p => p.PersonalidadIips)
                .HasForeignKey(d => d.Idgrado)
                .HasConstraintName("FK__PERSONALI__IDGra__71D1E811");
        });

        modelBuilder.Entity<PersonalidadIvp>(entity =>
        {
            entity.HasKey(e => e.IdpersonalidadIv).HasName("PK__PERSONAL__5106C55655C2A7E8");

            entity.ToTable("PERSONALIDAD_IVP");

            entity.Property(e => e.IdpersonalidadIv).HasColumnName("IDPersonalidadIV");
            entity.Property(e => e.EspirituDeTrabajo)
                .HasMaxLength(20)
                .IsUnicode(false)
                .HasColumnName("Espiritu_de_Trabajo");
            entity.Property(e => e.Genero)
                .HasMaxLength(15)
                .IsUnicode(false);
            entity.Property(e => e.Grado)
                .HasMaxLength(15)
                .IsUnicode(false);
            entity.Property(e => e.Idaño).HasColumnName("IDAño");
            entity.Property(e => e.Idest).HasColumnName("IDEst");
            entity.Property(e => e.Idestudiante)
                .HasMaxLength(15)
                .IsUnicode(false)
                .HasColumnName("IDEstudiante");
            entity.Property(e => e.Idgrado)
                .HasMaxLength(20)
                .IsUnicode(false)
                .HasColumnName("IDGrado");
            entity.Property(e => e.Jornada)
                .HasMaxLength(15)
                .IsUnicode(false);
            entity.Property(e => e.Moralidad)
                .HasMaxLength(20)
                .IsUnicode(false);
            entity.Property(e => e.NombreEstudiante)
                .HasMaxLength(70)
                .IsUnicode(false)
                .HasColumnName("Nombre_Estudiante");
            entity.Property(e => e.OrdenYPresentacion)
                .HasMaxLength(20)
                .IsUnicode(false)
                .HasColumnName("Orden_y_Presentacion");
            entity.Property(e => e.Puntualidad)
                .HasMaxLength(20)
                .IsUnicode(false);
            entity.Property(e => e.Seccion)
                .HasMaxLength(15)
                .IsUnicode(false);
            entity.Property(e => e.Sociabilidad)
                .HasMaxLength(20)
                .IsUnicode(false);

            entity.HasOne(d => d.IdañoNavigation).WithMany(p => p.PersonalidadIvps)
                .HasForeignKey(d => d.Idaño)
                .HasConstraintName("FK__PERSONALI__IDAño__797309D9");

            entity.HasOne(d => d.IdestNavigation).WithMany(p => p.PersonalidadIvps)
                .HasForeignKey(d => d.Idest)
                .HasConstraintName("FK__PERSONALI__IDEst__7A672E12");

            entity.HasOne(d => d.IdgradoNavigation).WithMany(p => p.PersonalidadIvps)
                .HasForeignKey(d => d.Idgrado)
                .HasConstraintName("FK__PERSONALI__IDGra__7B5B524B");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
