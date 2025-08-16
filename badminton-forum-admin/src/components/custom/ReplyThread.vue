<template>
  <div class="reply-thread" :class="`depth-${Math.min(depth, maxDepth)}`">
    <!-- 回覆主體 -->
    <div class="reply-content-wrapper">
      <!-- 左側連接線 -->
      <div v-if="depth > 0" class="reply-connector"></div>

      <!-- 回覆內容 -->
      <div class="reply-content">
        <!-- 回覆頭部 -->
        <div class="reply-header">
          <div class="flex items-center gap-2">
            <!-- 折疊按鈕 -->
            <button v-if="hasChildren" class="collapse-btn" :aria-expanded="!isCollapsed" @click="toggleCollapse">
              <svg
                class="w-4 h-4 transition-transform"
                :class="{ 'rotate-90': !isCollapsed }"
                fill="none"
                stroke="currentColor"
                viewBox="0 0 24 24"
              >
                <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M9 5l7 7-7 7" />
              </svg>
            </button>

            <!-- 使用者頭像 -->
            <div class="user-avatar">
              {{ reply.authorName.charAt(0).toUpperCase() }}
            </div>

            <!-- 使用者名稱 -->
            <span class="font-medium text-gray-900 dark:text-white" :class="{ 'text-gray-500': reply.isDeleted }">
              {{ reply.authorName }}
            </span>

            <!-- 時間 -->
            <span class="text-sm text-gray-500 dark:text-gray-400">
              · {{ formatTime(reply.createdAt) }}
              <span v-if="reply.updatedAt && !reply.isDeleted">（已編輯）</span>
            </span>

            <!-- 如果是回覆某人 -->
            <span v-if="reply.parentReplyId && parentAuthor" class="text-sm text-gray-500 dark:text-gray-400">
              回覆 @{{ parentAuthor }}
            </span>

            <!-- 已刪除標記 -->
            <span v-if="reply.isDeleted" class="text-sm text-red-500 dark:text-red-400"> [已刪除] </span>
          </div>

          <!-- 操作按鈕 -->
          <div class="reply-actions">
            <!-- 回覆按鈕（未刪除的回覆才顯示） -->
            <button v-if="!reply.isDeleted" class="action-btn" title="回覆" @click="toggleReplyForm">
              <svg class="w-4 h-4" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                <path
                  stroke-linecap="round"
                  stroke-linejoin="round"
                  stroke-width="2"
                  d="M3 10h10a8 8 0 018 8v2M3 10l6 6m-6-6l6-6"
                />
              </svg>
              <span class="ml-1">回覆</span>
            </button>

            <!-- 編輯按鈕（作者本人） -->
            <button v-if="isAuthor && !isEditing" class="action-btn" title="編輯" @click="startEdit">
              <svg class="w-4 h-4" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                <path
                  stroke-linecap="round"
                  stroke-linejoin="round"
                  stroke-width="2"
                  d="M11 5H6a2 2 0 00-2 2v11a2 2 0 002 2h11a2 2 0 002-2v-5m-1.414-9.414a2 2 0 112.828 2.828L11.828 15H9v-2.828l8.586-8.586z"
                />
              </svg>
              <span class="ml-1">編輯</span>
            </button>

            <!-- 刪除按鈕（作者本人） -->
            <button
              v-if="isAuthor"
              :disabled="isDeleting"
              class="action-btn text-red-600 hover:bg-red-50 dark:hover:bg-red-900/20"
              title="刪除"
              @click="deleteReply"
            >
              <svg class="w-4 h-4" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                <path
                  stroke-linecap="round"
                  stroke-linejoin="round"
                  stroke-width="2"
                  d="M19 7l-.867 12.142A2 2 0 0116.138 21H7.862a2 2 0 01-1.995-1.858L5 7m5 4v6m4-6v6m1-10V4a1 1 0 00-1-1h-4a1 1 0 00-1 1v3M4 7h16"
                />
              </svg>
              <span class="ml-1">{{ isDeleting ? '刪除中...' : '刪除' }}</span>
            </button>
          </div>
        </div>

        <!-- 回覆內容（可折疊） -->
        <div v-show="!isCollapsed" class="reply-body">
          <!-- 編輯模式 -->
          <div v-if="isEditing" class="edit-mode mt-2">
            <RichTextEditor
              ref="editorRef"
              v-model="editContent"
              placeholder="編輯您的回覆..."
              class="mb-2"
              @editDiagram="handleEditDiagram"
            />

            <!-- 戰術圖編輯器（當編輯戰術圖時顯示） -->
            <div v-if="showDiagramEditor" class="diagram-editor-modal">
              <div class="diagram-editor-header">
                <span>編輯戰術圖</span>
                <button class="close-btn" @click="closeDiagramEditor">✕</button>
              </div>
              <BadmintonCourtDiagram v-model="editingDiagramData" class="diagram-editor" />
              <div class="diagram-editor-actions">
                <button class="btn-primary" @click="saveDiagramEdit">保存戰術圖</button>
                <button class="btn-secondary" @click="closeDiagramEditor">取消</button>
              </div>
            </div>

            <div class="flex gap-2">
              <button :disabled="!editContent.trim()" class="btn-primary text-sm" @click="saveEdit">儲存</button>
              <button class="btn-secondary text-sm" @click="cancelEdit">取消</button>
            </div>
          </div>

          <!-- 一般顯示模式 -->
          <div v-else class="prose prose-sm max-w-none dark:prose-invert mt-2">
            <RichTextDisplay :content="reply.content" display-context="reply" :default-expanded="false" />
          </div>

          <!-- 內嵌回覆表單 -->
          <div v-if="showReplyForm" class="inline-reply-form">
            <ReplyInput
              :post-id="postId"
              :parent-reply-id="reply.id"
              :parent-author="reply.authorName"
              :parent-diagram-data="extractDiagramFromContent(reply.content)"
              @submitted="handleReplySubmitted"
              @cancel="showReplyForm = false"
            />
          </div>

          <!-- 子回覆（遞迴） -->
          <div v-if="hasChildren" class="child-replies">
            <ReplyThread
              v-for="childReply in reply.children"
              :key="childReply.id"
              :reply="childReply"
              :post-id="postId"
              :depth="depth + 1"
              :all-replies="allReplies"
              @replyAdded="$emit('reply-added', $event)"
            />
          </div>
        </div>

        <!-- 折疊時顯示的摘要 -->
        <div v-if="isCollapsed && hasChildren" class="collapsed-summary">
          <span class="text-sm text-gray-500 dark:text-gray-400"> 已折疊 {{ countDescendants(reply) }} 則回覆 </span>
        </div>
      </div>
    </div>
  </div>
