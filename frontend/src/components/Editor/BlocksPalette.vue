<template>
  <div class="p-4">
    <h2 class="text-sm font-semibold text-gray-900 mb-4">📦 Agregar Elementos</h2>

    <div class="space-y-2">
      <div v-for="category in blockCategories" :key="category.name" class="mb-6">
        <h3 class="text-xs font-medium text-gray-500 uppercase tracking-wide mb-2">
          {{ category.name }}
        </h3>

        <div class="space-y-1">
          <button
            v-for="block in category.blocks"
            :key="block.type"
            @click="addBlock(block.type)"
            class="w-full flex items-center gap-3 px-3 py-2.5 text-left text-sm text-gray-700 bg-white border border-gray-200 rounded-lg hover:bg-purple-50 hover:border-purple-300 hover:text-purple-700 transition-all group"
          >
            <span class="text-xl">{{ block.icon }}</span>
            <div class="flex-1">
              <div class="font-medium">{{ block.label }}</div>
              <div class="text-xs text-gray-500 group-hover:text-purple-600">{{ block.description }}</div>
            </div>
          </button>
        </div>
      </div>
    </div>

    <!-- Quick Settings -->
    <div class="mt-6 pt-6 border-t border-gray-200">
      <h3 class="text-xs font-medium text-gray-500 uppercase tracking-wide mb-3">
        ⚙️ Configuración General
      </h3>

      <div class="space-y-3">
        <div>
          <label class="block text-xs font-medium text-gray-700 mb-1">Color de fondo</label>
          <input
            v-model="store.invitation.backgroundColor"
            type="color"
            class="w-full h-9 rounded cursor-pointer"
          />
        </div>

        <div>
          <label class="block text-xs font-medium text-gray-700 mb-2">Imagen de fondo</label>
          <input
            type="file"
            @change="uploadBackgroundImage"
            accept="image/*"
            class="w-full text-xs text-gray-500 file:mr-2 file:py-1.5 file:px-3 file:rounded file:border-0 file:text-xs file:font-semibold file:bg-purple-50 file:text-purple-700 hover:file:bg-purple-100"
          />
          <p v-if="uploadingBg" class="text-xs text-gray-500 mt-1">Subiendo...</p>
        </div>

        <div>
          <label class="block text-xs font-medium text-gray-700 mb-2">Música de fondo</label>
          <input
            type="file"
            @change="uploadMusic"
            accept="audio/*"
            class="w-full text-xs text-gray-500 file:mr-2 file:py-1.5 file:px-3 file:rounded file:border-0 file:text-xs file:font-semibold file:bg-purple-50 file:text-purple-700 hover:file:bg-purple-100"
          />
          <p v-if="uploadingMusic" class="text-xs text-gray-500 mt-1">Subiendo...</p>
          <p v-if="store.invitation.musicUrl" class="text-xs text-green-600 mt-1">✓ Música agregada</p>
        </div>

        <div>
          <label class="block text-xs font-medium text-gray-700 mb-1">Email para confirmaciones</label>
          <input
            v-model="store.invitation.formEmail"
            type="email"
            placeholder="tu@email.com"
            class="w-full px-3 py-2 text-sm border border-gray-300 rounded-lg focus:ring-2 focus:ring-purple-500 focus:border-transparent"
          />
        </div>
      </div>
    </div>

    <!-- Guest List Management -->
    <div class="mt-6 pt-6 border-t border-gray-200">
      <h3 class="text-xs font-medium text-gray-500 uppercase tracking-wide mb-3">
        👥 Lista de Invitados
      </h3>

      <div class="space-y-3">
        <div>
          <label class="block text-xs font-medium text-gray-700 mb-2">Subir Excel con invitados</label>
          <input
            type="file"
            @change="uploadGuestList"
            accept=".xlsx,.xls"
            class="w-full text-xs text-gray-500 file:mr-2 file:py-1.5 file:px-3 file:rounded file:border-0 file:text-xs file:font-semibold file:bg-purple-50 file:text-purple-700 hover:file:bg-purple-100"
          />
          <p v-if="uploadingGuestList" class="text-xs text-blue-600 mt-1">⏳ {{ guestListSuccess || 'Procesando archivo...' }}</p>
          <p v-else-if="guestListError" class="text-xs text-red-600 mt-1">❌ {{ guestListError }}</p>
          <p v-else-if="guestListSuccess" class="text-xs text-green-600 mt-1">✓ {{ guestListSuccess }}</p>
          <p v-else class="text-xs text-gray-500 mt-1">No necesitas guardar primero • .xlsx o .xls</p>
        </div>

        <div v-if="guestCount > 0" class="bg-purple-50 rounded-lg p-3">
          <div class="flex items-center justify-between mb-2">
            <span class="text-sm font-semibold text-purple-900">Total de invitados: {{ guestCount }}</span>
            <button
              @click="viewGuestList"
              class="text-xs text-purple-600 hover:text-purple-800 font-medium"
            >
              Ver lista →
            </button>
          </div>
          <button
            @click="clearGuestList"
            class="w-full text-xs text-red-600 hover:text-red-800 font-medium py-1"
          >
            Limpiar lista
          </button>
        </div>

        <div class="bg-blue-50 rounded-lg p-3">
          <p class="text-xs text-blue-800 font-medium mb-1">📋 Formato del Excel:</p>
          <ul class="text-xs text-blue-700 space-y-0.5 ml-3">
            <li>• Columna A: Nombre</li>
            <li>• Columna B: Celular</li>
            <li>• Columna C: Núm. de invitados</li>
            <li>• Columna D: Nombres invitados</li>
            <li>• Columna E: Evento con hospedaje</li>
            <li>• Columna F: Hospedaje permitido</li>
          </ul>
        </div>
      </div>
    </div>

    <!-- Cover Page Settings -->
    <div class="mt-6 pt-6 border-t border-gray-200">
      <h3 class="text-xs font-medium text-gray-500 uppercase tracking-wide mb-3">
        🏠 Página de Portada
      </h3>

      <div class="space-y-3">
        <div>
          <label class="flex items-center gap-2 cursor-pointer">
            <input
              v-model="store.invitation.coverEnabled"
              type="checkbox"
              class="w-4 h-4 text-purple-600 rounded"
            />
            <span class="text-xs font-medium text-gray-700">Habilitar página inicial</span>
          </label>
        </div>

        <div v-if="store.invitation.coverEnabled" class="space-y-3 pl-6 border-l-2 border-purple-200">
          <div>
            <label class="block text-xs font-medium text-gray-700 mb-1">Nombre de la homenajeada</label>
            <input
              v-model="store.invitation.coverName"
              type="text"
              placeholder="María García"
              class="w-full px-3 py-2 text-sm border border-gray-300 rounded-lg focus:ring-2 focus:ring-purple-500 focus:border-transparent"
            />
          </div>

          <div>
            <label class="block text-xs font-medium text-gray-700 mb-1">Tamaño del nombre</label>
            <input
              v-model="store.invitation.coverNameFontSize"
              type="text"
              placeholder="6rem"
              class="w-full px-3 py-2 text-sm border border-gray-300 rounded-lg focus:ring-2 focus:ring-purple-500 focus:border-transparent"
            />
            <p class="text-xs text-gray-500 mt-1">Ejemplos: 6rem, 72px, 4em</p>
          </div>

          <div>
            <label class="block text-xs font-medium text-gray-700 mb-1">Texto del botón</label>
            <input
              v-model="store.invitation.coverButtonText"
              type="text"
              placeholder="Ver Invitación"
              class="w-full px-3 py-2 text-sm border border-gray-300 rounded-lg focus:ring-2 focus:ring-purple-500 focus:border-transparent"
            />
          </div>

          <div>
            <label class="block text-xs font-medium text-gray-700 mb-1">Tamaño del botón</label>
            <input
              v-model="store.invitation.coverButtonFontSize"
              type="text"
              placeholder="1.125rem"
              class="w-full px-3 py-2 text-sm border border-gray-300 rounded-lg focus:ring-2 focus:ring-purple-500 focus:border-transparent"
            />
            <p class="text-xs text-gray-500 mt-1">Ejemplos: 1.125rem, 18px, 1.2em</p>
          </div>

          <div>
            <label class="block text-xs font-medium text-gray-700 mb-1">Color de fondo</label>
            <input
              v-model="store.invitation.coverBackgroundColor"
              type="color"
              class="w-full h-9 rounded cursor-pointer"
            />
          </div>

          <div>
            <label class="block text-xs font-medium text-gray-700 mb-2">Imagen de fondo</label>
            <input
              type="file"
              @change="uploadCoverBackgroundImage"
              accept="image/*"
              class="w-full text-xs text-gray-500 file:mr-2 file:py-1.5 file:px-3 file:rounded file:border-0 file:text-xs file:font-semibold file:bg-purple-50 file:text-purple-700 hover:file:bg-purple-100"
            />
            <p v-if="uploadingCoverBg" class="text-xs text-gray-500 mt-1">Subiendo...</p>
          </div>

          <div>
            <label class="block text-xs font-medium text-gray-700 mb-1">Color del texto</label>
            <input
              v-model="store.invitation.coverTextColor"
              type="color"
              class="w-full h-9 rounded cursor-pointer"
            />
          </div>

          <div>
            <label class="block text-xs font-medium text-gray-700 mb-1">Color del botón</label>
            <input
              v-model="store.invitation.coverButtonColor"
              type="color"
              class="w-full h-9 rounded cursor-pointer"
            />
          </div>

          <div>
            <label class="block text-xs font-medium text-gray-700 mb-1">Color texto del botón</label>
            <input
              v-model="store.invitation.coverButtonTextColor"
              type="color"
              class="w-full h-9 rounded cursor-pointer"
            />
          </div>

          <div>
            <label class="block text-xs font-medium text-gray-700 mb-1">Fuente</label>
            <select
              v-model="store.invitation.coverFontFamily"
              class="w-full px-3 py-2 text-sm border border-gray-300 rounded-lg focus:ring-2 focus:ring-purple-500 focus:border-transparent"
            >
              <optgroup label="Sans-serif (modernas)">
                <option value="Arial">Arial</option>
                <option value="Montserrat">Montserrat</option>
                <option value="Open Sans">Open Sans</option>
                <option value="Raleway">Raleway</option>
                <option value="Poppins">Poppins</option>
                <option value="Roboto">Roboto</option>
                <option value="Oswald">Oswald</option>
                <option value="Bebas Neue">Bebas Neue</option>
              </optgroup>
              <optgroup label="Serif (elegantes)">
                <option value="Playfair Display">Playfair Display</option>
                <option value="Lora">Lora</option>
                <option value="Merriweather">Merriweather</option>
                <option value="Crimson Text">Crimson Text</option>
              </optgroup>
              <optgroup label="Script (manuscritas)">
                <option value="Dancing Script">Dancing Script</option>
                <option value="Pacifico">Pacifico</option>
                <option value="Great Vibes">Great Vibes</option>
                <option value="Satisfy">Satisfy</option>
              </optgroup>
              <optgroup label="Monospace">
                <option value="Courier New">Courier New</option>
              </optgroup>
            </select>
          </div>
        </div>
      </div>
    </div>
  </div>
