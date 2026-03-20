CREATE OR ALTER PROCEDURE SP_CrearTarjeta
@IdCuentaHabiente INT,
@IdCuenta INT,
@IdTipoTarjeta INT
AS
BEGIN
	SET NOCOUNT ON
	DECLARE @NumeroTarjeta VARCHAR(25)
	DECLARE @PIN CHAR(4)
	DECLARE @CVV CHAR(3)
	DECLARE @FechaExpiracion DATE

BEGIN TRY 
	BEGIN TRANSACTION
	--validar
	IF NOT EXISTS (SELECT 1 FROM Cuenta WHERE IdCuentahabiente = @IdCuentaHabiente)
	BEGIN
       RAISERROR('La cuenta no pertenece al cuentahabiente o no existe.', 16, 1);
    END

	IF NOT EXISTS (SELECT 1 FROM Cuentahabiente WHERE Id = @IdCuentaHabiente)
	BEGIN
       RAISERROR('El cuentahabiente no existe.', 16, 1);
    END

	SET @NumeroTarjeta = '4532' + FORMAT(ABS(CHECKSUM(NEWID())) % 1000000000000, '000000000000')
	SET @PIN = FORMAT(ABS(CHECKSUM(NEWID()))% 10000, '0000')
	SET @CVV = FORMAT(ABS(CHECKSUM(NEWID())) % 1000, '000')
	SET @FechaExpiracion = DATEADD(YEAR, 5, GETDATE())

	INSERT INTO Tarjeta(NumeroTarjeta, PIN, CVV, NombreEnTarjeta, FechaExpiracion, 
    Estado, IdTipoTarjeta, IdCuentahabiente, IdCuenta)
	SELECT @NumeroTarjeta, @PIN, @CVV, UPPER(PrimerNombre + ' ' + PrimerApellido), @FechaExpiracion,1, 
	@IdTipoTarjeta, @IdCuentahabiente, @IdCuenta
	FROM Cuentahabiente
	WHERE Id = @IdCuentaHabiente

	COMMIT TRANSACTION

	SELECT 
       @NumeroTarjeta AS NumeroTarjeta, 
       @PIN AS PIN_Generado, 
       @CVV AS CVV, 
       @FechaExpiracion AS Vence

END TRY
BEGIN CATCH 
	IF @@TRANCOUNT > 0 ROLLBACK TRANSACTION;
    THROW
END CATCH 
END