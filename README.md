# 🏗️ Plataforma de Microservicios – Clientes, Facturas y Auditoría

> Sistema distribuido de gestión empresarial construido con arquitectura de microservicios, implementando Clean Architecture y patrones de diseño modernos.

---

## 📋 Tabla de Contenidos
- [Descripción General](#-descripción-general)
- [Principios Arquitectónicos](#-principios-arquitectónicos)
  - [Arquitectura de Microservicios](#arquitectura-de-microservicios)
  - [Clean Architecture](#clean-architecture)
  - [Patrón MVC](#patrón-mvc)
- [Arquitectura del Sistema](#-arquitectura-del-sistema)
- [Requisitos Previos](#-requisitos-previos)
- [Inicio Rápido](#-inicio-rápido)
- [Infraestructura con Docker Compose](#-infraestructura-con-docker-compose)
- [Microservicios](#-microservicios)
  - [Microservicio de Clientes](#1-microservicio-de-clientes)
  - [Microservicio de Facturas](#2-microservicio-de-facturas)
  - [Microservicio de Auditoría](#3-microservicio-de-auditoría)
- [Pruebas Automatizadas](#-pruebas-automatizadas)
- [Solución de Problemas](#-solución-de-problemas)
- [Estructura del Proyecto](#-estructura-del-proyecto)

---

## 🎯 Descripción General

Esta plataforma implementa un sistema de gestión empresarial distribuido en tres microservicios independientes:

- **🧑‍💼 Microservicio de Clientes**: Gestiona el ciclo de vida completo de clientes con persistencia en Oracle Database
- **📄 Microservicio de Facturas**: Procesa facturas, valida clientes y mantiene integridad referencial
- **📊 Microservicio de Auditoría**: Centraliza eventos de auditoría de todos los servicios en MongoDB

Cada microservicio es **autónomo**, **desplegable independientemente** y se comunica mediante **APIs REST** siguiendo principios de diseño moderno.

---

## 🏛️ Principios Arquitectónicos

### Arquitectura de Microservicios

La solución implementa los siguientes principios fundamentales de microservicios:

#### ✅ **Descomposición por Dominio de Negocio**
- Cada microservicio representa un **bounded context** específico (Clientes, Facturas, Auditoría)
- Responsabilidades claramente definidas y separadas
- Cada servicio gestiona su propio dominio de datos

#### ✅ **Autonomía e Independencia**
- **Base de datos por servicio**: Clientes y Facturas usan Oracle (esquemas separados), Auditoría usa MongoDB
- **Tecnologías heterogéneas**: .NET Core (C#) para servicios de negocio, Ruby/Sinatra para auditoría
- **Despliegue independiente**: Cada servicio puede actualizarse sin afectar a los demás

#### ✅ **Comunicación mediante APIs REST**
- Interfaces bien definidas con contratos JSON
- Comunicación síncrona HTTP para operaciones de negocio (Facturas → Clientes)
- Comunicación asíncrona para eventos de auditoría (fire-and-forget)

#### ✅ **Resiliencia y Tolerancia a Fallos**
- Manejo de errores HTTP con códigos de estado apropiados
- Validaciones en cada capa para prevenir propagación de errores
- Logs y auditoría para trazabilidad completa

### Clean Architecture

Cada microservicio .NET sigue los principios de **Clean Architecture** (Arquitectura Hexagonal):

#### 📦 **Capas de la Arquitectura**

```
┌─────────────────────────────────────────┐
│         WebApi / Api (Presentation)     │  ← Controllers, DTOs, Middleware
├─────────────────────────────────────────┤
│         Application (Use Cases)         │  ← Services, Interfaces, Mappers
├─────────────────────────────────────────┤
│         Domain (Business Logic)         │  ← Entities, Value Objects, Rules
├─────────────────────────────────────────┤
│    Infrastructure (External Concerns)   │  ← Repositories, HTTP Clients, DB
└─────────────────────────────────────────┘
```

#### 🎯 **Separación de Responsabilidades**

1. **Domain (Núcleo del Negocio)**
   - Entidades: `Cliente.cs`, `Factura.cs`
   - Lógica de negocio pura sin dependencias externas
   - Validaciones de dominio (ej: monto > 0, email válido)

2. **Application (Casos de Uso)**
   - Servicios: `ClienteService.cs`, `FacturaService.cs`
   - Orquestación de operaciones de negocio
   - Interfaces (puertos): `IClienteRepository`, `IClienteGateway`, `IAuditoriaClient`
   - DTOs para transferencia de datos

3. **Infrastructure (Adaptadores)**
   - Implementaciones concretas: `OracleClienteRepository`, `HttpClienteGateway`
   - Acceso a datos con Oracle Managed Data Access
   - Clientes HTTP para comunicación entre servicios
   - Configuración de base de datos

4. **WebApi/Api (Presentación)**
   - Controllers REST: `ClientesController`, `FacturasController`
   - Configuración de inyección de dependencias
   - Middleware y manejo de errores HTTP

#### ✨ **Beneficios Implementados**

- **Testabilidad**: Lógica de negocio aislada y fácil de probar
- **Mantenibilidad**: Cambios en infraestructura no afectan el dominio
- **Flexibilidad**: Fácil cambio de base de datos o frameworks
- **Inversión de dependencias**: El dominio no depende de detalles técnicos

### Patrón MVC

El **Microservicio de Auditoría** (Ruby/Sinatra) implementa una variante del patrón **MVC**:

#### 🎭 **Componentes MVC en Ruby**

1. **Model** (`models/evento.rb`)
   - Representa la entidad de negocio `Evento`
   - Validaciones y normalización de datos
   - Interacción con MongoDB

2. **View** (Respuestas JSON)
   - Serialización automática a JSON
   - Formato consistente de respuestas HTTP

3. **Controller** (`app.rb`)
   - Endpoints Sinatra que manejan requests HTTP
   - Orquestación de operaciones CRUD
   - Manejo de errores y códigos de estado

#### 🔄 **Flujo de Petición**

```
HTTP Request → Controller (app.rb) → Model (evento.rb) → MongoDB
                    ↓
              JSON Response
```

---

## 🏗️ Arquitectura del Sistema

```mermaid
flowchart TD
    subgraph "Microservicio Clientes (.NET)"
        A[ClientesController<br/>WebApi] --> B[ClienteService<br/>Application]
        B --> C[Cliente<br/>Domain]
        B --> D[OracleClienteRepository<br/>Infrastructure]
        D --> E[(Oracle DB<br/>CLIENTES)]
        B --> F[HttpAuditoriaClient<br/>Infrastructure]
    end
    
    subgraph "Microservicio Facturas (.NET)"
        G[FacturasController<br/>Api] --> H[FacturaService<br/>Application]
        H --> I[Factura<br/>Domain]
        H --> J[OracleFacturaRepository<br/>Infrastructure]
        J --> E
        H --> K[HttpClienteGateway<br/>Infrastructure]
        K -->|Valida Cliente| A
        H --> L[HttpAuditoriaClient<br/>Infrastructure]
    end
    
    subgraph "Microservicio Auditoría (Ruby)"
        M[Sinatra App<br/>app.rb] --> N[Evento Model<br/>models/evento.rb]
        N --> O[(MongoDB<br/>auditoria_db)]
    end
    
    F -->|POST /auditoria| M
    L -->|POST /auditoria| M
    
    style A fill:#e1f5ff
    style G fill:#e1f5ff
    style M fill:#ffe1f5
    style E fill:#fff4e1
    style O fill:#e1ffe1
```

---

## 📦 Requisitos Previos

### Software Requerido

| Componente | Versión | Propósito |
|------------|---------|-----------|
| **.NET SDK** | 7.0+ | Microservicios de Clientes y Facturas |
| **Ruby** | 3.0+ | Microservicio de Auditoría |
| **Bundler** | Latest | Gestión de gemas Ruby |
| **Docker Compose** | v2+ | Orquestación de bases de datos |

### Bases de Datos (via Docker Compose)

| Base de Datos | Versión | Puerto | Uso |
|---------------|---------|--------|-----|
| **Oracle Database XE** | 21.3.0 | 1521 | Clientes y Facturas |
| **MongoDB** | 6.0 | 27017 | Auditoría |

> **Nota**: Docker Compose gestiona automáticamente la infraestructura de bases de datos. No es necesario instalar Oracle o MongoDB localmente.

---

## 🚀 Inicio Rápido

### 1️⃣ Levantar Infraestructura

```bash
# Clonar el repositorio
git clone <repository-url>
cd pruebaTecnicaDVP

# Iniciar bases de datos con Docker Compose
docker compose up -d

# Verificar que los servicios estén corriendo
docker compose ps

# Ver logs (Oracle puede tardar 2-3 minutos en iniciar)
docker compose logs -f
```

### 2️⃣ Inicializar Base de Datos Oracle

```bash
# Ejecutar script de creación de tablas
docker exec -i oracle sqlplus system/oracle123@//localhost:1521/XEPDB1 < scripts/oracle/01-create-tables.sql
```

### 3️⃣ Iniciar Microservicios

**Terminal 1 - Clientes:**
```bash
cd microservicioClientes
dotnet restore
dotnet run --project Clientes.WebApi
# Disponible en http://localhost:5100
```

**Terminal 2 - Facturas:**
```bash
cd microservicioFacturas
dotnet restore
dotnet run --project Facturas.Api
# Disponible en http://localhost:5002
```

**Terminal 3 - Auditoría:**
```bash
cd microservicioAuditoria
bundle install
bundle exec ruby app.rb
# Disponible en http://localhost:5003
```

### 4️⃣ Verificar Funcionamiento

```bash
# Health check de auditoría
curl http://localhost:5003/health

# Crear un cliente
curl -X POST http://localhost:5100/api/clientes \
  -H "Content-Type: application/json" \
  -d '{"nombre":"Juan Pérez","identificacion":"123456789","correo":"juan@example.com","direccion":"Calle 123"}'

# Listar clientes
curl http://localhost:5100/api/clientes
```

---

## 🐳 Infraestructura con Docker Compose

### Servicios Configurados

El archivo `docker-compose.yml` orquesta dos contenedores de bases de datos:

#### 🔷 Oracle Database 21c Express Edition

```yaml
Imagen: container-registry.oracle.com/database/express:21.3.0-xe
Puerto: 1521 (database), 5500 (web console)
Credenciales:
  - Usuario: system
  - Password: oracle123
  - PDB: XEPDB1
Volumen: oracle-data (persistencia)
Healthcheck: Cada 30s
```

**Conexión:**
```
system/oracle123@//localhost:1521/XEPDB1
```

#### 🟢 MongoDB 6

```yaml
Imagen: mongo:6
Puerto: 27017
Credenciales:
  - Usuario: admin
  - Password: admin123
Volumen: mongo-data (persistencia)
Base de datos: auditoria_db
```

### Comandos Útiles

```bash
# Iniciar servicios en segundo plano
docker compose up -d

# Ver logs en tiempo real
docker compose logs -f

# Ver logs de un servicio específico
docker compose logs -f oracle
docker compose logs -f mongodb

# Verificar estado de salud
docker inspect oracle --format='{{.State.Health.Status}}'

# Detener servicios
docker compose down

# Detener y eliminar volúmenes (⚠️ elimina todos los datos)
docker compose down -v

# Reiniciar un servicio específico
docker compose restart oracle
```

### Conectar a las Bases de Datos

**Oracle (SQL*Plus):**
```bash
docker exec -it oracle sqlplus system/oracle123@//localhost:1521/XEPDB1
```

**MongoDB (mongosh):**
```bash
docker exec -it mongodb mongosh -u admin -p admin123
```

### Scripts de Inicialización

Los scripts en `scripts/oracle/` y `scripts/mongo/` se ejecutan automáticamente al crear los contenedores por primera vez:

- **`scripts/oracle/01-create-tables.sql`**: Crea tablas CLIENTES y FACTURAS
- **`scripts/mongo/`**: Scripts opcionales de inicialización MongoDB

---

## 🔧 Microservicios

### 1️⃣ Microservicio de Clientes

> **Stack**: .NET 7, ASP.NET Core, Oracle Managed Data Access  
> **Puerto**: 5100  
> **Base de datos**: Oracle (tabla CLIENTES)

#### 📁 Estructura Clean Architecture

```
microservicioClientes/
├── Clientes.Domain/           # Capa de Dominio
│   └── Cliente.cs             # Entidad con lógica de negocio
├── Clientes.Application/      # Capa de Aplicación
│   ├── DTOs/                  # Data Transfer Objects
│   ├── Interfaces/            # Contratos (IClienteRepository, IAuditoriaClient)
│   └── Services/              # ClienteService (casos de uso)
├── Clientes.Infrastructure/   # Capa de Infraestructura
│   ├── OracleClienteRepository.cs  # Persistencia Oracle
│   └── HttpAuditoriaClient.cs      # Cliente HTTP auditoría
├── Clientes.WebApi/           # Capa de Presentación
│   ├── Controllers/           # ClientesController (API REST)
│   └── Program.cs             # Configuración e inyección de dependencias
└── Clientes.Tests/            # Pruebas unitarias
```

#### 🎯 Funcionalidades

- **Crear cliente**: Validación de datos, persistencia en Oracle, registro de auditoría
- **Consultar cliente**: Por ID o listado completo
- **Validaciones de dominio**:
  - Nombre requerido (máx. 200 caracteres)
  - Identificación única (máx. 50 caracteres)
  - Email válido (formato RFC 5322)
  - Dirección opcional (máx. 300 caracteres)

#### 🚀 Ejecución

```bash
cd microservicioClientes
dotnet restore
dotnet test                          # Ejecutar pruebas
dotnet run --project Clientes.WebApi # Iniciar servicio
```

#### 📡 API Endpoints

| Método | Endpoint | Descripción | Request Body |
|--------|----------|-------------|--------------|
| **POST** | `/api/clientes` | Crear nuevo cliente | `CreateClienteDto` |
| **GET** | `/api/clientes/{id}` | Obtener cliente por ID | - |
| **GET** | `/api/clientes` | Listar todos los clientes | - |

#### 📝 Ejemplo de Uso

**Crear Cliente:**
```bash
curl -X POST http://localhost:5100/api/clientes \
  -H "Content-Type: application/json" \
  -d '{
    "nombre": "María González",
    "identificacion": "987654321",
    "correo": "maria.gonzalez@example.com",
    "direccion": "Av. Principal 456"
  }'
```

**Respuesta (201 Created):**
```json
{
  "id": 1,
  "nombre": "María González",
  "identificacion": "987654321",
  "correo": "maria.gonzalez@example.com",
  "direccion": "Av. Principal 456",
  "fechaCreacion": "2025-10-19T18:30:00Z",
  "activo": true
}
```

---

### 2️⃣ Microservicio de Facturas

> **Stack**: .NET 7, ASP.NET Core, Oracle Managed Data Access, HttpClient  
> **Puerto**: 5002  
> **Base de datos**: Oracle (tabla FACTURAS)

#### 📁 Estructura Clean Architecture

```
microservicioFacturas/
├── Facturas.Domain/           # Capa de Dominio
│   └── Factura.cs             # Entidad con reglas de negocio
├── Facturas.Application/      # Capa de Aplicación
│   ├── DTOs/                  # Data Transfer Objects
│   ├── Interfaces/            # IFacturaRepository, IClienteGateway, IAuditoriaClient
│   └── Services/              # FacturaService (orquestación)
├── Facturas.Infrastructure/   # Capa de Infraestructura
│   ├── OracleFacturaRepository.cs  # Persistencia Oracle
│   ├── HttpClienteGateway.cs       # Cliente HTTP para validar clientes
│   └── HttpAuditoriaClient.cs      # Cliente HTTP auditoría
├── Facturas.Api/              # Capa de Presentación
│   ├── Controllers/           # FacturasController (API REST)
│   └── Program.cs             # Configuración DI
└── Facturas.Tests/            # Pruebas unitarias
```

#### 🎯 Funcionalidades

- **Crear factura**: Valida cliente existente, monto positivo, registra auditoría
- **Consultar factura**: Por ID o rango de fechas
- **Integraciones**:
  - Valida existencia de cliente en Microservicio de Clientes (HTTP)
  - Registra eventos en Microservicio de Auditoría (HTTP)
- **Validaciones de dominio**:
  - Cliente ID requerido y existente
  - Monto total > 0
  - Fecha de emisión válida
  - Estado por defecto: PENDIENTE

#### 🚀 Ejecución

```bash
cd microservicioFacturas
dotnet restore
dotnet test                      # Ejecutar pruebas
dotnet run --project Facturas.Api # Iniciar servicio
```

#### 📡 API Endpoints

| Método | Endpoint | Descripción | Query Params |
|--------|----------|-------------|--------------|
| **POST** | `/api/facturas` | Crear nueva factura | - |
| **GET** | `/api/facturas/{id}` | Obtener factura por ID | - |
| **GET** | `/api/facturas` | Listar facturas | `desde`, `hasta` (fechas) |

#### 📝 Ejemplo de Uso

**Crear Factura:**
```bash
curl -X POST http://localhost:5002/api/facturas \
  -H "Content-Type: application/json" \
  -d '{
    "clienteId": 1,
    "fechaEmision": "2025-10-19",
    "montoTotal": 250000.50
  }'
```

**Respuesta (201 Created):**
```json
{
  "id": 1,
  "clienteId": 1,
  "fechaEmision": "2025-10-19T00:00:00Z",
  "montoTotal": 250000.50,
  "estado": "PENDIENTE"
}
```

**Listar Facturas por Rango:**
```bash
curl "http://localhost:5002/api/facturas?desde=2025-10-01&hasta=2025-10-31"
```

---

### 3️⃣ Microservicio de Auditoría

> **Stack**: Ruby 3, Sinatra, MongoDB Ruby Driver  
> **Puerto**: 5003  
> **Base de datos**: MongoDB (colección eventos)

#### 📁 Estructura MVC

```
microservicioAuditoria/
├── app.rb                     # Controller (endpoints Sinatra)
├── models/
│   └── evento.rb              # Model (entidad Evento)
├── config/
│   └── database.rb            # Configuración MongoDB
├── Gemfile                    # Dependencias Ruby
└── README.md                  # Documentación específica
```

#### 🎯 Funcionalidades

- **Registrar eventos**: Normaliza datos, valida campos requeridos, persiste en MongoDB
- **Consultar eventos**: Listar todos o filtrar por entidadId
- **Health check**: Verifica conectividad con MongoDB
- **Normalización automática**: Soporta camelCase, snake_case, PascalCase
- **Timestamps**: Agrega automáticamente `timestamp` y `fechaCreacion` en UTC

#### 🚀 Ejecución

```bash
cd microservicioAuditoria
bundle install                 # Instalar dependencias
bundle exec ruby app.rb        # Iniciar servicio
```

#### 📡 API Endpoints

| Método | Endpoint | Descripción | Respuesta |
|--------|----------|-------------|-----------|
| **POST** | `/auditoria` | Registrar evento | `201 Created` |
| **GET** | `/auditoria` | Listar todos los eventos | Array de eventos |
| **GET** | `/auditoria/{entidadId}` | Buscar por entidad | Array de eventos |
| **GET** | `/health` | Health check | `{"status": "ok"}` |
| **GET** | `/` | Información del servicio | Metadata |

#### 📝 Ejemplo de Uso

**Registrar Evento:**
```bash
curl -X POST http://localhost:5003/auditoria \
  -H "Content-Type: application/json" \
  -d '{
    "servicio": "facturas",
    "entidad": "Factura",
    "entidadId": "1",
    "accion": "CREATE",
    "detalles": "Factura creada por $250,000.50"
  }'
```

**Respuesta (201 Created):**
```json
{
  "mensaje": "Evento registrado exitosamente",
  "evento": {
    "_id": "507f1f77bcf86cd799439011",
    "servicio": "facturas",
    "entidad": "Factura",
    "entidadId": "1",
    "accion": "CREATE",
    "detalles": "Factura creada por $250,000.50",
    "timestamp": "2025-10-19T18:35:00.000Z",
    "fechaCreacion": "2025-10-19T18:35:00.000Z"
  }
}
```

**Listar Eventos:**
```bash
curl http://localhost:5003/auditoria
```

**Buscar por Entidad:**
```bash
curl http://localhost:5003/auditoria/1
```

---

## 🧪 Pruebas Automatizadas

### Microservicio de Clientes

```bash
cd microservicioClientes
dotnet test --verbosity normal

# Ejecutar pruebas con cobertura
dotnet test /p:CollectCoverage=true
```

**Pruebas incluidas:**
- ✅ Validaciones de entidad `Cliente`
- ✅ Casos de uso en `ClienteService`
- ✅ Mapeo de DTOs
- ✅ Manejo de errores

### Microservicio de Facturas

```bash
cd microservicioFacturas
dotnet test --verbosity normal

# Ejecutar pruebas con cobertura
dotnet test /p:CollectCoverage=true
```

**Pruebas incluidas:**
- ✅ Validaciones de entidad `Factura`
- ✅ Casos de uso en `FacturaService`
- ✅ Integración con gateway de clientes
- ✅ Manejo de errores de validación

### Microservicio de Auditoría

```bash
cd microservicioAuditoria

# Pruebas manuales con curl
curl http://localhost:5003/health

# Prueba de creación de evento
curl -X POST http://localhost:5003/auditoria \
  -H "Content-Type: application/json" \
  -d '{"servicio":"test","entidad":"Test","entidadId":"1","accion":"TEST"}'
```

### Pruebas de Integración End-to-End

```bash
# 1. Crear un cliente
CLIENT_ID=$(curl -s -X POST http://localhost:5100/api/clientes \
  -H "Content-Type: application/json" \
  -d '{"nombre":"Test User","identificacion":"TEST123","correo":"test@example.com"}' \
  | jq -r '.id')

# 2. Crear una factura para ese cliente
curl -X POST http://localhost:5002/api/facturas \
  -H "Content-Type: application/json" \
  -d "{\"clienteId\":$CLIENT_ID,\"fechaEmision\":\"2025-10-19\",\"montoTotal\":100000}"

# 3. Verificar eventos de auditoría
curl http://localhost:5003/auditoria
```

---

## 🔧 Solución de Problemas

### 🔴 Problemas con Docker Compose

#### Oracle no inicia
```bash
# Verificar logs
docker compose logs oracle

# Verificar recursos (Oracle requiere mínimo 2GB RAM)
docker stats

# Reiniciar contenedor
docker compose restart oracle

# Esperar más tiempo (puede tardar hasta 5 minutos)
docker compose logs -f oracle | grep "DATABASE IS READY TO USE"
```

#### MongoDB no conecta
```bash
# Verificar estado
docker compose ps mongodb

# Verificar logs
docker compose logs mongodb

# Probar conexión
docker exec -it mongodb mongosh --eval "db.adminCommand('ping')"
```

#### Puerto ocupado
```bash
# Windows (PowerShell)
netstat -ano | findstr :1521
netstat -ano | findstr :27017

# Detener proceso
taskkill /PID <PID> /F
```

### 🟡 Problemas con Microservicios .NET

#### Error de conexión a Oracle
```bash
# Verificar cadena de conexión en appsettings.json
# Debe ser: "User Id=system;Password=oracle123;Data Source=localhost:1521/XEPDB1"

# Probar conexión manualmente
docker exec -it oracle sqlplus system/oracle123@//localhost:1521/XEPDB1
```

#### Dependencias no resueltas
```bash
cd microservicioClientes
dotnet clean
dotnet restore --force
dotnet build
```

#### Puerto 5100/5002 ocupado
```bash
# Cambiar puerto en launchSettings.json o usar variable de entorno
export ASPNETCORE_URLS="http://localhost:5101"
dotnet run --project Clientes.WebApi
```

### 🟢 Problemas con Microservicio Ruby

#### Gemas no instaladas
```bash
cd microservicioAuditoria
bundle install --path vendor/bundle
bundle exec ruby app.rb
```

#### Error de conexión MongoDB
```bash
# Verificar configuración en config/database.rb
# Por defecto: mongodb://localhost:27017/auditoria_db

# Si MongoDB tiene autenticación:
# mongodb://admin:admin123@localhost:27017/auditoria_db?authSource=admin
```

#### Puerto 5003 ocupado
```bash
# Cambiar puerto en app.rb
# set :port, 5004

# O usar variable de entorno
PORT=5004 bundle exec ruby app.rb
```

### ⚠️ Errores Comunes de API

| Error | Causa | Solución |
|-------|-------|----------|
| **ORA-00001: unique constraint violated** | Identificación de cliente duplicada | Usar identificación única |
| **400 Bad Request - Cliente no existe** | ClienteId inválido en factura | Verificar que el cliente exista primero |
| **400 Bad Request - Datos inválidos** | JSON malformado o campos faltantes | Validar estructura JSON |
| **500 Internal Server Error** | Error de base de datos o servicio caído | Revisar logs del servicio |
| **503 Service Unavailable** | Servicio dependiente no disponible | Verificar que todos los servicios estén corriendo |

---

## 📂 Estructura del Proyecto

```
pruebaTecnicaDVP/
│
├── 📄 README.md                          # Este archivo (documentación completa)
├── 🐳 docker-compose.yml                 # Orquestación de bases de datos
│
├── 📁 microservicioClientes/             # Microservicio .NET - Clientes
│   ├── Clientes.Domain/                  # Entidades y lógica de negocio
│   ├── Clientes.Application/             # Casos de uso y DTOs
│   ├── Clientes.Infrastructure/          # Repositorios y clientes HTTP
│   ├── Clientes.WebApi/                  # API REST (puerto 5100)
│   └── Clientes.Tests/                   # Pruebas unitarias
│
├── 📁 microservicioFacturas/             # Microservicio .NET - Facturas
│   ├── Facturas.Domain/                  # Entidades y reglas de negocio
│   ├── Facturas.Application/             # Casos de uso y DTOs
│   ├── Facturas.Infrastructure/          # Repositorios y gateways
│   ├── Facturas.Api/                     # API REST (puerto 5002)
│   └── Facturas.Tests/                   # Pruebas unitarias
│
├── 📁 microservicioAuditoria/            # Microservicio Ruby - Auditoría
│   ├── app.rb                            # Aplicación Sinatra (puerto 5003)
│   ├── models/evento.rb                  # Modelo de eventos
│   ├── config/database.rb                # Configuración MongoDB
│   ├── Gemfile                           # Dependencias Ruby
│   └── README.md                         # Documentación específica
│
└── 📁 scripts/                           # Scripts de inicialización
    ├── oracle/
    │   └── 01-create-tables.sql          # DDL para tablas Oracle
    └── mongo/
        └── (scripts opcionales)
```

---

## 🎓 Resumen de Tecnologías

| Componente | Tecnologías |
|------------|-------------|
| **Microservicios .NET** | C#, .NET 7, ASP.NET Core, Oracle.ManagedDataAccess |
| **Microservicio Ruby** | Ruby 3, Sinatra, MongoDB Ruby Driver |
| **Bases de Datos** | Oracle Database 21c XE, MongoDB 6 |
| **Contenedores** | Docker, Docker Compose |
| **Patrones** | Clean Architecture, Microservicios, MVC, Repository, Gateway |
| **Comunicación** | REST APIs, HTTP/JSON |
| **Testing** | xUnit, Moq (para .NET) |

---

## 📚 Recursos Adicionales

### Documentación de APIs

- **Swagger UI Clientes**: http://localhost:5100/swagger (si está habilitado)
- **Swagger UI Facturas**: http://localhost:5002/swagger (si está habilitado)

### Monitoreo

```bash
# Ver logs en tiempo real de todos los servicios
docker compose logs -f

# Monitorear recursos
docker stats

# Ver procesos .NET
dotnet --list-runtimes
```

---

## 👨‍💻 Autor

**Juan Camilo Rodriguez Amaya**

**Proyecto de Prueba Técnica - Arquitectura de Microservicios**

Implementación de sistema distribuido con Clean Architecture, siguiendo principios SOLID y mejores prácticas de desarrollo.

---

## 📝 Licencia

Este proyecto es parte de una prueba técnica y está disponible para fines educativos y de evaluación.

---

<div align="center">

**🚀 ¡Gracias por revisar este proyecto! 🚀**

*Construido con ❤️ usando .NET, Ruby, Oracle y MongoDB*

</div>
