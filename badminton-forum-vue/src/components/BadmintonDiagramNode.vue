<template>
  <node-view-wrapper class="badminton-diagram-node">
    <div class="diagram-preview">
      <div class="diagram-label">
        🏸 羽球戰術圖: {{ description }}
      </div>
      <div class="diagram-actions" v-if="editor.isEditable">
        <button type="button" @click="editDiagram" class="btn-edit">編輯</button>
        <button type="button" @click="deleteDiagram" class="btn-delete">刪除</button>
      </div>
    </div>
  </node-view-wrapper>
</template>

<script setup>
import { NodeViewWrapper } from '@tiptap/vue-3'
import { computed } from 'vue'

const props = defineProps({
  node: Object,
  editor: Object,
  deleteNode: Function,
  updateAttributes: Function,
  getPos: Function
})

const description = computed(() => {
  return props.node.attrs.data?.description || '戰術示意圖'
})

const editDiagram = () => {
  // 觸發編輯事件，傳遞 getPos 函數而非固定位置
  props.editor.commands.focus()
  props.editor.emit('edit-diagram', {
    getPos: props.getPos,
    data: props.node.attrs.data
  })
}

const deleteDiagram = () => {
  props.deleteNode()
}
</script>

<style scoped>
.badminton-diagram-node {
  margin: 1em 0;
}

.diagram-preview {
  border: 2px dashed #4CAF50;
  border-radius: 8px;
  padding: 12px;
  background-color: #f0f9ff;
  display: flex;
  justify-content: space-between;
  align-items: center;
}

.diagram-label {
  font-weight: 500;
  color: #2563eb;
}

.diagram-actions {
  display: flex;
  gap: 8px;
}

.btn-edit,
.btn-delete {
  padding: 4px 12px;
  border-radius: 4px;
  border: 1px solid #ddd;
  background: white;
  cursor: pointer;
  font-size: 14px;
  transition: all 0.2s;
}

.btn-edit:hover {
  background-color: #4CAF50;
  color: white;
  border-color: #4CAF50;
}

.btn-delete:hover {
  background-color: #f44336;
  color: white;
  border-color: #f44336;
}

/* 深色模式支援 */
:root.dark .diagram-preview {
  background-color: #1e293b;
  border-color: #10b981;
}

:root.dark .diagram-label {
  color: #60a5fa;
}

:root.dark .btn-edit,
:root.dark .btn-delete {
  background: #374151;
  border-color: #4b5563;
  color: #e5e7eb;
}
</style>