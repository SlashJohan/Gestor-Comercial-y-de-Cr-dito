--CREATE DATABASE GestorComercialCredito;
--GO
--USE GestorComercialCredito;
--GO


--CREATE TABLE Empresa (
--    EmpresaId INT IDENTITY PRIMARY KEY,
--    Nit VARCHAR(20) NOT NULL,
--    Nombre VARCHAR(150) NOT NULL,
--    Activa BIT NOT NULL DEFAULT 1,
--    FechaCreacion DATETIME NOT NULL DEFAULT GETDATE()
--);


--CREATE TABLE Periodo (
--    PeriodoId INT IDENTITY PRIMARY KEY,
--    Anio INT NOT NULL
--);


--CREATE TABLE CuentaPUC (
--    CuentaId INT IDENTITY PRIMARY KEY,
--    Codigo VARCHAR(20) NOT NULL,
--    Nombre VARCHAR(150) NOT NULL,
--    TipoCuenta VARCHAR(50) NOT NULL,
--    Activa BIT NOT NULL DEFAULT 1
--);


--CREATE TABLE MovimientoContable (
--    MovimientoId INT IDENTITY PRIMARY KEY,
--    EmpresaId INT NOT NULL,
--    PeriodoId INT NOT NULL,
--    CuentaId INT NOT NULL,
--    Valor DECIMAL(18,2) NOT NULL,

--    CONSTRAINT FK_Mov_Empresa FOREIGN KEY (EmpresaId) REFERENCES Empresa(EmpresaId),
--    CONSTRAINT FK_Mov_Periodo FOREIGN KEY (PeriodoId) REFERENCES Periodo(PeriodoId),
--    CONSTRAINT FK_Mov_Cuenta FOREIGN KEY (CuentaId) REFERENCES CuentaPUC(CuentaId)
--);


--CREATE TABLE Indicador (
--    IndicadorId INT IDENTITY PRIMARY KEY,
--    Nombre VARCHAR(100) NOT NULL,
--    Descripcion VARCHAR(250) NULL,
--    Activo BIT NOT NULL DEFAULT 1
--);


--CREATE TABLE IndicadorFormula (
--    FormulaId INT IDENTITY PRIMARY KEY,
--    IndicadorId INT NOT NULL,
--    FormulaSQL VARCHAR(500) NOT NULL,

--    CONSTRAINT FK_Formula_Indicador FOREIGN KEY (IndicadorId) REFERENCES Indicador(IndicadorId)
--);


--CREATE TABLE ResultadoIndicador (
--    ResultadoId INT IDENTITY PRIMARY KEY,
--    EmpresaId INT NOT NULL,
--    PeriodoId INT NOT NULL,
--    IndicadorId INT NOT NULL,
--    Valor DECIMAL(18,4) NOT NULL,
--    FechaCalculo DATETIME NOT NULL DEFAULT GETDATE(),

--    CONSTRAINT FK_Res_Empresa FOREIGN KEY (EmpresaId) REFERENCES Empresa(EmpresaId),
--    CONSTRAINT FK_Res_Periodo FOREIGN KEY (PeriodoId) REFERENCES Periodo(PeriodoId),
--    CONSTRAINT FK_Res_Indicador FOREIGN KEY (IndicadorId) REFERENCES Indicador(IndicadorId)
--);


/***DATOS BÁSICOS DE PRUEBA***/

/*Empresa*/
--USE GestorComercialCredito;
--INSERT INTO Empresa (Nit, Nombre)
--VALUES ('900123456', 'Empresa Demo');


/*Periodo*/
--USE GestorComercialCredito;
--INSERT INTO Periodo (Anio)
--VALUES (2024);


/*Cuentas PUC*/
--USE GestorComercialCredito;
--INSERT INTO CuentaPUC (Codigo, Nombre, TipoCuenta) VALUES
--('1105', 'Caja', 'ACTIVO_CORRIENTE'),
--('2105', 'Proveedores', 'PASIVO_CORRIENTE'),
--('4105', 'Ingresos Operacionales', 'INGRESO'),
--('5905', 'Utilidad Neta', 'UTILIDAD_NETA'),
--('3105', 'Patrimonio', 'PATRIMONIO');

