<template>
  <div
    class="min-h-screen"
    :style="{
      backgroundColor: invitation.backgroundColor || '#f8f9fa',
      backgroundImage: invitation.backgroundImage ? `url(${getImageUrl(invitation.backgroundImage)})` : 'none',
      backgroundSize: 'cover',
      backgroundPosition: 'center',
      backgroundAttachment: 'fixed'
    }"
  >
    <!-- Music Player (Fixed) -->
    <MusicPlayer v-if="invitation.musicUrl" :musicUrl="getImageUrl(invitation.musicUrl)" :should-play="shouldPlayMusic" />

    <!-- Blocks Container -->
    <div class="max-w-4xl mx-auto">
      <div v-if="blocks && blocks.length > 0" class="space-y-0">
        <div
          v-for="block in blocks"
          :key="block.id"
          :style="getBlockStyles(block)"
        >
          <component
            :is="getBlockComponent(block.type)"
            :block="block"
            :preview-mode="true"
          />
        </div>
      </div>

      <!-- Empty State (should not happen in viewer, but just in case) -->
      <div v-else class="min-h-screen flex items-center justify-center">
        <div class="text-center p-8">
          <div class="text-6xl mb-4">📭</div>
          <h3 class="text-2xl font-semibold text-gray-700 mb-2">Invitación vacía</h3>
          <p class="text-gray-500">Esta invitación no tiene contenido aún.</p>
        </div>
      </div>
    </div>
  </div>
</template>

<script setup>
import { defineProps, defineEmits, computed } from 'vue'
import MusicPlayer from '../Common/MusicPlayer.vue'

// Import Block Components
import TextBlock from '../Editor/Blocks/TextBlock.vue'
import HeadingBlock from '../Editor/Blocks/HeadingBlock.vue'
import ImageBlock from '../Editor/Blocks/ImageBlock.vue'
import CarouselBlock from '../Editor/Blocks/CarouselBlock.vue'
import EventInfoBlock from '../Editor/Blocks/EventInfoBlock.vue'
import CountdownBlock from '../Editor/Blocks/CountdownBlock.vue'
import MapBlock from '../Editor/Blocks/MapBlock.vue'
import ButtonBlock from '../Editor/Blocks/ButtonBlock.vue'
import DividerBlock from '../Editor/Blocks/DividerBlock.vue'
import SpacerBlock from '../Editor/Blocks/SpacerBlock.vue'
import FormBlock from '../Editor/Blocks/FormBlock.vue'

const props = defineProps({
  invitation: {
    type: Object,
    required: true
  },
  shouldPlayMusic: {
    type: Boolean,
    default: false
  }
})

defineEmits(['submit-confirmation'])

const API_BASE_URL = import.meta.env.VITE_API_URL?.replace('/api', '') || 'http://localhost:5000'

// Get blocks from invitation (could be in sections or blocks property)
const blocks = computed(() => {
  return props.invitation.sections || props.invitation.blocks || []
})

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

const getBlockStyles = (block) => {
  if (!block.style) return {}

  return {
    backgroundColor: block.style.backgroundColor || 'transparent',
    color: block.style.color || '#000000',
    fontFamily: block.style.fontFamily || 'Montserrat',
    fontSize: block.style.fontSize || '16px',
    textAlign: block.style.textAlign || 'center',
    padding: block.style.padding || '20px',
    borderRadius: block.style.borderRadius || '8px'
  }
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
