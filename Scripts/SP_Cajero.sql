CREATE OR ALTER PROCEDURE SP_Cajero
@PIN CHAR(4),
@NumeroTarjeta VARCHAR(25),
@MontoIngresado DECIMAL(18,2)
AS
BEGIN 
    SET NOCOUNT ON;
    SET TRANSACTION ISOLATION LEVEL READ COMMITTED;

    DECLARE @IdCuenta INT;
    DECLARE @IdTarjeta INT;
    DECLARE @SaldoActual DECIMAL(18,2);
    DECLARE @EstadoTarjeta BIT;
    DECLARE @Intentos INT;
    DECLARE @UsuarioSistema VARCHAR(100) = SUSER_SNAME();

    SELECT @IdTarjeta = Id, @IdCuenta = IdCuenta, @EstadoTarjeta = Estado, @Intentos = ISNULL(IntentosFallidos, 0)
    FROM Tarjeta 
    WHERE NumeroTarjeta = @NumeroTarjeta

    IF @IdTarjeta IS NULL 
        RAISERROR('Tarjeta no reconocida.', 16, 1)

    IF @EstadoTarjeta = 0 
        RAISERROR('La tarjeta se encuentra bloqueada. Contacte al banco.', 16, 1)

    IF @MontoIngresado <= 0 
        RAISERROR('El monto del retiro debe ser mayor a 0', 16, 1)

BEGIN TRY

    IF NOT EXISTS (SELECT 1 FROM Tarjeta 
	WHERE Id = @IdTarjeta AND PIN = @PIN)
    BEGIN
        UPDATE Tarjeta SET IntentosFallidos = @Intentos + 1, 
        Estado = CASE WHEN @Intentos + 1 >= 3 THEN 0 ELSE 1 END,
        FechaBloqueo = CASE WHEN @Intentos + 1 >= 3 THEN GETDATE() ELSE NULL END
        WHERE Id = @IdTarjeta

        IF @Intentos + 1 >= 3
            RAISERROR('PIN incorrecto. Tarjeta bloqueada por seguridad.', 16, 1)
        ELSE
            RAISERROR('PIN incorrecto. Intente de nuevo.', 16, 1)
    END

    BEGIN TRANSACTION;

    UPDATE Tarjeta SET IntentosFallidos = 0 WHERE Id = @IdTarjeta

    SELECT @SaldoActual = Saldo FROM Cuenta WITH(UPDLOCK, ROWLOCK)
    WHERE Id = @IdCuenta AND Estado = 1

    IF @SaldoActual < @MontoIngresado 
    BEGIN
        RAISERROR('Fondos insuficientes.', 16, 1)
    END

	IF @MontoIngresado > 2000.00
	BEGIN
		RAISERROR('El monto máximo por transacción es de Q2,000.00.', 16, 1)
	END

    UPDATE Cuenta SET Saldo = Saldo - @MontoIngresado 
    WHERE Id = @IdCuenta

    INSERT INTO BitacoraTransacciones (IdCuenta, IdTarjeta, IdTipoTransaccion, Monto, SaldoAnterior, SaldoNuevo, Usuario, Detalle, Exito)
    VALUES (@IdCuenta, @IdTarjeta, 4, @MontoIngresado, @SaldoActual, (@SaldoActual - @MontoIngresado), @UsuarioSistema, 'Retiro exitoso en cajero', 1)

    COMMIT TRANSACTION

    SELECT 'Retiro exitoso' AS Mensaje, (@SaldoActual - @MontoIngresado) AS SaldoRestante

END TRY
BEGIN CATCH
    IF @@TRANCOUNT > 0 ROLLBACK TRANSACTION
    IF @IdCuenta IS NOT NULL AND ERROR_MESSAGE() NOT LIKE '%PIN incorrecto%'
    BEGIN
        INSERT INTO BitacoraTransacciones (IdCuenta, IdTarjeta, IdTipoTransaccion, Monto, SaldoAnterior, SaldoNuevo, Usuario, Detalle, Exito)
        VALUES (@IdCuenta, @IdTarjeta, 4, @MontoIngresado, ISNULL(@SaldoActual,0), ISNULL(@SaldoActual,0), @UsuarioSistema, ERROR_MESSAGE(), 0);
    END
    ;THROW; 
END CATCH
END