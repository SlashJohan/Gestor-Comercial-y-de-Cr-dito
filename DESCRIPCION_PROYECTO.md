# Gestor Comercial y de Crédito - Descripción del Proyecto

## 📋 Resumen Ejecutivo

**Gestor Comercial y de Crédito** es una aplicación web desarrollada en **.NET 7.0 con Razor Pages** que permite gestionar información financiera de empresas, cargar datos contables desde archivos Excel o PDF, y calcular automáticamente indicadores económicos mediante procedimientos almacenados en SQL Server.

Este proyecto fue desarrollado como **prueba técnica** para el cargo de **Desarrollador .NET Intermedio** y cumple con todos los requisitos funcionales especificados.

---

## 🎯 Objetivo Principal

Automatizar la carga de información financiera de múltiples empresas, almacenarla en una base de datos SQL Server, y calcular indicadores económicos básicos de forma dinámica y paramétrica.

---

## 🏗️ Arquitectura y Tecnologías

### Backend / Frontend
- **.NET 7.0** con **ASP.NET Core Razor Pages**
- **Dapper** para acceso a datos (ORM ligero)
- **Microsoft.Data.SqlClient** para conexión a SQL Server
- **ClosedXML** para lectura de archivos Excel (.xlsx)
- **UglyToad.PdfPig** para lectura de archivos PDF

### Base de Datos
- **SQL Server** (tablas, procedimientos almacenados, funciones)
- **Procedimiento almacenado** `sp_CalcularIndicador` para cálculo dinámico de indicadores

### Patrón de Diseño
- **Repository Pattern** para abstracción de acceso a datos
- **Service Layer** para lógica de negocio
- **Dependency Injection** para gestión de dependencias

---

## 📊 Funcionalidades Principales

### 1. Gestión de Empresas
- **Crear** nuevas empresas (NIT, Nombre)
- **Modificar** información de empresas existentes
- **Inactivar/Activar** empresas (soft delete)
- Validación de datos y manejo de errores

### 2. Gestión de Cuentas (PUC - Plan Único de Cuentas)
- **Crear** cuentas contables con código, nombre y tipo
- **Modificar** información de cuentas
- **Inactivar/Activar** cuentas
- Tipos de cuenta soportados:
  - `ACTIVO_CORRIENTE`
  - `PASIVO_CORRIENTE`
  - `INGRESO`
  - `UTILIDAD_NETA`
  - `PATRIMONIO`
  - Y otros según necesidad

### 3. Gestión de Indicadores
- **Crear** indicadores económicos con nombre y descripción
- **Definir fórmulas SQL** para cada indicador de forma paramétrica
- **Modificar** y **eliminar** indicadores
- Las fórmulas usan variables predefinidas:
  - `@ActivoCorriente`
  - `@PasivoCorriente`
  - `@Ingresos`
  - `@UtilidadNeta`
  - `@ActivoTotal`

### 4. Carga de Archivos Excel / PDF
- **Subida de archivos** Excel (.xlsx) o PDF
- **Procesamiento automático** de datos contables
- **Formato esperado**:
  - Primera fila: encabezados (`NIT`, `NombreEmpresa`, `Anio`, `CodigoCuenta`, `Valor`)
  - Filas siguientes: datos de movimientos contables
- **Funcionalidades**:
  - Creación automática de empresas si no existen (por NIT)
  - Creación automática de periodos si no existen (por año)
  - Validación de cuentas (deben existir previamente)
  - Inserción masiva de movimientos contables
  - **Cálculo automático de indicadores** tras la carga

### 5. Consulta de Indicadores
- **Visualización** de indicadores calculados
- **Filtros**:
  - Por empresa (NIT)
  - Por año (periodo)
  - Por indicador
- Muestra: empresa, NIT, indicador, periodo, valor calculado y fecha de cálculo

---

## 🔄 Flujo de Trabajo

### Proceso Completo

1. **Configuración Inicial** (una vez):
   - Crear cuentas PUC en "Gestión > Cuentas"
   - Crear indicadores y definir sus fórmulas en "Gestión > Indicadores"
   - (Opcional) Crear empresas manualmente en "Gestión > Empresas"

2. **Carga de Datos**:
   - Preparar archivo Excel/PDF con datos contables
   - Subir archivo en "Carga de Archivo"
   - El sistema:
     - Lee y valida el archivo
     - Crea empresas y periodos si no existen
     - Inserta movimientos contables en la BD
     - **Ejecuta automáticamente** `sp_CalcularIndicador` para cada combinación (Empresa, Periodo, Indicador)
     - Guarda resultados en `ResultadoIndicador`

3. **Consulta de Resultados**:
   - Ir a "Consulta de Indicadores"
   - Aplicar filtros según necesidad
   - Visualizar indicadores calculados

---

## 📈 Indicadores Económicos Implementados

El sistema incluye **5 indicadores** predefinidos (según requisitos de la prueba):

1. **Liquidez corriente**: `Activo corriente / Pasivo corriente`
   - Mide la capacidad de la empresa para cubrir deudas de corto plazo

2. **Margen neto**: `(Utilidad neta / Ingresos operacionales) × 100`
   - Mide la ganancia final sobre las ventas

3. **ROA (Rentabilidad sobre activos)**: `(Utilidad neta / Activo total) × 100`
   - Evalúa la eficiencia en el uso de los activos

