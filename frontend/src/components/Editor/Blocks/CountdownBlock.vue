<template>
  <div class="bg-white bg-opacity-90 rounded-lg p-6 shadow-lg">
    <h3 v-if="block.content.title" class="text-xl font-semibold mb-4">{{ block.content.title }}</h3>
    <div class="flex justify-center gap-4">
      <div v-for="(value, key) in timeLeft" :key="key" class="text-center">
        <div class="text-4xl font-bold">{{ value }}</div>
        <div class="text-sm text-gray-600">{{ labels[key] }}</div>
      </div>
    </div>
  </div>
</template>

<script setup>
import { ref, onMounted, onUnmounted, computed } from 'vue'

const props = defineProps({
  block: Object,
  previewMode: Boolean
})

const timeLeft = ref({ days: 0, hours: 0, minutes: 0, seconds: 0 })
const labels = { days: 'Días', hours: 'Horas', minutes: 'Minutos', seconds: 'Segundos' }

let interval = null

const calculateTimeLeft = () => {
  if (!props.block.content.targetDate) {
    timeLeft.value = { days: 15, hours: 10, minutes: 30, seconds: 45 }
    return
  }

  const difference = +new Date(props.block.content.targetDate) - +new Date()

  if (difference > 0) {
    timeLeft.value = {
      days: Math.floor(difference / (1000 * 60 * 60 * 24)),
      hours: Math.floor((difference / (1000 * 60 * 60)) % 24),
      minutes: Math.floor((difference / 1000 / 60) % 60),
      seconds: Math.floor((difference / 1000) % 60)
    }
  }
}

onMounted(() => {
  calculateTimeLeft()
  interval = setInterval(calculateTimeLeft, 1000)
})

onUnmounted(() => {
  if (interval) clearInterval(interval)
})
</script>
