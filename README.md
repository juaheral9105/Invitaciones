# Sistema de Invitaciones de XV Años

Sistema completo para crear invitaciones personalizadas de XV años con editor visual, vista pública y confirmación de asistencia.

## Tecnologías Utilizadas

### Frontend
- **Vue.js 3** con Composition API
- **Vite** como bundler
- **TailwindCSS** para estilos
- **Vue Router** para navegación
- **Pinia** para manejo de estado
- **Axios** para peticiones HTTP

### Backend
- **.NET 7 Web API**
- **Entity Framework Core** para ORM
- **PostgreSQL** como base de datos
- **MailKit** para envío de emails

## Características

- Editor visual de invitaciones con:
  - Personalización de textos, colores y fuentes
  - Carga de imágenes de fondo
  - Secciones configurables (texto, imágenes, listas)
  - Integración con Google Maps
  - Reproductor de música MP3
  - Vista previa en tiempo real

- Vista pública de invitaciones (solo lectura)
- Formulario de confirmación de asistencia
- Notificaciones por email

## Requisitos Previos

- **Node.js** 16+ y npm
- **.NET 7 SDK**
- **PostgreSQL** 12+
- Cuenta de Gmail (opcional, para envío de emails)

## Configuración

### 1. Base de Datos PostgreSQL

Crear la base de datos:

```sql
CREATE DATABASE invitaciones_db;
```

### 2. Configuración del Backend

1. Navegar a la carpeta backend:
```bash
cd backend
```

2. Configurar la cadena de conexión en `appsettings.json`:
```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Host=localhost;Port=5432;Database=invitaciones_db;Username=tu_usuario;Password=tu_password"
  }
}
```

3. Configurar el servicio de email (opcional):
```json
{
  "Email": {
    "SmtpHost": "smtp.gmail.com",
    "SmtpPort": "587",
    "SmtpUser": "tu_email@gmail.com",
    "SmtpPassword": "tu_app_password",
    "FromEmail": "tu_email@gmail.com",
    "FromName": "Invitaciones XV Años"
  }
}
```

**Nota:** Para Gmail, necesitas generar una "App Password" en tu cuenta:
- Ve a https://myaccount.google.com/security
- Activa la verificación en 2 pasos
- Genera una contraseña de aplicación

4. Ejecutar migraciones:
```bash
dotnet ef migrations add InitialCreate
dotnet ef database update
```

5. Ejecutar la API:
```bash
dotnet run
```

La API estará disponible en `http://localhost:5000`

### 3. Configuración del Frontend

1. Navegar a la carpeta frontend:
```bash
cd frontend
```

2. Instalar dependencias:
```bash
npm install
```

3. Crear archivo `.env` basado en `.env.example`:
```env
VITE_API_URL=http://localhost:5000/api
VITE_GOOGLE_MAPS_API_KEY=tu_api_key_de_google_maps
```

**Obtener API Key de Google Maps:**
- Ve a https://console.cloud.google.com/
- Crea un proyecto nuevo o selecciona uno existente
- Habilita "Maps JavaScript API"
- Crea credenciales (API Key)

4. Ejecutar el frontend:
```bash
npm run dev
```

El frontend estará disponible en `http://localhost:5173`

## Estructura del Proyecto

```
Proyecto Invitacion/
├── frontend/                    # Aplicación Vue.js
│   ├── src/
│   │   ├── components/         # Componentes reutilizables
│   │   │   ├── Editor/        # Componentes del editor
│   │   │   ├── Viewer/        # Componentes de vista pública
│   │   │   └── Common/        # Componentes comunes
│   │   ├── views/             # Vistas principales
│   │   │   ├── Editor/        # Vista del editor
│   │   │   └── Viewer/        # Vista pública
│   │   ├── stores/            # Stores de Pinia
│   │   ├── services/          # Servicios API
│   │   ├── router/            # Configuración de rutas
│   │   └── main.js
│   └── package.json
│
└── backend/                     # API .NET 7
    ├── Controllers/            # Controladores API
    ├── Models/                 # Modelos de datos
    ├── Data/                   # DbContext
    ├── Services/               # Servicios (Email, etc.)
    ├── DTOs/                   # Data Transfer Objects
    └── Program.cs

```

