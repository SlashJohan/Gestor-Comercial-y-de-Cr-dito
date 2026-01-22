--USE GestorComercialCredito;
--SELECT * FROM CuentaPUC;

--USE GestorComercialCredito;
--SELECT * FROM Indicador;

--USE GestorComercialCredito;
--SELECT * FROM IndicadorFormula;


--SELECT DB_NAME();


--USE GestorComercialCredito;
--GO

--SELECT name, schema_id
--FROM sys.tables
--WHERE name = 'ResultadoIndicador';

--SELECT *
--FROM dbo.IndicadorFormula;


--UPDATE dbo.IndicadorFormula
--SET FormulaSQL = '@ActivoCorriente / NULLIF(@PasivoCorriente, 0)'
--WHERE IndicadorId = 1;


--USE GestorComercialCredito;
--GO


--SELECT name, schema_id
--FROM sys.tables
--WHERE name = 'ResultadoIndicador';


--EXEC dbo.sp_CalcularIndicador
--    @EmpresaId = 1,
--    @PeriodoId = 1,
--    @IndicadorId = 1;

--SELECT * FROM dbo.ResultadoIndicador;

