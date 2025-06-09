-- Crear base de datos principal y archivo de log
CREATE DATABASE Almacen
ON PRIMARY
(
    NAME = AlmacenData,
    FILENAME = 'D:\DBAlmacen\Almacen\Almacen.mdf',
    MAXSIZE = 40GB,
    SIZE = 50MB,
    FILEGROWTH = 20MB
)
LOG ON
(
    NAME = AlmacenLog,
    FILENAME = 'D:\DBAlmacen\Almacen\Almacen_log.ldf',
    MAXSIZE = 50GB,
    SIZE = 80MB,
    FILEGROWTH = 30MB
)
GO

USE Almacen
GO

-- Filegroup y archivos para Login
ALTER DATABASE Almacen ADD FILEGROUP FGLogin
GO
ALTER DATABASE Almacen ADD FILE
(
    NAME = Sesion,
    FILENAME = 'D:\DBAlmacen\Login\Sesion.NDF',
    MAXSIZE = 2GB,
    SIZE = 50MB,
    FILEGROWTH = 20MB
) TO FILEGROUP FGLogin
GO
ALTER DATABASE Almacen ADD LOG FILE
(
    NAME = SesionLog,
    FILENAME = 'D:\DBAlmacen\Login\SesionLog.LDF',
    MAXSIZE = 3GB,
    SIZE = 80MB,
    FILEGROWTH = 30MB
)
GO

-- Filegroup y archivos para Productos
ALTER DATABASE Almacen ADD FILEGROUP FGProductos
GO
ALTER DATABASE Almacen ADD FILE
(
    NAME = Productos,
    FILENAME = 'D:\DBAlmacen\Productos\Productos.NDF',
    MAXSIZE = 18GB,
    SIZE = 50MB,
    FILEGROWTH = 20MB
) TO FILEGROUP FGProductos
GO
ALTER DATABASE Almacen ADD LOG FILE
(
    NAME = ProductosLog,
    FILENAME = 'D:\DBAlmacen\Productos\ProductosLog.LDF',
    MAXSIZE = 17GB,
    SIZE = 80MB,
    FILEGROWTH = 30MB
)
GO

-- Filegroup y archivos para Pedidos
ALTER DATABASE Almacen ADD FILEGROUP FGPedidos
GO
ALTER DATABASE Almacen ADD FILE
(
    NAME = Pedidos,
    FILENAME = 'D:\DBAlmacen\Pedidos\Pedidos.NDF',
    MAXSIZE = 15GB,
    SIZE = 50MB,
    FILEGROWTH = 20MB
) TO FILEGROUP FGPedidos
GO
ALTER DATABASE Almacen ADD LOG FILE
(
    NAME = Pedidos_log,
    FILENAME = 'D:\DBAlmacen\Pedidos\PedidosLog.LDF',
    MAXSIZE = 15GB,
    SIZE = 50MB,
    FILEGROWTH = 20MB
)
GO

-- Schemas para organización lógica
CREATE SCHEMA schLogin
GO
CREATE SCHEMA schProductos
GO
CREATE SCHEMA schPedidos
GO

-- Tabla de Roles
CREATE TABLE schLogin.Rol
(
    idRol INT IDENTITY(1,1) PRIMARY KEY,
    tipoRol VARCHAR(50) NOT NULL
) ON FGLogin
GO

INSERT INTO schLogin.Rol VALUES
('Administrador'),
('Usuario')
GO

-- Tabla de Usuarios
CREATE TABLE schLogin.Usuario
(
    idUser INT IDENTITY(1,1) PRIMARY KEY,
    nombres VARCHAR(100) NOT NULL,
    apellidos VARCHAR(100) NOT NULL,
    dni VARCHAR(8) NOT NULL,
    idRol INT REFERENCES schLogin.Rol,
    login VARCHAR(20) NOT NULL,
    password VARCHAR(30) NOT NULL,
    correo VARCHAR(150) NOT NULL UNIQUE,
    estado VARCHAR(20) NOT NULL DEFAULT 'Aprobado'
) ON FGLogin
GO

-- Listar Roles
CREATE OR ALTER PROCEDURE usp_listarRol AS
    SELECT * FROM schLogin.Rol
GO

-- Crear Usuario
CREATE OR ALTER PROCEDURE usp_CrearUser
    @nombres VARCHAR(100),
    @apellidos VARCHAR(100),
    @dni VARCHAR(8),
    @idRol INT,
    @login VARCHAR(20),
    @password VARCHAR(30),
    @correo VARCHAR(150),
    @estado VARCHAR(20)