## Uso de la Aplicación

### Crear una Invitación

1. Accede a `http://localhost:5173`
2. Haz clic en "Crear Nueva Invitación"
3. Personaliza tu invitación:
   - Agrega título y texto
   - Sube imágenes de fondo
   - Cambia colores y fuentes
   - Agrega ubicación con Google Maps
   - Sube música de fondo
   - Configura secciones personalizadas
4. Guarda la invitación
5. Comparte la URL pública con tus invitados

### Ver Invitación (Modo Público)

Los invitados accederán a una URL como:
```
http://localhost:5173/invitation/{id}
```

En esta vista podrán:
- Ver la invitación completa
- Escuchar la música
- Ver la ubicación en el mapa
- Confirmar su asistencia mediante el formulario

### Confirmar Asistencia

1. En la vista pública, ir al formulario al final
2. Completar los datos:
   - Nombre
   - Email (opcional)
   - Teléfono (opcional)
   - Confirmación de asistencia
   - Número de invitados
   - Mensaje (opcional)
3. Enviar formulario
4. El organizador recibirá un email de notificación

## API Endpoints

### Invitaciones

- `GET /api/invitations` - Listar todas las invitaciones
- `GET /api/invitations/{id}` - Obtener una invitación
- `POST /api/invitations` - Crear invitación
- `PUT /api/invitations/{id}` - Actualizar invitación
- `DELETE /api/invitations/{id}` - Eliminar invitación
- `POST /api/invitations/upload-image` - Subir imagen
- `POST /api/invitations/upload-music` - Subir música

### Confirmaciones

- `POST /api/confirmations/{invitationId}` - Crear confirmación
- `GET /api/confirmations/invitation/{invitationId}` - Listar confirmaciones

## Despliegue (Hosting Gratuito)

### Frontend (Vercel/Netlify)

1. **Vercel:**
```bash
npm install -g vercel
cd frontend
vercel
```

2. **Netlify:**
```bash
npm install -g netlify-cli
cd frontend
npm run build
netlify deploy --prod
```

### Backend (Railway/Render)

1. **Railway:**
   - Conecta tu repositorio Git
   - Railway detectará automáticamente .NET
   - Configura las variables de entorno

2. **Render:**
   - Crea un nuevo Web Service
   - Conecta tu repositorio
   - Build command: `dotnet publish -c Release -o out`
   - Start command: `dotnet out/InvitacionesAPI.dll`

### Base de Datos (Supabase)

1. Crea cuenta en https://supabase.com
2. Crea un nuevo proyecto
3. Obtén la connection string de PostgreSQL
4. Actualiza `appsettings.json` con la nueva connection string

## Solución de Problemas

### Error de CORS
Asegúrate de que la URL del frontend esté en la política CORS del backend ([Program.cs:25-30](backend/Program.cs#L25-L30))

### Error de conexión a PostgreSQL
Verifica que PostgreSQL esté corriendo y las credenciales sean correctas

### Emails no se envían
Verifica la configuración de email en `appsettings.json` y que uses una App Password de Gmail

### Imágenes no se cargan
Asegúrate de que la carpeta `wwwroot/uploads` tenga permisos de escritura

## Próximas Mejoras

- [ ] Drag and drop para reordenar secciones
- [ ] Más plantillas prediseñadas
- [ ] Galería de imágenes
- [ ] Contador regresivo para el evento
- [ ] Integración con WhatsApp para compartir
- [ ] Panel de administración para ver todas las confirmaciones
- [ ] Exportar lista de invitados a Excel
- [ ] Temas predefinidos (elegante, moderno, clásico)

## Licencia

Este proyecto es de código abierto y está disponible bajo la licencia MIT.
