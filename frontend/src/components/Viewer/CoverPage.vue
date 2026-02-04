<template>
  <div
    class="absolute inset-0 flex items-center justify-center overflow-hidden transition-opacity duration-1000"
    :class="{ 'opacity-0 pointer-events-none': !visible }"
    :style="{
      backgroundColor: backgroundColor,
      backgroundImage: backgroundImage ? `url(${backgroundImage})` : 'none',
      backgroundSize: 'cover',
      backgroundPosition: 'center',
      width: '100%',
      height: '100%'
    }"
  >
    <!-- Overlay opcional para mejorar legibilidad -->
    <div class="absolute inset-0 bg-black opacity-20"></div>

    <!-- Contenido de la portada -->
    <div class="relative z-10 text-center px-6 animate-fade-in">
      <!-- Nombre de la homenajeada -->
      <h1
        class="font-bold mb-8 animate-slide-up"
        :style="{
          fontFamily: fontFamily || 'Playfair Display',
          color: textColor || '#ffffff',
          fontSize: nameFontSize || '6rem'
        }"
      >
        {{ coverName || 'Nombre' }}
      </h1>

      <!-- Línea decorativa -->
      <div class="w-32 h-1 mx-auto mb-8 bg-white opacity-80"></div>

      <!-- Botón de entrada -->
      <button
        @click="$emit('enter')"
        class="px-10 py-4 font-semibold rounded-full shadow-2xl transform transition-all duration-300 hover:scale-110 hover:shadow-3xl animate-bounce-slow"
        :style="{
          backgroundColor: buttonColor || '#ffffff',
          color: buttonTextColor || '#000000',
          fontFamily: fontFamily || 'Montserrat',
          fontSize: buttonFontSize || '1.125rem'
        }"
      >
        {{ buttonText || 'Ver Invitación' }}
      </button>

      <!-- Indicador de scroll opcional -->
      <div class="mt-12 animate-bounce">
        <svg class="w-6 h-6 mx-auto opacity-70" fill="none" stroke="currentColor" :style="{ color: textColor || '#ffffff' }" viewBox="0 0 24 24">
          <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M19 14l-7 7m0 0l-7-7m7 7V3"></path>
        </svg>
      </div>
    </div>
  </div>
</template>

<script setup>
import { ref, onMounted } from 'vue'

const props = defineProps({
  visible: {
    type: Boolean,
    default: true
  },
  coverName: {
    type: String,
    default: 'Nombre'
  },
  buttonText: {
    type: String,
    default: 'Ver Invitación'
  },
  backgroundColor: {
    type: String,
    default: '#f3e5f5'
  },
  backgroundImage: {
    type: String,
    default: null
  },
  textColor: {
    type: String,
    default: '#ffffff'
  },
  buttonColor: {
    type: String,
    default: '#ffffff'
  },
  buttonTextColor: {
    type: String,
    default: '#000000'
  },
  fontFamily: {
    type: String,
    default: 'Playfair Display'
  },
  nameFontSize: {
    type: String,
    default: '6rem'
  },
  buttonFontSize: {
    type: String,
    default: '1.125rem'
  }
})

defineEmits(['enter'])
</script>

<style scoped>
@keyframes fadeIn {
  from {
    opacity: 0;
  }
  to {
    opacity: 1;
  }
}

@keyframes slideUp {
  from {
    opacity: 0;
    transform: translateY(30px);
  }
  to {
    opacity: 1;
    transform: translateY(0);
  }
}

@keyframes bounceSlow {
  0%, 100% {
    transform: translateY(0);
  }
  50% {
    transform: translateY(-10px);
  }
}

.animate-fade-in {
  animation: fadeIn 1.5s ease-out;
}

.animate-slide-up {
  animation: slideUp 1s ease-out 0.3s both;
}

.animate-bounce-slow {
  animation: bounceSlow 3s ease-in-out infinite;
}

.shadow-3xl {
  box-shadow: 0 25px 50px -12px rgba(0, 0, 0, 0.5);
}
</style>
