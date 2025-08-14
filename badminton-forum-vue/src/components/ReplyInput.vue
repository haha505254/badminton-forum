<template>
  <div class="reply-input">
    <!-- 回覆對象提示 -->
    <div v-if="parentAuthor" class="replying-to">
      <span class="text-sm text-gray-600 dark:text-gray-400">
        回覆 <strong>@{{ parentAuthor }}</strong>
      </span>
    </div>
    
    <!-- 輸入區域 -->
    <div class="input-wrapper">
      <!-- 文字編輯器（始終顯示） -->
      <div class="input-content">
        <RichTextEditor 
          v-model="content" 
          :placeholder="placeholder"
          ref="editorRef"
          class="mini-editor"
        />
      </div>
      
      <!-- 戰術圖切換按鈕 -->
      <div class="input-toolbar">
        <button
          type="button"
          @click="toggleDiagramMode"
          class="toolbar-btn"
          :class="{ active: showDiagram }"
          :title="showDiagram ? '隱藏戰術圖' : '添加戰術圖'"
        >
          🏸 {{ showDiagram ? '隱藏戰術圖' : '添加戰術圖' }}
        </button>
        <button
          v-if="props.parentDiagramData && !hasDiagram"
          type="button"
          @click="loadParentDiagram"
          class="toolbar-btn"
          title="引用父回覆的戰術圖"
        >
          📋 引用戰術圖
        </button>
        <span v-if="hasDiagram" class="diagram-status">
          ✓ 已添加戰術圖
        </span>
      </div>
      
      <!-- 戰術圖編輯器（可選顯示） -->
      <div v-if="showDiagram" class="diagram-section">
        <div class="diagram-header">
          <span class="diagram-title">編輯戰術圖</span>
          <button 
            @click="clearDiagram"
            class="clear-btn"
            title="清除戰術圖"
          >
            清除
          </button>
        </div>
        <div class="diagram-container">
          <BadmintonCourtDiagram
            v-model="diagramData"
            class="mini-diagram"
          />
          <div class="diagram-hint">
            捲動查看完整戰術圖
          </div>
        </div>
      </div>
    </div>
    
    <!-- 操作按鈕 -->
    <div class="input-actions">
      <button 
        @click="$emit('cancel')"
        class="btn-cancel"
      >
        取消
      </button>
      <button 
        @click="submitReply"
        :disabled="!canSubmit"
        class="btn-submit"
      >
        <span v-if="submitting">發送中...</span>
        <span v-else>發送回覆</span>
      </button>
    </div>
  </div>
</template>

<script setup>
import { ref, computed, watch, nextTick, onMounted } from 'vue'
import { useAuthStore } from '../stores/auth'
import { repliesApi } from '../api/replies'
import RichTextEditor from './RichTextEditor.vue'
import BadmintonCourtDiagram from './BadmintonCourtDiagram.vue'

const props = defineProps({
  postId: {
    type: [Number, String],
    required: true
  },
  parentReplyId: {
    type: Number,
    default: null
  },
  parentAuthor: {
    type: String,
    default: null
  },
  parentDiagramData: {
    type: Object,
    default: null
  }
})

const emit = defineEmits(['submitted', 'cancel'])

const authStore = useAuthStore()

// 狀態
const content = ref('')
const showDiagram = ref(false)
const submitting = ref(false)
const editorRef = ref(null)

// 戰術圖資料 - 預設為空白，使用者可選擇是否載入父回覆戰術圖
const diagramData = ref({
  players: [],
  shuttle: null,
  arrows: [],
  textAnnotations: [],
  description: ''
})

// 計算屬性
const placeholder = computed(() => {
  return props.parentAuthor 
    ? `回覆 @${props.parentAuthor}...` 
    : '寫下您的回覆...'
})

const canSubmit = computed(() => {
  const hasTextContent = content.value && content.value.replace(/<[^>]*>/g, '').trim().length > 0
  const hasValidDiagram = hasDiagram.value
  return (hasTextContent || hasValidDiagram) && !submitting.value && authStore.isAuthenticated
})

