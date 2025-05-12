--SISTEMA ANGLOSAJON
USE DBSMILE;

ALTER TABLE AÑO
ADD Sistema VARCHAR(100),
    SistemaPeriodo VARCHAR(100);

ALTER TABLE ASIGNATURA
ADD Sistema VARCHAR(100),
    SistemaClase VARCHAR(100),
	SistemaTiempo VARCHAR(100);

ALTER TABLE GRADO
ADD Sistema VARCHAR(100),
    SistemaClase VARCHAR(100),
	SistemaTiempo VARCHAR(100);

-- LA TABLA DOCENTES NO SE MODIFICA

ALTER TABLE MATRICULA
ADD Sistema VARCHAR(100),
    SistemaClase VARCHAR(100),
	SistemaTiempo VARCHAR(100),
	FotoData VARBINARY(MAX);

ALTER TABLE NOTAS
ADD Sistema VARCHAR(100),
    SistemaClase VARCHAR(100),
	SistemaTiempo VARCHAR(100);

--LA TABLA PADRE NO SE HACEN CAMBIOS.

ALTER TABLE PERSONALIDAD
ADD Sistema VARCHAR(100),
    SistemaClase VARCHAR(100),
	SistemaTiempo VARCHAR(100);

ALTER TABLE PERSONALIDAD_II
ADD Sistema VARCHAR(100),
    SistemaClase VARCHAR(100),
	SistemaTiempo VARCHAR(100);

ALTER TABLE PERSONALIDAD_III
ADD Sistema VARCHAR(100),
    SistemaClase VARCHAR(100),
	SistemaTiempo VARCHAR(100);

ALTER TABLE PERSONALIDAD_IV
ADD Sistema VARCHAR(100),
    SistemaClase VARCHAR(100),
	SistemaTiempo VARCHAR(100);

--PROCEDIMIENTO ALMACENADO PARA ACELERAR LA VELOCIDAD DE CARGA EN EL INDEX PADRES
USE [DBIMAVA]
GO

SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

-- CREACIÓN (no ALTER) del procedimiento
CREATE PROCEDURE [dbo].[sp_GetAllPadres]
AS
BEGIN
    SELECT 
        IDIMVEncargado, 
        Idpadre, 
        Nombre_Padre, 
        Parentesco, 
        Profesion, 
        Genero, 
        Telefono_Padre, 
        Cel_Padre, 
        Correo, 
        Direccion_Padre, 
        Observacion
    FROM PADRE
END
GO

--PROCEDIMIENTO ALMACENADO PARA ACELERAR LA VELOCIDAD DE CARGA EN EL INDEX ASIGNATURAS
CREATE PROCEDURE sp_GetAllAsignaturas
AS
BEGIN
    SELECT a.Idaño, a.Idgrado, a.Grado, a.Seccion, a.Jornada, a.NivelDescripcion,
           a.Idasignatura, a.Asignatura1, a.Periodo, 
           anio.Idaño AS IdañoNavigation_Idaño,
           grado.Idgrado AS IdgradoNavigation_Idgrado
    FROM Asignaturas a
    LEFT JOIN Años anio ON a.Idaño = anio.Idaño
    LEFT JOIN Grados grado ON a.Idgrado = grado.Idgrado
END


-- Paso 1: Agregar la columna IDAño
ALTER TABLE [DBIMAVA].[dbo].[PADRE]
ADD [IDAño] INT;
-- Paso 2: Agregar la clave foránea
ALTER TABLE [DBIMAVA].[dbo].[PADRE]
ADD CONSTRAINT FK_PADRE_AÑO
FOREIGN KEY ([IDAño]) REFERENCES [DBIMAVA].[dbo].[AÑO]([IDAño]);