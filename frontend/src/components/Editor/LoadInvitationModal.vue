<template>
  <div v-if="show" class="fixed inset-0 bg-black bg-opacity-50 flex items-center justify-center z-50 p-4">
    <div class="bg-white rounded-lg p-8 max-w-md w-full">
      <h3 class="text-2xl font-bold text-gray-900 mb-4">
        Cargar Invitación
      </h3>

      <!-- Loading State -->
      <div v-if="loading" class="text-center py-8">
        <div class="animate-spin rounded-full h-12 w-12 border-b-4 border-purple-600 mx-auto mb-4"></div>
        <p class="text-gray-600">Cargando invitaciones...</p>
      </div>

      <!-- Error State -->
      <div v-else-if="error" class="text-center py-8">
        <div class="text-6xl mb-4">❌</div>
        <p class="text-red-600 mb-4">{{ error }}</p>
        <button
          @click="$emit('close')"
          class="px-6 py-2 bg-gray-200 text-gray-800 rounded-lg hover:bg-gray-300 transition-colors"
        >
          Cerrar
        </button>
      </div>

      <!-- No Invitations State -->
      <div v-else-if="invitations.length === 0" class="text-center py-8">
        <div class="text-6xl mb-4">📭</div>
        <p class="text-gray-600 mb-2 font-medium">No hay invitaciones guardadas</p>
        <p class="text-sm text-gray-500 mb-6">Crea y guarda tu primera invitación</p>
        <button
          @click="$emit('close')"
          class="px-6 py-2 bg-purple-600 text-white rounded-lg hover:bg-purple-700 transition-colors"
        >
          Cerrar
        </button>
      </div>

      <!-- Selection State -->
      <div v-else>
        <div class="mb-4">
          <label class="block text-sm font-medium text-gray-700 mb-2">
            Selecciona una invitación
          </label>
          <select
            v-model="selectedId"
            class="w-full px-3 py-2 text-sm border border-gray-300 rounded-lg focus:ring-2 focus:ring-purple-500 focus:border-transparent"
          >
            <option value="">-- Selecciona --</option>
            <option v-for="inv in invitations" :key="inv.id" :value="inv.id">
              {{ getDisplayName(inv) }}
            </option>
          </select>
        </div>

        <div class="bg-yellow-50 border border-yellow-200 rounded-lg p-3 mb-6">
          <p class="text-xs text-yellow-800">
            ⚠️ Se perderán los cambios no guardados en la invitación actual
          </p>
        </div>

        <div class="flex gap-3">
          <button
            @click="$emit('close')"
            class="flex-1 px-6 py-2 bg-gray-200 text-gray-800 rounded-lg hover:bg-gray-300 transition-colors"
          >
            Cancelar
          </button>
          <button
            @click="loadInvitation"
            :disabled="!selectedId"
            class="flex-1 px-6 py-2 bg-purple-600 text-white rounded-lg hover:bg-purple-700 transition-colors disabled:opacity-50 disabled:cursor-not-allowed"
          >
            Cargar
          </button>
        </div>
      </div>
    </div>
  </div>
</template>

<script setup>
import { ref, watch } from 'vue'
import { invitationService } from '../../services/api'

const props = defineProps({
  show: Boolean
})

const emit = defineEmits(['close', 'load'])

const invitations = ref([])
const selectedId = ref('')
const loading = ref(false)
const error = ref('')

// Fetch invitations when modal is shown
watch(() => props.show, async (newValue) => {
  if (newValue) {
    selectedId.value = ''
    error.value = ''
    loading.value = true

    try {
      const response = await invitationService.getAll()
      invitations.value = response.data
    } catch (err) {
      console.error('Error loading invitations:', err)
      error.value = 'Error al cargar las invitaciones'
    } finally {
      loading.value = false
    }
  }
})

const getDisplayName = (invitation) => {
  // Show title if available, otherwise show ID
  if (invitation.title && invitation.title.trim()) {
    return invitation.title
  }
  return `Invitación ${invitation.id.substring(0, 8)}`
}

const loadInvitation = async () => {
  if (!selectedId.value) return

  loading.value = true
  error.value = ''

  try {
    const response = await invitationService.getById(selectedId.value)
    emit('load', response.data)
    emit('close')
  } catch (err) {
    console.error('Error loading invitation:', err)
    error.value = 'Error al cargar la invitación seleccionada'
    loading.value = false
  }
}
</script>