// 檢查是否有有效的戰術圖資料
const hasDiagram = computed(() => {
  return diagramData.value && (
    diagramData.value.players?.length > 0 ||
    diagramData.value.shuttle ||
    diagramData.value.arrows?.length > 0 ||
    diagramData.value.textAnnotations?.length > 0
  )
})

// 切換戰術圖模式
const toggleDiagramMode = () => {
  showDiagram.value = !showDiagram.value
}

// 清除戰術圖
const clearDiagram = () => {
  diagramData.value = {
    players: [],
    shuttle: null,
    arrows: [],
    textAnnotations: [],
    description: ''
  }
  showDiagram.value = false
}

// 載入父回覆的戰術圖
const loadParentDiagram = () => {
  if (props.parentDiagramData) {
    diagramData.value = { ...props.parentDiagramData }
    // 修改描述以表示這是回應
    diagramData.value.description = props.parentDiagramData.description 
      ? `回應：${props.parentDiagramData.description}` 
      : '回應戰術圖'
    showDiagram.value = true
  }
}

// 提交回覆
const submitReply = async () => {
  if (!canSubmit.value) return
  
  submitting.value = true
  try {
    // 組合最終內容
    let finalContent = content.value || ''
    
    // 如果有戰術圖，添加到內容中
    if (hasDiagram.value) {
      const diagramHtml = `
        <div class="badminton-diagram-placeholder" data-diagram='${JSON.stringify(diagramData.value)}'>
          <p>[羽球戰術圖: ${diagramData.value.description || '戰術示意圖'}]</p>
        </div>
      `
      // 如果有文字內容，在後面添加戰術圖；否則只有戰術圖
      finalContent = finalContent ? `${finalContent}\n${diagramHtml}` : diagramHtml
    }
    
    const replyData = {
      content: finalContent,
      parentReplyId: props.parentReplyId
    }
    
    const response = await repliesApi.createReply(props.postId, replyData)
    
    // 清空表單
    content.value = ''
    showDiagram.value = false
    diagramData.value = {
      players: [],
      shuttle: null,
      arrows: [],
      textAnnotations: [],
      description: ''
    }
    
    // 通知父元件
    emit('submitted', response.data)
  } catch (error) {
    console.error('Failed to submit reply:', error)
    alert('發送回覆失敗，請重試')
  } finally {
    submitting.value = false
  }
}

// 自動聚焦編輯器
onMounted(() => {
  // 不再自動顯示戰術圖編輯器，讓使用者自行選擇
  nextTick(() => {
    if (editorRef.value?.editor) {
      editorRef.value.editor.commands.focus()
    }
  })
})
</script>

<style scoped>
.reply-input {
  background: white;
  border: 1px solid #e5e7eb;
  border-radius: 0.5rem;
  padding: 0.75rem;
}

:root.dark .reply-input {
  background: #1f2937;
  border-color: #374151;
}

.replying-to {
  margin-bottom: 0.5rem;
  padding-bottom: 0.5rem;
  border-bottom: 1px solid #f3f4f6;
}

:root.dark .replying-to {
  border-color: #374151;
}

.input-wrapper {
  margin-bottom: 0.5rem;
}

.input-toolbar {
  display: flex;
  align-items: center;
  gap: 0.75rem;
  margin-top: 0.5rem;
  padding-top: 0.5rem;
  border-top: 1px solid #e5e7eb;
}

:root.dark .input-toolbar {
  border-color: #374151;
}

.toolbar-btn {
  padding: 0.25rem 0.5rem;
  background: #f9fafb;
  border: 1px solid #e5e7eb;
  border-radius: 0.375rem;
  font-size: 1rem;
  cursor: pointer;
  transition: all 0.2s;
}

:root.dark .toolbar-btn {
  background: #374151;
  border-color: #4b5563;
}

.toolbar-btn:hover {
  background: #f3f4f6;
}

:root.dark .toolbar-btn:hover {
  background: #4b5563;
}

.toolbar-btn.active {
  background: #10b981;
  color: white;
  border-color: #10b981;
}

.input-content {
  min-height: 80px;
}

