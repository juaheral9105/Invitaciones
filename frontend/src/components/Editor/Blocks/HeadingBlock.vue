<template>
  <DecoratedBlock
    :decoration-enabled="block.content.decorationEnabled"
    :decoration-image="block.content.decorationImage"
    :decoration-position="block.content.decorationPosition"
    :decoration-width="block.content.decorationWidth"
    :decoration-opacity="block.content.decorationOpacity"
  >
    <component
      :is="block.content.level || 'h1'"
      class="font-bold cursor-text"
      :class="{
        'text-5xl': block.content.level === 'h1',
        'text-4xl': block.content.level === 'h2',
        'text-3xl': block.content.level === 'h3',
        'text-2xl': block.content.level === 'h4'
      }"
      :contenteditable="!previewMode"
      @blur="updateContent"
      v-html="block.content.text"
    ></component>
  </DecoratedBlock>
</template>

<script setup>
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

const updateContent = (e) => {
  store.updateBlockContent(props.block.id, {
    text: e.target.innerHTML
  })
}
</script>
