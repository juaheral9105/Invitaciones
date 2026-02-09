<template>
  <div class="h-screen flex flex-col bg-gray-50">
    <!-- Top Toolbar - Hidden in preview mode -->
    <header v-if="!previewMode" class="bg-white border-b border-gray-200 shadow-sm">
      <div class="px-4 py-3 flex items-center justify-between">
        <div class="flex items-center gap-4">
          <router-link to="/" class="text-gray-600 hover:text-gray-900">
            <svg class="w-6 h-6" fill="none" stroke="currentColor" viewBox="0 0 24 24">
              <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M15 19l-7-7 7-7" />
            </svg>
          </router-link>
          <div>
            <h1 class="text-lg font-semibold text-gray-900">Editor Visual</h1>
            <p class="text-xs text-gray-500">{{ isEditing ? 'Editando' : 'Nueva invitación' }}</p>
          </div>
        </div>

        <div class="flex items-center gap-2">
          <!-- Indicador de auto-guardado -->
          <span v-if="lastSaved" class="text-xs text-gray-500">
            💾 Auto-guardado: {{ formatTime(lastSaved) }}
          </span>

          <button
            @click="showLoadModal = true"
            class="px-3 py-2 text-sm font-medium text-gray-700 bg-white border border-gray-300 rounded-lg hover:bg-gray-50"
            title="Cargar Invitación Guardada"
          >
            📥 Cargar Invitación
          </button>
          <button
            @click="exportDraft"
            class="px-3 py-2 text-sm font-medium text-gray-700 bg-white border border-gray-300 rounded-lg hover:bg-gray-50"
            title="Exportar borrador"
          >
            📥 Exportar
          </button>

          <button
            @click="importDraft"
            class="px-3 py-2 text-sm font-medium text-gray-700 bg-white border border-gray-300 rounded-lg hover:bg-gray-50"
            title="Importar borrador"
          >
            📤 Importar
          </button>

          <button
            @click="clearDraft"
            class="px-3 py-2 text-sm font-medium text-gray-700 bg-white border border-gray-300 rounded-lg hover:bg-gray-50"
            title="Limpiar borrador"
          >
            🗑️
          </button>

          <button
            @click="previewMode = !previewMode"
            class="px-4 py-2 text-sm font-medium text-gray-700 bg-white border border-gray-300 rounded-lg hover:bg-gray-50"
          >
            {{ previewMode ? '✏️ Editar' : '👁️ Vista Previa' }}
          </button>
          <button
            @click="saveInvitation"
            :disabled="saving"
            class="px-6 py-2 text-sm font-medium text-white bg-purple-600 rounded-lg hover:bg-purple-700 disabled:opacity-50"
          >
            {{ saving ? 'Guardando...' : '💾 Guardar' }}
          </button>
        </div>
      </div>
    </header>

    <!-- Preview Mode Toggle Button - Floating in preview mode -->
    <button
      v-if="previewMode"
      @click="previewMode = false"
      class="fixed top-4 right-4 z-[80] px-4 py-2 text-sm font-medium text-white bg-purple-600 rounded-lg hover:bg-purple-700 shadow-lg"
    >
      ✏️ Editar
    </button>

    <!-- Music Player - Visible in preview mode, hidden in edit mode -->
    <MusicPlayerFixed
      v-if="store.invitation.musicUrl && !previewMode"
      :music-url="getFullUrl(store.invitation.musicUrl)"
      @remove="removeMusic"
    />

    <!-- Music Player for Preview Mode - Fixed and always visible -->
    <div
      v-if="store.invitation.musicUrl && previewMode"
      class="fixed right-6 top-1/2 -translate-y-1/2 z-[60]"
    >
      <MusicPlayer :music-url="getFullUrl(store.invitation.musicUrl)" />
    </div>

    <!-- Cover Page in Preview Mode -->
    <div
      v-if="store.invitation.coverEnabled && showCover && previewMode"
      style="position: fixed; top: 0; left: 0; right: 0; bottom: 0; width: 100vw; height: 100vh; z-index: 70;"
    >
      <CoverPage
        :visible="true"
        :cover-name="store.invitation.coverName"
        :button-text="store.invitation.coverButtonText"
        :background-color="store.invitation.coverBackgroundColor"
        :background-image="getFullUrl(store.invitation.coverBackgroundImage)"
        :text-color="store.invitation.coverTextColor"
        :button-color="store.invitation.coverButtonColor"
        :button-text-color="store.invitation.coverButtonTextColor"
        :font-family="store.invitation.coverFontFamily"
        :name-font-size="store.invitation.coverNameFontSize"
        :button-font-size="store.invitation.coverButtonFontSize"
        @enter="enterInvitation"
      />
    </div>

    <!-- Main Content -->
    <div class="flex-1 flex overflow-hidden">
      <!-- Left Sidebar - Blocks Palette (only in edit mode) -->
      <aside v-if="!previewMode" class="w-64 bg-white border-r border-gray-200 overflow-y-auto">
        <BlocksPalette />
      </aside>

      <!-- Center Canvas -->
      <main class="flex-1 overflow-hidden flex flex-col">
        <DesignCanvas :preview-mode="previewMode" />
      </main>

      <!-- Right Sidebar - Properties Panel (only in edit mode and when block selected) -->
      <aside v-if="!previewMode && store.selectedBlockId" class="w-80 bg-white border-l border-gray-200 overflow-y-auto">
        <PropertiesPanel />
      </aside>
    </div>

    <!-- Toast Notification -->
    <Transition name="toast">
      <div v-if="message" class="fixed bottom-4 right-4 z-50">
        <div
          :class="[
            'px-6 py-4 rounded-lg shadow-lg flex items-center gap-3',
            messageType === 'success' ? 'bg-green-500' : 'bg-red-500',
            'text-white'
          ]"
        >
          <span class="text-xl">{{ messageType === 'success' ? '✅' : '❌' }}</span>
          <span>{{ message }}</span>
        </div>
      </div>
    </Transition>

    <!-- Load Invitation Modal -->
    <LoadInvitationModal
      :show="showLoadModal"
      @close="showLoadModal = false"
      @load="handleLoadInvitation"
    />
  </div>
