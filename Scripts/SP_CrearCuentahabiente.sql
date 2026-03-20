CREATE OR ALTER PROCEDURE SP_CrearCuentahabiente
    @CUI VARCHAR(50),
    @PrimerNombre VARCHAR(50),
    @SegundoNombre VARCHAR(50) = NULL,
    @PrimerApellido VARCHAR(50),
    @SegundoApellido VARCHAR(50) = NULL,
    @FechaNacimiento DATE,
    @Direccion VARCHAR(500) = NULL,
    @Telefono VARCHAR(20) = NULL,
    @Email VARCHAR(100) = NULL
AS
BEGIN
    SET NOCOUNT ON;
    SET TRANSACTION ISOLATION LEVEL SERIALIZABLE; 

    BEGIN TRY
        BEGIN TRANSACTION;
        IF EXISTS (SELECT 1 FROM Cuentahabiente WITH (UPDLOCK) WHERE CUI = @CUI)
        BEGIN
            RAISERROR('El cliente con este CUI ya está registrado.', 16, 1);
        END

        INSERT INTO Cuentahabiente (
            CUI, PrimerNombre, SegundoNombre, PrimerApellido, 
            SegundoApellido, FechaNacimiento, Direccion, Telefono, Email
        ) VALUES (
            @CUI, @PrimerNombre, @SegundoNombre, @PrimerApellido, 
            @SegundoApellido, @FechaNacimiento, @Direccion, @Telefono, @Email
        );

        COMMIT TRANSACTION;
    END TRY
    BEGIN CATCH 
        IF @@TRANCOUNT > 0 
            ROLLBACK TRANSACTION;
        THROW;
    END CATCH
END
