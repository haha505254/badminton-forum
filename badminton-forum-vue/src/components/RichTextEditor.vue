<template>
  <div class="rich-text-editor">
    <div class="editor-toolbar" v-if="editor">
      <button
        @click="editor.chain().focus().toggleBold().run()"
        :class="{ 'is-active': editor.isActive('bold') }"
        class="toolbar-button"
        title="粗體 (Ctrl+B)"
      >
        <strong>B</strong>
      </button>
      
      <button
        @click="editor.chain().focus().toggleItalic().run()"
        :class="{ 'is-active': editor.isActive('italic') }"
        class="toolbar-button"
        title="斜體 (Ctrl+I)"
      >
        <em>I</em>
      </button>
      
      <button
        @click="editor.chain().focus().toggleStrike().run()"
        :class="{ 'is-active': editor.isActive('strike') }"
        class="toolbar-button"
        title="刪除線"
      >
        <s>S</s>
      </button>
      
      <div class="toolbar-separator"></div>
      
      <button
        @click="editor.chain().focus().toggleBulletList().run()"
        :class="{ 'is-active': editor.isActive('bulletList') }"
        class="toolbar-button"
        title="項目符號"
      >
        • 列表
      </button>
      
      <button
        @click="editor.chain().focus().toggleOrderedList().run()"
        :class="{ 'is-active': editor.isActive('orderedList') }"
        class="toolbar-button"
        title="編號列表"
      >
        1. 列表
      </button>
      
      <div class="toolbar-separator"></div>
      
      <button
        @click="editor.chain().focus().toggleBlockquote().run()"
        :class="{ 'is-active': editor.isActive('blockquote') }"
        class="toolbar-button"
        title="引用"
      >
        " 引用
      </button>
      
      <button
        @click="editor.chain().focus().toggleCodeBlock().run()"
        :class="{ 'is-active': editor.isActive('codeBlock') }"
        class="toolbar-button"
        title="程式碼區塊"
      >
        &lt;/&gt; 程式碼
      </button>
      
      <div class="toolbar-separator"></div>
      
      <button
        @click="editor.chain().focus().setHorizontalRule().run()"
        class="toolbar-button"
        title="水平線"
      >
        — 分隔線
      </button>
      
      <button
        @click="editor.chain().focus().undo().run()"
        :disabled="!editor.can().undo()"
        class="toolbar-button"
        title="復原 (Ctrl+Z)"
      >
        ↶ 復原
      </button>
      
      <button
        @click="editor.chain().focus().redo().run()"
        :disabled="!editor.can().redo()"
        class="toolbar-button"
        title="重做 (Ctrl+Y)"
      >
        ↷ 重做
      </button>
    </div>
    
    <div class="editor-content">
      <editor-content :editor="editor" />
    </div>
  </div>
</template>

<script setup>
import { useEditor, EditorContent } from '@tiptap/vue-3'
import StarterKit from '@tiptap/starter-kit'
import { watch, onBeforeUnmount } from 'vue'

const props = defineProps({
  modelValue: {
    type: String,
    default: ''
  },
  placeholder: {
    type: String,
    default: '開始輸入...'
  }
})

const emit = defineEmits(['update:modelValue'])

const editor = useEditor({
  extensions: [
    StarterKit.configure({
      heading: false // 關閉標題功能
    })
  ],
  content: props.modelValue,
  editorProps: {
    attributes: {
      class: 'prose prose-sm focus:outline-none',
      placeholder: props.placeholder
    }
  },
  onUpdate: ({ editor }) => {
    emit('update:modelValue', editor.getHTML())
  }
})

// 監聽外部值的改變
watch(() => props.modelValue, (value) => {
  if (editor.value && value !== editor.value.getHTML()) {
    editor.value.commands.setContent(value, false)
  }
})

onBeforeUnmount(() => {
  editor.value?.destroy()
})
</script>

<style scoped>
.rich-text-editor {
  border: 1px solid #ddd;
  border-radius: 8px;
  overflow: hidden;
}

.editor-toolbar {
  display: flex;
  flex-wrap: wrap;
  gap: 4px;
  padding: 8px;
  background-color: #f8f8f8;
  border-bottom: 1px solid #ddd;
}

.toolbar-button {
  padding: 6px 12px;
  background: white;
  border: 1px solid #ddd;
  border-radius: 4px;
  cursor: pointer;
  font-size: 14px;
  transition: all 0.2s;
}

.toolbar-button:hover:not(:disabled) {
  background-color: #f0f0f0;
}

.toolbar-button:disabled {
  opacity: 0.5;
  cursor: not-allowed;
}

.toolbar-button.is-active {
  background-color: #4CAF50;
  color: white;
  border-color: #4CAF50;
}

.toolbar-separator {
  width: 1px;
  background-color: #ddd;
  margin: 0 4px;
}

.editor-content {
  min-height: 200px;
  max-height: 500px;
  overflow-y: auto;
}

:deep(.ProseMirror) {
  padding: 16px;
  min-height: 200px;
}

:deep(.ProseMirror:focus) {
  outline: none;
}

:deep(.ProseMirror p) {
  margin: 0 0 1em 0;
}

:deep(.ProseMirror p:last-child) {
  margin-bottom: 0;
}

:deep(.ProseMirror ul, .ProseMirror ol) {
  padding-left: 1.5em;
  margin: 0 0 1em 0;
}

:deep(.ProseMirror ul) {
  list-style-type: disc;
}

:deep(.ProseMirror ol) {
  list-style-type: decimal;
}

:deep(.ProseMirror li) {
  margin: 0.25em 0;
}

:deep(.ProseMirror blockquote) {
  padding-left: 1em;
  border-left: 3px solid #ddd;
  margin: 0 0 1em 0;
  color: #666;
}

:deep(.ProseMirror pre) {
  background-color: #f5f5f5;
  border: 1px solid #ddd;
  border-radius: 4px;
  padding: 0.75em 1em;
  margin: 0 0 1em 0;
  overflow-x: auto;
}

:deep(.ProseMirror code) {
  background-color: #f5f5f5;
  padding: 0.2em 0.4em;
  border-radius: 3px;
  font-family: monospace;
  font-size: 0.9em;
}

:deep(.ProseMirror pre code) {
  background: none;
  padding: 0;
}

:deep(.ProseMirror hr) {
  border: none;
  border-top: 1px solid #ddd;
  margin: 1.5em 0;
}

:deep(.ProseMirror-placeholder::before) {
  content: attr(data-placeholder);
  color: #aaa;
  pointer-events: none;
  position: absolute;
}
</style>