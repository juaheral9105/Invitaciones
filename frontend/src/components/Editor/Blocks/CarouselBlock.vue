<template>
  <DecoratedBlock
    :decoration-enabled="block.content.decorationEnabled"
    :decoration-image="block.content.decorationImage"
    :decoration-position="block.content.decorationPosition"
    :decoration-width="block.content.decorationWidth"
    :decoration-opacity="block.content.decorationOpacity"
  >
    <div class="carousel-container">
      <div v-if="!block.content.images || block.content.images.length === 0" class="bg-gray-100 border-2 border-dashed border-gray-300 rounded-lg p-8 text-center">
      <div class="text-4xl mb-2">🎠</div>
      <p class="text-gray-500 text-sm">Selecciona este bloque para agregar imágenes al carrusel</p>
    </div>
    <div v-else class="relative">
      <swiper
        :modules="modules"
        :slides-per-view="getSlidesPerView()"
        :space-between="20"
        :loop="block.content.images.length > getSlidesPerView()"
        :autoplay="block.content.autoplay ? { delay: block.content.interval || 3000 } : false"
        :pagination="{ clickable: true }"
        :navigation="true"
        :breakpoints="{
          320: { slidesPerView: 1, spaceBetween: 10 },
          640: { slidesPerView: Math.min(2, getSlidesPerView()), spaceBetween: 15 },
          1024: { slidesPerView: getSlidesPerView(), spaceBetween: 20 }
        }"
        class="rounded-lg shadow-lg"
      >
        <swiper-slide v-for="(image, index) in block.content.images" :key="index" class="flex items-center justify-center bg-gray-100" style="height: 400px;">
          <img :src="getImageUrl(image)" class="max-w-full max-h-full object-contain" style="height: 100%; width: auto;" />
        </swiper-slide>
      </swiper>
    </div>
    </div>
  </DecoratedBlock>
</template>

<script setup>
import DecoratedBlock from './DecoratedBlock.vue'
import { Swiper, SwiperSlide } from 'swiper/vue'
import { Autoplay, Pagination, Navigation } from 'swiper/modules'
import 'swiper/css'
import 'swiper/css/pagination'
import 'swiper/css/navigation'

const props = defineProps({
  block: Object,
  previewMode: Boolean
})

const modules = [Autoplay, Pagination, Navigation]

const API_BASE_URL = import.meta.env.VITE_API_URL?.replace('/api', '') || 'http://localhost:5000'

const getImageUrl = (url) => {
  if (!url) return ''
  if (url.startsWith('http')) return url
  return `${API_BASE_URL}${url}`
}

const getSlidesPerView = () => {
  // Usar slidesPerView configurado o por defecto mostrar 3 fotos
  const configured = props.block.content.slidesPerView || 3
  const totalImages = props.block.content.images?.length || 0

  // Si hay menos imágenes que el configurado, mostrar solo las disponibles
  return Math.min(configured, totalImages)
}
</script>

<style scoped>
.carousel-container {
  width: 100%;
}
</style>
