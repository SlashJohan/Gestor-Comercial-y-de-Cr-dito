-- ============================================================
-- Seed: 5 indicadores económicos con fórmulas (prueba técnica GCC)
-- Ejecutar en base de datos GestorComercialCredito.
-- Las fórmulas usan variables del sp_CalcularIndicador:
--   @ActivoCorriente, @PasivoCorriente, @Ingresos, @UtilidadNeta, @ActivoTotal
-- ============================================================

USE GestorComercialCredito;
GO

-- Si ya existen, omitir o eliminar según prefiera. Aquí se insertan solo si no hay datos.

IF NOT EXISTS (SELECT 1 FROM Indicador)
BEGIN
    SET IDENTITY_INSERT Indicador ON;

    INSERT INTO Indicador (IndicadorId, Nombre, Descripcion, Activo) VALUES
    (1, 'Liquidez corriente', 'Activo corriente / Pasivo corriente. Capacidad para cubrir deudas de corto plazo.', 1),
    (2, 'Margen neto', '(Utilidad neta / Ingresos operacionales) × 100. Ganancia final sobre las ventas.', 1),
    (3, 'ROA', '(Utilidad neta / Activo total) × 100. Rentabilidad sobre activos.', 1),
    (4, 'Capital de trabajo', 'Activo corriente - Pasivo corriente. Recursos para operar a corto plazo.', 1),
    (5, 'Rotación de activos', 'Ingresos operacionales / Activo total. Eficiencia del uso de activos.', 1);

    SET IDENTITY_INSERT Indicador OFF;
END
GO

-- Fórmulas (deben usar solo: @ActivoCorriente, @PasivoCorriente, @Ingresos, @UtilidadNeta, @ActivoTotal)
IF NOT EXISTS (SELECT 1 FROM IndicadorFormula)
BEGIN
    INSERT INTO IndicadorFormula (IndicadorId, FormulaSQL) VALUES
    (1, '@ActivoCorriente / NULLIF(@PasivoCorriente, 0)'),
    (2, '(@UtilidadNeta / NULLIF(@Ingresos, 0)) * 100'),
    (3, '(@UtilidadNeta / NULLIF(@ActivoTotal, 0)) * 100'),
    (4, '@ActivoCorriente - @PasivoCorriente'),
    (5, '@Ingresos / NULLIF(@ActivoTotal, 0)');
END
GO