</template>

<script setup>
import { ref, computed } from 'vue'
import { useAuthStore } from '../stores/auth'
import { repliesApi } from '../api/replies'
import RichTextDisplay from './RichTextDisplay.vue'
import ReplyInput from './ReplyInput.vue'
import RichTextEditor from './RichTextEditor.vue'
import BadmintonCourtDiagram from './BadmintonCourtDiagram.vue'

const props = defineProps({
  reply: {
    type: Object,
    required: true,
  },
  postId: {
    type: [Number, String],
    required: true,
  },
  depth: {
    type: Number,
    default: 0,
  },
  allReplies: {
    type: Array,
    default: () => [],
  },
})

const emit = defineEmits(['reply-added', 'reply-updated', 'reply-deleted'])

const authStore = useAuthStore()

// 最大顯示深度
const maxDepth = 5

// 狀態
const isCollapsed = ref(false)
const showReplyForm = ref(false)
const isEditing = ref(false)
const editContent = ref('')
const isDeleting = ref(false)
const showDiagramEditor = ref(false)
const editingDiagramData = ref(null)
const editingDiagramPos = ref(null)
const editorRef = ref(null)

// 計算屬性
const hasChildren = computed(() => {
  return props.reply.children && props.reply.children.length > 0
})

// 是否為作者本人
const isAuthor = computed(() => {
  return (
    authStore.isAuthenticated &&
    authStore.user?.id &&
    props.reply.authorId === authStore.user.id &&
    !props.reply.isDeleted
  )
})

