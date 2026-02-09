import { defineStore } from 'pinia'
import { ref } from 'vue'

export const useInvitationStore = defineStore('invitation', () => {
  const invitation = ref({
    id: null,
    title: '',
    backgroundColor: '#f8f9fa',
    backgroundImage: null,
    musicUrl: null,
    formEmail: '',
    blocks: [], // Array of draggable blocks
    // Cover page configuration
    coverEnabled: true,
    coverName: 'Nombre de la Homenajeada',
    coverButtonText: 'Ver Invitación',
    coverBackgroundColor: '#f3e5f5',
    coverBackgroundImage: null,
    coverTextColor: '#ffffff',
    coverButtonColor: '#ffffff',
    coverButtonTextColor: '#000000',
    coverFontFamily: 'Playfair Display',
    coverNameFontSize: '6rem',
    coverButtonFontSize: '1.125rem'
  })

  const selectedBlockId = ref(null)

  const setInvitation = (data) => {
    invitation.value = { ...invitation.value, ...data }
  }

  const resetInvitation = () => {
    invitation.value = {
      id: null,
      title: '',
      backgroundColor: '#f8f9fa',
      backgroundImage: null,
      musicUrl: null,
      formEmail: '',
      blocks: [],
      coverEnabled: true,
      coverName: 'Nombre de la Homenajeada',
      coverButtonText: 'Ver Invitación',
      coverBackgroundColor: '#f3e5f5',
      coverBackgroundImage: null,
      coverTextColor: '#ffffff',
      coverButtonColor: '#ffffff',
      coverButtonTextColor: '#000000',
      coverFontFamily: 'Playfair Display',
      coverNameFontSize: '6rem',
      coverButtonFontSize: '1.125rem'
    }
    selectedBlockId.value = null
  }

  // Block management
  const addBlock = (blockType) => {
    const newBlock = {
      id: Date.now().toString(),
      type: blockType,
      position: { x: 50, y: invitation.value.blocks.length * 150 + 100 },
      size: { width: 600, height: 'auto' },
      style: {
        backgroundColor: 'transparent',
        color: '#000000',
        fontFamily: 'Montserrat',
        fontSize: '16px',
        textAlign: 'center',
        padding: '20px',
        borderRadius: '8px'
      },
      content: getDefaultContent(blockType)
    }
    invitation.value.blocks.push(newBlock)
    selectedBlockId.value = newBlock.id
  }

  const getDefaultContent = (type) => {
    // Propiedades de decoración comunes
    const decorationDefaults = {
      decorationEnabled: false,
      decorationImage: '',
      decorationPosition: 'right', // 'left', 'center', 'right'
      decorationWidth: '50%',
      decorationOpacity: 0.3
    }

    const defaults = {
      text: { text: 'Haz clic para editar', title: '', ...decorationDefaults },
      heading: { text: 'Título Principal', level: 'h1', ...decorationDefaults },
      image: { url: '', alt: 'Imagen', caption: '', ...decorationDefaults },
      carousel: { images: [], autoplay: true, interval: 3000, slidesPerView: 3, ...decorationDefaults },
      countdown: { targetDate: null, title: 'Faltan', ...decorationDefaults },
      map: { lat: null, lng: null, title: 'Ubicación', ...decorationDefaults },
      eventInfo: {
        date: '',
        time: '',
        venue: '',
        address: '',
        showIcons: true,
        ...decorationDefaults
      },
      button: { text: 'Click aquí', url: '', style: 'primary', ...decorationDefaults },
      divider: { style: 'solid', color: '#e5e7eb', width: '100%' },
      spacer: { height: 50 },
      form: {
        enabled: true,
        title: 'Confirmar asistencia',
        description: 'Con gran ilusión, quiero invitarte a acompañarme en uno de los días más felices de mi vida. Nada me hará más feliz que celebrar este día tan especial rodeada de quienes quiero.',
        subtitle: 'Agradezco la confirmación de asistencia a la celebración.',
        // Configuración del campo de teléfono
        phoneFieldWidth: '100%',
        phoneFieldMarginTop: '0px',
        phoneFieldMarginBottom: '24px',
        // Mensaje de saludo personalizable
        greetingMessage: '¡Hola {name}! Esta invitación es para ti y {guests}',
        // Configuración del botón
        buttonText: 'Responder Invitación',
        buttonColor: '#9333ea',
        buttonTextColor: '#ffffff',
        buttonFontSize: '1rem',
        // Decoración de fondo
        ...decorationDefaults,
        customFields: []
      }
    }
    return defaults[type] || {}
  }

  const updateBlock = (blockId, updates) => {
    const index = invitation.value.blocks.findIndex(b => b.id === blockId)
    if (index !== -1) {
      invitation.value.blocks[index] = {
        ...invitation.value.blocks[index],
        ...updates
      }
    }
  }

  const updateBlockContent = (blockId, content) => {
    const index = invitation.value.blocks.findIndex(b => b.id === blockId)
    if (index !== -1) {
      invitation.value.blocks[index].content = {
        ...invitation.value.blocks[index].content,
        ...content
      }
    }
  }

  const updateBlockStyle = (blockId, style) => {
    const index = invitation.value.blocks.findIndex(b => b.id === blockId)
    if (index !== -1) {
      invitation.value.blocks[index].style = {
        ...invitation.value.blocks[index].style,
        ...style
      }
    }
  }

  const updateBlockPosition = (blockId, position) => {
    const index = invitation.value.blocks.findIndex(b => b.id === blockId)
    if (index !== -1) {
      invitation.value.blocks[index].position = position
    }
  }

  const deleteBlock = (blockId) => {
    invitation.value.blocks = invitation.value.blocks.filter(b => b.id !== blockId)
    if (selectedBlockId.value === blockId) {
      selectedBlockId.value = null
    }
  }

  const duplicateBlock = (blockId) => {
    const block = invitation.value.blocks.find(b => b.id === blockId)
    if (block) {
      const newBlock = {
        ...JSON.parse(JSON.stringify(block)),
        id: Date.now().toString(),
        position: { x: block.position.x + 20, y: block.position.y + 20 }
      }
      invitation.value.blocks.push(newBlock)
      selectedBlockId.value = newBlock.id
    }
  }

  const moveBlockUp = (blockId) => {
    const index = invitation.value.blocks.findIndex(b => b.id === blockId)
    if (index > 0) {
      const temp = invitation.value.blocks[index]
      invitation.value.blocks[index] = invitation.value.blocks[index - 1]
      invitation.value.blocks[index - 1] = temp
    }
  }

  const moveBlockDown = (blockId) => {
    const index = invitation.value.blocks.findIndex(b => b.id === blockId)
    if (index < invitation.value.blocks.length - 1) {
      const temp = invitation.value.blocks[index]
      invitation.value.blocks[index] = invitation.value.blocks[index + 1]
      invitation.value.blocks[index + 1] = temp
    }
  }

  const selectBlock = (blockId) => {
    selectedBlockId.value = blockId
  }

  const deselectBlock = () => {
    selectedBlockId.value = null
  }

  return {
    invitation,
    selectedBlockId,
    setInvitation,
    resetInvitation,
    addBlock,
    updateBlock,
    updateBlockContent,
    updateBlockStyle,
    updateBlockPosition,
    deleteBlock,
    duplicateBlock,
    moveBlockUp,
    moveBlockDown,
    selectBlock,
    deselectBlock
  }
})
