<template>
  <div class="rich-text-display" v-html="content"></div>
</template>

<script setup>
defineProps({
  content: {
    type: String,
    required: true
  }
})
</script>

<style scoped>
.rich-text-display {
  line-height: 1.6;
  color: #333;
}

:deep(p) {
  margin: 0 0 1em 0;
}

:deep(p:last-child) {
  margin-bottom: 0;
}

:deep(strong) {
  font-weight: bold;
}

:deep(em) {
  font-style: italic;
}

:deep(s) {
  text-decoration: line-through;
}

:deep(ul),
:deep(ol) {
  padding-left: 1.5em;
  margin: 0 0 1em 0;
}

:deep(ul) {
  list-style-type: disc;
}

:deep(ol) {
  list-style-type: decimal;
}

:deep(li) {
  margin: 0.25em 0;
}

:deep(blockquote) {
  padding-left: 1em;
  border-left: 3px solid #ddd;
  margin: 0 0 1em 0;
  color: #666;
  font-style: italic;
}

:deep(pre) {
  background-color: #f5f5f5;
  border: 1px solid #ddd;
  border-radius: 4px;
  padding: 0.75em 1em;
  margin: 0 0 1em 0;
  overflow-x: auto;
}

:deep(code) {
  background-color: #f5f5f5;
  padding: 0.2em 0.4em;
  border-radius: 3px;
  font-family: monospace;
  font-size: 0.9em;
}

:deep(pre code) {
  background: none;
  padding: 0;
}

:deep(hr) {
  border: none;
  border-top: 1px solid #ddd;
  margin: 1.5em 0;
}
</style>