
-- =============================================
CREATE OR ALTER PROCEDURE SP_ObtenerBitacora
    @IdCuenta         INT      = NULL,
    @IdTarjeta        INT      = NULL,
    @IdTipoTransaccion INT     = NULL,
    @FechaDesde       DATETIME = NULL,
    @FechaHasta       DATETIME = NULL,
    @SoloExitosos     BIT      = NULL
AS
BEGIN
    SET NOCOUNT ON;
    SELECT b.Id, b.IdCuenta, c.NumeroCuenta, b.IdTarjeta,
           b.IdTipoTransaccion, tt.Descripcion AS TipoTransaccion,
           b.Monto, b.SaldoAnterior, b.SaldoNuevo,
           b.FechaMovimiento, b.FechaCheque, b.NumeroCheque,
           b.Usuario, b.Detalle, b.Exito
    FROM BitacoraTransacciones b
    INNER JOIN Cuenta c ON b.IdCuenta = c.Id
    INNER JOIN TipoTransaccion tt ON b.IdTipoTransaccion = tt.Id
    WHERE (@IdCuenta IS NULL OR b.IdCuenta = @IdCuenta)
      AND (@IdTarjeta IS NULL OR b.IdTarjeta = @IdTarjeta)
      AND (@IdTipoTransaccion IS NULL OR b.IdTipoTransaccion = @IdTipoTransaccion)
      AND (@FechaDesde IS NULL OR b.FechaMovimiento >= @FechaDesde)
      AND (@FechaHasta IS NULL OR b.FechaMovimiento <= @FechaHasta)
      AND (@SoloExitosos IS NULL OR b.Exito = @SoloExitosos)
    ORDER BY b.FechaMovimiento DESC;
END;
GO



-- =============================================
CREATE OR ALTER PROCEDURE SP_ObtenerBitacoraPorNumeroCuenta
    @NumeroCuenta VARCHAR(20),
    @FechaDesde   DATETIME = NULL,
    @FechaHasta   DATETIME = NULL
AS
BEGIN
    SET NOCOUNT ON;
    DECLARE @IdCuenta INT;

    SELECT @IdCuenta = Id FROM Cuenta WHERE NumeroCuenta = @NumeroCuenta;

    IF @IdCuenta IS NULL
        RAISERROR('Cuenta no encontrada.', 16, 1);

    EXEC SP_ObtenerBitacora
        @IdCuenta   = @IdCuenta,
        @FechaDesde = @FechaDesde,
        @FechaHasta = @FechaHasta;
END;
GO