AS
BEGIN
    IF EXISTS (SELECT 1 FROM schLogin.Usuario WHERE correo = @correo)
    BEGIN
        RAISERROR('Correo electrónico ya usado', 16, 1);
        RETURN;
    END
    INSERT INTO schLogin.Usuario (nombres, apellidos, dni, idRol, login, password, correo, estado)
    VALUES (@nombres, @apellidos, @dni, @idRol, @login, @password, @correo, @estado);
    SELECT SCOPE_IDENTITY() AS NuevoId;
END
GO

-- Listar Usuarios
CREATE OR ALTER PROCEDURE usp_ListarUser AS
    SELECT idUser, nombres, apellidos, dni, R.idRol, R.tipoRol, login, password, correo, estado
    FROM schLogin.Usuario U JOIN schLogin.Rol R ON U.idRol = R.idRol
GO

-- Buscar Usuario por ID
CREATE OR ALTER PROCEDURE usp_ObtenerUsuarioPorId
    @idUsuario INT
AS
SELECT idUser, nombres, apellidos, dni, R.idRol, R.tipoRol, login, password, correo, U.estado
FROM schLogin.Usuario U
JOIN schLogin.Rol R ON U.idRol = R.idRol
WHERE idUser = @idUsuario
GO

-- Buscar Usuario por Correo
CREATE OR ALTER PROCEDURE usp_BuscarCorreo
    @correo VARCHAR(150)
AS
BEGIN
    SELECT U.idUser, U.nombres, U.apellidos, U.dni, U.idRol, R.tipoRol, U.login, U.password, U.correo, U.estado
    FROM schLogin.Usuario U
    INNER JOIN schLogin.Rol R ON U.idRol = R.idRol
    WHERE U.correo = @correo;
END
GO

-- Actualizar Login/Password
CREATE OR ALTER PROCEDURE usp_ActualizarUser
    @idUser INT,
    @newLogin VARCHAR(20) = NULL,
    @newPassword VARCHAR(30) = NULL
AS
BEGIN
    IF NOT EXISTS (SELECT 1 FROM schLogin.Usuario WHERE idUser = @idUser)
    BEGIN
        RAISERROR('Usuario no encontrado.', 16, 1);
        RETURN;
    END
    UPDATE schLogin.Usuario
    SET login = ISNULL(@newLogin, login),
        password = ISNULL(@newPassword, password)
    WHERE idUser = @idUser;
END
GO

-- Iniciar Sesión
CREATE OR ALTER PROCEDURE usp_InicioSesion
    @login VARCHAR(20),
    @password VARCHAR(30)
AS
BEGIN
    IF NOT EXISTS (SELECT 1 FROM schLogin.Usuario WHERE login = @login AND password = @password)
    BEGIN
        RAISERROR('Usuario o contraseña incorrecta.', 16, 1);
        RETURN;
    END
    IF EXISTS (SELECT 1 FROM schLogin.Usuario WHERE login = @login AND password = @password AND estado = 'pendiente')
    BEGIN
        RAISERROR('Estado Pendiente.', 16, 1);
        RETURN;
    END
   SELECT 
        U.idUser,
        U.nombres,
        U.apellidos,
        R.tipoRol,
        U.idRol,
        U.login,
        U.correo,
        U.estado
    FROM schLogin.Usuario U
    INNER JOIN schLogin.Rol R ON U.idRol = R.idRol
    WHERE U.login = @login AND U.password = @password;
END;
GO

-- Cambiar Estado o Eliminar Usuario
CREATE OR ALTER PROCEDURE usp_CambiarEstado
    @idUsuario INT,
    @nuevoEstado VARCHAR(20)
AS
BEGIN
    IF @nuevoEstado = 'Rechazado'
    BEGIN
        DELETE FROM schLogin.Usuario WHERE idUser = @idUsuario;
    END
    ELSE
    BEGIN
        UPDATE schLogin.Usuario SET estado = @nuevoEstado WHERE idUser = @idUsuario;
    END
END
GO

-- Tabla de Categorías
CREATE TABLE schProductos.Categoria
(
    idCate INT IDENTITY(1,1) PRIMARY KEY NOT NULL,
    nomCate VARCHAR(50) NOT NULL
) ON FGProductos
GO

INSERT INTO schProductos.Categoria VALUES
('Lacteos'), ('Electronica'), ('Limpieza'), ('infusion'), ('Bebidas'),
('Panadería'), ('Carnes y Embutidos'), ('Frutas y Verduras'), ('Abarrotes'),
('Cuidado Personal'), ('Hogar'), ('Mascotas'), ('Congelados'), ('Snacks y Botanas')
GO

