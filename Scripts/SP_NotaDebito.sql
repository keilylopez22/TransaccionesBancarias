CREATE OR ALTER PROCEDURE SP_NotaDebito
@NumeroTarjeta VARCHAR(25),
@Monto DECIMAL(18,2),
@Detalle NVARCHAR(500),
@NumeroDocumento VARCHAR(50) 
AS
BEGIN
    SET NOCOUNT ON;
    SET TRANSACTION ISOLATION LEVEL READ COMMITTED;

    DECLARE @IdCuenta INT;
    DECLARE @IdTarjeta INT;
    DECLARE @SaldoActual DECIMAL(18,2);
    DECLARE @EstadoTarjeta BIT;
    DECLARE @UsuarioSistema VARCHAR(100) = SUSER_SNAME();

    SELECT @IdTarjeta = Id, @IdCuenta = IdCuenta, @EstadoTarjeta = Estado  
    FROM Tarjeta WHERE NumeroTarjeta = @NumeroTarjeta;

    BEGIN TRY
	    
		IF @Monto <= 0
		BEGIN
			RAISERROR('Error: El monto de la nota de débito debe ser mayor a cero.', 16, 1);
		END
        IF @IdTarjeta IS NULL 
        BEGIN
            RAISERROR('Tarjeta no existe.', 16, 1);
        END

        IF @EstadoTarjeta = 0 
        BEGIN
            RAISERROR('Tarjeta bloqueada.', 16, 1);
        END

        BEGIN TRANSACTION;

            SELECT @SaldoActual = Saldo 
            FROM Cuenta WITH(UPDLOCK, ROWLOCK, HOLDLOCK)
            WHERE Id = @IdCuenta AND Estado = 1;

            IF EXISTS (SELECT 1 FROM BitacoraTransacciones 
                       WHERE NumeroDocumento = @NumeroDocumento AND Exito = 1)
            BEGIN
                RAISERROR('Error: La Nota de Débito No. %s ya fue aplicada anteriormente.', 16, 1, @NumeroDocumento);
            END

            IF @SaldoActual IS NULL
			BEGIN
                RAISERROR('Cuenta no encontrada o inactiva.', 16, 1);
			END

			IF (@SaldoActual - @Monto) < 0
            BEGIN
                RAISERROR('Fondos insuficientes. El saldo no puede quedar en negativo.', 16, 1);
            END

            UPDATE Cuenta SET Saldo = Saldo - @Monto WHERE Id = @IdCuenta;

            INSERT INTO BitacoraTransacciones (
                IdCuenta, IdTarjeta, IdTipoTransaccion, Monto, 
                SaldoAnterior, SaldoNuevo, Usuario, Detalle, Exito, NumeroDocumento
            ) VALUES ( 
                @IdCuenta, @IdTarjeta, 4, @Monto, 
                @SaldoActual, (@SaldoActual - @Monto), @UsuarioSistema, @Detalle, 1, @NumeroDocumento
            );

        COMMIT TRANSACTION;

        SELECT 'Nota aplicada' AS Mensaje, @NumeroDocumento AS Documento, (@SaldoActual - @Monto) AS SaldoFinal;

    END TRY
    BEGIN CATCH
        IF @@TRANCOUNT > 0 ROLLBACK TRANSACTION;
        
        IF ERROR_NUMBER() <> 50000 AND @IdTarjeta IS NOT NULL
        BEGIN
             INSERT INTO BitacoraTransacciones (IdCuenta, IdTarjeta, IdTipoTransaccion, Monto, SaldoAnterior, 
             SaldoNuevo, Usuario, Detalle, Exito, NumeroDocumento
        )
        VALUES (@IdCuenta,  @IdTarjeta, 4, @Monto, ISNULL(@SaldoActual, 0), ISNULL(@SaldoActual, 0), 
        @UsuarioSistema, 
        'FALLO: ' + LEFT(ERROR_MESSAGE(), 450), 0, @NumeroDocumento
        )
		END
        ;THROW;
    END CATCH
END