</template>

<script setup>
import { ref, computed } from 'vue'
import { useInvitationStore } from '../../stores/invitationStore'
import { invitationService, guestListService } from '../../services/api'

const store = useInvitationStore()
const uploadingBg = ref(false)
const uploadingMusic = ref(false)
const uploadingCoverBg = ref(false)
const uploadingGuestList = ref(false)
const guestListError = ref('')
const guestListSuccess = ref('')
const guestCount = ref(0)

const blockCategories = [
  {
    name: 'Contenido',
    blocks: [
      { type: 'heading', icon: '📝', label: 'Título', description: 'Encabezado grande' },
      { type: 'text', icon: '📄', label: 'Texto', description: 'Párrafo de texto' },
      { type: 'eventInfo', icon: '📅', label: 'Info del Evento', description: 'Fecha, hora, lugar' },
      { type: 'countdown', icon: '⏰', label: 'Contador', description: 'Cuenta regresiva' }
    ]
  },
  {
    name: 'Multimedia',
    blocks: [
      { type: 'image', icon: '🖼️', label: 'Imagen', description: 'Una imagen' },
      { type: 'carousel', icon: '🎠', label: 'Carrusel', description: 'Galería de imágenes' },
      { type: 'map', icon: '🗺️', label: 'Mapa', description: 'Google Maps' }
    ]
  },
  {
    name: 'Interacción',
    blocks: [
      { type: 'button', icon: '🔘', label: 'Botón', description: 'Botón de acción' },
      { type: 'form', icon: '📋', label: 'Formulario', description: 'Confirmación de asistencia' }
    ]
  },
  {
    name: 'Diseño',
    blocks: [
      { type: 'divider', icon: '➖', label: 'Separador', description: 'Línea divisoria' },
      { type: 'spacer', icon: '⬜', label: 'Espaciador', description: 'Espacio en blanco' }
    ]
  }
]