-- Tabla de Productos
CREATE TABLE schProductos.Producto
(
    idProd INT IDENTITY(1,1) PRIMARY KEY NOT NULL,
    nomProd VARCHAR(200) NOT NULL,
    marcaProd VARCHAR(50) NOT NULL,
    idCate INT REFERENCES schProductos.Categoria,
    precioUnit DECIMAL(10,2) NOT NULL,
    stock SMALLINT NOT NULL,
    activo BIT NOT NULL DEFAULT 1
) ON FGProductos
GO

-- Insertar Producto
CREATE OR ALTER PROCEDURE usp_InsertarProducto
@nomProd VARCHAR(200),
@marcaProd VARCHAR(50),
@idCate INT,
@precioUnit DECIMAL(10,2),
@stock SMALLINT
AS
INSERT INTO schProductos.Producto (nomProd, marcaProd, idCate, precioUnit, stock)
VALUES (@nomProd, @marcaProd, @idCate, @precioUnit, @stock);
GO

-- Actualizar Producto
CREATE OR ALTER PROCEDURE usp_ActualizarProducto
@idProd INT,
@nomProd VARCHAR(200),
@marcaProd VARCHAR(50),
@idCate INT,
@precioUnit DECIMAL(10,2),
@stock SMALLINT
AS
UPDATE schProductos.Producto
SET nomProd = @nomProd,
    marcaProd = @marcaProd,
    idCate = @idCate,
    precioUnit = @precioUnit,
    stock = @stock
WHERE idProd = @idProd;

-- Desactivar producto si el stock llega a cero
UPDATE schProductos.Producto
SET activo = 0
WHERE idProd = @idProd AND stock = 0;

-- Activar producto si el stock es mayor a cero
UPDATE schProductos.Producto
SET activo = 1
WHERE idProd = @idProd AND stock > 0;
GO

-- Listar Productos (Admin)
CREATE OR ALTER PROCEDURE usp_ListaProducAdmin AS
   SELECT  
   idProd,
   nomProd,
   marcaProd,
   C.idCate,
   C.nomCate,
   precioUnit,
   stock,
   activo
	FROM schProductos.Producto P join schProductos.Categoria C on P.idCate = C.idCate
GO

-- Listar Productos Paginado
CREATE OR ALTER PROCEDURE usp_listarProductos
AS
BEGIN
    SELECT 
        P.idProd,
        P.nomProd, 
        P.marcaProd, 
        C.nomCate, 
        P.precioUnit, 
        P.stock
    FROM schProductos.Producto P
    JOIN schProductos.Categoria C ON P.idCate = C.idCate
    WHERE P.activo = 1
    ORDER BY P.idProd;
END
GO

-- Búsqueda General de Productos
CREATE OR ALTER PROCEDURE usp_BuscarProductos
@busqueda NVARCHAR(200)
AS
SELECT nomProd, marcaProd, C.idCate, C.nomCate, precioUnit, stock
FROM schProductos.Producto P
JOIN schProductos.Categoria C ON P.idCate = C.idCate
WHERE
    P.nomProd LIKE '%' + @busqueda + '%'
    OR P.marcaProd LIKE '%' + @busqueda + '%'
    OR C.nomCate LIKE '%' + @busqueda + '%'
    OR CAST(P.idProd AS NVARCHAR) = @busqueda;
GO

-- Tabla de Pedidos
CREATE TABLE schPedidos.Pedido
(
    idPedido INT IDENTITY(1,1) PRIMARY KEY NOT NULL,
    idUser INT NOT NULL REFERENCES schLogin.Usuario(idUser),
    fecha DATETIME NOT NULL DEFAULT GETDATE(),
    total DECIMAL(12,2) NOT NULL,
    estado VARCHAR(20) NOT NULL DEFAULT 'pendiente'
) ON FGPedidos
GO

create or alter procedure usp_listarPedidos
as
select U.nombres,U.apellidos,U.dni,U.correo,fecha,total,P.estado
	from schPedidos.Pedido P join schLogin.Usuario U on P.idUser = U.idUser
go

exec usp_listarPedidos

-- Crear Pedido
CREATE OR ALTER PROCEDURE usp_CrearPedido
@idUser INT,
@total DECIMAL(12,2),
@estado VARCHAR(20) = 'Pendiente'
AS
INSERT INTO schPedidos.Pedido (idUser, total, estado)
VALUES (@idUser, @total, @estado);
SELECT SCOPE_IDENTITY() AS NuevoPedido;
GO

