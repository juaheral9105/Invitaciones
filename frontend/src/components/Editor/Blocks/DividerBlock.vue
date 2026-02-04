<template>
  <div class="py-4">
    <hr
      :style="{
        borderColor: block.content.color,
        borderStyle: block.content.style,
        width: block.content.width || '100%'
      }"
      class="border-t-2"
    />
  </div>
</template>

<script setup>
defineProps({
  block: Object,
  previewMode: Boolean
})
</script>
