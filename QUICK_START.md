# Quick Start Guide

## Problema Resuelto: Docker Compose en WSL 2

El error que recibiste se debe a que necesitas usar `docker compose` (sin guión) en lugar de `docker-compose`. Docker Compose v2 usa la nueva sintaxis.

## Comandos Correctos

### 1. Verificar que Docker funciona
```bash
docker --version
docker compose version
```

### 2. Iniciar los servicios
```bash
# Desde el directorio del proyecto
cd /home/kmilo07/prueba-tecnica

# Iniciar servicios en segundo plano
docker compose up -d

# Ver el progreso de inicio
docker compose logs -f
```

### 3. Verificar que los servicios están corriendo
```bash
# Ver estado de los contenedores
docker compose ps

# Ver logs específicos
docker compose logs oracle
docker compose logs mongodb
```

### 4. Conectar a Oracle (después de que inicie completamente)
```bash
# Esperar 2-3 minutos para que Oracle termine de inicializar
docker exec -it oracle sqlplus system/oracle123@//localhost:1521/XEPDB1

# Ejecutar el script de inicialización
docker exec -i oracle sqlplus system/oracle123@//localhost:1521/XEPDB1 < scripts/oracle/01-create-tables.sql
```

### 5. Verificar las tablas creadas
```sql
-- En sqlplus:
ALTER SESSION SET CONTAINER = XEPDB1;
SELECT table_name FROM user_tables;
SELECT COUNT(*) FROM CLIENTES;
SELECT COUNT(*) FROM FACTURAS;
```

### 6. Conectar a MongoDB
```bash
docker exec -it mongodb mongosh -u admin -p admin123
```

## Si algo sale mal

### Detener y limpiar
```bash
# Detener servicios
docker compose down

# Detener y eliminar volúmenes (elimina datos)
docker compose down -v

# Reiniciar desde cero
docker compose up -d
```

### Ver logs detallados
```bash
# Logs de Oracle
docker compose logs -f oracle

# Logs de MongoDB  
docker compose logs -f mongodb
```

## Puertos de Conexión

- **Oracle**: localhost:1521 (usuario: system, password: oracle123)
- **MongoDB**: localhost:27017 (usuario: admin, password: admin123)

## Próximos Pasos

Una vez que los servicios estén corriendo, puedes:
1. Conectar tu aplicación .NET a Oracle usando el puerto 1521
2. Usar MongoDB para logs de auditoría en el puerto 27017
3. Las tablas CLIENTES y FACTURAS estarán listas para usar
