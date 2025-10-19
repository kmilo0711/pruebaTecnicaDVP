# Microservicio de Auditoría

Servicio REST construido con Ruby y Sinatra para centralizar eventos de auditoría de los microservicios de Clientes y Facturas.

## Características
- Framework: Sinatra 3
- Base de datos: MongoDB (`auditoria_db`, colección `eventos`)
- Puerto por defecto: 5003
- Respuestas en formato JSON

## Estructura
```
microservicioAuditoria/
├── app.rb                  # Definición de endpoints Sinatra
├── config/
│   └── database.rb         # Conexión MongoDB
├── models/
│   └── evento.rb           # Lógica de eventos de auditoría
├── Gemfile                 # Dependencias Ruby
└── README.md               # Documentación del servicio
```

## Instalación
```bash
cd microservicioAuditoria
bundle install
```

## Ejecución
```bash
bundle exec ruby app.rb
```
El servicio quedará disponible en `http://localhost:5003`.

## Endpoints

### Crear evento
```http
POST /auditoria
Content-Type: application/json
```
Body requerido (`servicio`, `entidad`, `entidadId`, `accion`):
```json
{
  "servicio": "clientes",
  "entidad": "Cliente",
  "entidadId": "123",
  "accion": "CREATE",
  "detalles": "Cliente creado"
}
```

### Listar eventos
```http
GET /auditoria
```

### Buscar por entidad
```http
GET /auditoria/{entidadId}
```

### Health check
```http
GET /health
```

### Endpoint raíz
```http
GET /
```
Devuelve información del servicio y endpoints disponibles.

## Manejo de datos
- Se normalizan claves (`entidadId`, `entidad_id`, `EntidadId`, etc.).
- Se valida presencia de los campos requeridos.
- Se agrega automáticamente `timestamp` y `fechaCreacion` en UTC.

## Dependencias
- `sinatra`
- `mongo`
- `json`
- `webrick`

Instalación manual:
```bash
bundle install
```

## Integración con otros servicios
- Microservicio de Clientes (puerto 5100) y Facturas (puerto 5002) envían eventos vía POST `/auditoria`.

## Solución de problemas
- **400 Datos inválidos**: revisar campos requeridos y formato JSON.
- **500 Error interno**: verificar conexión a MongoDB (`localhost:27017`).
- **Puerto ocupado**: detener proceso previo (`lsof -i :5003`).
