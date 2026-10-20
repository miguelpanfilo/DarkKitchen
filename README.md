# DarkKitchen — Plataforma de pedidos para una cocina oculta

   [![Build and Test](https://github.com/miguelpanfilo/DarkKitchen/actions/workflows/build-test.yml/badge.svg)](https://github.com/miguelpanfilo/DarkKitchen/actions/workflows/build-test.yml) [![Code Analysis](https://github.com/miguelpanfilo/DarkKitchen/actions/workflows/code-analysis.yml/badge.svg)](https://github.com/miguelpanfilo/DarkKitchen/actions/workflows/code-analysis.yml)

Aplicación web full-stack para gestionar una **dark kitchen** (cocina que vende solo por delivery): catálogo de productos con fotos, promociones, pedidos con seguimiento de estado, reportes de ventas e importación de productos mediante **plugins**.

> Proyecto obligatorio de **Diseño de Aplicaciones 2** — Universidad ORT Uruguay (Licenciatura en Sistemas).
> Desarrollado en equipo por **Miguel Panfilo**, [@marianoapn](https://github.com/marianoapn) y [@agustinaiglesias-ort](https://github.com/agustinaiglesias-ort).

---

## ✨ Funcionalidades

- **Productos y promociones:** alta, edición y baja de productos con fotos; promociones que agrupan productos.
- **Pedidos con ciclo de vida:** cada pedido pasa por los estados *Pendiente → Preparado → En camino → Entregado*, con casos de *Demorado*, *No entregado* y *Cancelado*.
- **Tipos de envío e impuestos** aplicados al total del pedido.
- **Reportes:** ventas por período y productos más vendidos.
- **Importación de productos por plugins:** importadores de **CSV, JSON y XML** que se cargan dinámicamente desde una carpeta, sin recompilar ni reiniciar la aplicación.
- **Usuarios, roles y sesiones:** administradores, clientes y preparadores, con autenticación por sesión.
- **Auditoría:** registro de las acciones relevantes del sistema.

## 🧩 Diseño

| Patrón | Dónde se usa |
|---|---|
| **State** | Ciclo de vida del pedido (`PendingState`, `PreparedState`, `OnTheWayState`, `DeliveredState`, …) |
| **Visitor** | Generación de reportes sobre los pedidos (`SalesReportVisitor`, `TopProductsReportVisitor`) |
| **Builder** | Construcción de productos (`ProductBuilder`) |
| **Factory** | Creación de estados (`OrderStateFactory`) y de servicios (`FactoryService`) |
| **Plugins / reflection** | Importadores de productos cargados en tiempo de ejecución |

La solución está separada en capas con abstracciones entre ellas (`Dominio`, `LogicaDeNegocio` / `LogicaDeNegocioAbstracciones`, `Almacenamiento` / `AlmacenamientoAbstracciones`, `Contratos`, `Mapeo`, `DarkKitchenWebApi`), y se midió con **NDepend** (abstracción, inestabilidad y distancia de cada paquete).

## 🛠️ Stack

| Capa | Tecnologías |
|---|---|
| Frontend | **Angular 19** · TypeScript |
| Backend | **C# / .NET 8** · ASP.NET Core Web API (REST) · Swagger |
| Persistencia | **Entity Framework Core 8** · **SQL Server 2022** |
| Infraestructura | **Docker** y Docker Compose (API, frontend y base de datos) |
| Testing | **MSTest** · Moq · FluentAssertions · **700+ tests unitarios** · TDD |
| CI | **GitHub Actions** (build, tests y análisis de código) |

## ▶️ Cómo ejecutarlo

Requisito: [Docker Desktop](https://www.docker.com/) corriendo.

```bash
docker compose up --build
```

- Aplicación web: http://localhost:4200
- API REST: http://localhost:5080

Para cargar datos de prueba, imágenes de demo y los plugins de importación, seguí la **[guía de instalación completa](Datos/instalacion.txt)**.

### Credenciales de prueba

| Usuario | Contraseña | Notas |
|---|---|---|
| `admin@gmail.com` | `PasswordSeguraee3!` | Administrador (base vacía) |
| `lucas.gomez@test.com` | `PasswordSeguraee3!` | Disponible si se carga `Datos/db_with_data.sql` |

### Tests

```bash
cd DarkKitchen
dotnet test
```

## 📸 Capturas

<!-- Agregá acá 2 o 3 capturas: catálogo de productos, seguimiento de pedidos y reportes -->

---

👤 **Miguel Panfilo** — [LinkedIn](https://www.linkedin.com/in/miguel-panfilo-214aa2127) · [GitHub](https://github.com/miguelpanfilo)
