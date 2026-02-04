<template>
  <div v-if="musicUrl" class="fixed top-4 right-4 z-50">
    <div class="bg-white rounded-lg shadow-xl p-3 flex items-center gap-3 border-2 border-purple-200">
      <!-- Play/Pause Button -->
      <button
        @click="togglePlay"
        class="w-10 h-10 rounded-full bg-purple-600 text-white flex items-center justify-center hover:bg-purple-700 transition-colors"
      >
        <span v-if="!isPlaying" class="text-lg">▶️</span>
        <span v-else class="text-lg">⏸️</span>
      </button>

      <!-- Song Info -->
      <div class="flex items-center gap-2">
        <span class="text-sm font-medium text-gray-700">🎵 Música de fondo</span>

        <!-- Animated bars when playing -->
        <div v-if="isPlaying" class="flex items-center gap-0.5">
          <div class="w-0.5 h-3 bg-purple-600 animate-pulse" style="animation-delay: 0ms"></div>
          <div class="w-0.5 h-4 bg-purple-600 animate-pulse" style="animation-delay: 150ms"></div>
          <div class="w-0.5 h-2 bg-purple-600 animate-pulse" style="animation-delay: 300ms"></div>
          <div class="w-0.5 h-5 bg-purple-600 animate-pulse" style="animation-delay: 450ms"></div>
        </div>
      </div>

      <!-- Volume Control -->
      <input
        v-model="volume"
        @input="updateVolume"
        type="range"
        min="0"
        max="100"
        class="w-20 h-1"
      />
      <span class="text-xs text-gray-500">{{ volume }}%</span>

      <!-- Remove Button -->
      <button
        @click="removeMusic"
        class="p-1 hover:bg-red-50 rounded text-red-600"
        title="Quitar música"
      >
        ✕
      </button>

      <!-- Hidden Audio Element -->
      <audio ref="audioPlayer" :src="musicUrl" loop></audio>
    </div>
  </div>
</template>

<script setup>
import { ref, watch, onMounted } from 'vue'

const props = defineProps({
  musicUrl: {
    type: String,
    default: null
  }
})

const emit = defineEmits(['remove'])

const audioPlayer = ref(null)
const isPlaying = ref(false)
const volume = ref(50)

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

const updateVolume = () => {
  if (audioPlayer.value) {
    audioPlayer.value.volume = volume.value / 100
  }
}

const removeMusic = () => {
  if (audioPlayer.value) {
    audioPlayer.value.pause()
    isPlaying.value = false
  }
  emit('remove')
}

watch(() => props.musicUrl, (newUrl) => {
  if (audioPlayer.value) {
    audioPlayer.value.pause()
    isPlaying.value = false
  }
})

onMounted(() => {
  if (audioPlayer.value) {
    audioPlayer.value.volume = volume.value / 100
  }
})
</script>