</template>

<script setup>
import { ref, onMounted, watch } from 'vue'
import { useRoute, useRouter } from 'vue-router'
import { useInvitationStore } from '../../stores/invitationStore'
import { invitationService } from '../../services/api'
import BlocksPalette from '../../components/Editor/BlocksPalette.vue'
import DesignCanvas from '../../components/Editor/DesignCanvas.vue'
import PropertiesPanel from '../../components/Editor/PropertiesPanel.vue'
import MusicPlayerFixed from '../../components/Common/MusicPlayerFixed.vue'
import MusicPlayer from '../../components/Common/MusicPlayer.vue'
import CoverPage from '../../components/Viewer/CoverPage.vue'
import LoadInvitationModal from '../../components/Editor/LoadInvitationModal.vue'

const route = useRoute()
const router = useRouter()
const store = useInvitationStore()

const previewMode = ref(false)
const saving = ref(false)
const message = ref('')
const messageType = ref('success')
const isEditing = ref(false)
const autoSaveTimer = ref(null)
const lastSaved = ref(null)
const showCover = ref(true)
const showLoadModal = ref(false)

// Auto-guardado en localStorage
const saveToLocalStorage = () => {
  try {
    const data = {
      invitation: store.invitation,
      timestamp: new Date().toISOString(),
      version: '1.0'
    }
    localStorage.setItem('invitation_draft', JSON.stringify(data))
    lastSaved.value = new Date()
    console.log('✅ Borrador guardado automáticamente')
  } catch (error) {
    console.error('Error al guardar en localStorage:', error)
  }
}

