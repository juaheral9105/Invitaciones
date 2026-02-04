<template>
  <div
    class="min-h-screen"
    :style="{
      backgroundColor: invitation.backgroundColor,
      backgroundImage: invitation.backgroundImage ? `url(${getImageUrl(invitation.backgroundImage)})` : 'none',
      backgroundSize: 'cover',
      backgroundPosition: 'center',
      backgroundAttachment: 'fixed',
      color: invitation.textColor,
      fontFamily: invitation.fontFamily
    }"
  >
    <div class="container mx-auto px-4 py-12">
      <!-- Music Player (Fixed) -->
      <MusicPlayer v-if="invitation.musicUrl" :musicUrl="getImageUrl(invitation.musicUrl)" />

      <!-- Header -->
      <div class="text-center mb-12 animate-fade-in">
        <h1 class="text-6xl md:text-7xl font-bold mb-4" :style="{ color: invitation.textColor }">
          {{ invitation.title }}
        </h1>
        <p v-if="invitation.subtitle" class="text-2xl md:text-3xl opacity-90">
          {{ invitation.subtitle }}
        </p>
      </div>

      <!-- Celebrant Name -->
      <div v-if="invitation.celebrantName" class="text-center mb-12 animate-fade-in">
        <div class="inline-block bg-white bg-opacity-90 rounded-lg px-8 py-4">
          <p class="text-4xl md:text-5xl font-semibold text-purple-900">
            {{ invitation.celebrantName }}
          </p>
        </div>
      </div>

      <!-- Event Details -->
      <div class="max-w-3xl mx-auto mb-12 animate-fade-in">
        <div class="bg-white bg-opacity-95 rounded-xl shadow-2xl p-8">
          <h2 class="text-3xl font-bold mb-6 text-center text-purple-900">Detalles del Evento</h2>

          <div class="space-y-4 text-lg text-gray-800">
            <div v-if="invitation.eventDate" class="flex items-center">
              <span class="text-3xl mr-4">📅</span>
              <div>
                <span class="font-semibold">Fecha:</span>
                <span class="ml-2">{{ formatDate(invitation.eventDate) }}</span>
              </div>
            </div>

            <div v-if="invitation.eventTime" class="flex items-center">
              <span class="text-3xl mr-4">🕐</span>
              <div>
                <span class="font-semibold">Hora:</span>
                <span class="ml-2">{{ invitation.eventTime }}</span>
              </div>
            </div>

            <div v-if="invitation.venue" class="flex items-center">
              <span class="text-3xl mr-4">📍</span>
              <div>
                <span class="font-semibold">Lugar:</span>
                <span class="ml-2">{{ invitation.venue }}</span>
              </div>
            </div>

            <div v-if="invitation.address" class="flex items-center">
              <span class="text-3xl mr-4">🏠</span>
              <div>
                <span class="font-semibold">Dirección:</span>
                <span class="ml-2">{{ invitation.address }}</span>
              </div>
            </div>
          </div>
        </div>
      </div>

      <!-- Google Maps -->
      <GoogleMap
        v-if="invitation.locationLat && invitation.locationLng"
        :lat="parseFloat(invitation.locationLat)"
        :lng="parseFloat(invitation.locationLng)"
        :title="invitation.venue || 'Ubicación del evento'"
      />

      <!-- Confirmation Form -->
      <ConfirmationForm
        :formEmail="invitation.formEmail"
        @submit="$emit('submit-confirmation', $event)"
      />
    </div>
  </div>
</template>

<script setup>
import { defineProps, defineEmits } from 'vue'
import MusicPlayer from '../Common/MusicPlayer.vue'
import GoogleMap from '../Common/GoogleMap.vue'
import ConfirmationForm from '../Viewer/ConfirmationForm.vue'

defineProps({
  invitation: {
    type: Object,
    required: true
  }
})

defineEmits(['submit-confirmation'])

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

<style scoped>
@keyframes fadeIn {
  from {
    opacity: 0;
    transform: translateY(20px);
  }
  to {
    opacity: 1;
    transform: translateY(0);
  }
}

.animate-fade-in {
  animation: fadeIn 1s ease-out;
}
</style>
