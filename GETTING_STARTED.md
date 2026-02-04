# Guía de Inicio Rápido

## ✅ Proyecto Creado Exitosamente

Se ha creado un sistema completo de invitaciones de XV años con las siguientes características:

### Características Implementadas

1. **Editor Visual de Invitaciones**
   - Personalización de títulos y textos
   - Selector de colores (fondo y texto)
   - Selector de fuentes (5 fuentes elegantes)
   - Carga de imagen de fondo
   - Carga de música MP3
   - Vista previa en tiempo real

2. **Vista Pública de Invitaciones**
   - Diseño responsivo y elegante
   - Reproductor de música con controles
   - Mapa de Google Maps interactivo
   - Animaciones suaves

3. **Sistema de Confirmación**
   - Formulario completo de confirmación
   - Envío de notificaciones por email
   - Validación de datos

4. **Backend API REST**
   - CRUD completo de invitaciones
   - Gestión de confirmaciones
   - Upload de archivos (imágenes y música)
   - Base de datos PostgreSQL

## 📋 Próximos Pasos

### 1. Instalar PostgreSQL

Si no tienes PostgreSQL instalado:

**Windows:**
- Descarga desde https://www.postgresql.org/download/windows/
- Instala con las opciones por defecto
- Recuerda la contraseña del usuario `postgres`

**Mac:**
```bash
brew install postgresql
brew services start postgresql
```

**Linux:**
```bash
sudo apt-get install postgresql postgresql-contrib
sudo service postgresql start
```

### 2. Crear la Base de Datos

Abre la consola de PostgreSQL (psql) y ejecuta:

```sql
CREATE DATABASE invitaciones_db;
```

O usa pgAdmin para crear la base de datos gráficamente.

### 3. Configurar el Backend