// Cargar desde localStorage
const loadFromLocalStorage = () => {
  try {
    const saved = localStorage.getItem('invitation_draft')
    if (saved) {
      const data = JSON.parse(saved)
      return data.invitation
    }
  } catch (error) {
    console.error('Error al cargar desde localStorage:', error)
  }
  return null
}

// Auto-guardado con debounce (espera 2 segundos después del último cambio)
// DESHABILITADO: Ya no guardamos automáticamente en localStorage
// watch(() => store.invitation, () => {
//   if (autoSaveTimer.value) {
//     clearTimeout(autoSaveTimer.value)
//   }
//   autoSaveTimer.value = setTimeout(() => {
//     saveToLocalStorage()
//   }, 2000)
// }, { deep: true })

onMounted(async () => {
  const id = route.params.id
  if (id) {
    isEditing.value = true
    try {
      const response = await invitationService.getById(id)
      // Convert sections to blocks (backend returns sections, we use blocks internally)
      if (response.data.sections) {
        // Check if sections is already in blocks format or old format
        const firstSection = response.data.sections[0]
        if (firstSection && firstSection.id && firstSection.type) {
          // Already in blocks format, just rename
          response.data.blocks = response.data.sections
        } else {
          // Old sections format, need conversion
          response.data.blocks = convertSectionsToBlocks(response.data.sections)
        }
        delete response.data.sections
      }
      store.setInvitation(response.data)
    } catch (error) {
      showMessage('Error al cargar la invitación', 'error')
    }
  } else {
    // DESHABILITADO: Ya no cargamos automáticamente desde localStorage
    // const draft = loadFromLocalStorage()
    // if (draft) {
    //   const shouldLoad = confirm('Se encontró un borrador guardado. ¿Deseas cargarlo?')
    //   if (shouldLoad) {
    //     store.setInvitation(draft)
    //     showMessage('Borrador cargado exitosamente', 'success')
    //   }
    // }
  }
})

const convertSectionsToBlocks = (sections) => {
  // Convert old sections format to new blocks format
  return sections.map((section, index) => ({
    id: Date.now().toString() + index,
    type: section.type || 'text',
    position: { x: 50, y: index * 150 + 100 },
    size: { width: 600, height: 'auto' },
    style: {
      backgroundColor: 'transparent',
      color: '#000000',
      fontFamily: 'Montserrat',
      fontSize: '16px',
      textAlign: 'center',
      padding: '20px',
      borderRadius: '8px'
    },
    content: section
  }))
}

