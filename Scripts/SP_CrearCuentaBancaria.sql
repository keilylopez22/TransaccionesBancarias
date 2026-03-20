CREATE OR ALTER PROCEDURE SP_CrearCuentaBancaria
@IdCuentahabiente INT,
@IdTipoCuenta INT,
@SaldoInicial DECIMAL(18,2) = 0
AS
BEGIN
	SET NOCOUNT ON
	DECLARE @NuevoNumeroCuenta VARCHAR(25)
	DECLARE @IdCuentaGenerada INT

BEGIN TRY
		BEGIN TRANSACTION

		IF NOT EXISTS (SELECT 1 FROM Cuentahabiente WHERE Id = @IdCuentahabiente)
		BEGIN
            RAISERROR('El cuentahabiente no existe.', 16, 1);
        END 

		SET @NuevoNumeroCuenta = 
        '4000' +                                         
        RIGHT(CAST(YEAR(GETDATE()) AS VARCHAR), 2) +    
        FORMAT(GETDATE(), 'MM') +                      
        FORMAT(@IdTipoCuenta, '00') +                   
        FORMAT(@IdCuentahabiente % 10000, '0000') +     
        FORMAT(ABS(CHECKSUM(NEWID())) % 10000, '0000') +
        '01';

		INSERT INTO Cuenta (NumeroCuenta, Saldo, IdCuentahabiente, IdTipoCuenta, Estado)
        VALUES (@NuevoNumeroCuenta, @SaldoInicial, @IdCuentahabiente, @IdTipoCuenta, 1)

		SET @IdCuentaGenerada = SCOPE_IDENTITY()

		IF @SaldoInicial > 0
		BEGIN
			INSERT INTO BitacoraTransacciones (IdCuenta, IdTipoTransaccion, Monto, SaldoAnterior, SaldoNuevo, Detalle, Usuario)
            VALUES (@IdCuentaGenerada, 1, @SaldoInicial, 0, @SaldoInicial, 'Apertura de la cuenta', 'Sistema')
		END

		COMMIT TRANSACTION

		SELECT @IdCuentaGenerada AS IdCuenta, @NuevoNumeroCuenta AS NumeroCuenta
END TRY

BEGIN CATCH
	IF @@TRANCOUNT > 0 ROLLBACK TRANSACTION
    THROW
END CATCH

END