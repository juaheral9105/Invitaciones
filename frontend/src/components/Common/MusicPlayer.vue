<template>
  <div class="bg-white rounded-full shadow-lg p-4 flex items-center gap-3">
    <button
      @click="togglePlay"
      class="w-12 h-12 rounded-full bg-purple-600 text-white flex items-center justify-center hover:bg-purple-700 transition-colors"
    >
      <span v-if="!isPlaying" class="text-2xl">▶️</span>
      <span v-else class="text-2xl">⏸️</span>
    </button>

    <div v-if="isPlaying" class="flex items-center gap-2">
      <span class="text-sm font-medium text-gray-700">🎵</span>
      <div class="flex gap-1">
        <div class="w-1 h-4 bg-purple-600 animate-pulse" style="animation-delay: 0ms"></div>
        <div class="w-1 h-6 bg-purple-600 animate-pulse" style="animation-delay: 150ms"></div>
        <div class="w-1 h-3 bg-purple-600 animate-pulse" style="animation-delay: 300ms"></div>
      </div>
    </div>

    <audio ref="audioPlayer" :src="musicUrl" loop></audio>
  </div>
</template>

<script setup>
import { ref, onMounted, watch } from 'vue'

const props = defineProps({
  musicUrl: {
    type: String,
    required: true
  },
  shouldPlay: {
    type: Boolean,
    default: false
  }
})

const audioPlayer = ref(null)
const isPlaying = ref(false)

const togglePlay = () => {
  if (audioPlayer.value) {
    if (isPlaying.value) {
      audioPlayer.value.pause()
    } else {
      audioPlayer.value.play()
    }
    isPlaying.value = !isPlaying.value
  }
}

const startMusic = async () => {
  if (audioPlayer.value && !isPlaying.value) {
    audioPlayer.value.volume = 0.5
    try {
      await audioPlayer.value.play()
      isPlaying.value = true
    } catch (error) {
      console.log('Autoplay blocked by browser. User interaction required.')
    }
  }
}

// Watch for shouldPlay prop changes
watch(() => props.shouldPlay, (newValue) => {
  if (newValue) {
    startMusic()
  }
})

onMounted(async () => {
  // Auto-play if shouldPlay is true
  if (props.shouldPlay) {
    startMusic()
  }
})
</script>
