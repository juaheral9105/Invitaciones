<template>
  <div class="bg-white rounded-lg shadow-md p-6 space-y-6">
    <h2 class="text-xl font-semibold text-gray-900">Configuración</h2>

    <!-- Tabs -->
    <div class="flex border-b">
      <button
        v-for="tab in tabs"
        :key="tab.id"
        @click="activeTab = tab.id"
        :class="[
          'px-4 py-2 font-medium transition-colors',
          activeTab === tab.id
            ? 'border-b-2 border-purple-600 text-purple-600'
            : 'text-gray-600 hover:text-gray-900'
        ]"
      >
        {{ tab.label }}
      </button>
    </div>

    <!-- Tab Content -->
    <div class="space-y-4">
      <!-- General Tab -->
      <div v-if="activeTab === 'general'" class="space-y-4">
        <div>
          <label class="block text-sm font-medium text-gray-700 mb-1">Título</label>
          <input
            v-model="store.invitation.title"
            type="text"
            class="w-full px-3 py-2 border border-gray-300 rounded-lg focus:ring-2 focus:ring-purple-500 focus:border-transparent"
            placeholder="Ej: Mis XV Años"
          />
        </div>

        <div>
          <label class="block text-sm font-medium text-gray-700 mb-1">Subtítulo</label>
          <input
            v-model="store.invitation.subtitle"
            type="text"
            class="w-full px-3 py-2 border border-gray-300 rounded-lg focus:ring-2 focus:ring-purple-500 focus:border-transparent"
            placeholder="Ej: Una celebración especial"
          />
        </div>

        <div>
          <label class="block text-sm font-medium text-gray-700 mb-1">Nombre de la quinceañera</label>
          <input
            v-model="store.invitation.celebrantName"
            type="text"
            class="w-full px-3 py-2 border border-gray-300 rounded-lg focus:ring-2 focus:ring-purple-500 focus:border-transparent"
          />
        </div>

        <div class="grid grid-cols-2 gap-3">
          <div>
            <label class="block text-sm font-medium text-gray-700 mb-1">Fecha del evento</label>
            <input
              v-model="store.invitation.eventDate"
              type="date"
              class="w-full px-3 py-2 border border-gray-300 rounded-lg focus:ring-2 focus:ring-purple-500 focus:border-transparent"
            />
          </div>
          <div>
            <label class="block text-sm font-medium text-gray-700 mb-1">Hora</label>
            <input
              v-model="store.invitation.eventTime"
              type="time"
              class="w-full px-3 py-2 border border-gray-300 rounded-lg focus:ring-2 focus:ring-purple-500 focus:border-transparent"
            />
          </div>
        </div>

        <div>
          <label class="block text-sm font-medium text-gray-700 mb-1">Lugar</label>
          <input
            v-model="store.invitation.venue"
            type="text"
            class="w-full px-3 py-2 border border-gray-300 rounded-lg focus:ring-2 focus:ring-purple-500 focus:border-transparent"
            placeholder="Ej: Salón de Eventos Primavera"
          />
        </div>

        <div>
          <label class="block text-sm font-medium text-gray-700 mb-1">Dirección</label>
          <input
            v-model="store.invitation.address"
            type="text"
            class="w-full px-3 py-2 border border-gray-300 rounded-lg focus:ring-2 focus:ring-purple-500 focus:border-transparent"
            placeholder="Calle, número, ciudad"
          />
        </div>

        <div>
          <label class="block text-sm font-medium text-gray-700 mb-1">Email para confirmaciones</label>
          <input
            v-model="store.invitation.formEmail"
            type="email"
            class="w-full px-3 py-2 border border-gray-300 rounded-lg focus:ring-2 focus:ring-purple-500 focus:border-transparent"
            placeholder="tu@email.com"
          />
        </div>
      </div>

      <!-- Design Tab -->
      <div v-if="activeTab === 'design'" class="space-y-4">
        <div>
          <label class="block text-sm font-medium text-gray-700 mb-1">Color de fondo</label>
          <input
            v-model="store.invitation.backgroundColor"
            type="color"
            class="w-full h-10 rounded-lg cursor-pointer"
          />
        </div>

        <div>
          <label class="block text-sm font-medium text-gray-700 mb-1">Color de texto</label>
          <input
            v-model="store.invitation.textColor"
            type="color"
            class="w-full h-10 rounded-lg cursor-pointer"
          />
        </div>

        <div>
          <label class="block text-sm font-medium text-gray-700 mb-1">Fuente</label>
          <select
            v-model="store.invitation.fontFamily"
            class="w-full px-3 py-2 border border-gray-300 rounded-lg focus:ring-2 focus:ring-purple-500 focus:border-transparent"
          >
            <option value="Arial">Arial</option>
            <option value="Playfair Display">Playfair Display</option>
            <option value="Montserrat">Montserrat</option>
            <option value="Dancing Script">Dancing Script</option>
            <option value="Lora">Lora</option>
          </select>
        </div>

        <div>
          <label class="block text-sm font-medium text-gray-700 mb-2">Imagen de fondo</label>
          <input
            type="file"
            @change="uploadBackgroundImage"
            accept="image/*"
            class="w-full text-sm text-gray-500 file:mr-4 file:py-2 file:px-4 file:rounded-lg file:border-0 file:text-sm file:font-semibold file:bg-purple-50 file:text-purple-700 hover:file:bg-purple-100"
          />
          <p v-if="uploadingBg" class="text-xs text-gray-500 mt-1">Subiendo...</p>
        </div>
      </div>

      <!-- Media Tab -->
      <div v-if="activeTab === 'media'" class="space-y-4">
        <div>
          <label class="block text-sm font-medium text-gray-700 mb-2">Música de fondo (MP3)</label>
          <input
            type="file"
            @change="uploadMusic"
            accept="audio/*"
            class="w-full text-sm text-gray-500 file:mr-4 file:py-2 file:px-4 file:rounded-lg file:border-0 file:text-sm file:font-semibold file:bg-purple-50 file:text-purple-700 hover:file:bg-purple-100"
          />
          <p v-if="uploadingMusic" class="text-xs text-gray-500 mt-1">Subiendo...</p>
          <p v-if="store.invitation.musicUrl" class="text-xs text-green-600 mt-1">✓ Música agregada</p>
        </div>

        <div>
          <label class="block text-sm font-medium text-gray-700 mb-1">Coordenadas de ubicación</label>
          <div class="grid grid-cols-2 gap-2">
            <input
              v-model="store.invitation.locationLat"
              type="number"
              step="0.000001"
              placeholder="Latitud"
              class="w-full px-3 py-2 border border-gray-300 rounded-lg focus:ring-2 focus:ring-purple-500 focus:border-transparent text-sm"
            />
            <input
              v-model="store.invitation.locationLng"
              type="number"
              step="0.000001"
              placeholder="Longitud"
              class="w-full px-3 py-2 border border-gray-300 rounded-lg focus:ring-2 focus:ring-purple-500 focus:border-transparent text-sm"
            />
          </div>
          <p class="text-xs text-gray-500 mt-1">Puedes obtener las coordenadas en Google Maps</p>
        </div>
      </div>
    </div>
  </div>
