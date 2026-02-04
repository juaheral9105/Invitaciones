<template>
  <DecoratedBlock
    :decoration-enabled="block.content.decorationEnabled"
    :decoration-image="block.content.decorationImage"
    :decoration-position="block.content.decorationPosition"
    :decoration-width="block.content.decorationWidth"
    :decoration-opacity="block.content.decorationOpacity"
  >
    <div class="bg-white bg-opacity-90 rounded-lg p-6 shadow-lg">
      <h3 class="text-2xl font-bold mb-4 text-center">Detalles del Evento</h3>
      <div class="space-y-3">
        <div v-if="block.content.date" class="flex items-center gap-3">
          <span class="text-2xl">📅</span>
          <span><strong>Fecha:</strong> {{ block.content.date }}</span>
        </div>
        <div v-if="block.content.time" class="flex items-center gap-3">
          <span class="text-2xl">🕐</span>
          <span><strong>Hora:</strong> {{ block.content.time }}</span>
        </div>
        <div v-if="block.content.venue" class="flex items-center gap-3">
          <span class="text-2xl">📍</span>
          <span><strong>Lugar:</strong> {{ block.content.venue }}</span>
        </div>
        <div v-if="block.content.address" class="flex items-center gap-3">
          <span class="text-2xl">🏠</span>
          <span><strong>Dirección:</strong> {{ block.content.address }}</span>
        </div>
      </div>
    </div>
  </DecoratedBlock>
</template>

<script setup>
import DecoratedBlock from './DecoratedBlock.vue'
defineProps({
  block: Object,
  previewMode: Boolean
})
</script>
