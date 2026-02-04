<template>
  <div class="max-w-2xl mx-auto mb-12 animate-fade-in">
    <div class="bg-white bg-opacity-95 rounded-xl shadow-2xl p-8">
      <h2 class="text-3xl font-bold mb-6 text-center text-purple-900">💌 Confirmar Asistencia</h2>

      <form @submit.prevent="submitForm" class="space-y-4">
        <div>
          <label class="block text-sm font-semibold text-gray-700 mb-2">
            Nombre completo <span class="text-red-500">*</span>
          </label>
          <input
            v-model="formData.name"
            type="text"
            required
            class="w-full px-4 py-3 border border-gray-300 rounded-lg focus:ring-2 focus:ring-purple-500 focus:border-transparent"
            placeholder="Tu nombre completo"
          />
        </div>

        <div>
          <label class="block text-sm font-semibold text-gray-700 mb-2">
            Email
          </label>
          <input
            v-model="formData.email"
            type="email"
            class="w-full px-4 py-3 border border-gray-300 rounded-lg focus:ring-2 focus:ring-purple-500 focus:border-transparent"
            placeholder="tu@email.com (opcional)"
          />
        </div>

        <div>
          <label class="block text-sm font-semibold text-gray-700 mb-2">
            Teléfono
          </label>
          <input
            v-model="formData.phone"
            type="tel"
            class="w-full px-4 py-3 border border-gray-300 rounded-lg focus:ring-2 focus:ring-purple-500 focus:border-transparent"
            placeholder="Tu teléfono (opcional)"
          />
        </div>

        <div>
          <label class="block text-sm font-semibold text-gray-700 mb-2">
            ¿Asistirás? <span class="text-red-500">*</span>
          </label>
          <select
            v-model="formData.willAttend"
            required
            class="w-full px-4 py-3 border border-gray-300 rounded-lg focus:ring-2 focus:ring-purple-500 focus:border-transparent"
          >
            <option value="">Selecciona una opción</option>
            <option :value="true">✅ Sí, asistiré</option>
            <option :value="false">❌ No podré asistir</option>
          </select>
        </div>

        <div v-if="formData.willAttend === true">
          <label class="block text-sm font-semibold text-gray-700 mb-2">
            Número de invitados <span class="text-red-500">*</span>
          </label>
          <input
            v-model.number="formData.numberOfGuests"
            type="number"
            min="1"
            max="10"
            required
            class="w-full px-4 py-3 border border-gray-300 rounded-lg focus:ring-2 focus:ring-purple-500 focus:border-transparent"
          />
          <p class="text-xs text-gray-500 mt-1">Incluyéndote a ti</p>
        </div>

        <div>
          <label class="block text-sm font-semibold text-gray-700 mb-2">
            Mensaje
          </label>
          <textarea
            v-model="formData.message"
            rows="3"
            class="w-full px-4 py-3 border border-gray-300 rounded-lg focus:ring-2 focus:ring-purple-500 focus:border-transparent resize-none"
            placeholder="Deja un mensaje especial (opcional)"
          ></textarea>
        </div>

        <button
          type="submit"
          :disabled="submitting"
          class="w-full py-4 bg-purple-600 text-white rounded-lg font-bold text-lg hover:bg-purple-700 transition-colors disabled:opacity-50 disabled:cursor-not-allowed"
        >
          {{ submitting ? 'Enviando...' : 'Enviar Confirmación' }}
        </button>
      </form>

      <p class="text-center text-sm text-gray-500 mt-4">
        Tus datos son confidenciales y solo serán usados para confirmar tu asistencia
      </p>
    </div>
  </div>
</template>

<script setup>
import { ref, reactive } from 'vue'

defineProps({
  formEmail: {
    type: String,
    default: ''
  }
})

const emit = defineEmits(['submit'])

const submitting = ref(false)
const formData = reactive({
  name: '',
  email: '',
  phone: '',
  willAttend: '',
  numberOfGuests: 1,
  message: ''
})

const submitForm = async () => {
  submitting.value = true
  try {
    // Convert willAttend to boolean if it's string
    const data = {
      ...formData,
      willAttend: formData.willAttend === true || formData.willAttend === 'true'
    }

    emit('submit', data)

    // Reset form
    formData.name = ''
    formData.email = ''
    formData.phone = ''
    formData.willAttend = ''
    formData.numberOfGuests = 1
    formData.message = ''
  } catch (error) {
    console.error('Error submitting form:', error)
  } finally {
    submitting.value = false
  }
}
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
