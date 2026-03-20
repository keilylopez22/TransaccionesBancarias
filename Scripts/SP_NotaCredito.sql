CREATE OR ALTER PROCEDURE SP_NotaCredito
@NumeroCuenta VARCHAR(25),
@Monto DECIMAL(18,2),
@Detalle VARCHAR(500)
AS
BEGIN
	SET NOCOUNT ON
	SET TRANSACTION ISOLATION LEVEL READ COMMITTED

	DECLARE @IdCuenta INT;
	DECLARE @SaldoAnterior DECIMAL(18,2);
	DECLARE @NombreTitular VARCHAR(100);
	DECLARE @UsuarioSistema VARCHAR(100) = SUSER_SNAME();
BEGIN TRY
	
	IF @Monto <= 0
	BEGIN
		RAISERROR('El monto de la nota de credito debe ser mayor a 0', 16, 1)
	END 

BEGIN TRANSACTION

	SELECT @IdCuenta = c.Id,@NombreTitular = ch.PrimerNombre + ' ' + ch.PrimerApellido, @SaldoAnterior = c.Saldo 
	FROM Cuenta c WITH (UPDLOCK, ROWLOCK, HOLDLOCK)
	INNER JOIN Cuentahabiente ch ON c.IdCuentahabiente = ch.Id
	WHERE c.NumeroCuenta = @NumeroCuenta AND Estado = 1

	IF @IdCuenta IS NULL
	BEGIN
		RAISERROR('La cuenta no existe o está inactiva.', 16, 1)
	END

	UPDATE Cuenta SET Saldo = Saldo + @Monto
	WHERE Id = @IdCuenta

	INSERT INTO BitacoraTransacciones (IdCuenta, IdTipoTransaccion, Monto,SaldoAnterior, 
	SaldoNuevo, Usuario, Detalle, Exito)
	VALUES(@IdCuenta, 3, @Monto, @SaldoAnterior, (@SaldoAnterior + @Monto), @UsuarioSistema, @Detalle, 1)

COMMIT TRANSACTION

	SELECT 'Nota de Crédito aplicada con éxito' AS Mensaje, 
	@NombreTitular AS Titular,
	@Monto AS MontoAplicado, 
	(@SaldoAnterior + @Monto) AS NuevoSaldo;

END TRY

BEGIN CATCH
IF @@TRANCOUNT > 0 ROLLBACK TRANSACTION
	IF @IdCuenta IS NOT NULL
BEGIN
    INSERT INTO BitacoraTransacciones (IdCuenta, IdTipoTransaccion, Monto, 
	SaldoAnterior, SaldoNuevo, Usuario, Detalle, Exito)
    VALUES (@IdCuenta, 3,@Monto, ISNULL(@SaldoAnterior, 0), ISNULL(@SaldoAnterior, 0), @UsuarioSistema, ERROR_MESSAGE(), 0);
	END;
;THROW;
END CATCH
END