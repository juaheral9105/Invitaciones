<template>
  <div class="bg-white bg-opacity-95 rounded-xl shadow-lg p-8 max-w-2xl mx-auto" style="position: relative; overflow: hidden;">
    <!-- Decoration background image -->
    <div
      v-if="block.content.decorationEnabled && block.content.decorationImage"
      :style="{
        position: 'absolute',
        top: 0,
        bottom: 0,
        [block.content.decorationPosition]: 0,
        width: block.content.decorationWidth || '50%',
        backgroundImage: `url(${block.content.decorationImage})`,
        backgroundSize: 'contain',
        backgroundRepeat: 'no-repeat',
        backgroundPosition: block.content.decorationPosition === 'left' ? 'left center' : block.content.decorationPosition === 'right' ? 'right center' : 'center center',
        opacity: block.content.decorationOpacity || 0.3,
        pointerEvents: 'none',
        zIndex: 0
      }"
    ></div>

    <!-- Content wrapper with relative positioning -->
    <div style="position: relative; z-index: 1;">
      <!-- Título principal -->
      <h2 class="text-3xl font-bold mb-6 text-center">
        {{ block.content.title || 'Confirmar asistencia' }}
      </h2>

      <!-- Texto descriptivo editable -->
      <div class="text-center mb-8 leading-relaxed max-w-xl mx-auto">
        <p v-if="block.content.description" class="mb-3 whitespace-pre-line">
          {{ block.content.description }}
        </p>
        <p v-if="block.content.subtitle" class="font-medium whitespace-pre-line">
          {{ block.content.subtitle }}
        </p>
      </div>

      <!-- Formulario con campos completamente configurables -->
      <form @submit.prevent="handleSubmit" class="max-w-md mx-auto">
      <!-- Campo de teléfono (solo visible en preview mode) -->
      <div
        v-if="previewMode"
        :style="{
          width: block.content.phoneFieldWidth || '100%',
          marginTop: block.content.phoneFieldMarginTop || '0px',
          marginBottom: block.content.phoneFieldMarginBottom || '24px',
          marginLeft: 'auto',
          marginRight: 'auto'
        }"
      >
        <label class="block font-medium mb-2 text-sm w-full">
          Ingresa tu número de celular*
        </label>
        <input
          type="tel"
          v-model="formData.phone"
          @blur="validatePhone"
          placeholder="Ej: 3001234567"
          required
          class="w-full px-3 py-2 text-sm border border-gray-300 rounded-lg focus:outline-none focus:ring-2 focus:ring-purple-500"
          :class="{ 'border-red-500': phoneError }"
        />

        <!-- Mensaje de validación del teléfono -->
        <div v-if="isValidatingPhone" class="mt-1 text-xs text-blue-600">
          Validando teléfono...
        </div>
        <div v-if="phoneError" class="mt-1 text-xs text-red-600">
          {{ phoneError }}
        </div>
        <div v-if="phoneValidated && guestData" class="mt-1 text-xs text-green-600">
          ✓ {{ formattedGreetingMessage }}
        </div>
        <div v-if="phoneValidated && !guestData" class="mt-1 text-xs text-red-600">
          ❌ Este número no se encuentra en la lista de invitados
        </div>
      </div>

      <!-- Campos dinámicos (incluye radio buttons, texto, select) -->
      <div
        v-for="(field, index) in block.content.customFields"
        :key="index"
        v-show="previewMode ? (shouldShowAdditionalFields && evaluateFieldVisibility(field)) : true"
        :style="{
          width: field.width || '100%',
          marginTop: field.marginTop || '0px',
          marginBottom: field.marginBottom || '16px',
          marginLeft: 'auto',
          marginRight: 'auto'
        }"
      >
        <!-- Campo tipo Radio Buttons -->
        <div v-if="field.type === 'radio'">
            <label class="block font-semibold mb-3 text-center text-sm">
              {{ field.label }}{{ field.required ? '*' : '' }}
            </label>
            <div class="flex flex-col items-center space-y-2">
              <label
                v-for="(option, optIndex) in field.options"
                :key="optIndex"
                class="flex items-center space-x-2 cursor-pointer"
              >
                <input
                  type="radio"
                  :name="'radio-group-' + index"
                  v-model="formData.customFields[index]"
                  :value="option"
                  :required="field.required"
                  class="w-4 h-4 text-purple-600"
                />
                <span>{{ option }}</span>
              </label>
            </div>
          </div>

          <!-- Campo tipo Texto -->
          <div v-else-if="field.type === 'text'">
            <label class="block font-medium mb-2 text-sm">
              {{ field.label }}{{ field.required ? '*' : '' }}
            </label>
            <input
              type="text"
              v-model="formData.customFields[index]"
              :placeholder="field.placeholder || 'Escribir aquí...'"
              :required="field.required"
              class="w-full px-3 py-2 text-sm border border-gray-300 rounded-lg focus:outline-none focus:ring-2 focus:ring-purple-500"
            />
          </div>

          <!-- Campo tipo Lista (Select) -->
          <div v-else-if="field.type === 'select'">
            <label class="block font-medium mb-2 text-sm">
              {{ field.label }}{{ field.required ? '*' : '' }}
            </label>
            <select
              v-model="formData.customFields[index]"
              :required="field.required && field.visible"
              class="w-full px-3 py-2 text-sm border border-gray-300 rounded-lg focus:outline-none focus:ring-2 focus:ring-purple-500"
            >
              <option value="">{{ field.placeholder || 'Seleccionar opción...' }}</option>
              <option v-for="(option, optIndex) in field.options" :key="optIndex" :value="option">
                {{ option }}
              </option>
            </select>
          </div>
      </div>

      <!-- Botón de envío -->
      <div v-if="shouldShowSubmitButton" class="flex justify-center mt-8">
        <button
          type="submit"
          :style="{
            backgroundColor: block.content.buttonColor || '#9333ea',
            color: block.content.buttonTextColor || '#ffffff',
            fontSize: block.content.buttonFontSize || '1rem',
            padding: '12px 32px',
            borderRadius: '9999px',
            fontWeight: 'bold',
            textTransform: 'uppercase',
            letterSpacing: '0.05em',
            transition: 'all 0.3s ease',
            border: 'none',
            cursor: 'pointer',
            boxShadow: '0 4px 6px rgba(0, 0, 0, 0.1)'
          }"
          class="hover:scale-105 hover:shadow-lg"
        >
          {{ block.content.buttonText || 'Responder Invitación' }}
        </button>
      </div>
    </form>
    </div>
  </div>
