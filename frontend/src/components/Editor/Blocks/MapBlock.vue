<template>
  <DecoratedBlock
    :decoration-enabled="block.content.decorationEnabled"
    :decoration-image="block.content.decorationImage"
    :decoration-position="block.content.decorationPosition"
    :decoration-width="block.content.decorationWidth"
    :decoration-opacity="block.content.decorationOpacity"
  >
    <div class="bg-white bg-opacity-90 rounded-lg p-4 shadow-lg">
      <h3 v-if="block.content.title" class="text-xl font-semibold mb-3 text-center">{{ block.content.title }}</h3>
      <div v-if="!block.content.lat || !block.content.lng" class="bg-gray-100 border-2 border-dashed border-gray-300 rounded-lg p-8 text-center">
        <div class="text-4xl mb-2">🗺️</div>
        <p class="text-gray-500 text-sm">Configura las coordenadas del mapa en el panel de propiedades</p>
      </div>
      <div v-else :id="'map-' + block.id" class="w-full h-64 rounded-lg"></div>
    </div>
  </DecoratedBlock>
</template>

<script setup>
import { onMounted, watch } from 'vue'
import DecoratedBlock from './DecoratedBlock.vue'

const props = defineProps({
  block: Object,
  previewMode: Boolean
})

let map = null
let marker = null

const loadGoogleMapsScript = () => {
  return new Promise((resolve, reject) => {
    if (window.google && window.google.maps) {
      resolve()
      return
    }

    // Sin API key, usar enlace directo a Google Maps
    console.warn('Google Maps API key not found - showing fallback')
    reject(new Error('API key not configured'))
  })
}

const initMap = () => {
  if (!window.google || !window.google.maps) {
    showFallback()
    return
  }

  const mapElement = document.getElementById('map-' + props.block.id)
  if (!mapElement) return

  const position = { lat: props.block.content.lat, lng: props.block.content.lng }

  map = new window.google.maps.Map(mapElement, {
    center: position,
    zoom: 15,
    mapTypeControl: true,
    streetViewControl: true,
    fullscreenControl: true
  })

  marker = new window.google.maps.Marker({
    position: position,
    map: map,
    title: props.block.content.title || 'Ubicación',
    animation: window.google.maps.Animation.DROP
  })
}

const showFallback = () => {
  const mapElement = document.getElementById('map-' + props.block.id)
  if (mapElement) {
    const lat = props.block.content.lat
    const lng = props.block.content.lng
    mapElement.innerHTML = `
      <div class="flex flex-col items-center justify-center h-full bg-gray-100 rounded-lg p-4">
        <div class="text-4xl mb-2">🗺️</div>
        <p class="text-gray-600 mb-3 text-center">Vista previa del mapa</p>
        <a href="https://www.google.com/maps/search/?api=1&query=${lat},${lng}"
           target="_blank"
           class="px-4 py-2 bg-purple-600 text-white rounded-lg hover:bg-purple-700 transition-colors text-sm">
          Ver en Google Maps
        </a>
      </div>
    `
  }
}

onMounted(async () => {
  if (!props.block.content.lat || !props.block.content.lng) return

  try {
    await loadGoogleMapsScript()
    initMap()
  } catch (error) {
    showFallback()
  }
})

watch(() => [props.block.content.lat, props.block.content.lng], () => {
  if (map && marker && props.block.content.lat && props.block.content.lng) {
    const newPosition = { lat: props.block.content.lat, lng: props.block.content.lng }
    map.setCenter(newPosition)
    marker.setPosition(newPosition)
  } else if (props.block.content.lat && props.block.content.lng) {
    showFallback()
  }
})
</script>