// 找出父回覆的作者名稱
const parentAuthor = computed(() => {
  if (!props.reply.parentReplyId) return null
  const parent = props.allReplies.find((r) => r.id === props.reply.parentReplyId)
  return parent?.authorName || null
})

// 格式化時間
const formatTime = (date) => {
  const now = new Date()
  const replyDate = new Date(date)
  const diffMs = now - replyDate
  const diffMins = Math.floor(diffMs / 60000)

  if (diffMins < 1) return '剛剛'
  if (diffMins < 60) return `${diffMins} 分鐘前`

  const diffHours = Math.floor(diffMins / 60)
  if (diffHours < 24) return `${diffHours} 小時前`

  const diffDays = Math.floor(diffHours / 24)
  if (diffDays < 7) return `${diffDays} 天前`

  return replyDate.toLocaleDateString('zh-TW')
}

// 計算子回覆總數
const countDescendants = (reply) => {
  let count = 0
  if (reply.children) {
    count = reply.children.length
    reply.children.forEach((child) => {
      count += countDescendants(child)
    })
  }
  return count
}

// 切換折疊狀態
const toggleCollapse = () => {
  isCollapsed.value = !isCollapsed.value
}

// 切換回覆表單
const toggleReplyForm = () => {
  showReplyForm.value = !showReplyForm.value
}

// 處理回覆提交
const handleReplySubmitted = (newReply) => {
  showReplyForm.value = false
  emit('reply-added', newReply)
}

// 開始編輯
const startEdit = () => {
  editContent.value = props.reply.content
  isEditing.value = true
  showReplyForm.value = false
  showDiagramEditor.value = false
}

// 取消編輯
const cancelEdit = () => {
  isEditing.value = false
  editContent.value = ''
  showDiagramEditor.value = false
  editingDiagramData.value = null
  editingDiagramPos.value = null
}

// 處理戰術圖編輯
const handleEditDiagram = (event) => {
  editingDiagramData.value = { ...event.data }
  editingDiagramPos.value = event.getPos
  showDiagramEditor.value = true
}

// 保存戰術圖編輯
const saveDiagramEdit = () => {
  if (editorRef.value && editingDiagramPos.value && editingDiagramData.value) {
    const pos = typeof editingDiagramPos.value === 'function' ? editingDiagramPos.value() : editingDiagramPos.value
    editorRef.value.updateDiagram(pos, editingDiagramData.value)
    closeDiagramEditor()
  }
}

// 關閉戰術圖編輯器
const closeDiagramEditor = () => {
  showDiagramEditor.value = false
  editingDiagramData.value = null
  editingDiagramPos.value = null
}

// 從內容中提取戰術圖資料
const extractDiagramFromContent = (content) => {
  if (!content) return null

  // 查找所有的戰術圖標籤
  const diagramMatches = content.match(/data-diagram='([^']+)'/g)
  if (!diagramMatches || diagramMatches.length === 0) return null

  // 取最後一個戰術圖（通常是最新的）
  const lastMatch = diagramMatches[diagramMatches.length - 1]
  const dataMatch = lastMatch.match(/data-diagram='([^']+)'/)

  if (dataMatch && dataMatch[1]) {
    try {
      return JSON.parse(dataMatch[1])
    } catch (e) {
      console.error('Failed to parse diagram data:', e)
      return null
    }
  }

  return null
}

