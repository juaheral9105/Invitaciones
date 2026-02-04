<template>
  <div :style="{ height: `${block.content.height || 50}px` }" class="bg-transparent"></div>
</template>

<script setup>
defineProps({
  block: Object,
  previewMode: Boolean
})
</script>
