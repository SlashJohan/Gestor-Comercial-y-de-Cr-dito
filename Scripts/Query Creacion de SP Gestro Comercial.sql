--USE GestorComercialCredito;
--GO

CREATE OR ALTER PROCEDURE dbo.sp_CalcularIndicador
    @EmpresaId INT,
    @PeriodoId INT,
    @IndicadorId INT
AS
BEGIN
    SET NOCOUNT ON;

    DECLARE 
        @ActivoCorriente DECIMAL(18,4) = 0,
        @PasivoCorriente DECIMAL(18,4) = 0,
        @Ingresos DECIMAL(18,4) = 0,
        @UtilidadNeta DECIMAL(18,4) = 0,
        @ActivoTotal DECIMAL(18,4) = 0,
        @Resultado DECIMAL(18,4),
        @FormulaSQL NVARCHAR(500),
        @Sql NVARCHAR(MAX);

    -- Activo Corriente
    SELECT @ActivoCorriente = ISNULL(SUM(mc.Valor), 0)
    FROM dbo.MovimientoContable mc
    JOIN dbo.CuentaPUC c ON c.CuentaId = mc.CuentaId
    WHERE mc.EmpresaId = @EmpresaId
      AND mc.PeriodoId = @PeriodoId
      AND c.TipoCuenta = 'ACTIVO_CORRIENTE';

    -- Pasivo Corriente
    SELECT @PasivoCorriente = ISNULL(SUM(mc.Valor), 0)
    FROM dbo.MovimientoContable mc
    JOIN dbo.CuentaPUC c ON c.CuentaId = mc.CuentaId
    WHERE mc.EmpresaId = @EmpresaId
      AND mc.PeriodoId = @PeriodoId
      AND c.TipoCuenta = 'PASIVO_CORRIENTE';

    -- Ingresos
    SELECT @Ingresos = ISNULL(SUM(mc.Valor), 0)
    FROM dbo.MovimientoContable mc
    JOIN dbo.CuentaPUC c ON c.CuentaId = mc.CuentaId
    WHERE mc.EmpresaId = @EmpresaId
      AND mc.PeriodoId = @PeriodoId
      AND c.TipoCuenta = 'INGRESO';

    -- Utilidad Neta
    SELECT @UtilidadNeta = ISNULL(SUM(mc.Valor), 0)
    FROM dbo.MovimientoContable mc
    JOIN dbo.CuentaPUC c ON c.CuentaId = mc.CuentaId
    WHERE mc.EmpresaId = @EmpresaId
      AND mc.PeriodoId = @PeriodoId
      AND c.TipoCuenta = 'UTILIDAD_NETA';

    -- Activo Total (simplificado)
    SET @ActivoTotal = @ActivoCorriente;

    -- Obtener fórmula (usa variables, no valores)
    SELECT @FormulaSQL = FormulaSQL
    FROM dbo.IndicadorFormula
    WHERE IndicadorId = @IndicadorId;

    -- Armar SQL dinámico con parámetros
    SET @Sql = N'
        SELECT @ResultadoOUT = ' + @FormulaSQL;

    -- Ejecutar fórmula
    EXEC sp_executesql
        @Sql,
        N'
        @ActivoCorriente DECIMAL(18,4),
        @PasivoCorriente DECIMAL(18,4),
        @Ingresos DECIMAL(18,4),
        @UtilidadNeta DECIMAL(18,4),
        @ActivoTotal DECIMAL(18,4),
        @ResultadoOUT DECIMAL(18,4) OUTPUT',
        @ActivoCorriente,
        @PasivoCorriente,
        @Ingresos,
        @UtilidadNeta,
        @ActivoTotal,
        @Resultado OUTPUT;

    -- Insertar resultado
    INSERT INTO dbo.ResultadoIndicador (
        EmpresaId,
        PeriodoId,
        IndicadorId,
        Valor
    )
    VALUES (
        @EmpresaId,
        @PeriodoId,
        @IndicadorId,
        ISNULL(@Resultado, 0)
    );
END;
GO


