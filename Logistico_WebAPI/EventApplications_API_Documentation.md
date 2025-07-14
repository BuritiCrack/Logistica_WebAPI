# Event Applications API Documentation

## Descripción
Este conjunto de endpoints permite gestionar las aplicaciones de usuarios a eventos, incluyendo la capacidad de aplicar, cancelar, y que los administradores gestionen el estado de las aplicaciones.

## Endpoints Disponibles

### 1. Aplicar a un Evento
**POST** `/api/EventApplications/apply`
- **Requiere autenticación**: Sí
- **Roles permitidos**: Todos los usuarios autenticados

**Request Body:**
```json
{
  "eventId": 1,
  "comments": "Estoy muy interesado en participar en este evento"
}
```

**Response (200):**
```json
{
  "success": true,
  "message": "Aplicación enviada exitosamente. Estado: Pendiente.",
  "application": {
    "id": 1,
    "eventId": 1,
    "userId": "user-guid-123",
    "status": "Pending",
    "userComments": "Estoy muy interesado en participar en este evento",
    "registrationDate": "2025-07-11T10:30:00Z",
    "eventName": "Evento de Logística",
    "userName": "Juan Pérez"
  }
}
```

### 2. Cancelar Aplicación
**PUT** `/api/EventApplications/cancel/{eventId}`
- **Requiere autenticación**: Sí
- **Roles permitidos**: Todos los usuarios autenticados (solo pueden cancelar sus propias aplicaciones)

**Response (200):**
```json
{
  "success": true,
  "message": "Aplicación cancelada exitosamente.",
  "application": {
    "id": 1,
    "eventId": 1,
    "userId": "user-guid-123",
    "status": "CancelledByUser",
    "lastUpdated": "2025-07-11T11:00:00Z",
    "eventName": "Evento de Logística"
  }
}
```

### 3. Ver Mis Aplicaciones
**GET** `/api/EventApplications/my-applications`
- **Requiere autenticación**: Sí
- **Roles permitidos**: Todos los usuarios autenticados

**Response (200):**
```json
[
  {
    "id": 1,
    "eventId": 1,
    "status": "Pending",
    "userComments": "Muy interesado",
    "adminComments": null,
    "registrationDate": "2025-07-11T10:30:00Z",
    "lastUpdated": "2025-07-11T10:30:00Z",
    "event": {
      "id": 1,
      "name": "Evento de Logística",
      "description": "Descripción del evento",
      "place": "Centro de Convenciones",
      "startDate": "2025-08-15T09:00:00Z",
      "endDate": "2025-08-15T17:00:00Z",
      "payment": 50000.00,
      "photo": "evento.jpg"
    }
  }
]
```

### 4. Verificar si Aplicé a un Evento
**GET** `/api/EventApplications/check-application/{eventId}`
- **Requiere autenticación**: Sí
- **Roles permitidos**: Todos los usuarios autenticados

**Response (200):**
```json
{
  "eventId": 1,
  "userId": "user-guid-123",
  "hasApplied": true
}
```

### 5. Ver Aplicaciones de un Evento (Solo Admin)
**GET** `/api/EventApplications/event/{eventId}/applications`
- **Requiere autenticación**: Sí
- **Roles permitidos**: Admin

**Response (200):**
```json
[
  {
    "id": 1,
    "eventId": 1,
    "userId": "user-guid-123",
    "status": "Pending",
    "userComments": "Muy interesado",
    "adminComments": null,
    "registrationDate": "2025-07-11T10:30:00Z",
    "lastUpdated": "2025-07-11T10:30:00Z",
    "user": {
      "id": "user-guid-123",
      "fullName": "Juan Pérez",
      "email": "juan@example.com",
      "phoneNumber": "123456789",
      "document": "12345678",
      "age": "30",
      "experience": "5 años en logística",
      "skills": "Manejo de inventarios, coordinación"
    },
    "event": {
      "id": 1,
      "name": "Evento de Logística",
      "startDate": "2025-08-15T09:00:00Z",
      "endDate": "2025-08-15T17:00:00Z"
    }
  }
]
```

### 6. Actualizar Estado de Aplicación (Solo Admin)
**PUT** `/api/EventApplications/update-status`
- **Requiere autenticación**: Sí
- **Roles permitidos**: Admin

**Request Body:**
```json
{
  "applicationId": 1,
  "newStatus": "Accepted",
  "adminComments": "Perfil ideal para el evento"
}
```

**Response (200):**
```json
{
  "success": true,
  "message": "Estado de aplicación actualizado a: Aceptado",
  "application": {
    "id": 1,
    "eventId": 1,
    "userId": "user-guid-123",
    "status": "Accepted",
    "userComments": "Muy interesado",
    "adminComments": "Perfil ideal para el evento",
    "lastUpdated": "2025-07-11T12:00:00Z",
    "userName": "Juan Pérez",
    "eventName": "Evento de Logística"
  }
}
```

### 7. Ver Detalles de una Aplicación
**GET** `/api/EventApplications/{applicationId}`
- **Requiere autenticación**: Sí
- **Roles permitidos**: Propietario de la aplicación o Admin

**Response (200):**
```json
{
  "id": 1,
  "eventId": 1,
  "userId": "user-guid-123",
  "status": "Accepted",
  "userComments": "Muy interesado",
  "adminComments": "Perfil ideal para el evento",
  "registrationDate": "2025-07-11T10:30:00Z",
  "lastUpdated": "2025-07-11T12:00:00Z",
  "user": {
    "id": "user-guid-123",
    "fullName": "Juan Pérez",
    "email": "juan@example.com",
    "document": "12345678"
  },
  "event": {
    "id": 1,
    "name": "Evento de Logística",
    "description": "Descripción del evento",
    "place": "Centro de Convenciones",
    "startDate": "2025-08-15T09:00:00Z",
    "endDate": "2025-08-15T17:00:00Z",
    "payment": 50000.00
  }
}
```

## Estados de Aplicación

- **Pending**: Estado inicial cuando un usuario aplica a un evento
- **Accepted**: El administrador ha aceptado la aplicación
- **Rejected**: El administrador ha rechazado la aplicación
- **CancelledByUser**: El usuario ha cancelado su propia aplicación

## Reglas de Negocio

1. **Un usuario solo puede aplicar una vez por evento**
2. **No se puede aplicar a eventos que ya han comenzado**
3. **Los usuarios solo pueden cancelar sus propias aplicaciones**
4. **Solo los administradores pueden cambiar estados a Accepted/Rejected**
5. **Los administradores no pueden marcar una aplicación como "CancelledByUser"**
6. **No se puede cambiar el estado de una aplicación ya cancelada por el usuario**

## Códigos de Error Comunes

- **400**: Datos inválidos o reglas de negocio violadas
- **401**: Usuario no autenticado
- **403**: Usuario no autorizado para la acción
- **404**: Aplicación o evento no encontrado

## Ejemplo de Flujo Completo

1. **Usuario aplica a evento**: POST `/api/EventApplications/apply`
2. **Admin revisa aplicaciones**: GET `/api/EventApplications/event/1/applications`
3. **Admin acepta aplicación**: PUT `/api/EventApplications/update-status`
4. **Usuario verifica estado**: GET `/api/EventApplications/my-applications`
