
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
CREATE OR ALTER PROCEDURE SP_DesbloquearTarjeta
    @Id            INT           = NULL,
    @NumeroTarjeta VARCHAR(25)   = NULL,
    @Usuario       NVARCHAR(100) = NULL
AS
BEGIN
    SET NOCOUNT ON;

    DECLARE @IdTarjeta        INT;
    DECLARE @IdCuenta         INT;
    DECLARE @EstadoActual     BIT;
    DECLARE @IntentosActuales INT;
    DECLARE @NumTarjeta       VARCHAR(25);
    DECLARE @UsuarioFinal     NVARCHAR(100) = ISNULL(@Usuario, SUSER_SNAME());

    BEGIN TRY

        IF @Id IS NULL AND @NumeroTarjeta IS NULL
            RAISERROR('Debe proporcionar el Id o el Número de Tarjeta.', 16, 1);

        SELECT
            @IdTarjeta        = Id,
            @IdCuenta         = IdCuenta,
            @EstadoActual     = Estado,
            @IntentosActuales = IntentosFallidos,
            @NumTarjeta       = NumeroTarjeta
        FROM Tarjeta
        WHERE (@Id IS NULL           OR Id            = @Id)
          AND (@NumeroTarjeta IS NULL OR NumeroTarjeta = @NumeroTarjeta);

        IF @IdTarjeta IS NULL
            RAISERROR('Tarjeta no encontrada.', 16, 1);

        IF @EstadoActual = 1 AND @IntentosActuales = 0
            RAISERROR('La tarjeta ya se encuentra activa y sin intentos fallidos.', 16, 1);

        BEGIN TRANSACTION;

            UPDATE Tarjeta
            SET Estado           = 1,
                IntentosFallidos = 0,
                FechaBloqueo     = NULL
            WHERE Id = @IdTarjeta;

            INSERT INTO BitacoraTransacciones
                (IdCuenta, IdTarjeta, IdTipoTransaccion, Monto,
                 SaldoAnterior, SaldoNuevo, Usuario, Detalle, Exito)
            SELECT
                @IdCuenta, @IdTarjeta, 1, 0, Saldo, Saldo,
                @UsuarioFinal,
                'Desbloqueo administrativo de tarjeta. Intentos reiniciados desde ' +
                    CAST(@IntentosActuales AS VARCHAR) + ' a 0.',
                1
            FROM Cuenta WHERE Id = @IdCuenta;

        COMMIT TRANSACTION;

        SELECT
            'Tarjeta desbloqueada exitosamente.' AS Mensaje,
            @NumTarjeta                          AS NumeroTarjeta,
            @IntentosActuales                    AS IntentosPrevios,
            0                                    AS IntentosActuales,
            GETDATE()                            AS FechaDesbloqueo;

    END TRY
    BEGIN CATCH
        IF @@TRANCOUNT > 0 ROLLBACK TRANSACTION;
        THROW;
    END CATCH
END;
GO



