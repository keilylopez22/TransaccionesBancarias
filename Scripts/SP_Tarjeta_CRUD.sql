-- =============================================
-- SP_ObtenerTarjetas: Todas, por Id o por cuenta
-- =============================================
CREATE OR ALTER PROCEDURE SP_ObtenerTarjetas
    @Id       INT = NULL,
    @IdCuenta INT = NULL
AS
BEGIN
    SET NOCOUNT ON;
    SELECT t.Id, t.NumeroTarjeta, t.NombreEnTarjeta, t.FechaExpiracion,
           t.IntentosFallidos, t.FechaBloqueo, t.Estado, t.FechaEmision,
           t.IdTipoTarjeta, tt.Descripcion AS TipoTarjeta,
           t.IdCuenta, c.NumeroCuenta
    FROM Tarjeta t
    INNER JOIN TiposTarjeta tt ON t.IdTipoTarjeta = tt.Id
    INNER JOIN Cuenta c ON t.IdCuenta = c.Id
    WHERE (@Id IS NULL OR t.Id = @Id)
      AND (@IdCuenta IS NULL OR t.IdCuenta = @IdCuenta);
END;
GO

-- =============================================
-- SP_ActualizarTarjeta
-- Permite bloquear/desbloquear y resetear intentos
-- =============================================
CREATE OR ALTER PROCEDURE SP_ActualizarTarjeta
    @Id               INT,
    @Estado           BIT,
    @IntentosFallidos INT = 0,
    @FechaBloqueo     DATETIME = NULL
AS
BEGIN
    SET NOCOUNT ON;
    BEGIN TRY
        IF NOT EXISTS (SELECT 1 FROM Tarjeta WHERE Id = @Id)
            RAISERROR('Tarjeta no encontrada.', 16, 1);

        UPDATE Tarjeta
        SET Estado           = @Estado,
            IntentosFallidos = @IntentosFallidos,
            FechaBloqueo     = @FechaBloqueo
        WHERE Id = @Id;

        SELECT 'Tarjeta actualizada exitosamente.' AS Mensaje;
    END TRY
    BEGIN CATCH
        THROW;
    END CATCH
END;
GO

-- =============================================
-- SP_DesbloquearTarjeta
-- Resetea intentos y activa la tarjeta
-- =============================================
CREATE OR ALTER PROCEDURE SP_DesbloquearTarjeta
    @Id INT
AS
BEGIN
    SET NOCOUNT ON;
    BEGIN TRY
        IF NOT EXISTS (SELECT 1 FROM Tarjeta WHERE Id = @Id)
            RAISERROR('Tarjeta no encontrada.', 16, 1);

        UPDATE Tarjeta
        SET Estado           = 1,
            IntentosFallidos = 0,
            FechaBloqueo     = NULL
        WHERE Id = @Id;

        SELECT 'Tarjeta desbloqueada exitosamente.' AS Mensaje;
    END TRY
    BEGIN CATCH
        THROW;
    END CATCH
END;
GO

-- =============================================
-- SP_EliminarTarjeta
-- Elimina solo si no tiene movimientos en bitácora
-- =============================================
CREATE OR ALTER PROCEDURE SP_EliminarTarjeta
    @Id INT
AS
BEGIN
    SET NOCOUNT ON;
    BEGIN TRY
        IF NOT EXISTS (SELECT 1 FROM Tarjeta WHERE Id = @Id)
            RAISERROR('Tarjeta no encontrada.', 16, 1);

        IF EXISTS (SELECT 1 FROM BitacoraTransacciones WHERE IdTarjeta = @Id)
            RAISERROR('No se puede eliminar: la tarjeta tiene movimientos registrados.', 16, 1);

        DELETE FROM Tarjeta WHERE Id = @Id;

        SELECT 'Tarjeta eliminada exitosamente.' AS Mensaje;
    END TRY
    BEGIN CATCH
        THROW;
    END CATCH
END;
GO