// 保存編輯
const saveEdit = async () => {
  if (!editContent.value.trim()) return

  try {
    await repliesApi.updateReply(props.postId, props.reply.id, editContent.value)
    emit('reply-updated', { 
      id: props.reply.id, 
      content: editContent.value,
      updatedAt: new Date().toISOString()
    })
    isEditing.value = false
    // 不直接修改 props，而是通過 emit 通知父組件更新
  } catch (error) {
    console.error('Failed to update reply:', error)
    alert('更新回覆失敗')
  }
}

// 刪除回覆
const deleteReply = async () => {
  if (!confirm('確定要刪除這則回覆嗎？')) return

  isDeleting.value = true
  try {
    await repliesApi.deleteReply(props.postId, props.reply.id)
    emit('reply-deleted', props.reply.id)
    // 不直接修改 props，通過 emit 通知父組件該回覆已被刪除
  } catch (error) {
    console.error('Failed to delete reply:', error)
    alert('刪除回覆失敗')
  } finally {
    isDeleting.value = false
  }
}
</script>

<style scoped>
.reply-thread {
  position: relative;
}

/* 深度樣式 */
.depth-1 {
  padding-left: 1.5rem;
}
.depth-2 {
  padding-left: 3rem;
}
.depth-3 {
  padding-left: 4.5rem;
}
.depth-4 {
  padding-left: 6rem;
}
.depth-5 {
  padding-left: 7.5rem;
}

/* 手機版減少縮排 */
@media (max-width: 640px) {
  .depth-1 {
    padding-left: 0.75rem;
  }
  .depth-2 {
    padding-left: 1.5rem;
  }
  .depth-3 {
    padding-left: 2.25rem;
  }
  .depth-4 {
    padding-left: 3rem;
  }
  .depth-5 {
    padding-left: 3.75rem;
  }
}

.reply-content-wrapper {
  position: relative;
  margin-bottom: 1rem;
}

