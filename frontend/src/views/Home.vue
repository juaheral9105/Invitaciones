<template>
  <div class="min-h-screen flex items-center justify-center bg-gradient-to-br from-pink-100 via-purple-100 to-blue-100">
    <div class="text-center space-y-8 p-8">
      <h1 class="text-6xl font-bold text-purple-900 mb-4">
        Invitaciones de XV Años
      </h1>
      <p class="text-xl text-gray-700 max-w-2xl mx-auto">
        Crea invitaciones personalizadas y hermosas para tu celebración de XV años
      </p>

      <div class="flex gap-4 justify-center mt-8">
        <router-link
          to="/editor"
          class="px-8 py-4 bg-purple-600 text-white rounded-lg font-semibold text-lg hover:bg-purple-700 transition-colors shadow-lg"
        >
          Crear Nueva Invitación
        </router-link>
      </div>

      <div class="mt-12 grid grid-cols-1 md:grid-cols-3 gap-6 max-w-4xl mx-auto">
        <div class="bg-white p-6 rounded-lg shadow-md">
          <div class="text-4xl mb-3">🎨</div>
          <h3 class="font-semibold text-lg mb-2">Personalizable</h3>
          <p class="text-gray-600">Cambia colores, fuentes, fondos e imágenes</p>
        </div>

        <div class="bg-white p-6 rounded-lg shadow-md">
          <div class="text-4xl mb-3">📍</div>
          <h3 class="font-semibold text-lg mb-2">Ubicación</h3>
          <p class="text-gray-600">Integra Google Maps para guiar a tus invitados</p>
        </div>

        <div class="bg-white p-6 rounded-lg shadow-md">
          <div class="text-4xl mb-3">🎵</div>
          <h3 class="font-semibold text-lg mb-2">Música</h3>
          <p class="text-gray-600">Agrega tu canción favorita de fondo</p>
        </div>
      </div>
    </div>
  </div>
</template>

<script setup>
</script>
