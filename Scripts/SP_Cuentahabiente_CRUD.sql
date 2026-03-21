-- =============================================
-- SP_ObtenerCuentahabientes Obtener todos o por Id
-- =============================================
CREATE OR ALTER PROCEDURE SP_ObtenerCuentahabientes
    @Id INT = NULL
AS
BEGIN
    SET NOCOUNT ON;
    SELECT Id, CUI, PrimerNombre, SegundoNombre, PrimerApellido, SegundoApellido,
           FechaNacimiento, FechaRegistro
    FROM Cuentahabiente
    WHERE (@Id IS NULL OR Id = @Id);
END;
GO

-- =============================================
-- SP_ActualizarCuentahabiente
-- =============================================
CREATE OR ALTER PROCEDURE SP_ActualizarCuentahabiente
    @Id              INT,
    @CUI             VARCHAR(50),
    @PrimerNombre    VARCHAR(50),
    @SegundoNombre   VARCHAR(50) = NULL,
    @PrimerApellido  VARCHAR(50),
    @SegundoApellido VARCHAR(50) = NULL,
    @FechaNacimiento DATE
AS
BEGIN
    SET NOCOUNT ON;
    BEGIN TRY
        IF NOT EXISTS (SELECT 1 FROM Cuentahabiente WHERE Id = @Id)
            RAISERROR('Cuentahabiente no encontrado.', 16, 1);

        IF EXISTS (SELECT 1 FROM Cuentahabiente WHERE CUI = @CUI AND Id <> @Id)
            RAISERROR('El CUI ya está registrado en otro cuentahabiente.', 16, 1);

        UPDATE Cuentahabiente
        SET CUI             = @CUI,
            PrimerNombre    = @PrimerNombre,
            SegundoNombre   = @SegundoNombre,
            PrimerApellido  = @PrimerApellido,
            SegundoApellido = @SegundoApellido,
            FechaNacimiento = @FechaNacimiento
        WHERE Id = @Id;

        SELECT 'Cuentahabiente actualizado exitosamente.' AS Mensaje;
    END TRY
    BEGIN CATCH
        THROW;
    END CATCH
END;
GO