4. **Capital de trabajo**: `Activo corriente - Pasivo corriente`
   - Recursos disponibles para operar a corto plazo

5. **Rotación de activos**: `Ingresos operacionales / Activo total`
   - Eficiencia en el uso de los activos para generar ventas

**Nota**: El sistema permite crear más indicadores con fórmulas personalizadas.

---

## 🗄️ Estructura de Base de Datos

### Tablas Principales

- **Empresa**: Información de empresas (NIT, Nombre, Activa)
- **CuentaPUC**: Plan de cuentas contables (Código, Nombre, TipoCuenta, Activa)
- **Periodo**: Periodos contables (Año)
- **MovimientoContable**: Movimientos financieros (EmpresaId, PeriodoId, CuentaId, Valor)
- **Indicador**: Definición de indicadores (Nombre, Descripción, Activo)
- **IndicadorFormula**: Fórmulas SQL de cada indicador (IndicadorId, FormulaSQL)
- **ResultadoIndicador**: Resultados calculados (EmpresaId, PeriodoId, IndicadorId, Valor, FechaCalculo)

### Procedimientos Almacenados

- **`sp_CalcularIndicador`**: Calcula un indicador específico para una empresa y periodo dados
  - Suma movimientos contables por tipo de cuenta
  - Ejecuta fórmula SQL dinámica
  - Inserta resultado en `ResultadoIndicador`

---

## 🎨 Interfaz de Usuario

- **Diseño moderno** con Bootstrap 5
- **Navegación intuitiva** con menú desplegable
- **Formularios validados** con mensajes de error claros
- **Tablas responsivas** para visualización de datos
- **Mensajes de éxito/error** mediante TempData

---

## 📁 Estructura del Proyecto

```
GestorComercialCredito.Web/
├── Models/              # Modelos de datos (Empresa, Cuenta, Indicador, etc.)
├── Pages/               # Razor Pages (UI)
│   ├── Companies/       # Gestión de empresas
│   ├── Cuentas/         # Gestión de cuentas PUC
│   ├── Indicadores/     # Gestión de indicadores
│   ├── CargaArchivo/    # Carga de archivos Excel/PDF
│   └── ConsultaIndicadores/  # Consulta de resultados
├── Repositories/        # Capa de acceso a datos (Repository Pattern)
├── Services/            # Servicios de negocio
│   ├── CargaArchivoService.cs
│   └── IndicadorCalculationService.cs
├── Scripts/             # Scripts SQL
│   ├── database-schema.sql
│   ├── Query Creacion de SP Gestro Comercial.sql
│   └── Seed_5_Indicadores.sql
└── wwwroot/             # Archivos estáticos (CSS, JS)
```

---

## ✅ Cumplimiento de Requisitos

### Requerimientos Funcionales

✅ **Carga de datos**: Sistema permite cargar información cualitativa y financiera  
✅ **Estructura de datos**: Datos almacenados en tablas normalizadas  
✅ **Procesamiento**: Cálculo de indicadores en BD mediante procedimientos almacenados  
✅ **Fórmulas paramétricas**: Indicadores con fórmulas SQL dinámicas  
✅ **Interfaz de usuario**: 5 formularios/páginas requeridas implementadas

### Entregables

✅ **Código fuente completo**: Proyecto .NET 7.0 con toda la funcionalidad  
✅ **Backup de BD**: Scripts SQL para recrear la base de datos

---

## 🚀 Características Técnicas Destacadas

1. **Cálculo dinámico**: Las fórmulas de indicadores se ejecutan como SQL dinámico, permitiendo modificarlas sin cambiar código
2. **Carga masiva**: Inserción eficiente de múltiples movimientos contables
3. **Validación robusta**: Verificación de datos antes de insertar
4. **Manejo de errores**: Mensajes claros y manejo de excepciones
5. **Separación de responsabilidades**: Arquitectura limpia con Repository y Service layers

---

## 📝 Notas de Implementación

- El sistema **crea automáticamente** empresas y periodos si no existen durante la carga
- Las **cuentas PUC deben existir previamente** (creadas manualmente)
- Los **indicadores se calculan automáticamente** tras cada carga exitosa
- El cálculo **reemplaza resultados previos** para las mismas combinaciones (Empresa, Periodo)
- Soporte para **Excel (.xlsx)** y **PDF** (con limitaciones: PDF debe tener texto extraíble)

---

## 🔧 Configuración

- **Cadena de conexión**: Configurada en `appsettings.json`
- **Tamaño máximo de archivo**: 10MB (configurable en `Program.cs`)
- **Base de datos**: SQL Server (nombre: `GestorComercialCredito`)

---

## 📚 Documentación Adicional

- **`INSTRUCCIONES_ENTREGA.md`**: Guía para ejecutar y entregar el proyecto
- **`Scripts/database-schema.sql`**: Estructura completa de la base de datos
- **`Scripts/Seed_5_Indicadores.sql`**: Script para insertar los 5 indicadores predefinidos

---

## 👨‍💻 Desarrollo

**Tecnología**: .NET 7.0, ASP.NET Core Razor Pages, SQL Server  
**Tiempo estimado de desarrollo**: 6 horas (según especificación de prueba técnica)  
**Patrón**: Repository Pattern + Service Layer  
**ORM**: Dapper (micro-ORM)

---

## 📄 Licencia

Proyecto desarrollado como prueba técnica. Todos los derechos reservados.
