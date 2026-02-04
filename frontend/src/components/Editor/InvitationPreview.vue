<template>
  <div class="bg-white rounded-lg shadow-lg overflow-hidden">
    <div
      class="min-h-[600px] p-8"
      :style="{
        backgroundColor: invitation.backgroundColor,
        backgroundImage: invitation.backgroundImage ? `url(${getImageUrl(invitation.backgroundImage)})` : 'none',
        backgroundSize: 'cover',
        backgroundPosition: 'center',
        color: invitation.textColor,
        fontFamily: invitation.fontFamily
      }"
    >
      <!-- Header -->
      <div class="text-center mb-8">
        <h1 class="text-5xl font-bold mb-2" :style="{ color: invitation.textColor }">
          {{ invitation.title || 'Título de la Invitación' }}
        </h1>
        <p v-if="invitation.subtitle" class="text-xl opacity-90">
          {{ invitation.subtitle }}
        </p>
      </div>

      <!-- Celebrant Name -->
      <div v-if="invitation.celebrantName" class="text-center mb-8">
        <p class="text-3xl font-semibold">
          {{ invitation.celebrantName }}
        </p>
      </div>

      <!-- Event Details -->
      <div class="max-w-2xl mx-auto bg-white bg-opacity-90 rounded-lg p-6 mb-8 text-gray-800">
        <h3 class="text-2xl font-semibold mb-4 text-center text-purple-900">Detalles del Evento</h3>

        <div v-if="invitation.eventDate" class="mb-3">
          <span class="font-semibold">📅 Fecha:</span>
          {{ formatDate(invitation.eventDate) }}
        </div>

        <div v-if="invitation.eventTime" class="mb-3">
          <span class="font-semibold">🕐 Hora:</span>
          {{ invitation.eventTime }}
        </div>

        <div v-if="invitation.venue" class="mb-3">
          <span class="font-semibold">📍 Lugar:</span>
          {{ invitation.venue }}
        </div>

        <div v-if="invitation.address" class="mb-3">
          <span class="font-semibold">🏠 Dirección:</span>
          {{ invitation.address }}
        </div>
      </div>

      <!-- Map Preview -->
      <div v-if="invitation.locationLat && invitation.locationLng" class="max-w-2xl mx-auto mb-8">
        <div class="bg-white bg-opacity-90 rounded-lg p-4">
          <h3 class="text-xl font-semibold mb-3 text-center text-purple-900">Ubicación</h3>
          <div class="w-full h-64 bg-gray-200 rounded-lg flex items-center justify-center">
            <p class="text-gray-600">Mapa (se cargará en vista pública)</p>
          </div>
        </div>
      </div>

      <!-- Music Player Preview -->
      <div v-if="invitation.musicUrl" class="max-w-2xl mx-auto mb-8">
        <div class="bg-white bg-opacity-90 rounded-lg p-4">
          <h3 class="text-xl font-semibold mb-3 text-center text-purple-900">🎵 Música</h3>
          <audio controls class="w-full" :src="getImageUrl(invitation.musicUrl)">
            Tu navegador no soporta el reproductor de audio.
          </audio>
        </div>
      </div>

      <!-- Confirmation Form Preview -->
      <div class="max-w-2xl mx-auto">
        <div class="bg-white bg-opacity-95 rounded-lg p-6">
          <h3 class="text-2xl font-semibold mb-4 text-center text-purple-900">Confirmar Asistencia</h3>
          <div class="space-y-3">
            <input
              type="text"
              placeholder="Nombre completo"
              disabled
              class="w-full px-4 py-2 border border-gray-300 rounded-lg bg-gray-50"
            />
            <input
              type="email"
              placeholder="Email (opcional)"
              disabled
              class="w-full px-4 py-2 border border-gray-300 rounded-lg bg-gray-50"
            />
            <select disabled class="w-full px-4 py-2 border border-gray-300 rounded-lg bg-gray-50">
              <option>¿Asistirás?</option>
            </select>
            <button
              disabled
              class="w-full py-3 bg-purple-600 text-white rounded-lg font-semibold opacity-50 cursor-not-allowed"
            >
              Enviar Confirmación
            </button>
          </div>
          <p class="text-center text-sm text-gray-600 mt-3">Vista previa del formulario</p>
        </div>
      </div>
    </div>
  </div>
</template>

<script setup>
import { computed } from 'vue'
import { useInvitationStore } from '../../stores/invitationStore'

const store = useInvitationStore()
const invitation = computed(() => store.invitation)

const API_BASE_URL = import.meta.env.VITE_API_URL?.replace('/api', '') || 'http://localhost:5000'

const getImageUrl = (url) => {
  if (!url) return ''
  if (url.startsWith('http')) return url
  return `${API_BASE_URL}${url}`
}

const formatDate = (dateString) => {
  if (!dateString) return ''
  const date = new Date(dateString)
  return date.toLocaleDateString('es-ES', {
    weekday: 'long',
    year: 'numeric',
    month: 'long',
    day: 'numeric'
  })
}
</script>