</template>

<script setup>
import { ref } from 'vue'
import { useInvitationStore } from '../../stores/invitationStore'
import { invitationService } from '../../services/api'

const store = useInvitationStore()
const activeTab = ref('general')
const uploadingBg = ref(false)
const uploadingMusic = ref(false)

const tabs = [
  { id: 'general', label: 'General' },
  { id: 'design', label: 'Diseño' },
  { id: 'media', label: 'Media' }
]

const uploadBackgroundImage = async (event) => {
  const file = event.target.files[0]
  if (!file) return

  uploadingBg.value = true
  const formData = new FormData()
  formData.append('file', file)

  try {
    const response = await invitationService.uploadImage(formData)
    store.invitation.backgroundImage = response.data.url
  } catch (error) {
    console.error('Error uploading image:', error)
    alert('Error al subir la imagen')
  } finally {
    uploadingBg.value = false
  }
}

const uploadMusic = async (event) => {
  const file = event.target.files[0]
  if (!file) return

  uploadingMusic.value = true
  const formData = new FormData()
  formData.append('file', file)

  try {
    const response = await invitationService.uploadMusic(formData)
    store.invitation.musicUrl = response.data.url
  } catch (error) {
    console.error('Error uploading music:', error)
    alert('Error al subir la música')
  } finally {
    uploadingMusic.value = false
  }
}
</script>
