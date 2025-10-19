# Microservicio de Auditoría

Microservicio desarrollado en Ruby con Sinatra para el registro y consulta de eventos de auditoría. Utiliza MongoDB como base de datos para almacenar los eventos de auditoría de otros microservicios.

## Características

- **Framework**: Sinatra (Ruby)
- **Base de datos**: MongoDB
- **Puerto**: 5003
- **Arquitectura**: API REST
- **Formato**: JSON

## Estructura del Proyecto

```
microservicioAuditoria/
├── app.rb                 # Aplicación principal Sinatra
├── config/
│   └── database.rb        # Configuración de conexión MongoDB
├── models/
│   └── evento.rb          # Modelo de datos para eventos
├── Gemfile               # Dependencias Ruby
└── README.md             # Documentación
```

## Instalación y Configuración

### Prerrequisitos

- Ruby 3.0+
- MongoDB 4.4+
- Bundler

### Instalación

1. Instalar dependencias:
```bash
bundle install
```

2. Asegurar que MongoDB esté ejecutándose:
```bash
# En sistemas con systemd
sudo systemctl start mongod

# O usando Docker
docker run -d -p 27017:27017 --name mongodb mongo:latest
```

3. Ejecutar el microservicio:
```bash
bundle exec ruby app.rb
```

El servicio estará disponible en `http://localhost:5003`

## API Endpoints

### 1. Crear Evento de Auditoría

**POST** `/auditoria`

Registra un nuevo evento de auditoría en el sistema.

**Headers:**
```
Content-Type: application/json
```

**Body:**
```json
{
  "servicio": "clientes",
  "entidad": "Cliente",
  "entidadId": "123",
  "accion": "CREATE",
  "detalles": "Cliente creado con ID 123"
}
```

**Campos requeridos:**
- `servicio`: Nombre del microservicio que genera el evento
- `entidad`: Tipo de entidad afectada
- `entidadId`: ID de la entidad afectada
- `accion`: Acción realizada (CREATE, UPDATE, DELETE, etc.)

**Campos opcionales:**
- `detalles`: Información adicional sobre el evento

**Respuesta exitosa (201):**
```json
{
  "message": "Evento de auditoría creado exitosamente",
  "evento": {
    "id": "64f8a1b2c3d4e5f6a7b8c9d0",
    "servicio": "clientes",
    "entidad": "Cliente",
    "entidadId": "123",
    "accion": "CREATE",
    "timestamp": "2023-09-06T10:30:00.000Z",
    "detalles": "Cliente creado con ID 123"
  }
}
```

**Ejemplo con curl:**
```bash
curl -X POST http://localhost:5003/auditoria \
  -H "Content-Type: application/json" \
  -d '{
    "servicio": "clientes",
    "entidad": "Cliente", 
    "entidadId": "123",
    "accion": "CREATE",
    "detalles": "Cliente creado con ID 123"
  }'
```

### 2. Consultar Eventos por Entidad

**GET** `/auditoria/:entidad_id`

Obtiene todos los eventos de auditoría para una entidad específica.

**Parámetros:**
- `entidad_id`: ID de la entidad a consultar

**Respuesta exitosa (200):**
```json
{
  "message": "Eventos encontrados para entidadId: 123",
  "total": 2,
  "eventos": [
    {
      "id": "64f8a1b2c3d4e5f6a7b8c9d0",
      "servicio": "clientes",
      "entidad": "Cliente",
      "entidadId": "123",
      "accion": "UPDATE",
      "timestamp": "2023-09-06T11:00:00.000Z",
      "detalles": "Cliente actualizado"
    },
    {
      "id": "64f8a1b2c3d4e5f6a7b8c9d1",
      "servicio": "clientes",
      "entidad": "Cliente",
      "entidadId": "123",
      "accion": "CREATE",
      "timestamp": "2023-09-06T10:30:00.000Z",
      "detalles": "Cliente creado con ID 123"
    }
  ]
}
```

**Ejemplo con curl:**
```bash
curl http://localhost:5003/auditoria/123
```

### 3. Listar Todos los Eventos

**GET** `/auditoria`

Obtiene todos los eventos de auditoría del sistema, ordenados por timestamp descendente.

**Respuesta exitosa (200):**
```json
{
  "message": "Todos los eventos de auditoría",
  "total": 5,
  "eventos": [
    {
      "id": "64f8a1b2c3d4e5f6a7b8c9d2",
      "servicio": "facturas",
      "entidad": "Factura",
      "entidadId": "456",
      "accion": "CREATE",
      "timestamp": "2023-09-06T12:00:00.000Z",
      "detalles": "Factura creada"
    }
  ]
}
```

**Ejemplo con curl:**
```bash
curl http://localhost:5003/auditoria
```

### 4. Health Check

**GET** `/health`

Endpoint para verificar el estado del servicio.

**Respuesta (200):**
```json
{
  "status": "OK",
  "service": "Microservicio de Auditoría",
  "timestamp": "2023-09-06T12:30:00.000Z"
}
```

## Manejo de Errores

### Errores de Validación (400)
```json
{
  "error": "Datos inválidos",
  "message": "Campos requeridos faltantes: servicio, accion"
}
```

### Errores del Servidor (500)
```json
{
  "error": "Error interno del servidor",
  "message": "Descripción del error"
}
```

## Base de Datos

### Colección: eventos

Estructura del documento:
```json
{
  "_id": ObjectId,
  "servicio": String,
  "entidad": String,
  "entidadId": String,
  "accion": String,
  "detalles": String (opcional),
  "timestamp": Date,
  "fechaCreacion": Date
}
```

### Configuración MongoDB

- **Host**: localhost
- **Puerto**: 27017
- **Base de datos**: auditoria_db
- **Colección**: eventos

## Integración con Otros Microservicios

Este microservicio está diseñado para recibir eventos de auditoría de:

- **Microservicio de Clientes** (puerto 5100)
- **Microservicio de Facturas** (puerto 5002)

### Ejemplo de integración desde C#:

```csharp
public async Task RegistrarAuditoria(string entidad, string entidadId, string accion, string detalles = null)
{
    var evento = new
    {
        servicio = "clientes",
        entidad = entidad,
        entidadId = entidadId,
        accion = accion,
        detalles = detalles
    };

    var json = JsonSerializer.Serialize(evento);
    var content = new StringContent(json, Encoding.UTF8, "application/json");
    
    await _httpClient.PostAsync("http://localhost:5003/auditoria", content);
}
```

## Comandos Útiles

```bash
# Instalar dependencias
bundle install

# Ejecutar el servicio
bundle exec ruby app.rb

# Ejecutar en modo desarrollo con recarga automática
bundle exec rerun ruby app.rb

# Verificar estado del servicio
curl http://localhost:5003/health
```

## Logs y Monitoreo

El servicio registra automáticamente:
- Timestamp de cada evento
- Información completa de la entidad afectada
- Detalles de la acción realizada
- Servicio origen del evento

Los eventos se almacenan permanentemente en MongoDB y pueden ser consultados para auditorías y análisis posteriores.