/* 戰術圖狀態指示 */
.diagram-status {
  font-size: 0.875rem;
  color: #10b981;
  display: flex;
  align-items: center;
  gap: 0.25rem;
}

:root.dark .diagram-status {
  color: #34d399;
}

/* 戰術圖區塊 */
.diagram-section {
  margin-top: 0.75rem;
  padding: 0.75rem;
  background: #f9fafb;
  border: 1px solid #e5e7eb;
  border-radius: 0.375rem;
}

:root.dark .diagram-section {
  background: #111827;
  border-color: #374151;
}

.diagram-header {
  display: flex;
  justify-content: space-between;
  align-items: center;
  margin-bottom: 0.5rem;
}

.diagram-title {
  font-weight: 500;
  color: #374151;
  font-size: 0.875rem;
}

:root.dark .diagram-title {
  color: #d1d5db;
}

.clear-btn {
  padding: 0.25rem 0.5rem;
  background: white;
  color: #ef4444;
  border: 1px solid #fca5a5;
  border-radius: 0.25rem;
  font-size: 0.75rem;
  cursor: pointer;
  transition: all 0.2s;
}

:root.dark .clear-btn {
  background: #7f1d1d;
  color: #fca5a5;
  border-color: #991b1b;
}

.clear-btn:hover {
  background: #fee2e2;
}

:root.dark .clear-btn:hover {
  background: #991b1b;
}

/* 戰術圖容器 */
.diagram-container {
  position: relative;
}

/* 提示文字 */
.diagram-hint {
  position: absolute;
  top: 8px;
  right: 8px;
  font-size: 0.75rem;
  color: #6b7280;
  background: rgba(255, 255, 255, 0.95);
  padding: 4px 8px;
  border-radius: 4px;
  box-shadow: 0 1px 3px rgba(0, 0, 0, 0.1);
  pointer-events: none;
  z-index: 10;
}

:root.dark .diagram-hint {
  background: rgba(31, 41, 55, 0.95);
  color: #9ca3af;
}

/* 精簡版編輯器樣式 */
.mini-editor :deep(.rich-text-editor) {
  border: 1px solid #e5e7eb;
  border-radius: 0.375rem;
}

:root.dark .mini-editor :deep(.rich-text-editor) {
  border-color: #4b5563;
}

.mini-editor :deep(.editor-toolbar) {
  padding: 0.375rem;
  gap: 0.125rem;
}

.mini-editor :deep(.toolbar-button) {
  padding: 0.25rem 0.5rem;
  font-size: 0.75rem;
}

.mini-editor :deep(.editor-content) {
  min-height: 60px;
  max-height: 200px;
}

.mini-editor :deep(.ProseMirror) {
  padding: 0.5rem;
  min-height: 60px;
  font-size: 0.875rem;
}

/* 精簡版戰術圖 */
.mini-diagram {
  height: 400px;  /* 增加高度以適應戰術圖 */
  border: 1px solid #e5e7eb;
  border-radius: 0.375rem;
  overflow: auto;  /* 允許滾動 */
  position: relative;
  background: white;
}

:root.dark .mini-diagram {
  border-color: #4b5563;
  background: #1f2937;
}

.input-actions {
  display: flex;
  justify-content: flex-end;
  gap: 0.5rem;
}

.btn-cancel,
.btn-submit {
  padding: 0.375rem 0.75rem;
  border-radius: 0.375rem;
  font-size: 0.875rem;
  font-weight: 500;
  transition: all 0.2s;
}

.btn-cancel {
  background: white;
  color: #6b7280;
  border: 1px solid #e5e7eb;
}

:root.dark .btn-cancel {
  background: #374151;
  color: #d1d5db;
  border-color: #4b5563;
}

.btn-cancel:hover {
  background: #f9fafb;
}

:root.dark .btn-cancel:hover {
  background: #4b5563;
}

.btn-submit {
  background: #2563eb;
  color: white;
  border: 1px solid #2563eb;
}

.btn-submit:hover:not(:disabled) {
  background: #1d4ed8;
}

.btn-submit:disabled {
  opacity: 0.5;
  cursor: not-allowed;
}
</style>