-- Buscar por Nombre Usuario
CREATE OR ALTER PROCEDURE usp_BuscarPedidoUsuario
    @nombreUsuario NVARCHAR(100)
AS
BEGIN
    SELECT U.nombres,U.apellidos,U.dni,U.correo,fecha,total,P.estado
    FROM schPedidos.Pedido p
    JOIN schLogin.Usuario u ON p.idUser = u.idUser
    WHERE u.nombres LIKE '%' + @nombreUsuario + '%'
	OR u.login LIKE '%' + @nombreUsuario + '%'
	OR u.apellidos LIKE '%' + @nombreUsuario + '%'
    ORDER BY p.fecha DESC;
END
GO

-- Tabla de Detalle de Pedido
CREATE TABLE schPedidos.DetallePedido
(
    idDetalle INT IDENTITY(1,1) PRIMARY KEY NOT NULL,
    idPedido INT NOT NULL REFERENCES schPedidos.Pedido(idPedido),
    idProd INT NOT NULL REFERENCES schProductos.Producto(idProd),
    cantidad INT NOT NULL,
    precioUnit DECIMAL(10,2) NOT NULL,
    subtotal DECIMAL(12,2) NOT NULL
) ON FGPedidos
GO

-- Insertar Detalle de Pedido
CREATE OR ALTER PROCEDURE usp_InsertarDetallePedido
@idPedido INT,
@idProd INT,
@cantidad INT
AS
BEGIN
    DECLARE @precioUnit DECIMAL(10,2);
    DECLARE @stockActual INT;
    DECLARE @cantidadActual INT;
    SELECT @precioUnit = precioUnit, @stockActual = stock
    FROM schProductos.Producto
    WHERE idProd = @idProd;
    IF @stockActual IS NULL OR @stockActual < @cantidad RETURN;
    SELECT @cantidadActual = cantidad
    FROM schPedidos.DetallePedido
    WHERE idPedido = @idPedido AND idProd = @idProd;
    IF @cantidadActual IS NULL
    BEGIN
        INSERT INTO schPedidos.DetallePedido (idPedido, idProd, cantidad, precioUnit, subtotal)
        VALUES (@idPedido, @idProd, @cantidad, @precioUnit, @precioUnit * @cantidad);
    END
    ELSE
    BEGIN
        UPDATE schPedidos.DetallePedido
        SET cantidad = cantidad + @cantidad,
            subtotal = (cantidad + @cantidad) * precioUnit
        WHERE idPedido = @idPedido AND idProd = @idProd;
    END
    UPDATE schProductos.Producto
    SET stock = stock - @cantidad
    WHERE idProd = @idProd;
    UPDATE schPedidos.Pedido
    SET total = (
        SELECT SUM(subtotal)
        FROM schPedidos.DetallePedido
        WHERE idPedido = @idPedido
    )
    WHERE idPedido = @idPedido;
    UPDATE schProductos.Producto
    SET activo = 0
    WHERE idProd = @idProd AND stock <= 0;
END
GO

-- Eliminar producto y pedido
create or alter procedure usp_EliminarProducto
	@idProd int
as
-- Elimina los detalles de pedido que usan este producto
	delete from schPedidos.DetallePedido where idProd = @idProd
-- Elimina el producto del catalogo
	delete from schProductos.Producto where idProd = @idProd
go

-- Comprar Producto
CREATE OR ALTER PROCEDURE usp_ComprarProducto
    @idUser INT,
    @idProd INT,
    @cantidad INT
AS
BEGIN
    SET NOCOUNT ON;

    -- Verificar stock disponible
    DECLARE @stockActual INT, @precioUnit DECIMAL(10,2);
    SELECT @stockActual = stock, @precioUnit = precioUnit
    FROM schProductos.Producto
    WHERE idProd = @idProd AND activo = 1;

    IF @stockActual IS NULL OR @stockActual < @cantidad
    BEGIN
        RAISERROR('Stock insuficiente o producto inactivo.', 16, 1);
        RETURN;
    END

    -- Crear el pedido
    INSERT INTO schPedidos.Pedido (idUser, total, estado)
    VALUES (@idUser, @precioUnit * @cantidad, 'Pendiente');

    DECLARE @idPedido INT = SCOPE_IDENTITY();

    -- Insertar detalle de pedido
    INSERT INTO schPedidos.DetallePedido (idPedido, idProd, cantidad, precioUnit, subtotal)
    VALUES (@idPedido, @idProd, @cantidad, @precioUnit, @precioUnit * @cantidad);

    -- Descontar stock
    UPDATE schProductos.Producto
    SET stock = stock - @cantidad
    WHERE idProd = @idProd;

    -- Desactivar producto si stock llega a cero
    UPDATE schProductos.Producto
    SET activo = 0
    WHERE idProd = @idProd AND stock <= 0;
