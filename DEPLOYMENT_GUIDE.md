# Guía de Despliegue - Sistema de Invitaciones

Esta guía te llevará paso a paso para publicar tu aplicación en internet de forma **GRATUITA**.

## Arquitectura del Despliegue

- **Frontend (Vue.js)**: Vercel
- **Backend (.NET + PostgreSQL)**: Railway
- **Archivos**: Cloudinary (para imágenes y música)

---

## PARTE 1: Preparar el Código

### 1.1. Inicializar Git (si no lo has hecho)

```bash
cd "e:\CURSO IA Claude\Proyecto Invitacion"
git init
git add .
git commit -m "Initial commit - ready for deployment"
```

### 1.2. Crear cuenta en GitHub

1. Ve a https://github.com
2. Crea una cuenta gratuita
3. Crea un nuevo repositorio llamado `invitaciones-app`
4. NO inicialices con README

### 1.3. Subir código a GitHub

```bash
git remote add origin https://github.com/TU-USUARIO/invitaciones-app.git
git branch -M main
git push -u origin main
```

---

## PARTE 2: Desplegar Backend en Railway

### 2.1. Crear cuenta en Railway

1. Ve a https://railway.app
2. Regístrate con GitHub (Sign up with GitHub)
3. Autoriza Railway para acceder a tus repositorios

### 2.2. Crear nuevo proyecto

1. Click en "New Project"
2. Selecciona "Deploy from GitHub repo"
3. Busca y selecciona tu repositorio `invitaciones-app`
4. Railway detectará automáticamente que es .NET

### 2.3. Configurar el servicio Backend

1. Click en el servicio que se creó
2. Ve a "Settings" → "General"
3. En "Root Directory" escribe: `backend`
4. En "Start Command" escribe: `dotnet run`

### 2.4. Agregar PostgreSQL

1. Click en "+ New" → "Database" → "Add PostgreSQL"
2. Railway creará automáticamente la base de datos
3. Conecta la base de datos al servicio backend

### 2.5. Configurar Variables de Entorno

1. Click en tu servicio backend
2. Ve a "Variables"
3. Agrega estas variables:

```
ASPNETCORE_ENVIRONMENT=Production
ASPNETCORE_URLS=http://+:8080
ConnectionStrings__DefaultConnection=<COPIAR_DE_POSTGRESQL>
Email__SmtpHost=smtp.gmail.com
Email__SmtpPort=587
Email__SmtpUser=nuryvanessamesa@gmail.com
Email__SmtpPassword=yhvl ieqh hkbl huky
Email__FromEmail=nuryvanessamesa@gmail.com
Email__FromName=Invitaciones XV Años
```

**IMPORTANTE**: Para `ConnectionStrings__DefaultConnection`:
1. Click en el servicio PostgreSQL
2. Ve a "Variables" tab
3. Copia el valor de `DATABASE_URL`
4. Pégalo en la variable `ConnectionStrings__DefaultConnection` del backend

### 2.6. Obtener URL del Backend

1. Ve a tu servicio backend
2. Click en "Settings" → "Networking"
3. Click en "Generate Domain"
4. Copia la URL (será algo como: `https://tu-app.up.railway.app`)
5. **GUARDA ESTA URL**, la necesitarás para el frontend

---

## PARTE 3: Desplegar Frontend en Vercel

### 3.1. Crear cuenta en Vercel

1. Ve a https://vercel.com
2. Regístrate con GitHub
3. Autoriza Vercel

### 3.2. Importar proyecto

1. Click en "Add New" → "Project"
2. Busca y selecciona tu repositorio `invitaciones-app`
3. Click en "Import"

### 3.3. Configurar el proyecto

1. En "Framework Preset" selecciona: **Vite**
2. En "Root Directory" click en "Edit" y escribe: `frontend`
3. En "Build Command": `npm run build`
4. En "Output Directory": `dist`

### 3.4. Configurar Variables de Entorno

Click en "Environment Variables" y agrega:

```
VITE_API_URL=https://tu-backend.up.railway.app/api
```

**IMPORTANTE**: Reemplaza `https://tu-backend.up.railway.app` con la URL que copiaste de Railway en el paso 2.6

### 3.5. Desplegar

1. Click en "Deploy"
2. Espera 2-3 minutos
3. Una vez termine, Vercel te dará una URL como: `https://tu-app.vercel.app`

---

## PARTE 4: Configurar CORS en el Backend

El backend necesita permitir peticiones desde tu frontend de Vercel.

### 4.1. Actualizar Program.cs

Agrega la URL de Vercel a la configuración de CORS en `backend/Program.cs`:

```csharp
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll",
        builder =>
        {
            builder
                .WithOrigins(
                    "http://localhost:5173",
                    "http://localhost:5174",
                    "https://tu-app.vercel.app" // ← Agregar esta línea
                )
                .AllowAnyMethod()
                .AllowAnyHeader();
        });
});
```

### 4.2. Commitear y Push

```bash
git add .
git commit -m "Add Vercel URL to CORS"
git push
```

Railway y Vercel se redesplegarán automáticamente.

---

## PARTE 5: Probar la Aplicación

1. Visita tu URL de Vercel: `https://tu-app.vercel.app`
2. Crea una invitación de prueba
3. Sube el Excel con invitados
4. Prueba el formulario de confirmación

---

## URLs Finales

Después del despliegue tendrás:

- **Frontend**: https://tu-app.vercel.app
- **Backend**: https://tu-backend.up.railway.app
- **Base de Datos**: Alojada en Railway (privada)

---

## Solución de Problemas

### Si el backend no inicia:

1. Ve a Railway → tu servicio backend → "Deployments"
2. Click en el deployment más reciente
3. Ve a "View Logs"
4. Busca errores en rojo

### Si el frontend no conecta:

1. Abre la consola del navegador (F12)
2. Ve a la pestaña "Console"
3. Busca errores de CORS o conexión
4. Verifica que `VITE_API_URL` esté correcta

### Si no llegan los emails:

1. Verifica que las variables de Email estén correctas en Railway
2. Asegúrate de que el SMTP password sea el de aplicación de Gmail

---

## Costos

- **Vercel**: 100% GRATIS (hosting ilimitado para hobby)
- **Railway**:
  - Plan gratuito: $5 USD de crédito mensual
  - Tu app debería consumir ~$3-4 mensuales
  - Después del crédito gratuito: ~$5 USD/mes

---

## Próximos Pasos (Opcional)

### Dominio Personalizado

Si decides comprar un dominio:

1. Compra en Namecheap/GoDaddy (~$12/año)
2. En Vercel → Settings → Domains → Add Domain
3. Sigue las instrucciones para configurar DNS
4. En Railway → Settings → Domains → Custom Domain

### Cloudinary para Archivos

Para no depender de URLs locales:

1. Crea cuenta en https://cloudinary.com (gratis)
2. Sube tus imágenes/música allí
3. Usa las URLs de Cloudinary en tu app

---

¡Tu aplicación está lista para el mundo! 🎉
