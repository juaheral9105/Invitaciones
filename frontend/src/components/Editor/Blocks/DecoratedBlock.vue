<template>
  <div :style="containerStyle">
    <!-- Decoration background image -->
    <div
      v-if="decorationEnabled && decorationImage"
      :style="decorationStyle"
    ></div>

    <!-- Content wrapper with relative positioning -->
    <div style="position: relative; z-index: 1;">
      <slot></slot>
    </div>
  </div>
</template>

<script setup>
import { computed } from 'vue'

const props = defineProps({
  decorationEnabled: Boolean,
  decorationImage: String,
  decorationPosition: {
    type: String,
    default: 'right'
  },
  decorationWidth: {
    type: String,
    default: '50%'
  },
  decorationOpacity: {
    type: Number,
    default: 0.3
  },
  containerClass: {
    type: String,
    default: ''
  }
})

const containerStyle = computed(() => ({
  position: 'relative',
  overflow: 'hidden'
}))

const decorationStyle = computed(() => {
  const backgroundPosition =
    props.decorationPosition === 'left' ? 'left center' :
    props.decorationPosition === 'right' ? 'right center' :
    'center center'

  return {
    position: 'absolute',
    top: 0,
    bottom: 0,
    [props.decorationPosition]: 0,
    width: props.decorationWidth,
    backgroundImage: `url(${props.decorationImage})`,
    backgroundSize: 'contain',
    backgroundRepeat: 'no-repeat',
    backgroundPosition: backgroundPosition,
    opacity: props.decorationOpacity,
    pointerEvents: 'none',
    zIndex: 0
  }
})
</script>
