# Plantilla de Lista de Invitados

## Formato del archivo Excel

El archivo Excel debe tener las siguientes columnas en este orden:

| Columna | Nombre del Campo | Tipo | Descripción | Ejemplo |
|---------|-----------------|------|-------------|---------|
| A | Nombre | Texto | Nombre completo del invitado titular | "Juan Pérez" |
| B | Celular | Texto | Número de teléfono (identificador único) | "3001234567" |
| C | Número de invitados | Número | Cantidad de personas que puede traer | 3 |
| D | Nombre de invitados | Texto | Nombres de acompañantes (separados por coma) | "María Pérez, Carlos Pérez" |
| E | Evento tiene hospedaje | Booleano | Si el evento incluye hospedaje | "Si" o "No" |
| F | Hospedaje permitido | Booleano | Si este invitado tiene derecho a hospedaje | "Si" o "No" |

## Ejemplo de datos

```
Nombre              | Celular    | Número de invitados | Nombre de invitados        | Evento tiene hospedaje | Hospedaje permitido
--------------------|------------|---------------------|----------------------------|----------------------|--------------------
Juan Pérez          | 3001234567 | 3                   | María Pérez, Carlos Pérez  | Si                   | Si
Ana García          | 3009876543 | 2                   | Pedro García               | Si                   | No
Roberto Silva       | 3005556789 | 1                   |                            | Si                   | Si
```

## Notas importantes:

1. **La primera fila debe contener los encabezados** (se omite al procesar)
2. **El campo "Celular" es obligatorio** y debe ser único por invitación
3. **Valores booleanos aceptados**: "Si", "Sí", "Yes", "True", "1" (para verdadero), cualquier otro valor o vacío = falso
4. **El archivo debe ser formato .xlsx o .xls**
5. Los campos vacíos se procesarán con valores por defecto:
   - Número de invitados: 0
   - Nombre de invitados: vacío
   - Evento tiene hospedaje: falso
   - Hospedaje permitido: falso

## Endpoints disponibles

### Cargar archivo Excel
```
POST /api/GuestList/upload/{invitationId}
Content-Type: multipart/form-data
Body: file (archivo Excel)
```

### Obtener invitados por invitación
```
GET /api/GuestList/{invitationId}?page=1&pageSize=50
```

### Buscar invitado por teléfono
```
GET /api/GuestList/{invitationId}/phone/{phone}
```

### Actualizar invitado
```
PUT /api/GuestList/{id}
Body: GuestListItem JSON
```

### Eliminar invitado
```
DELETE /api/GuestList/{id}
```

### Agregar invitado individual
```
POST /api/GuestList/{invitationId}/guest
Body: GuestListItem JSON
```

### Limpiar lista completa
```
DELETE /api/GuestList/{invitationId}/clear
```
