<template>
  <div>
    <div v-if="!block.content.url" class="bg-gray-100 border-2 border-dashed border-gray-300 rounded-lg p-8 text-center">
      <div class="text-4xl mb-2">🖼️</div>
      <p class="text-gray-500 text-sm">Selecciona este bloque y sube una imagen</p>
    </div>
    <div v-else>
      <img
        :src="getImageUrl(block.content.url)"
        :alt="block.content.alt"
        class="rounded-lg shadow-lg"
        :style="{
          width: block.content.width || 'auto',
          height: block.content.height || 'auto',
          maxWidth: '100%',
          objectFit: 'contain',
          display: 'block',
          margin: '0 auto'
        }"
      />
      <p v-if="block.content.caption" class="mt-2 text-sm italic text-gray-600">
        {{ block.content.caption }}
      </p>
    </div>
  </div>
</template>

<script setup>
const props = defineProps({
  block: Object,
  previewMode: Boolean
})

const API_BASE_URL = import.meta.env.VITE_API_URL?.replace('/api', '') || 'http://localhost:5000'

const getImageUrl = (url) => {
  if (!url) return ''
  if (url.startsWith('http')) return url
  // Handle old format URLs from local development (/uploads/images/...)
  // These should not work in production since Railway uses ephemeral storage
  if (url.startsWith('/uploads/')) {
    console.warn('Old image URL format detected:', url, '- Images uploaded to local storage will not work in production')
    return `${API_BASE_URL}${url}`
  }
  return `${API_BASE_URL}${url}`
}
</script>