END;
GO

-- Historial de Pedidos por Usuario
CREATE OR ALTER PROCEDURE usp_HistorialPedidosUsuario
@idUser INT
AS
SELECT 
	p.idPedido,
	u.nombres,
    pr.nomProd,
    d.cantidad,
    d.precioUnit,
    d.subtotal,
    p.fecha,
    p.total,
	p.estado
FROM schPedidos.Pedido p
JOIN schPedidos.DetallePedido d ON p.idPedido = d.idPedido
JOIN schProductos.Producto pr ON d.idProd = pr.idProd
join schLogin.Usuario u on p.idUser = u.idUser
WHERE p.idUser = @idUser
ORDER BY p.fecha DESC, p.idPedido DESC;
GO

-- Ejemplo de inserciones y pruebas
EXEC usp_CrearUser 'Osiander Stivent', 'Carhuas Marallano', '74643027', 1, 'MRBILL32', 'D@k12345', 'stivent456@gmail.com','Aprobado'
EXEC usp_ListarUser
EXEC usp_InicioSesion 'MRBILL32','D@k12345'
go

EXEC usp_InsertarProducto 'Yogurt Griego', 'Chobani', 1, 25.50, 30;
EXEC usp_InsertarProducto 'Yogurt Natural', 'Gloria', 1, 3.50, 100;         -- Lacteos
EXEC usp_InsertarProducto 'Leche Entera', 'Laive', 1, 4.20, 80;             -- Lacteos
EXEC usp_InsertarProducto 'Queso Fresco', 'Bonlé', 1, 7.50, 60;             -- Lacteos
EXEC usp_InsertarProducto 'Smartphone A15', 'Samsung', 2, 1200.00, 20;      -- Electronica
EXEC usp_InsertarProducto 'Audífonos Bluetooth', 'Xiaomi', 2, 150.00, 50;   -- Electronica
EXEC usp_InsertarProducto 'Detergente Líquido', 'Ariel', 3, 25.00, 40;      -- Limpieza
EXEC usp_InsertarProducto 'Jabón en Barra', 'Bolívar', 3, 2.50, 100;        -- Limpieza
EXEC usp_InsertarProducto 'Té Verde', 'Hornimans', 4, 8.00, 30;             -- Infusión
EXEC usp_InsertarProducto 'Café Instantáneo', 'Nescafé', 4, 15.00, 25;      -- Infusión
EXEC usp_InsertarProducto 'Gaseosa 1.5L', 'Coca-Cola', 5, 7.00, 60;         -- Bebidas
EXEC usp_InsertarProducto 'Jugo de Naranja', 'Valle', 5, 6.50, 40;          -- Bebidas
EXEC usp_InsertarProducto 'Pan de Molde', 'Bimbo', 6, 5.00, 50;             -- Panadería
EXEC usp_InsertarProducto 'Jamón Cocido', 'San Fernando', 7, 12.00, 30;     -- Carnes y Embutidos
EXEC usp_InsertarProducto 'Manzana Roja', 'Delicia', 8, 2.00, 100;          -- Frutas y Verduras
EXEC usp_InsertarProducto 'Arroz Extra', 'Costeño', 9, 3.80, 80;            -- Abarrotes
EXEC usp_InsertarProducto 'Shampoo', 'Sedal', 10, 10.00, 40;                -- Cuidado Personal
EXEC usp_InsertarProducto 'Detergente en Polvo', 'Ace', 3, 18.00, 60;       -- Limpieza
EXEC usp_InsertarProducto 'Papel Higiénico 4 rollos', 'Elite', 11, 9.00, 30;-- Hogar
EXEC usp_InsertarProducto 'Alimento para Perro', 'Dog Chow', 12, 45.00, 20; -- Mascotas
EXEC usp_InsertarProducto 'Helado de Vainilla', 'D´Onofrio', 13, 12.00, 25; -- Congelados
EXEC usp_InsertarProducto 'Helado de Vainilla', 'D´Onofrio', 13, 12.00, 0; -- Congelados
go

exec usp_ListaProducAdmin
go