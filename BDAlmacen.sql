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

-- === Tabla de Usuarios === --
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

-- === Tabla de Categorías === --
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

-- === Tabla de Productos === --
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

IF EXISTS (
    SELECT 1 FROM schProductos.Producto
    WHERE nomProd = @nomProd
      AND marcaProd = @marcaProd
      AND idCate = @idCate
      AND precioUnit = @precioUnit
)
BEGIN
    RAISERROR('El producto ya existe.', 16, 1);
    RETURN;
END

BEGIN
    INSERT INTO schProductos.Producto (nomProd, marcaProd, idCate, precioUnit, stock)
    VALUES (@nomProd, @marcaProd, @idCate, @precioUnit, @stock);

    -- Desactivar producto si stock llega a cero
    UPDATE schProductos.Producto
    SET activo = 0
    WHERE idProd = SCOPE_IDENTITY() AND stock <= 0;
END;
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

-- Listar Productos (Clientes)
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

-- Buscar Productos (Clientes)
CREATE OR ALTER PROCEDURE usp_BuscarProductosUsuarioPag
    @busqueda NVARCHAR(200) = NULL,
    @numeroPagina INT,
    @registrosPorPagina INT,
    @totalRegistros INT OUTPUT
AS
BEGIN
    SET NOCOUNT ON;

    IF @busqueda IS NULL OR LTRIM(RTRIM(@busqueda)) = ''
        SET @busqueda = '%';
    ELSE
        SET @busqueda = '%' + @busqueda + '%';

    -- Calcular total de registros activos que coinciden con la búsqueda
    SELECT @totalRegistros = COUNT(*)
    FROM schProductos.Producto P
    JOIN schProductos.Categoria C ON P.idCate = C.idCate
    WHERE
        (
            P.nomProd LIKE @busqueda
            OR P.marcaProd LIKE @busqueda
            OR C.nomCate LIKE @busqueda
        )
        AND P.activo = 1;

    -- Obtener registros paginados activos ordenados por idProd y nomProd
    SELECT P.idProd, P.nomProd, P.marcaProd, C.idCate, C.nomCate, P.precioUnit, P.stock
    FROM schProductos.Producto P
    JOIN schProductos.Categoria C ON P.idCate = C.idCate
    WHERE
        (
            P.nomProd LIKE @busqueda
            OR P.marcaProd LIKE @busqueda
            OR C.nomCate LIKE @busqueda
        )
        AND P.activo = 1
    ORDER BY P.idProd ASC, P.nomProd ASC
    OFFSET (@numeroPagina - 1) * @registrosPorPagina ROWS
    FETCH NEXT @registrosPorPagina ROWS ONLY;
END;
GO

-- Buscar Productos (Admin)
CREATE OR ALTER PROCEDURE usp_BuscarProductosAdminPag
    @busqueda NVARCHAR(200) = NULL,
    @numeroPagina INT,
    @registrosPorPagina INT,
    @totalRegistros INT OUTPUT
AS
BEGIN
    SET NOCOUNT ON;

    IF @busqueda IS NULL OR LTRIM(RTRIM(@busqueda)) = ''
        SET @busqueda = '%';
    ELSE
        SET @busqueda = '%' + @busqueda + '%';

    SELECT @totalRegistros = COUNT(*)
    FROM schProductos.Producto P JOIN schProductos.Categoria C ON P.idCate = C.idCate
    WHERE
        P.nomProd LIKE @busqueda
        OR P.marcaProd LIKE @busqueda
        OR C.nomCate LIKE @busqueda

    SELECT P.idProd, P.nomProd, P.marcaProd, C.idCate, C.nomCate, P.precioUnit, P.stock, P.activo
    FROM schProductos.Producto P
    JOIN schProductos.Categoria C ON P.idCate = C.idCate
    WHERE
        P.nomProd LIKE @busqueda
        OR P.marcaProd LIKE @busqueda
        OR C.nomCate LIKE @busqueda
    ORDER BY P.idProd ASC, P.nomProd ASC
    OFFSET (@numeroPagina - 1) * @registrosPorPagina ROWS
    FETCH NEXT @registrosPorPagina ROWS ONLY;
END;
GO

-- Buscar ID para Comprar producto
CREATE or alter PROCEDURE usp_BuscarProductoPorId
    @IdProd INT
AS
BEGIN
    SET NOCOUNT ON;

    SELECT 
        p.IdProd,
        p.NomProd,
        p.MarcaProd,
        p.PrecioUnit,
        p.Stock,
        p.IdCate,
        c.NomCate,
        p.Activo
    FROM schProductos.Producto p
    INNER JOIN schProductos.Categoria c ON p.IdCate = c.IdCate
    WHERE p.IdProd = @IdProd;
END;
go

