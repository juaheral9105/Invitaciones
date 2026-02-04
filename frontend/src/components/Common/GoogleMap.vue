<template>
  <div class="max-w-4xl mx-auto mb-12 animate-fade-in">
    <div class="bg-white bg-opacity-95 rounded-xl shadow-2xl p-6">
      <h2 class="text-3xl font-bold mb-6 text-center text-purple-900">📍 Ubicación</h2>

      <div id="map" class="w-full h-96 rounded-lg"></div>

      <div class="mt-4 text-center">
        <a
          :href="`https://www.google.com/maps/search/?api=1&query=${lat},${lng}`"
          target="_blank"
          rel="noopener noreferrer"
          class="inline-block px-6 py-3 bg-purple-600 text-white rounded-lg hover:bg-purple-700 transition-colors font-semibold"
        >
          Abrir en Google Maps
        </a>
      </div>
    </div>
  </div>
</template>

<script setup>
import { onMounted, watch } from 'vue'

const props = defineProps({
  lat: {
    type: Number,
    required: true
  },
  lng: {
    type: Number,
    required: true
  },
  title: {
    type: String,
    default: 'Ubicación'
  }
})

let map = null
let marker = null

const loadGoogleMapsScript = () => {
  return new Promise((resolve, reject) => {
    if (window.google && window.google.maps) {
      resolve()
      return
    }

    const apiKey = import.meta.env.VITE_GOOGLE_MAPS_API_KEY
    if (!apiKey) {
      console.warn('Google Maps API key not found')
      reject(new Error('API key not configured'))
      return
    }

    const script = document.createElement('script')
    script.src = `https://maps.googleapis.com/maps/api/js?key=${apiKey}`
    script.async = true
    script.defer = true
    script.onload = () => resolve()
    script.onerror = () => reject(new Error('Failed to load Google Maps'))
    document.head.appendChild(script)
  })
}

const initMap = () => {
  if (!window.google || !window.google.maps) {
    console.error('Google Maps not loaded')
    return
  }

  const mapElement = document.getElementById('map')
  if (!mapElement) return

  const position = { lat: props.lat, lng: props.lng }

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
    title: props.title,
    animation: window.google.maps.Animation.DROP
  })

  // Info window
  const infoWindow = new window.google.maps.InfoWindow({
    content: `<div style="padding: 8px;"><strong>${props.title}</strong></div>`
  })

  marker.addListener('click', () => {
    infoWindow.open(map, marker)
  })
}

onMounted(async () => {
  try {
    await loadGoogleMapsScript()
    initMap()
  } catch (error) {
    console.error('Error loading Google Maps:', error)
    // Show fallback
    const mapElement = document.getElementById('map')
    if (mapElement) {
      mapElement.innerHTML = `
        <div class="flex items-center justify-center h-full bg-gray-100 rounded-lg">
          <p class="text-gray-600">No se pudo cargar el mapa.
            <a href="https://www.google.com/maps/search/?api=1&query=${props.lat},${props.lng}"
               target="_blank"
               class="text-purple-600 hover:underline">
              Ver en Google Maps
            </a>
          </p>
        </div>
      `
    }
  }
})

watch(() => [props.lat, props.lng], () => {
  if (map && marker) {
    const newPosition = { lat: props.lat, lng: props.lng }
    map.setCenter(newPosition)
    marker.setPosition(newPosition)
  }
})
</script>

<style scoped>
@keyframes fadeIn {
  from {
    opacity: 0;
    transform: translateY(20px);
  }
  to {
    opacity: 1;
    transform: translateY(0);
  }
}

.animate-fade-in {
  animation: fadeIn 1s ease-out;
}
</style>
