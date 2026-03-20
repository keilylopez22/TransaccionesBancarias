CREATE OR ALTER PROCEDURE SP_CajeroMaestro
	@IdTipoTransaccion INT,
	@NumeroCuenta VARCHAR(25) = NULL,
	@NumeroTarjeta VARCHAR(25) = NULL,
	@PIN CHAR (4) = NULL,
	@Monto DECIMAL (10,2),
	@NumeroCheque VARCHAR(20) = NULL,
	@FechaCheque DATE= NULL,
	@DetallePersonalizado VARCHAR (500) = NULL 

AS
BEGIN
	SET NOCOUNT ON;
	SET TRANSACTION ISOLATION LEVEL SERIALIZABLE;
	DECLARE @UsuarioSistema VARCHAR (100) = SUSER_SNAME();
	BEGIN TRY 
		IF @IdTipoTransaccion =1 
		BEGIN
			EXEC SP_Deposito @NumeroCuenta= @NumeroCuenta, @Monto = @Monto;
		END

		ELSE IF @IdTipoTransaccion =2
		BEGIN
			EXEC SP_PagoCheque @NumeroCuenta= @NumeroCuenta, @Monto= @Monto, @NumeroCheque= @NumeroCheque, @FechaCheque = @FechaCheque;
		END

		ELSE IF @IdTipoTransaccion = 3
		BEGIN
			EXEC SP_NotaCredito @Numerocuenta = @Numerocuenta, @Monto = @Monto, @Detalle= @DetallePersonalizado;
		END

		ELSE IF @IdTipoTransaccion = 4
		BEGIN
			EXEC SP_NotaDebito @PIN =@PIN, @NumeroTarjeta = @NumeroTarjeta, @MontoIngresado = @Monto; 
		END

		ELSE 
		BEGIN
			RAISERROR('Tipo de transaccion no valido', 16,1);
		END
	END TRY
	BEGIN CATCH
		DECLARE @ErrorMessage VARCHAR(4000)= ERROR_MESSAGE();
		DECLARE @ErrorSeverity INT = ERROR_SEVERITY();
		DECLARE @ErrorState INT = ERROR_STATE();

		RAISERROR (@ErrorMessage, @ErrorSeverity, @ErrorState);
	END CATCH
END;