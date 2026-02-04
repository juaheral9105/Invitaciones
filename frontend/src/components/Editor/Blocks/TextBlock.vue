<template>
  <DecoratedBlock
    :decoration-enabled="block.content.decorationEnabled"
    :decoration-image="block.content.decorationImage"
    :decoration-position="block.content.decorationPosition"
    :decoration-width="block.content.decorationWidth"
    :decoration-opacity="block.content.decorationOpacity"
  >
    <div
      class="min-h-[60px] cursor-text"
      :contenteditable="!previewMode"
      @blur="updateContent"
      @input="onInput"
      v-html="block.content.text"
    ></div>
  </DecoratedBlock>
</template>

<script setup>
import { ref } from 'vue'
import DecoratedBlock from './DecoratedBlock.vue'
import { useInvitationStore } from '../../../stores/invitationStore'

const props = defineProps({
  block: {
    type: Object,
    required: true
  },
  previewMode: {
    type: Boolean,
    default: false
  }
})

const store = useInvitationStore()
const localText = ref(props.block.content.text)

const onInput = (e) => {
  localText.value = e.target.innerHTML
}

const updateContent = (e) => {
  store.updateBlockContent(props.block.id, {
    text: e.target.innerHTML
  })
}
</script>