-- === Tabla Pedido === --
CREATE TABLE schPedidos.Pedido
(
    idPedido INT IDENTITY(1,1) PRIMARY KEY NOT NULL,
    idUser INT NOT NULL REFERENCES schLogin.Usuario(idUser),
    fecha DATETIME NOT NULL DEFAULT GETDATE(),
    total DECIMAL(12,2) NOT NULL,
    estado VARCHAR(20) NOT NULL DEFAULT 'pendiente'
) ON FGPedidos
GO

-- Crear Pedido(automatizado)
CREATE OR ALTER PROCEDURE usp_CrearPedido
    @idUser INT,
    @total DECIMAL(12,2),
    @estado VARCHAR(20) = 'Pendiente',
    @NuevoPedido INT OUTPUT
AS
BEGIN
    INSERT INTO schPedidos.Pedido (idUser, total, estado)
    VALUES (@idUser, @total, @estado);

    SET @NuevoPedido = SCOPE_IDENTITY();
END;
GO

-- Listar Pedido (automatizado)
create or alter procedure usp_listarPedidos
as
select U.nombres,U.apellidos,U.dni,U.correo,fecha,total,P.estado
	from schPedidos.Pedido P 
	join schLogin.Usuario U on P.idUser = U.idUser
go

-- Eliminar producto y pedido
create or alter procedure usp_EliminarProducto
	@idProd int
as
-- Elimina los detalles de pedido que usan este producto
	delete from schPedidos.DetallePedido where idProd = @idProd
-- Elimina el producto del catalogo
	delete from schProductos.Producto where idProd = @idProd
go

-- Buscador para admin
CREATE OR ALTER PROCEDURE usp_BuscarPedidoUsuario
    @nombreUsuario NVARCHAR(100)
AS
BEGIN
    SELECT 
        p.idPedido,
        u.nombres,
        u.apellidos,
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
    JOIN schLogin.Usuario u ON p.idUser = u.idUser
    WHERE u.nombres LIKE '%' + @nombreUsuario + '%'
       OR u.login LIKE '%' + @nombreUsuario + '%'
       OR u.apellidos LIKE '%' + @nombreUsuario + '%'
    ORDER BY p.fecha DESC;
END;
GO

-- buscador para usuario
CREATE OR ALTER PROCEDURE usp_BuscarPedidoUsuarioFiltrado
    @idUser INT,
    @nombreUsuario NVARCHAR(100),
    @producto NVARCHAR(100)
AS
BEGIN
    SELECT 
        p.idPedido,
        u.nombres,
        u.apellidos,
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
    JOIN schLogin.Usuario u ON p.idUser = u.idUser
    WHERE p.idUser = @idUser
      AND (
           @nombreUsuario = '' OR
           u.nombres LIKE '%' + @nombreUsuario + '%'
           OR u.login LIKE '%' + @nombreUsuario + '%'
           OR u.apellidos LIKE '%' + @nombreUsuario + '%'
          )
      AND (
           @producto = '' OR
           pr.nomProd LIKE '%' + @producto + '%'
          )
    ORDER BY p.fecha DESC;
END
GO

-- === Tabla Carrito === --
CREATE TABLE schPedidos.Carrito
(
    idCarrito INT IDENTITY(1,1) PRIMARY KEY,
    idUser INT NOT NULL REFERENCES schLogin.Usuario(idUser),
    fechaCreacion DATETIME NOT NULL DEFAULT GETDATE()
) ON FGPedidos;
GO

-- == Detalle Carrito === --
CREATE TABLE schPedidos.DetalleCarrito
(
    idDetalleCarrito INT IDENTITY(1,1) PRIMARY KEY,
    idCarrito INT NOT NULL REFERENCES schPedidos.Carrito(idCarrito),
    idProd INT NOT NULL REFERENCES schProductos.Producto(idProd),
    cantidad INT NOT NULL,
    precioUnit DECIMAL(10,2) NOT NULL,
    subtotal AS (cantidad * precioUnit) PERSISTED
) ON FGPedidos;
GO

-- Crear Carrito
CREATE OR ALTER PROCEDURE usp_CrearCarrito
    @idUser INT,
    @idCarrito INT OUTPUT
AS
BEGIN
    SET NOCOUNT ON;

    SELECT TOP 1 @idCarrito = idCarrito
    FROM schPedidos.Carrito
    WHERE idUser = @idUser
    ORDER BY fechaCreacion DESC;

    IF @idCarrito IS NULL
    BEGIN
        INSERT INTO schPedidos.Carrito (idUser)
        VALUES (@idUser);

        SET @idCarrito = SCOPE_IDENTITY();
    END
END;
GO

-- Agregar Producto al Carrito
CREATE OR ALTER PROCEDURE usp_AgregarProductoCarrito
    @idCarrito INT,
    @idProd INT,
    @cantidad INT
