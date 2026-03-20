CREATE OR ALTER PROCEDURE SP_Deposito
@NumeroCuenta VARCHAR(20),
@Monto DECIMAL(18,2)
AS
BEGIN
    SET NOCOUNT ON;
    SET TRANSACTION ISOLATION LEVEL READ COMMITTED

	DECLARE @IdCuenta INT;
    DECLARE @SaldoAnterior DECIMAL(18,2);
	DECLARE @NombreTitular VARCHAR(100);
	DECLARE @UsuarioSistema VARCHAR(100) = SUSER_SNAME();
BEGIN TRY 

IF @Monto <= 0
BEGIN
	RAISERROR('El monto del deposito debe ser mayor a 0', 16, 1)
END 

BEGIN TRANSACTION

SELECT @IdCuenta = c.Id, @SaldoAnterior = c.Saldo, @NombreTitular = ch.PrimerNombre + ' ' + ch.PrimerApellido
FROM Cuenta c WITH (UPDLOCK, ROWLOCK, HOLDLOCK)
INNER JOIN Cuentahabiente ch ON c.IdCuentahabiente = ch.Id
WHERE c.NumeroCuenta = @NumeroCuenta AND Estado = 1

IF @IdCuenta IS NULL
BEGIN
	RAISERROR('La cuenta no existe o está inactiva.', 16, 1)
END

UPDATE Cuenta SET Saldo = Saldo + @Monto 
WHERE Id = @IdCuenta

INSERT INTO BitacoraTransacciones( IdCuenta, IdTipoTransaccion,Monto, SaldoAnterior, 
SaldoNuevo, Usuario, Detalle, Exito
)
VALUES(@IdCuenta,1, @Monto,@SaldoAnterior, (@SaldoAnterior + @Monto), 
@UsuarioSistema,'Depósito de efectivo realizado con éxito al titular' + @NombreTitular + 'con numero de cuenta NO. ' + @NumeroCuenta,1)

COMMIT TRANSACTION 

SELECT @NumeroCuenta AS Cuenta,  @Monto AS MontoDepositado, 
(@SaldoAnterior + @Monto) AS NuevoSaldo

END TRY

BEGIN CATCH
IF @@TRANCOUNT > 0 ROLLBACK TRANSACTION
IF @IdCuenta IS NOT NULL
        BEGIN
          INSERT INTO BitacoraTransacciones (IdCuenta, IdTipoTransaccion, Monto, SaldoAnterior, SaldoNuevo, Usuario, Detalle, Exito)
           VALUES (@IdCuenta, 1, @Monto, ISNULL(@SaldoAnterior, 0), ISNULL(@SaldoAnterior, 0), @UsuarioSistema, ERROR_MESSAGE(), 0);
        END;
;THROW
END CATCH
END