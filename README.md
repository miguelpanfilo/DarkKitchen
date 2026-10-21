# DarkKitchen — Plataforma de pedidos para una cocina oculta

   [![Build and Test](https://github.com/miguelpanfilo/DarkKitchen/actions/workflows/build-test.yml/badge.svg)](https://github.com/miguelpanfilo/DarkKitchen/actions/workflows/build-test.yml) [![Code Analysis](https://github.com/miguelpanfilo/DarkKitchen/actions/workflows/code-analysis.yml/badge.svg)](https://github.com/miguelpanfilo/DarkKitchen/actions/workflows/code-analysis.yml)

Aplicación web full-stack para gestionar una **dark kitchen** (cocina que vende solo por delivery): catálogo de productos con fotos, promociones, pedidos con seguimiento de estado, reportes de ventas e importación de productos mediante **plugins**.

> Proyecto obligatorio de **Diseño de Aplicaciones 2** — Universidad ORT Uruguay (Licenciatura en Sistemas).
> Desarrollado en equipo por **Miguel Panfilo**, [@marianoapn](https://github.com/marianoapn) y [@agustinaiglesias-ort](https://github.com/agustinaiglesias-ort).

---

## Funcionalidades

- **Productos y promociones:** alta, edición y baja de productos con fotos; promociones que agrupan productos.
- **Pedidos con ciclo de vida:** cada pedido pasa por los estados *Pendiente → Preparado → En camino → Entregado*, con casos de *Demorado*, *No entregado* y *Cancelado*.
- **Tipos de envío e impuestos** aplicados al total del pedido.
- **Reportes:** ventas por período y productos más vendidos.
- **Importación de productos por plugins:** importadores de **CSV, JSON y XML** que se cargan dinámicamente desde una carpeta, sin recompilar ni reiniciar la aplicación.
- **Usuarios, roles y sesiones:** administradores, clientes y preparadores, con autenticación por sesión.
- **Auditoría:** registro de las acciones relevantes del sistema.

## Diseño

| Patrón | Dónde se usa |
|---|---|
| **State** | Ciclo de vida del pedido (`PendingState`, `PreparedState`, `OnTheWayState`, `DeliveredState`, …) |
| **Visitor** | Generación de reportes sobre los pedidos (`SalesReportVisitor`, `TopProductsReportVisitor`) |
| **Builder** | Construcción de productos (`ProductBuilder`) |
| **Factory** | Creación de estados (`OrderStateFactory`) y de servicios (`FactoryService`) |
| **Plugins / reflection** | Importadores de productos cargados en tiempo de ejecución |

La solución está separada en capas con abstracciones entre ellas (`Dominio`, `LogicaDeNegocio` / `LogicaDeNegocioAbstracciones`, `Almacenamiento` / `AlmacenamientoAbstracciones`, `Contratos`, `Mapeo`, `DarkKitchenWebApi`), y se midió con **NDepend** (abstracción, inestabilidad y distancia de cada paquete).

## Stack

| Capa | Tecnologías |
|---|---|
| Frontend | **Angular 19** · TypeScript |
| Backend | **C# / .NET 8** · ASP.NET Core Web API (REST) · Swagger |
| Persistencia | **Entity Framework Core 8** · **SQL Server 2022** |
| Infraestructura | **Docker** y Docker Compose (API, frontend y base de datos) |
| Testing | **MSTest** · Moq · FluentAssertions · **700+ tests unitarios** · TDD |
| CI | **GitHub Actions** (build, tests y análisis de código) |

## Cómo ejecutarlo

**Requisito:** [Docker Desktop](https://www.docker.com/) corriendo.

### 1. Levantar los contenedores

```bash
git clone https://github.com/miguelpanfilo/DarkKitchen.git
cd DarkKitchen
docker compose up --build
```

Esperá hasta ver `dk-webapi | Now listening on: http://[::]:8080`.

### 2. Cargar las fotos de demo

En otra terminal, desde la carpeta del repo:

```bash
docker cp DarkKitchen/demo-assets/fotos-productos/. dk-webapi:/app/photos/
```

### 3. Copiar los plugins de importación

Copiá los `.dll` de `DarkKitchenplugins/` a `DarkKitchen/plugins/`. El sistema los detecta solo, sin reiniciar.

### 4. Cargar los datos de prueba

Conectate a SQL Server con SSMS, Azure Data Studio o la extensión de VS Code:

| Servidor | Usuario | Contraseña |
|---|---|---|
| `localhost,1433` | `sa` | `Passw1rd` |

y ejecutá `Datos/db_with_data.sql`.

### 5. Abrir la aplicación

- **Aplicación web:** http://localhost:4200
- **API REST:** http://localhost:5080

### Credenciales de prueba

| Usuario | Contraseña | Rol |
|---|---|---|
| `admin@gmail.com` | `PasswordSeguraee3!` | Administrador |
| `lucas.gomez@test.com` | `PasswordSeguraee3!` | Disponible tras cargar los datos de prueba |

Para el detalle completo, seguí la [guía de instalación](Datos/instalacion.txt).

### Tests

```bash
cd DarkKitchen
dotnet test
```
### Tests

```bash
cd DarkKitchen
dotnet test
```
## Capturas

| Inicio | Catálogo de productos |
|:---:|:---:|
| <img src="https://github.com/user-attachments/assets/da958aad-5f89-459c-bfb2-193a2afc7f0d" alt="Pantalla de inicio" width="100%"> | <img src="https://github.com/user-attachments/assets/516c225c-a1b6-4a63-9843-9084672890a8" alt="Catálogo de productos" width="100%"> |
| **Gestión de órdenes** | **Importación de productos por plugins** |
| <img src="https://github.com/user-attachments/assets/c54d0935-20cf-4f85-b9b8-7ed6398b6b5b" alt="Gestión de órdenes" width="100%"> | <img src="https://github.com/user-attachments/assets/d6aba76f-07e4-433b-807e-2375f24fcefd" alt="Importación de productos" width="100%"> |


---

👤 **Miguel Panfilo** — [LinkedIn](https://www.linkedin.com/in/miguel-panfilo-214aa2127) · [GitHub](https://github.com/miguelpanfilo)
