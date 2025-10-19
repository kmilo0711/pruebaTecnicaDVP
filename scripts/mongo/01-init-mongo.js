// Script de inicialización para MongoDB
// Crear base de datos y colecciones de ejemplo

// Cambiar a la base de datos del proyecto
db = db.getSiblingDB('prueba_tecnica');

// Crear colección de logs de auditoría
db.createCollection('audit_logs');

// Crear índices para optimizar consultas
db.audit_logs.createIndex({ "timestamp": 1 });
db.audit_logs.createIndex({ "entity_type": 1, "entity_id": 1 });
db.audit_logs.createIndex({ "action": 1 });

// Insertar documento de ejemplo
db.audit_logs.insertOne({
    entity_type: "CLIENTE",
    entity_id: 1,
    action: "CREATE",
    timestamp: new Date(),
    user_id: "system",
    changes: {
        nombre: "Juan Pérez",
        identificacion: "12345678",
        correo: "juan.perez@email.com"
    }
});

// Crear colección para configuraciones
db.createCollection('configurations');

// Insertar configuración inicial
db.configurations.insertOne({
    key: "app_version",
    value: "1.0.0",
    description: "Versión actual de la aplicación",
    created_at: new Date()
});

// Mostrar resumen
print("Base de datos 'prueba_tecnica' inicializada correctamente");
print("Colecciones creadas:");
db.getCollectionNames().forEach(function(collection) {
    print("- " + collection + ": " + db.getCollection(collection).countDocuments() + " documentos");
});