const saveInvitation = async () => {
  saving.value = true
  try {
    const data = {
      title: store.invitation.title || 'Invitación sin título',
      ...store.invitation,
      // Send blocks as sections for backend compatibility
      sections: store.invitation.blocks,
      // Remove blocks from the data object to avoid sending it
      blocks: undefined
    }

    if (data.id) {
      await invitationService.update(data.id, data)
      showMessage('✅ Invitación actualizada exitosamente', 'success')
    } else {
      const response = await invitationService.create(data)
      // Backend returns sections, convert to blocks
      if (response.data.sections && !response.data.blocks) {
        response.data.blocks = response.data.sections
        delete response.data.sections
      }
      store.setInvitation(response.data)
      showMessage('✅ Invitación creada exitosamente', 'success')
      router.push({ name: 'EditorEdit', params: { id: response.data.id } })
    }
  } catch (error) {
    showMessage('❌ Error al guardar la invitación', 'error')
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

const API_BASE_URL = import.meta.env.VITE_API_URL?.replace('/api', '') || 'http://localhost:5247'

const getFullUrl = (url) => {
  if (!url) return ''
  if (url.startsWith('http')) return url
  return `${API_BASE_URL}${url}`
}

const removeMusic = () => {
  store.invitation.musicUrl = null
  showMessage('Música eliminada', 'success')
}

// Formatear tiempo
const formatTime = (date) => {
  const now = new Date()
  const diff = Math.floor((now - date) / 1000) // segundos

  if (diff < 60) return 'hace unos segundos'
  if (diff < 3600) return `hace ${Math.floor(diff / 60)} min`
  if (diff < 86400) return `hace ${Math.floor(diff / 3600)} h`
  return date.toLocaleTimeString('es-ES', { hour: '2-digit', minute: '2-digit' })
}

// Exportar borrador como archivo JSON
const exportDraft = () => {
  try {
    const data = {
      invitation: store.invitation,
      timestamp: new Date().toISOString(),
      version: '1.0'
    }
    const blob = new Blob([JSON.stringify(data, null, 2)], { type: 'application/json' })
    const url = URL.createObjectURL(blob)
    const a = document.createElement('a')
    a.href = url
    a.download = `invitacion_borrador_${new Date().toISOString().split('T')[0]}.json`
    document.body.appendChild(a)
    a.click()
    document.body.removeChild(a)
    URL.revokeObjectURL(url)
    showMessage('✅ Borrador exportado exitosamente', 'success')
  } catch (error) {
    showMessage('❌ Error al exportar borrador', 'error')
    console.error(error)
  }
}

// Importar borrador desde archivo JSON
const importDraft = () => {
  const input = document.createElement('input')
  input.type = 'file'
  input.accept = '.json'
  input.onchange = (e) => {
    const file = e.target.files[0]
    if (!file) return

    const reader = new FileReader()
    reader.onload = (event) => {
      try {
        const data = JSON.parse(event.target.result)
        if (data.invitation) {
          store.setInvitation(data.invitation)
          // saveToLocalStorage() // DESHABILITADO
          showMessage('✅ Borrador importado exitosamente', 'success')
        } else {
          showMessage('❌ Archivo inválido', 'error')
        }
      } catch (error) {
        showMessage('❌ Error al leer el archivo', 'error')
        console.error(error)
      }
    }
    reader.readAsText(file)
  }
  input.click()
}

// Limpiar borrador
const clearDraft = () => {
  if (confirm('¿Estás seguro de que quieres eliminar el borrador guardado? Esta acción no se puede deshacer.')) {
    localStorage.removeItem('invitation_draft')
    lastSaved.value = null
    showMessage('🗑️ Borrador eliminado', 'success')
  }
}

// Handle cover page transition
const enterInvitation = () => {
  showCover.value = false
}

// Handle loading an invitation from the modal
const handleLoadInvitation = async (invitationData) => {
  try {
    // Convert sections to blocks (backend returns sections, we use blocks internally)
    if (invitationData.sections) {
      // Check if sections is already in blocks format or old format
      const firstSection = invitationData.sections[0]
      if (firstSection && firstSection.id && firstSection.type) {
        // Already in blocks format, just rename
        invitationData.blocks = invitationData.sections
      } else {
        // Old sections format, need conversion
        invitationData.blocks = convertSectionsToBlocks(invitationData.sections)
      }
      delete invitationData.sections
    }

    // Set the invitation in the store
    store.setInvitation(invitationData)

    // Update URL to reflect loaded invitation
    router.push({ name: 'EditorEdit', params: { id: invitationData.id } })

    showMessage('✅ Invitación cargada exitosamente', 'success')
  } catch (error) {
    console.error('Error loading invitation:', error)
    showMessage('❌ Error al cargar la invitación', 'error')
  }
}

// Watch preview mode changes to reset cover page
watch(previewMode, (newValue) => {
  if (newValue && store.invitation.coverEnabled) {
    showCover.value = true
  }
})
</script>

<style scoped>
.toast-enter-active,
.toast-leave-active {
  transition: all 0.3s ease;
}

.toast-enter-from,
.toast-leave-to {
  opacity: 0;
  transform: translateY(20px);
}
</style>
