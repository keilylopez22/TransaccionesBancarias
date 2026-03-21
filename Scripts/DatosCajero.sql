INSERT INTO TiposCuenta (Id, Descripcion) VALUES
(1, 'Ahorro'),
(2, 'Monetaria'),
(3, 'Empresarial');

INSERT INTO TiposTarjeta (Id, Descripcion) VALUES
(1, 'D�bito'),
(2, 'Cr�dito');

INSERT INTO TipoTransaccion (Id, Descripcion) VALUES
(1, 'Dep�sito'),
(2, 'Retiro'),
(3, 'Transferencia'),
(4, 'Pago de servicios');

INSERT INTO Cuentahabiente 
(CUI, PrimerNombre, SegundoNombre, PrimerApellido, SegundoApellido, FechaNacimiento, Direccion, Telefono, Email)
VALUES
('3012456780101', 'Juan', 'Carlos', 'P�rez', 'L�pez', '1990-05-10', 'Zona 1, Ciudad', '55551234', 'juan@gmail.com'),
('3012456780102', 'Mar�a', 'Elena', 'G�mez', 'Ram�rez', '1985-08-22', 'Zona 10, Ciudad', '55552345', 'maria@gmail.com'),
('3012456780103', 'Luis', NULL, 'Hern�ndez', 'Castro', '1992-11-15', 'Mixco', '55553456', 'luis@gmail.com'),
('3012456780104', 'Ana', 'Sof�a', 'Morales', NULL, '2000-01-30', 'Villa Nueva', '55554567', 'ana@gmail.com');

INSERT INTO Cuenta 
(NumeroCuenta, Saldo, IdCuentahabiente, IdTipoCuenta)
VALUES
('1000000001', 1500.00, 1, 1),
('1000000002', 2500.50, 2, 2),
('1000000003', 500.75, 3, 1),
('1000000004', 10000.00, 4, 3);

INSERT INTO Tarjeta 
(NumeroTarjeta, PIN, CVV, NombreEnTarjeta, FechaExpiracion, IdTipoTarjeta, IdCuentahabiente, IdCuenta)
VALUES
('BCRD2403MO0025123479', '1234', '123', 'JUAN PEREZ', '2028-12-31', 1, 1, 1),
('BCRD2403MO0025123480', '2345', '234', 'MARIA GOMEZ', '2027-11-30', 2, 2, 2),
('BCRD2403MO0025123481', '3456', '345', 'LUIS HERNANDEZ', '2029-05-15', 1, 3, 3),
('BCRD2403MO0025123482', '4567', '456', 'ANA MORALES', '2030-08-20', 2, 4, 4);

