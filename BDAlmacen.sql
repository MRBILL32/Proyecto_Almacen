create database Almacen
on primary
(
    NAME = AlmacenData,
    FILENAME = 'D:\DBAlmacen\Almacen\Almacen.mdf',
    MAXSIZE = 20GB,
	SIZE = 50MB,
    FILEGROWTH = 20MB
)
LOG ON
(
    NAME = AlmacenLog,
    FILENAME = 'D:\DBAlmacen\Almacen\Almacen_log.ldf',
    MAXSIZE = 25GB,
	SIZE = 80MB,
    FILEGROWTH = 30MB
)
go

use Almacen
go

-- Login
alter database Almacen add filegroup FGLogin
go

alter database Almacen add file
(
	name = Sesion,
	filename = 'D:\DBAlmacen\Login\Sesion.NDF',
	maxsize = 1gb,
	size = 50mb,
	filegrowth = 20mb
)
to filegroup FGLogin
go

alter database Almacen add log file
(
	name = SesionLog,
	filename = 'D:\DBAlmacen\Login\SesionLog.LDF',
	maxsize = 2gb,
	size = 80mb,
	filegrowth = 30mb
)
go

-- Productos
alter database Almacen add filegroup FGProductos
go

alter database Almacen add file
(
	name = Productos,
	filename = 'D:\DBAlmacen\Productos\Productos.NDF',
	maxsize = 10gb,
	size = 50mb,
	filegrowth = 20mb
)
to filegroup FGProductos
go

alter database Almacen add log file
(
	name = ProductosLog,
	filename = 'D:\DBAlmacen\Productos\ProductosLog.LDF',
	maxsize = 15gb,
	size = 80mb,
	filegrowth = 30mb
)
go

sp_helpdb Almacen
go

/*====== schemas y filegroups =====*/

create schema schLogin
go

create schema schProductos
go

create table schLogin.Rol
(
	idRol int identity(1,1) primary key,
	tipoRol varchar(50) not null
) 
on FGLogin
go

insert into schLogin.Rol values
/*1*/('Administrador'),
/*2*/('Usuario')
go

create or alter procedure usp_listarRol
as
	select * from schLogin.Rol
go

create table schLogin.Usuario
(
	idUser int identity(1,1) primary key,
	nombres varchar(100) not null,
	apellidos varchar(100) not null,
	dni varchar(7) not null,
	idRol int references schLogin.Rol,
	login varchar(20) not null,
	password varchar(30) not null,
	correo varchar(150) not null unique,
	estado varchar(20) not null default 'aprobado'
) on FGLogin;
go

/*===== Procedimientos Almacenados =====*/

/*=== Crear Usuarios ===*/
CREATE OR ALTER PROCEDURE usp_CrearUser
    @nombres VARCHAR(100),
    @apellidos VARCHAR(100),
    @dni VARCHAR(7),
    @idRol INT,
    @login VARCHAR(20),
    @password VARCHAR(30),
    @correo VARCHAR(150),
    @estado VARCHAR(20)
AS
BEGIN
    -- Verificar si el correo ya existe
    IF EXISTS (SELECT 1 FROM schLogin.Usuario WHERE correo = @correo)
    BEGIN
        RAISERROR('Correo electrónico ya usado', 16, 1);
        RETURN;
    END

    -- Insertar nuevo usuario
    INSERT INTO schLogin.Usuario (nombres, apellidos, dni, idRol, login, password, correo, estado)
    VALUES (@nombres, @apellidos, @dni, @idRol, @login, @password, @correo, @estado);

    -- Devolver el ID recién creado
    SELECT SCOPE_IDENTITY() AS NuevoId;
END
go

exec usp_CrearUser 'Osiander Stivent', 'Carhuas Marallano', '74643027', 1, 'MRBILL32', 'D@k12345', 'stivent456@gmail.com','aprobado'
exec usp_CrearUser 'Francisca ', 'Perez', '7456871', 2, 'Fran71', '12345', 'franperez@gmail.com','aprobado'
go

/*=== Listar Usuarios ===*/
create or alter procedure usp_ListarUser
as
	select idUser,nombres,apellidos,dni, R.idRol, R.tipoRol,login,password,correo,estado
		from schLogin.Usuario U join schLogin.Rol R on U.idRol = R.idRol
go

exec usp_ListarUser
go

/*=== Buscar por ID ===*/

CREATE or alter PROCEDURE usp_ObtenerUsuarioPorId
    @idUsuario INT
AS
SELECT 
    idUser,
    nombres,
    apellidos,
    dni,           -- ← ¡Asegúrate de incluir esto!
    R.idRol,
    R.tipoRol,
    login,
    password,
    correo,
    U.estado
FROM schLogin.Usuario U
JOIN schLogin.Rol R ON U.idRol = R.idRol 
WHERE idUser = @idUsuario
go

exec usp_ObtenerUsuarioPorId 18
go

/*=== Actualizar Usuario o contraseña ===*/
create or alter procedure usp_ActualizarUser
	@idUser int,
	@oldPassword varchar(30),
	@newLogin varchar(20) = null,
	@newPassword varchar(30) = null
as
begin
	-- Verificar credenciales actuales
	if not exists (
		select 1 from schLogin.Usuario
		where idUser = @idUser and password = @oldPassword
	)
	begin
		raiserror('Contraseña actual incorrecta.', 16, 1);
		return;
	end

	-- Actualizar solo si se proporciona un nuevo valor
	update schLogin.Usuario
	set 
		login = isnull(@newLogin, login),
		password = isnull(@newPassword, password)
	where idUser = @idUser;
end;
go

exec usp_ActualizarUser 1, 'D@k12345', 'MRBILL32', 'D@k12345'
go

/*=== Iniciar Sesion ===*/
CREATE OR ALTER PROCEDURE usp_InicioSesion
    @login VARCHAR(20),
    @password VARCHAR(30) 
AS
BEGIN
    IF NOT EXISTS (
        SELECT 1
        FROM schLogin.Usuario
        WHERE login = @login AND password = @password
    )
    BEGIN
        RAISERROR('Usuario o contraseña incorrecta.', 16, 1);
        RETURN;
    END

    SELECT 
        U.idUser,
        U.nombres,
        U.apellidos,
        R.idRol,
        R.tipoRol,
        U.login,
        U.correo,
		U.estado
    FROM schLogin.Usuario U
    INNER JOIN schLogin.Rol R ON U.idRol = R.idRol
    WHERE U.login = @login AND U.password = @password;
END;
GO
EXEC usp_InicioSesion @login='MRBILL32', @password='D@k12345'
go

/*=== Cambiar Estado ===*/
CREATE OR ALTER PROCEDURE usp_CambiarEstado
    @idUsuario INT,
    @nuevoEstado VARCHAR(20)
AS
BEGIN
    IF @nuevoEstado = 'Rechazado'
    BEGIN
        DELETE FROM schLogin.Usuario
        WHERE idUser = @idUsuario;
    END
    ELSE
    BEGIN
        UPDATE schLogin.Usuario
        SET estado = @nuevoEstado
        WHERE idUser = @idUsuario;
    END
END
go

EXEC usp_CambiarEstado @idUsuario = 18, @nuevoEstado = 'Rechazado'
exec usp_ListarUser
go

/*===== Productos =====*/

