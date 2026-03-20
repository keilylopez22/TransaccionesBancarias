-- =============================================
-- SP_ObtenerCuentas: Todas, por Id o por cuentahabiente
-- =============================================
CREATE OR ALTER PROCEDURE SP_ObtenerCuentas
    @Id                INT = NULL,
    @IdCuentahabiente  INT = NULL,
    @NumeroCuenta      VARCHAR(20) = NULL
AS
BEGIN
    SET NOCOUNT ON;
    SELECT c.Id, c.NumeroCuenta, c.Saldo, c.FechaApertura, c.Estado,
           c.IdCuentahabiente, c.IdTipoCuenta, tc.Descripcion AS TipoCuenta,
           ch.PrimerNombre + ' ' + ch.PrimerApellido AS Titular
    FROM Cuenta c
    INNER JOIN TiposCuenta tc ON c.IdTipoCuenta = tc.Id
    INNER JOIN Cuentahabiente ch ON c.IdCuentahabiente = ch.Id
    WHERE (@Id IS NULL OR c.Id = @Id)
      AND (@IdCuentahabiente IS NULL OR c.IdCuentahabiente = @IdCuentahabiente)
      AND (@NumeroCuenta IS NULL OR c.NumeroCuenta = @NumeroCuenta);
END;
GO

-- =============================================
-- SP_ActualizarCuenta
-- Permite cambiar el tipo de cuenta y el estado
-- =============================================
CREATE OR ALTER PROCEDURE SP_ActualizarCuenta
    @Id          INT,
    @IdTipoCuenta INT,
    @Estado      BIT
AS
BEGIN
    SET NOCOUNT ON;
    BEGIN TRY
        IF NOT EXISTS (SELECT 1 FROM Cuenta WHERE Id = @Id)
            RAISERROR('Cuenta no encontrada.', 16, 1);

        UPDATE Cuenta
        SET IdTipoCuenta = @IdTipoCuenta,
            Estado       = @Estado
        WHERE Id = @Id;

        SELECT 'Cuenta actualizada exitosamente.' AS Mensaje;
    END TRY
    BEGIN CATCH
        THROW;
    END CATCH
END;
GO

-- =============================================
-- SP_EliminarCuenta
-- Elimina solo si no tiene tarjetas ni movimientos
-- =============================================
CREATE OR ALTER PROCEDURE SP_EliminarCuenta
    @Id INT
AS
BEGIN
    SET NOCOUNT ON;
    BEGIN TRY
        IF NOT EXISTS (SELECT 1 FROM Cuenta WHERE Id = @Id)
            RAISERROR('Cuenta no encontrada.', 16, 1);

        IF EXISTS (SELECT 1 FROM Tarjeta WHERE IdCuenta = @Id)
            RAISERROR('No se puede eliminar: la cuenta tiene tarjetas asociadas.', 16, 1);

        IF EXISTS (SELECT 1 FROM BitacoraTransacciones WHERE IdCuenta = @Id)
            RAISERROR('No se puede eliminar: la cuenta tiene movimientos registrados.', 16, 1);

        DELETE FROM Cuenta WHERE Id = @Id;

        SELECT 'Cuenta eliminada exitosamente.' AS Mensaje;
    END TRY
    BEGIN CATCH
        THROW;
    END CATCH
END;
GO