</template>

<script setup>
import { ref, watch, computed } from 'vue'
import { guestListService, confirmationService } from '../../../services/api'
import { useInvitationStore } from '../../../stores/invitationStore'

const props = defineProps({
  block: Object,
  previewMode: Boolean,
  invitationId: String,
  formEmail: String
})

const store = useInvitationStore()

const formData = ref({
  phone: '',
  customFields: []
})

const guestData = ref(null)
const phoneValidated = ref(false)
const phoneError = ref('')
const isValidatingPhone = ref(false)

// Inicializar customFields array cuando cambien los campos
watch(() => props.block.content.customFields, (newFields) => {
  if (newFields && newFields.length > formData.value.customFields.length) {
    formData.value.customFields = new Array(newFields.length).fill('')
  }
}, { immediate: true })

// Debug: Monitorear cambios en la decoración
watch(() => props.block.content.decorationImage, (newImage) => {
  console.log('Decoration image changed in FormBlock:', {
    enabled: props.block.content.decorationEnabled,
    image: newImage,
    position: props.block.content.decorationPosition,
    width: props.block.content.decorationWidth,
    opacity: props.block.content.decorationOpacity
  })
}, { immediate: true })

// Validar teléfono y buscar invitado
const validatePhone = async () => {
  const phone = formData.value.phone.trim()

  if (!phone) {
    phoneError.value = ''
    phoneValidated.value = false
    guestData.value = null
    return
  }

  // Si no hay invitationId, permitir envío sin validación
  const invitationId = props.invitationId || store.invitation.id
  if (!invitationId) {
    phoneValidated.value = true
    guestData.value = null
    return
  }

  isValidatingPhone.value = true
  phoneError.value = ''

  try {
    const response = await guestListService.getGuestByPhone(invitationId, phone)
    guestData.value = response.data
    console.log('Guest data received from API:', guestData.value)
    console.log('Guest names field:', guestData.value.guestNames)
    console.log('Type of guestNames:', typeof guestData.value.guestNames)
    phoneValidated.value = true
    phoneError.value = ''

    // Pre-rellenar campos con datos del invitado
    prefillGuestData()
  } catch (error) {
    if (error.response?.status === 404) {
      // Invitado no encontrado en la lista, permitir continuar
      guestData.value = null
      phoneValidated.value = true
      phoneError.value = ''
    } else {
      phoneError.value = 'Error al validar el teléfono'
      phoneValidated.value = false
    }
  } finally {
    isValidatingPhone.value = false
  }
}