AS
BEGIN
    SET NOCOUNT ON;

    DECLARE @precioUnit DECIMAL(10,2);
    DECLARE @stockActual INT;

    SELECT @precioUnit = precioUnit, @stockActual = stock
    FROM schProductos.Producto
    WHERE idProd = @idProd AND activo = 1;

    IF @stockActual IS NULL OR @stockActual < @cantidad
    BEGIN
        RAISERROR('Stock insuficiente o producto inactivo.', 16, 1);
        RETURN;
    END

    IF EXISTS (SELECT 1 FROM schPedidos.DetalleCarrito WHERE idCarrito = @idCarrito AND idProd = @idProd)
    BEGIN
        UPDATE schPedidos.DetalleCarrito
        SET cantidad = cantidad + @cantidad
        WHERE idCarrito = @idCarrito AND idProd = @idProd;
    END
    ELSE
    BEGIN
        INSERT INTO schPedidos.DetalleCarrito (idCarrito, idProd, cantidad, precioUnit)
        VALUES (@idCarrito, @idProd, @cantidad, @precioUnit);
    END
END;
GO

-- listar carrito
CREATE OR ALTER PROCEDURE usp_ListarCarrito
    @idCarrito INT
AS
BEGIN
    SET NOCOUNT ON;

    SELECT dc.idDetalleCarrito, dc.idProd, p.nomProd, p.marcaProd, dc.cantidad, dc.precioUnit, dc.subtotal
    FROM schPedidos.DetalleCarrito dc
    JOIN schProductos.Producto p ON dc.idProd = p.idProd
    WHERE dc.idCarrito = @idCarrito;
END;
GO

-- Eliminar producto del carrito
CREATE OR ALTER PROCEDURE usp_EliminarProductoCarrito
    @idCarrito INT,
    @idProd INT
AS
BEGIN
    DELETE FROM schPedidos.DetalleCarrito
    WHERE idCarrito = @idCarrito AND idProd = @idProd;
END;
GO

-- vaciar carrito
CREATE OR ALTER PROCEDURE usp_VaciarCarrito
    @idCarrito INT
AS
BEGIN
    DELETE FROM schPedidos.DetalleCarrito WHERE idCarrito = @idCarrito;
END;
GO

CREATE OR ALTER PROCEDURE usp_EfectuarCompra
    @idCarrito INT,
    @idUser INT
AS
BEGIN
    SET NOCOUNT ON;

    DECLARE @total DECIMAL(12,2) = 0;
    DECLARE @idPedido INT;

    -- Crear pedido con total 0 temporalmente y obtener el idPedido
    EXEC usp_CrearPedido @idUser, 0, 'Pendiente', @NuevoPedido = @idPedido OUTPUT;

    -- Cursor para recorrer productos del carrito
    DECLARE detalle_cursor CURSOR FOR
    SELECT idProd, cantidad FROM schPedidos.DetalleCarrito WHERE idCarrito = @idCarrito;

    DECLARE @idProd INT, @cantidad INT;

    OPEN detalle_cursor;
    FETCH NEXT FROM detalle_cursor INTO @idProd, @cantidad;

    WHILE @@FETCH_STATUS = 0
    BEGIN
        EXEC usp_InsertarDetallePedido @idPedido, @idProd, @cantidad;
        FETCH NEXT FROM detalle_cursor INTO @idProd, @cantidad;
    END

    CLOSE detalle_cursor;
    DEALLOCATE detalle_cursor;

    -- Actualizar total del pedido
    SELECT @total = SUM(subtotal) FROM schPedidos.DetallePedido WHERE idPedido = @idPedido;
    UPDATE schPedidos.Pedido SET total = @total WHERE idPedido = @idPedido;

    -- Vaciar carrito
    DELETE FROM schPedidos.DetalleCarrito WHERE idCarrito = @idCarrito;
    DELETE FROM schPedidos.Carrito WHERE idCarrito = @idCarrito;
END;
GO

-- === Tabla de Detalle de Pedido === --
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

-- Historial de Pedidos por Usuario
CREATE OR ALTER PROCEDURE usp_HistorialPedidosUsuario
@idUser INT
AS
SELECT 
	p.idPedido,
	u.nombres,
	u.apellidos,
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

CREATE OR ALTER PROCEDURE usp_HistorialPedidosAdmin
AS
SELECT 
    p.idPedido,
    u.nombres,
    u.apellidos,
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
JOIN schLogin.Usuario u ON p.idUser = u.idUser
ORDER BY p.fecha DESC, p.idPedido DESC;
GO

-- Ejemplo de inserciones y pruebas
EXEC usp_CrearUser 'Osiander Stivent', 'Carhuas Marallano', '74643027', 1, 'MRBILL32', 'D@k12345', 'stivent456@gmail.com','Aprobado'
exec usp_CrearUser 'yakuza','zx','12345678',2,'yakuzazx','12345','yakuza@gmail.com','Aprobado'
EXEC usp_ListarUser
EXEC usp_InicioSesion 'MRBILL32','D@k12345'
go

EXEC usp_InsertarProducto 'Yogurt Griego', 'Chobani', 1, 25.50, 30;			-- Lacteos
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
go

exec usp_ListaProducAdmin
go