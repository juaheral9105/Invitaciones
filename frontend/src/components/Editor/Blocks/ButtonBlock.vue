<template>
  <div>
    <a
      :href="block.content.url || '#'"
      :target="block.content.url ? '_blank' : '_self'"
      class="inline-block px-8 py-3 rounded-lg font-semibold transition-colors"
      :class="{
        'bg-purple-600 text-white hover:bg-purple-700': block.content.style === 'primary',
        'bg-white text-purple-600 border-2 border-purple-600 hover:bg-purple-50': block.content.style === 'outline',
        'bg-gray-200 text-gray-800 hover:bg-gray-300': block.content.style === 'secondary'
      }"
    >
      {{ block.content.text }}
    </a>
  </div>
</template>

<script setup>
defineProps({
  block: Object,
  previewMode: Boolean
})
</script>