const addBlock = (type) => {
  store.addBlock(type)
}

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

const uploadCoverBackgroundImage = async (event) => {
  const file = event.target.files[0]
  if (!file) return

  uploadingCoverBg.value = true
  const formData = new FormData()
  formData.append('file', file)

  try {
    const response = await invitationService.uploadImage(formData)
    store.invitation.coverBackgroundImage = response.data.url
  } catch (error) {
    console.error('Error uploading cover image:', error)
    alert('Error al subir la imagen de portada')
  } finally {
    uploadingCoverBg.value = false
  }
}

const uploadGuestList = async (event) => {
  const file = event.target.files[0]
  if (!file) return

  uploadingGuestList.value = true
  guestListError.value = ''
  guestListSuccess.value = ''

  try {
    let invitationId = store.invitation.id

    // Siempre intentar guardar/actualizar la invitación primero
    guestListSuccess.value = 'Guardando invitación...'

    const invitationData = {
      title: store.invitation.title || 'Invitación sin título',
      sectionsJson: JSON.stringify(store.invitation.blocks),
      backgroundColor: store.invitation.backgroundColor,
      backgroundImage: store.invitation.backgroundImage,
      musicUrl: store.invitation.musicUrl,
      formEmail: store.invitation.formEmail
    }

    if (invitationId) {
      // Si hay ID, intentar actualizar (puede que no exista en BD)
      try {
        await invitationService.update(invitationId, invitationData)
        console.log('Invitation updated successfully')
      } catch (updateError) {
        // Si falla la actualización, crear una nueva
        console.log('Update failed, creating new invitation')
        const invitationResponse = await invitationService.create(invitationData)
        invitationId = invitationResponse.data.id
        store.invitation.id = invitationId

        // Actualizar la URL del navegador
        if (window.history.pushState) {
          window.history.pushState({}, '', `/editor/${invitationId}`)
        }
      }
    } else {
      // Si no hay ID, crear nueva
      const invitationResponse = await invitationService.create(invitationData)
      invitationId = invitationResponse.data.id
      store.invitation.id = invitationId

      // Actualizar la URL del navegador
      if (window.history.pushState) {
        window.history.pushState({}, '', `/editor/${invitationId}`)
      }
    }

    guestListSuccess.value = 'Subiendo lista de invitados...'

    // Ahora subir el archivo Excel
    const formData = new FormData()
    formData.append('file', file)

    const response = await guestListService.uploadExcel(invitationId, formData)
    guestCount.value = response.data.totalGuests
    guestListSuccess.value = `${response.data.totalGuests} invitados cargados exitosamente`
    setTimeout(() => guestListSuccess.value = '', 5000)
  } catch (error) {
    console.error('Error uploading guest list:', error)
    console.error('Error response:', error.response)
    console.error('Current invitation ID:', store.invitation.id)
    guestListError.value = error.response?.data?.message || error.message || 'Error al procesar el archivo Excel'
  } finally {
    uploadingGuestList.value = false
    event.target.value = ''
  }
}

const clearGuestList = async () => {
  if (!confirm('¿Estás seguro de que quieres eliminar toda la lista de invitados?')) {
    return
  }

  try {
    await guestListService.clearGuestList(store.invitation.id)
    guestCount.value = 0
    guestListSuccess.value = 'Lista de invitados eliminada'
    setTimeout(() => guestListSuccess.value = '', 3000)
  } catch (error) {
    console.error('Error clearing guest list:', error)
    guestListError.value = 'Error al limpiar la lista'
  }
}

const viewGuestList = () => {
  alert('Funcionalidad de ver lista completa - Por implementar')
  // TODO: Implementar modal o vista para ver/editar invitados
}

// Cargar el conteo de invitados cuando se carga el componente
const loadGuestCount = async () => {
  if (store.invitation.id) {
    try {
      const response = await guestListService.getGuests(store.invitation.id, 1, 1)
      guestCount.value = response.data.totalGuests
    } catch (error) {
      // No mostrar error si no hay lista aún
      console.log('No guest list yet')
    }
  }
}

// Cargar el conteo cuando se monta el componente
loadGuestCount()
</script>
