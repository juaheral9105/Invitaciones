<template>
  <div class="min-h-screen bg-gray-100">
    <!-- Header -->
    <header class="bg-white shadow-sm border-b">
      <div class="max-w-7xl mx-auto px-4 py-4 flex justify-between items-center">
        <div>
          <h1 class="text-2xl font-bold text-purple-900">Editor de Invitación</h1>
          <p class="text-sm text-gray-600">{{ isEditing ? 'Editando invitación' : 'Nueva invitación' }}</p>
        </div>
        <div class="flex gap-3">
          <button
            @click="previewMode = !previewMode"
            class="px-4 py-2 bg-gray-200 text-gray-700 rounded-lg hover:bg-gray-300 transition-colors"
          >
            {{ previewMode ? 'Editar' : 'Vista Previa' }}
          </button>
          <button
            @click="saveInvitation"
            :disabled="saving"
            class="px-6 py-2 bg-purple-600 text-white rounded-lg hover:bg-purple-700 transition-colors disabled:opacity-50"
          >
            {{ saving ? 'Guardando...' : 'Guardar' }}
          </button>
        </div>
      </div>
    </header>

    <div class="max-w-7xl mx-auto px-4 py-8">
      <div class="grid grid-cols-1 lg:grid-cols-3 gap-6">
        <!-- Editor Panel -->
        <div v-if="!previewMode" class="lg:col-span-1 space-y-6">
          <EditorPanel />
        </div>

        <!-- Preview Panel -->
        <div :class="previewMode ? 'lg:col-span-3' : 'lg:col-span-2'">
          <InvitationPreview />
        </div>
      </div>
    </div>

    <!-- Success/Error Messages -->
    <div v-if="message" class="fixed bottom-4 right-4 z-50">
      <div
        :class="[
          'px-6 py-4 rounded-lg shadow-lg',
          messageType === 'success' ? 'bg-green-500' : 'bg-red-500',
          'text-white'
        ]"
      >
        {{ message }}
      </div>
    </div>
  </div>
</template>

<script setup>
import { ref, onMounted } from 'vue'
import { useRoute, useRouter } from 'vue-router'
import { useInvitationStore } from '../../stores/invitationStore'
import { invitationService } from '../../services/api'
import EditorPanel from '../../components/Editor/EditorPanel.vue'
import InvitationPreview from '../../components/Editor/InvitationPreview.vue'

const route = useRoute()
const router = useRouter()
const store = useInvitationStore()

const previewMode = ref(false)
const saving = ref(false)
const message = ref('')
const messageType = ref('success')
const isEditing = ref(false)

onMounted(async () => {
  const id = route.params.id
  if (id) {
    isEditing.value = true
    try {
      const response = await invitationService.getById(id)
      store.setInvitation(response.data)
    } catch (error) {
      showMessage('Error al cargar la invitación', 'error')
    }
  }
})

const saveInvitation = async () => {
  saving.value = true
  try {
    const data = store.invitation

    if (data.id) {
      await invitationService.update(data.id, data)
      showMessage('Invitación actualizada exitosamente', 'success')
    } else {
      const response = await invitationService.create(data)
      store.setInvitation(response.data)
      showMessage('Invitación creada exitosamente', 'success')
      // Redirect to edit mode
      router.push({ name: 'EditorEdit', params: { id: response.data.id } })
    }
  } catch (error) {
    showMessage('Error al guardar la invitación', 'error')
    console.error(error)
  } finally {
    saving.value = false
  }
}

const showMessage = (msg, type) => {
  message.value = msg
  messageType.value = type
  setTimeout(() => {
    message.value = ''
  }, 3000)
}
</script>
