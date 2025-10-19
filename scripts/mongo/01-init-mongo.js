db = db.getSiblingDB('prueba_tecnica');
db.createCollection('audit_logs');
db.audit_logs.createIndex({ "timestamp": 1 });
db.audit_logs.createIndex({ "entity_type": 1, "entity_id": 1 });
db.audit_logs.createIndex({ "action": 1 });
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
db.createCollection('configurations');
db.configurations.insertOne({
    key: "app_version",
    value: "1.0.0",
    description: "Versión actual de la aplicación",
    created_at: new Date()
});
print("Base de datos 'prueba_tecnica' inicializada correctamente");
print("Colecciones creadas:");
db.getCollectionNames().forEach(function(collection) {
    print("- " + collection + ": " + db.getCollection(collection).countDocuments() + " documentos");
});