// Pre-rellenar campos con datos del invitado
const prefillGuestData = () => {
  if (!guestData.value) return

  props.block.content.customFields.forEach((field, index) => {
    // Pre-rellenar campos según su label o tipo
    const label = field.label.toLowerCase()

    if (label.includes('nombre') && !label.includes('acompañante')) {
      formData.value.customFields[index] = guestData.value.name
    } else if (label.includes('acompañante') || label.includes('invitado')) {
      formData.value.customFields[index] = guestData.value.guestNames || ''
    } else if (label.includes('número') && label.includes('invitado')) {
      formData.value.customFields[index] = guestData.value.numberOfGuests?.toString() || ''
    }
  })
}

// Evaluar si un campo debe mostrarse según las reglas de visibilidad
const evaluateFieldVisibility = (field) => {
  // Si las reglas no están habilitadas, siempre mostrar
  if (!field.visibilityRules?.enabled) return true

  // Si no hay datos del invitado, mostrar todos los campos
  if (!guestData.value) return true

  const { condition, showWhen, value } = field.visibilityRules

  // Si no hay condición configurada, mostrar
  if (!condition) return true

  let fieldValue

  // Obtener el valor del campo según la condición
  switch(condition) {
    case 'accommodationAllowed':
      fieldValue = guestData.value.accommodationAllowed
      break
    case 'eventHasAccommodation':
      fieldValue = guestData.value.eventHasAccommodation
      break
    case 'numberOfGuests':
      fieldValue = guestData.value.numberOfGuests
      break
    case 'hasGuestNames':
      fieldValue = !!guestData.value.guestNames && guestData.value.guestNames.trim() !== ''
      break
    default:
      return true
  }


  // Debug logging
  console.log('Evaluando visibilidad:', {
    fieldLabel: field.label,
    condition,
    showWhen,
    configuredValue: value,
    configuredValueType: typeof value,
    actualFieldValue: fieldValue,
    actualFieldValueType: typeof fieldValue,
    guestData: guestData.value
  })

  // Aplicar el operador de comparación
  let result
  switch(showWhen) {
    case 'equals':
      // Normalizar valores booleanos para comparación
      if (typeof fieldValue === 'boolean') {
        result = fieldValue === (value === 'true' || value === true)
      } else {
        result = String(fieldValue) === String(value)
      }
      break
    case 'notEquals':
      if (typeof fieldValue === 'boolean') {
        result = fieldValue !== (value === 'true' || value === true)
      } else {
        result = String(fieldValue) !== String(value)
      }
      break
    case 'greaterThan':
      result = Number(fieldValue) > Number(value)
      break
    case 'lessThan':
      result = Number(fieldValue) < Number(value)
      break
    default:
      result = true
  }

  console.log('Resultado de visibilidad:', result)
  return result
}