1. Ve a la carpeta `backend` y edita `appsettings.json`:

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Host=localhost;Port=5432;Database=invitaciones_db;Username=postgres;Password=TU_PASSWORD_AQUI"
  }
}
```

2. Ejecutar las migraciones:

```bash
cd backend
dotnet ef migrations add InitialCreate
dotnet ef database update
```

Si `dotnet ef` no está instalado:
```bash
dotnet tool install --global dotnet-ef
```

3. Ejecutar el backend:

```bash
dotnet run
```

El backend estará en: http://localhost:5000

### 4. Configurar el Frontend

1. Edita el archivo `frontend/.env`:

```env
VITE_API_URL=http://localhost:5000/api
VITE_GOOGLE_MAPS_API_KEY=tu_api_key_aqui
```

2. Obtener una API Key de Google Maps (Opcional pero recomendado):
   - Ve a https://console.cloud.google.com/
   - Crea un proyecto o selecciona uno existente
   - Habilita "Maps JavaScript API"
   - En "Credenciales", crea una API Key
   - Copia la key y pégala en el archivo `.env`

3. Ejecutar el frontend:

```bash
cd frontend
npm run dev
```

El frontend estará en: http://localhost:5173

### 5. Configurar Email (Opcional)

Para que funcione el envío de emails al confirmar asistencia:

1. Edita `backend/appsettings.json`:

```json
{
  "Email": {
    "SmtpHost": "smtp.gmail.com",
    "SmtpPort": "587",
    "SmtpUser": "tu_email@gmail.com",
    "SmtpPassword": "tu_app_password_aqui",
    "FromEmail": "tu_email@gmail.com",
    "FromName": "Invitaciones XV Años"
  }
}
```

2. Generar App Password en Gmail:
   - Ve a https://myaccount.google.com/security
   - Activa la verificación en 2 pasos
   - Busca "Contraseñas de aplicaciones"
   - Genera una nueva contraseña
   - Usa esa contraseña (no tu contraseña de Gmail)

## 🎯 Probando la Aplicación

### Crear tu primera invitación:

1. Abre http://localhost:5173
2. Haz clic en "Crear Nueva Invitación"
3. Completa los datos:
   - Título: "Mis XV Años"
   - Nombre: Tu nombre
   - Fecha y hora del evento
   - Lugar y dirección
   - Email para recibir confirmaciones
4. Cambia los colores y la fuente
5. Sube una imagen de fondo (opcional)
6. Sube música MP3 (opcional)
7. Agrega coordenadas de Google Maps (opcional)
8. Haz clic en "Guardar"

### Ver la invitación pública:

1. Copia el ID de la invitación de la URL
2. Comparte esta URL con tus invitados:
   ```
   http://localhost:5173/invitation/[ID]
   ```
3. Tus invitados podrán:
   - Ver la invitación hermosa
   - Escuchar la música
   - Ver la ubicación en el mapa
   - Confirmar su asistencia

## 🚀 Despliegue a Producción

Cuando estés listo para publicar tu aplicación:

### Frontend (Vercel - Gratis)

```bash
cd frontend
npm install -g vercel
vercel
```

### Backend (Railway - Gratis)

1. Crea cuenta en https://railway.app
2. Conecta tu repositorio GitHub
3. Railway detectará automáticamente .NET
4. Configura las variables de entorno
5. Despliega

### Base de Datos (Supabase - Gratis)

1. Crea cuenta en https://supabase.com
2. Crea un nuevo proyecto
3. Obtén la connection string de PostgreSQL
4. Actualiza `appsettings.json` con la nueva URL

## 📝 Estructura de Archivos Creados

```
Proyecto Invitacion/
├── README.md                          # Documentación completa
├── GETTING_STARTED.md                 # Esta guía
├── .gitignore                         # Archivos a ignorar en Git
├── frontend/                          # Aplicación Vue.js
│   ├── src/
│   │   ├── views/
│   │   │   ├── Home.vue              # Página de inicio
│   │   │   ├── Editor/
│   │   │   │   └── EditorView.vue    # Editor de invitaciones
│   │   │   └── Viewer/
│   │   │       └── InvitationView.vue # Vista pública
│   │   ├── components/
│   │   │   ├── Editor/
│   │   │   │   ├── EditorPanel.vue   # Panel de configuración
│   │   │   │   └── InvitationPreview.vue # Vista previa
│   │   │   ├── Viewer/
│   │   │   │   ├── InvitationDisplay.vue # Visualización
│   │   │   │   └── ConfirmationForm.vue # Formulario
│   │   │   └── Common/
│   │   │       ├── MusicPlayer.vue   # Reproductor de música
│   │   │       └── GoogleMap.vue     # Mapa de Google
│   │   ├── stores/
│   │   │   └── invitationStore.js    # Estado global
│   │   ├── services/
│   │   │   └── api.js                # Cliente API
│   │   ├── router/
│   │   │   └── index.js              # Rutas
│   │   └── main.js
│   └── .env                          # Variables de entorno
│
└── backend/                           # API .NET 7
    ├── Controllers/
    │   ├── InvitationsController.cs  # CRUD invitaciones
    │   └── ConfirmationsController.cs # Confirmaciones
    ├── Models/
    │   ├── Invitation.cs             # Modelo de invitación
    │   └── Confirmation.cs           # Modelo de confirmación
    ├── Data/
    │   └── ApplicationDbContext.cs   # Contexto de base de datos
    ├── Services/
    │   ├── IEmailService.cs          # Interfaz de email
    │   └── EmailService.cs           # Servicio de email
    ├── DTOs/
    │   ├── InvitationDto.cs          # DTO de invitación
    │   └── ConfirmationDto.cs        # DTO de confirmación
    ├── appsettings.json              # Configuración
    └── Program.cs                     # Punto de entrada
```

## ❓ Solución de Problemas Comunes

### El backend no inicia

**Error:** "Cannot connect to PostgreSQL"
- Verifica que PostgreSQL esté corriendo
- Verifica usuario y contraseña en `appsettings.json`
- Verifica que la base de datos exista

**Error:** "dotnet ef command not found"
```bash
dotnet tool install --global dotnet-ef
```

### El frontend no muestra datos

- Verifica que el backend esté corriendo en http://localhost:5000
- Verifica que `VITE_API_URL` en `.env` sea correcta
- Abre la consola del navegador (F12) para ver errores

### Google Maps no se muestra

- Verifica que `VITE_GOOGLE_MAPS_API_KEY` esté configurada
- Verifica que la API esté habilitada en Google Cloud Console
- Asegúrate de que la API Key no tenga restricciones

### Los emails no se envían

- Verifica la configuración SMTP en `appsettings.json`
- Usa una App Password de Gmail, no tu contraseña normal
- Verifica que la verificación en 2 pasos esté activada

## 🎉 ¡Listo!

Tu sistema de invitaciones está completo y listo para usar. Disfruta creando hermosas invitaciones para eventos de XV años.

Si tienes preguntas o encuentras algún problema, revisa el archivo `README.md` para más detalles.
