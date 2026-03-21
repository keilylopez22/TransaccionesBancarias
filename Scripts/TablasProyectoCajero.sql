CREATE TABLE Cuentahabiente (
    Id INT PRIMARY KEY IDENTITY(1,1),
    CUI VARCHAR(50) UNIQUE NOT NULL,
    PrimerNombre VARCHAR(50) NOT NULL,
	SegundoNombre VARCHAR(50) NULL,
	PrimerApellido VARCHAR(50) NOT NULL,
	SegundoApellido VARCHAR(50) NULL,
    FechaNacimiento DATE NOT NULL,
    Direccion VARCHAR(500),
    Telefono VARCHAR(20),
	Email VARCHAR(100) NULL,
    FechaRegistro DATETIME DEFAULT GETDATE()
	)

CREATE TABLE TiposCuenta (
    Id INT PRIMARY KEY,
    Descripcion NVARCHAR(50) NOT NULL 
	)

CREATE TABLE Cuenta (
    Id INT PRIMARY KEY IDENTITY(1,1),
    NumeroCuenta VARCHAR(20) UNIQUE NOT NULL,
    Saldo DECIMAL(18,2) CHECK( Saldo >= 0),
    FechaApertura DATETIME DEFAULT GETDATE(),
    Estado BIT DEFAULT 1, 
    IdCuentahabiente INT FOREIGN KEY REFERENCES Cuentahabiente(Id),
    IdTipoCuenta INT FOREIGN KEY REFERENCES TiposCuenta(Id),
);

CREATE TABLE TiposTarjeta(
    Id INT PRIMARY KEY,
    Descripcion NVARCHAR(50) NOT NULL
	)

CREATE TABLE Tarjeta (
    Id INT PRIMARY KEY IDENTITY(1,1),
    NumeroTarjeta VARCHAR(25) UNIQUE NOT NULL,
    PIN CHAR(4) NOT NULL, 
	CVV CHAR(3) NOT NULL,
	NombreEnTarjeta VARCHAR(26) NOT NULL,
    FechaExpiracion DATE NOT NULL,
	IntentosFallidos INT DEFAULT 0,
	FechaBloqueo DATETIME NULL,
    Estado BIT DEFAULT 0,
    FechaEmision DATETIME DEFAULT GETDATE(),
	IdTipoTarjeta INT FOREIGN KEY REFERENCES TiposTarjeta(Id),
	--IdCuentahabiente INT FOREIGN KEY REFERENCES Cuentahabiente(Id),
	IdCuenta INT FOREIGN KEY REFERENCES Cuenta(Id),
);

CREATE TABLE TipoTransaccion(
    Id INT PRIMARY KEY,
    Descripcion NVARCHAR(50) NOT NULL
	)

CREATE TABLE BitacoraTransacciones (
    Id BIGINT PRIMARY KEY IDENTITY(1,1),
    IdCuenta INT FOREIGN KEY REFERENCES Cuenta(Id),
    IdTarjeta INT NULL FOREIGN KEY REFERENCES Tarjeta(Id),
    IdTipoTransaccion INT NOT NULL FOREIGN KEY REFERENCES TipoTransaccion(Id),
    Monto DECIMAL(18,2) NOT NULL,
    SaldoAnterior DECIMAL(18,2) NOT NULL,
    SaldoNuevo DECIMAL(18,2) NOT NULL,
    FechaMovimiento DATETIME DEFAULT GETDATE(),
    FechaCheque DATE NULL,
	NumeroCheque VARCHAR(20) NULL,
    NumeroDocumento VARCHAR(50) NULL,
    Usuario NVARCHAR(100) NULL,
    Detalle NVARCHAR(500) NULL,
    Exito BIT DEFAULT 1 
);

