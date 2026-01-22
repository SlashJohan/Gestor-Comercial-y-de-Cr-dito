# Gestor Comercial de Crédito

Proyecto desarrollado..

##  Tecnologías utilizadas
- ASP.NET Core Razor Pages
- Dapper
- SQL Server
- Bootstrap
- ClosedXML / EPPlus (para carga de Excel)
- Git / GitHub

##  Arquitectura
El proyecto está organizado siguiendo una separación clara de responsabilidades:

- **Pages**: Razor Pages (UI)
- **Models**: Entidades del dominio
- **Repositories**: Acceso a datos con Dapper
- **Database**: Procedimientos almacenados y estructura SQL
- **docs/samples**: Archivos de ejemplo para pruebas

##  Gestión de Empresas
- CRUD completo de empresas
- Activación / inactivación lógica
- Validaciones de campos
- Persistencia en SQL Server

##  Carga de Archivos
El sistema admite carga de archivos **Excel (.xlsx)** y **PDF**.

### Formato del Excel
El archivo debe tener la siguiente estructura:

| NIT | NombreEmpresa | Anio | CodigoCuenta | Valor |
|-----|----------------|------|--------------|-------|

Ejemplos de archivos se encuentran en: la carpeta Docs


##  Indicadores Financieros
El diseño contempla el cálculo de indicadores financieros mediante procedimientos almacenados en SQL Server, ejecutados desde la aplicación usando Dapper.

##  Cómo ejecutar el proyecto
1. Clonar el repositorio
2. Configurar la cadena de conexión en `appsettings.json`
3. Ejecutar los scripts de base de datos
4. Iniciar el proyecto desde Visual Studio o `dotnet run`

##  Notas
- Se utilizan archivos de ejemplo para facilitar la validación funcional

---

**Autor:** Johan Dominguez