// Calcular si mostrar campos adicionales (después de validación de teléfono)
const shouldShowAdditionalFields = computed(() => {
  // Solo mostrar si el teléfono fue validado Y el invitado fue encontrado en el Excel
  return phoneValidated.value && guestData.value !== null
})

// Formatear el mensaje de saludo con los datos del invitado
const formattedGreetingMessage = computed(() => {
  if (!guestData.value) return ''

  const template = props.block.content.greetingMessage || '¡Hola {name}! Esta invitación es para ti y {guests}'

  // Debug: ver qué valor tiene guestNames
  console.log('Formatting greeting - guestNames value:', guestData.value.guestNames)
  console.log('Formatting greeting - full guestData:', guestData.value)

  return template
    .replace('{name}', guestData.value.name || '')
    .replace('{guests}', (guestData.value.guestNames && guestData.value.guestNames.trim() !== '')
      ? guestData.value.guestNames
      : '')
})

// Calcular si mostrar el botón de envío
const shouldShowSubmitButton = computed(() => {
  // En modo edición, siempre mostrar

console.log('prueba 1')

  if (!props.previewMode) return true

console.log('prueba 2')

  // En modo preview, requiere teléfono validado Y que el invitado esté en el Excel
  if (!phoneValidated.value) return false

console.log('prueba 3')

  if (!guestData.value) return false // NUEVO: Solo si está en el Excel

console.log('prueba 4')

// Verificar que TODOS los campos requeridos y visibles estén llenos
const hasRequiredFieldFilled = props.block.content.customFields.every((field, index) => {
  // Si el campo NO es requerido, no afecta la validación
  console.log(field.visibilityRules?.enabled)

  //if (!field.visibilityRules?.enabled){
    if (!field.required) return true

    // Si el campo no es visible, no se valida
    if (!evaluateFieldVisibility(field)) return true

    const value = formData.value.customFields[index]

    return value && String(value).trim() !== ''
  //}
})

    console.log('prueba 7')
  // Si no hay campos requeridos, mostrar el botón
  const hasRequiredFields = props.block.content.customFields.some(field => field.required )
    console.log(hasRequiredFields)
console.log('prueba 8')
  if (!hasRequiredFields) return true

  console.log(hasRequiredFieldFilled)
  return hasRequiredFieldFilled


})

const handleSubmit = async () => {
  try {
    // Verificar que hay un email configurado
    const emailTo = props.formEmail || store.invitation.formEmail
    if (!emailTo) {
      alert('Error: No hay un correo electrónico configurado para recibir confirmaciones')
      return
    }

    // Construir el objeto de datos del formulario
    const formDataToSend = {}

    // PRIMERO: Agregar datos del Excel (guestData) si existen
    if (guestData.value?.name) {
      formDataToSend['Nombre del invitado'] = guestData.value.name
    }

    if (guestData.value?.guestNames) {
      formDataToSend['Nombres de acompañantes'] = guestData.value.guestNames
    }

    // SEGUNDO: Agregar campos personalizados del formulario con sus labels
    props.block.content.customFields.forEach((field, index) => {
      const value = formData.value.customFields[index]
      if (value) {
        formDataToSend[field.label] = value
      }
    })

    // Enviar email
    await confirmationService.sendEmail({
      toEmail: emailTo,
      phone: formData.value.phone,
      formData: formDataToSend
    })

    alert('¡Gracias por confirmar tu asistencia! Se ha enviado la confirmación.')

    // Limpiar el formulario
    formData.value.phone = ''
    formData.value.customFields = new Array(props.block.content.customFields.length).fill('')
    phoneValidated.value = false
    guestData.value = null
  } catch (error) {
    console.error('Error sending confirmation:', error)
    alert('Error al enviar la confirmación. Por favor, intenta de nuevo.')
  }
}
</script>
