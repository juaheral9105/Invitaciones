<template>
  <div class="h-full overflow-auto bg-gray-100 p-8">
    <div class="max-w-4xl mx-auto">
      <!-- Canvas with invitation background -->
      <div
        ref="canvasRef"
        class="min-h-screen bg-white shadow-2xl rounded-lg overflow-hidden"
        :style="{
          backgroundColor: store.invitation.backgroundColor,
          backgroundImage: store.invitation.backgroundImage
            ? `url(${getImageUrl(store.invitation.backgroundImage)})`
            : 'none',
          backgroundSize: 'cover',
          backgroundPosition: 'center'
        }"
        @click="handleCanvasClick"
      >
        <!-- Empty State -->
        <div
          v-if="store.invitation.blocks.length === 0"
          class="flex items-center justify-center min-h-screen"
        >
          <div class="text-center p-8">
            <div class="text-6xl mb-4">📦</div>
            <h3 class="text-2xl font-semibold text-gray-700 mb-2">Empieza a diseñar</h3>
            <p class="text-gray-500">Agrega elementos desde el panel izquierdo</p>
          </div>
        </div>

        <!-- Blocks Container -->
        <draggable
          v-else
          v-model="store.invitation.blocks"
          item-key="id"
          handle=".drag-handle"
          class="space-y-0"
          @end="onDragEnd"
        >
          <template #item="{ element: block, index }">
            <div
              :key="block.id"
              class="relative group"
              :class="{ 'ring-2 ring-purple-500': !props.previewMode && store.selectedBlockId === block.id }"
              @click.stop="!props.previewMode && store.selectBlock(block.id)"
            >
              <!-- Block Controls (visible on hover or when selected) -->
              <div
                v-if="!props.previewMode && (store.selectedBlockId === block.id || hoveredBlockId === block.id)"
                class="absolute -top-10 left-0 right-0 flex items-center justify-between bg-purple-600 text-white px-3 py-1.5 rounded-t-lg text-xs z-10"
                @mouseenter="hoveredBlockId = block.id"
                @mouseleave="hoveredBlockId = null"
              >
                <div class="flex items-center gap-2">
                  <span class="drag-handle cursor-move">⋮⋮</span>
                  <span class="font-medium">{{ getBlockLabel(block.type) }}</span>
                </div>
                <div class="flex gap-1">
                  <button
                    @click.stop="moveBlockUp(block.id, index)"
                    :disabled="index === 0"
                    class="p-1 hover:bg-purple-700 rounded disabled:opacity-50 disabled:cursor-not-allowed"
                    title="Subir"
                  >
                    ⬆️
                  </button>
                  <button
                    @click.stop="moveBlockDown(block.id, index)"
                    :disabled="index === store.invitation.blocks.length - 1"
                    class="p-1 hover:bg-purple-700 rounded disabled:opacity-50 disabled:cursor-not-allowed"
                    title="Bajar"
                  >
                    ⬇️
                  </button>
                  <button
                    @click.stop="store.duplicateBlock(block.id)"
                    class="p-1 hover:bg-purple-700 rounded"
                    title="Duplicar"
                  >
                    📋
                  </button>
                  <button
                    @click.stop="store.deleteBlock(block.id)"
                    class="p-1 hover:bg-red-600 rounded"
                    title="Eliminar"
                  >
                    🗑️
                  </button>
                </div>
              </div>

              <!-- Block Content -->
              <div
                class="transition-all"
                :style="getBlockStyles(block)"
                @mouseenter="!props.previewMode && (hoveredBlockId = block.id)"
                @mouseleave="!props.previewMode && (hoveredBlockId = null)"
              >
                <component
                  :is="getBlockComponent(block.type)"
                  :block="block"
                  :preview-mode="props.previewMode"
                />
              </div>
            </div>
          </template>
        </draggable>
      </div>
    </div>
  </div>
</template>

<script setup>
import { ref, computed } from 'vue'
import { useInvitationStore } from '../../stores/invitationStore'
import draggable from 'vuedraggable'

// Block Components
import TextBlock from './Blocks/TextBlock.vue'
import HeadingBlock from './Blocks/HeadingBlock.vue'
import ImageBlock from './Blocks/ImageBlock.vue'
import CarouselBlock from './Blocks/CarouselBlock.vue'
import EventInfoBlock from './Blocks/EventInfoBlock.vue'
import CountdownBlock from './Blocks/CountdownBlock.vue'
import MapBlock from './Blocks/MapBlock.vue'
import ButtonBlock from './Blocks/ButtonBlock.vue'
import DividerBlock from './Blocks/DividerBlock.vue'
import SpacerBlock from './Blocks/SpacerBlock.vue'
import FormBlock from './Blocks/FormBlock.vue'

const props = defineProps({
  previewMode: {
    type: Boolean,
    default: false
  }
})

const store = useInvitationStore()
const canvasRef = ref(null)
const hoveredBlockId = ref(null)

const API_BASE_URL = import.meta.env.VITE_API_URL?.replace('/api', '') || 'http://localhost:5000'

const getImageUrl = (url) => {
  if (!url) return ''
  if (url.startsWith('http')) return url
  return `${API_BASE_URL}${url}`
}

const getBlockComponent = (type) => {
  const components = {
    text: TextBlock,
    heading: HeadingBlock,
    image: ImageBlock,
    carousel: CarouselBlock,
    eventInfo: EventInfoBlock,
    countdown: CountdownBlock,
    map: MapBlock,
    button: ButtonBlock,
    divider: DividerBlock,
    spacer: SpacerBlock,
    form: FormBlock
  }
  return components[type] || TextBlock
}

const getBlockLabel = (type) => {
  const labels = {
    text: 'Texto',
    heading: 'Título',
    image: 'Imagen',
    carousel: 'Carrusel',
    eventInfo: 'Info del Evento',
    countdown: 'Contador',
    map: 'Mapa',
    button: 'Botón',
    divider: 'Separador',
    spacer: 'Espaciador',
    form: 'Formulario'
  }
  return labels[type] || type
}

const getBlockStyles = (block) => {
  return {
    backgroundColor: block.style.backgroundColor,
    color: block.style.color,
    fontFamily: block.style.fontFamily,
    fontSize: block.style.fontSize,
    textAlign: block.style.textAlign,
    padding: block.style.padding,
    borderRadius: block.style.borderRadius,
    width: block.style.width || '100%',
    height: block.style.height || 'auto',
    display: 'block'
  }
}

const handleCanvasClick = (e) => {
  if (e.target === canvasRef.value) {
    store.deselectBlock()
  }
}

const moveBlockUp = (blockId, currentIndex) => {
  if (currentIndex === 0) return

  const blocks = [...store.invitation.blocks]
  const temp = blocks[currentIndex]
  blocks[currentIndex] = blocks[currentIndex - 1]
  blocks[currentIndex - 1] = temp

  store.invitation.blocks = blocks
}

const moveBlockDown = (blockId, currentIndex) => {
  if (currentIndex === store.invitation.blocks.length - 1) return

  const blocks = [...store.invitation.blocks]
  const temp = blocks[currentIndex]
  blocks[currentIndex] = blocks[currentIndex + 1]
  blocks[currentIndex + 1] = temp

  store.invitation.blocks = blocks
}

const onDragEnd = () => {
  // Blocks are already reordered by draggable v-model
  console.log('Blocks reordered')
}
</script>
