<template>
  <div v-if="loading" class="min-h-screen flex items-center justify-center bg-gray-100">
    <div class="text-center">
      <div class="animate-spin rounded-full h-16 w-16 border-b-4 border-purple-600 mx-auto mb-4"></div>
      <p class="text-gray-600">Cargando invitación...</p>
    </div>
  </div>

  <div v-else-if="error" class="min-h-screen flex items-center justify-center bg-gray-100">
    <div class="text-center">
      <div class="text-6xl mb-4">😔</div>
      <h2 class="text-2xl font-bold text-gray-800 mb-2">Invitación no encontrada</h2>
      <p class="text-gray-600">Lo sentimos, esta invitación no existe o ha sido eliminada.</p>
    </div>
  </div>

  <div v-else class="relative">
    <!-- Cover Page (Página inicial) -->
    <CoverPage
      v-if="invitation.coverEnabled && showCover"
      :visible="showCover"
      :cover-name="invitation.coverName"
      :button-text="invitation.coverButtonText"
      :background-color="invitation.coverBackgroundColor"
      :background-image="getFullUrl(invitation.coverBackgroundImage)"
      :text-color="invitation.coverTextColor"
      :button-color="invitation.coverButtonColor"
      :button-text-color="invitation.coverButtonTextColor"
      :font-family="invitation.coverFontFamily"
      :name-font-size="invitation.coverNameFontSize"
      :button-font-size="invitation.coverButtonFontSize"
      @enter="enterInvitation"
    />

    <!-- Invitación completa -->
    <div
      :class="{ 'opacity-0': showCover && invitation.coverEnabled }"
      class="transition-opacity duration-1000"
    >
      <InvitationDisplay :invitation="invitation" :should-play-music="shouldPlayMusic" @submit-confirmation="handleConfirmation" />
    </div>

    <!-- Success Modal -->
    <div v-if="showSuccessModal" class="fixed inset-0 bg-black bg-opacity-50 flex items-center justify-center z-50 p-4">
      <div class="bg-white rounded-lg p-8 max-w-md w-full text-center">
        <div class="text-6xl mb-4">✅</div>
        <h3 class="text-2xl font-bold text-gray-900 mb-2">¡Confirmación Enviada!</h3>
        <p class="text-gray-600 mb-6">Gracias por confirmar tu asistencia. Nos vemos en el evento.</p>
        <button
          @click="showSuccessModal = false"
          class="px-6 py-2 bg-purple-600 text-white rounded-lg hover:bg-purple-700 transition-colors"
        >
          Cerrar
        </button>
      </div>
    </div>
  </div>
</template>

<script setup>
import { ref, onMounted } from 'vue'
import { useRoute } from 'vue-router'
import { invitationService, confirmationService } from '../../services/api'
import InvitationDisplay from '../../components/Viewer/InvitationDisplay.vue'
import CoverPage from '../../components/Viewer/CoverPage.vue'

const route = useRoute()
const invitation = ref(null)
const loading = ref(true)
const error = ref(false)
const showSuccessModal = ref(false)
const showCover = ref(true)
const shouldPlayMusic = ref(false)

const API_BASE_URL = import.meta.env.VITE_API_URL?.replace('/api', '') || 'http://localhost:5247'

const getFullUrl = (url) => {
  if (!url) return ''
  if (url.startsWith('http')) return url
  return `${API_BASE_URL}${url}`
}

const enterInvitation = () => {
  showCover.value = false
  shouldPlayMusic.value = true
  // Scroll to top when entering invitation
  window.scrollTo(0, 0)
}

onMounted(async () => {
  // Always scroll to top when page loads
  window.scrollTo(0, 0)

  try {
    const response = await invitationService.getById(route.params.id)
    invitation.value = response.data

    // Start music automatically if music URL is configured
    if (invitation.value.musicUrl) {
      shouldPlayMusic.value = true
    }
  } catch (err) {
    console.error('Error loading invitation:', err)
    error.value = true
  } finally {
    loading.value = false
  }
})

const handleConfirmation = async (formData) => {
  try {
    await confirmationService.submit(route.params.id, formData)
    showSuccessModal.value = true
  } catch (err) {
    console.error('Error submitting confirmation:', err)
    alert('Error al enviar la confirmación. Por favor intenta nuevamente.')
  }
}
</script>
