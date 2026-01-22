# Instrucciones para entregar la prueba técnica GCC

## 1. Restaurar paquetes y compilar

```bash
dotnet restore
dotnet build
```

Si falla `UglyToad.PdfPig`, instálalo explícitamente:

```bash
dotnet add package UglyToad.PdfPig --version 1.7.0-custom-5
dotnet restore
dotnet build
```

## 2. Base de datos

Si ya tienes la base **GestorComercialCredito** creada, con datos y funcionando, solo verifica que `appsettings.json` apunte a la cadena de conexión correcta y ejecuta la aplicación.

**Estructura y validación**

- La estructura completa de la base de datos está en **`Scripts/database-schema.sql`**: creación de la BD, tablas (Empresa, CuentaPUC, Periodo, MovimientoContable, Indicador, IndicadorFormula, ResultadoIndicador), claves foráneas y el procedimiento almacenado `sp_CalcularIndicador`. Úsalo como referencia para validar el esquema o para recrear la BD desde cero si hace falta.

**Si montas todo desde cero**

1. Crea la BD y los objetos ejecutando `Scripts/database-schema.sql` (o adaptando las rutas de archivos .mdf/.ldf si tu instalación de SQL Server es distinta).
2. Opcional: **`Scripts/Query Creacion de SP Gestro Comercial.sql`** — definición del SP de cálculo (si no usas `database-schema.sql`).
3. Opcional: **`Scripts/Seed_5_Indicadores.sql`** — inserta los 5 indicadores con fórmulas (solo si la BD está vacía de indicadores).
4. Asegúrate de tener **Cuentas PUC** para la carga de archivos (p. ej. en Gestión > Cuentas), con tipos como `ACTIVO_CORRIENTE`, `PASIVO_CORRIENTE`, `INGRESO`, `UTILIDAD_NETA` y los códigos que uses en el Excel.

## 3. Ejecutar la aplicación

```bash
dotnet run
```

O desde Visual Studio / VS Code. Revisa `appsettings.json` (cadena de conexión) si la BD tiene otro nombre o instancia.

## 4. Backup de la base de datos (entregable)

En **SQL Server Management Studio** (o `sqlcmd`):

1. Clic derecho en la base **GestorComercialCredito** → **Tasks** → **Back Up...**
2. Destino: disco (p. ej. `GestorComercialCredito.bak`).
3. Ejecutar.

Incluye el `.bak` (o el script de schema + datos que uses) en lo que envíes.

## 5. Formato del archivo Excel para carga

En **Carga de Archivo** se admiten **Excel (.xlsx)** y **PDF**.  
El Excel debe tener **primera fila = encabezados** y luego una fila por movimiento:

| NIT | NombreEmpresa | Anio | CodigoCuenta | Valor |
|-----|----------------|------|--------------|-------|
| 900123456 | Empresa Demo | 2024 | 1105 | 1500000 |
| 900123456 | Empresa Demo | 2024 | 2105 | 800000 |

- **NIT**, **CodigoCuenta** y **Valor** son obligatorios.
- **Anio** y **NombreEmpresa** son opcionales (si no hay Anio, se omite la fila).
- Las cuentas deben existir en Gestión > Cuentas (PUC) con el mismo **Código**.

## 6. Entregables

- **Código fuente:** proyecto completo (por ejemplo, carpeta del repo o ZIP).
- **Backup** de la base de datos SQL Server (`.bak` o script equivalente).
- Envío por **WeTransfer** o **repositorio GitHub**, según indique la prueba.