/* 左側連接線 */
.reply-connector {
  position: absolute;
  left: -1rem;
  top: 0;
  bottom: 0;
  width: 2px;
  background: linear-gradient(180deg, transparent 0%, #e5e7eb 1rem, #e5e7eb calc(100% - 1rem), transparent 100%);
}

:root.dark .reply-connector {
  background: linear-gradient(180deg, transparent 0%, #374151 1rem, #374151 calc(100% - 1rem), transparent 100%);
}

.reply-content {
  background: white;
  border: 1px solid #e5e7eb;
  border-radius: 0.75rem;
  padding: 0.75rem;
  transition: all 0.2s;
}

:root.dark .reply-content {
  background: #1f2937;
  border-color: #374151;
}

.reply-content:hover {
  border-color: #d1d5db;
  box-shadow: 0 1px 3px rgba(0, 0, 0, 0.1);
}

:root.dark .reply-content:hover {
  border-color: #4b5563;
}

.reply-header {
  display: flex;
  align-items: center;
  justify-content: space-between;
  flex-wrap: wrap;
  gap: 0.5rem;
}

.collapse-btn {
  padding: 0.125rem;
  border-radius: 0.25rem;
  transition: all 0.2s;
}

.collapse-btn:hover {
  background: #f3f4f6;
}

:root.dark .collapse-btn:hover {
  background: #374151;
}

.user-avatar {
  width: 1.75rem;
  height: 1.75rem;
  background: #dbeafe;
  color: #2563eb;
  border-radius: 50%;
  display: flex;
  align-items: center;
  justify-content: center;
  font-size: 0.875rem;
  font-weight: 500;
}

:root.dark .user-avatar {
  background: #1e3a8a;
  color: #93c5fd;
}

.reply-actions {
  display: flex;
  gap: 0.5rem;
}

.action-btn {
  display: flex;
  align-items: center;
  padding: 0.25rem 0.5rem;
  border-radius: 0.375rem;
  font-size: 0.875rem;
  color: #6b7280;
  transition: all 0.2s;
}

.action-btn:hover {
  background: #f3f4f6;
  color: #111827;
}

:root.dark .action-btn:hover {
  background: #374151;
  color: #f3f4f6;
}

.reply-body {
  animation: slideDown 0.2s ease-out;
}

@keyframes slideDown {
  from {
    opacity: 0;
    transform: translateY(-0.5rem);
  }
  to {
    opacity: 1;
    transform: translateY(0);
  }
}

.collapsed-summary {
  margin-top: 0.5rem;
  padding: 0.25rem 0.5rem;
  background: #f9fafb;
  border-radius: 0.375rem;
}

:root.dark .collapsed-summary {
  background: #111827;
}

.inline-reply-form {
  margin-top: 0.75rem;
  padding-top: 0.75rem;
  border-top: 1px solid #e5e7eb;
  /* 確保回覆表單有足夠空間顯示戰術圖 */
  min-height: 120px;
}

:root.dark .inline-reply-form {
  border-color: #374151;
}

.child-replies {
  margin-top: 0.75rem;
}

/* 按鈕樣式 */
.btn-primary {
  padding: 0.375rem 0.75rem;
  background: #2563eb;
  color: white;
  border: 1px solid #2563eb;
  border-radius: 0.375rem;
  font-weight: 500;
  transition: all 0.2s;
}

.btn-primary:hover:not(:disabled) {
  background: #1d4ed8;
}

.btn-primary:disabled {
  opacity: 0.5;
  cursor: not-allowed;
}

.btn-secondary {
  padding: 0.375rem 0.75rem;
  background: white;
  color: #6b7280;
  border: 1px solid #e5e7eb;
  border-radius: 0.375rem;
  font-weight: 500;
  transition: all 0.2s;
}

:root.dark .btn-secondary {
  background: #374151;
  color: #d1d5db;
  border-color: #4b5563;
}

.btn-secondary:hover {
  background: #f9fafb;
}

:root.dark .btn-secondary:hover {
  background: #4b5563;
}

.edit-mode {
  padding: 0.75rem;
  background: #f9fafb;
  border-radius: 0.375rem;
  position: relative;
}

:root.dark .edit-mode {
  background: #111827;
}

/* 戰術圖編輯器模態框 */
.diagram-editor-modal {
  position: fixed;
  top: 50%;
  left: 50%;
  transform: translate(-50%, -50%);
  width: 90%;
  max-width: 800px;
  max-height: 90vh;
  background: white;
  border: 2px solid #e5e7eb;
  border-radius: 0.5rem;
  box-shadow:
    0 20px 25px -5px rgba(0, 0, 0, 0.1),
    0 10px 10px -5px rgba(0, 0, 0, 0.04);
  z-index: 1000;
  display: flex;
  flex-direction: column;
}

:root.dark .diagram-editor-modal {
  background: #1f2937;
  border-color: #374151;
}

.diagram-editor-header {
  display: flex;
  justify-content: space-between;
  align-items: center;
  padding: 1rem;
  border-bottom: 1px solid #e5e7eb;
  font-weight: 500;
}

:root.dark .diagram-editor-header {
  border-color: #374151;
}

.close-btn {
  width: 2rem;
  height: 2rem;
  display: flex;
  align-items: center;
  justify-content: center;
  border-radius: 0.375rem;
  transition: all 0.2s;
  cursor: pointer;
}

.close-btn:hover {
  background: #f3f4f6;
}

:root.dark .close-btn:hover {
  background: #374151;
}

.diagram-editor {
  flex: 1;
  overflow: auto;
  padding: 1rem;
}

.diagram-editor-actions {
  display: flex;
  gap: 0.5rem;
  justify-content: flex-end;
  padding: 1rem;
  border-top: 1px solid #e5e7eb;
}

:root.dark .diagram-editor-actions {
  border-color: #374151;
}
</style>
