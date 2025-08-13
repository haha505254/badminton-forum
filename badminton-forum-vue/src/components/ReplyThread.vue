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
            <button 
              v-if="hasChildren"
              @click="toggleCollapse"
              class="collapse-btn"
              :aria-expanded="!isCollapsed"
            >
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
            <span class="font-medium text-gray-900 dark:text-white">
              {{ reply.authorName }}
            </span>
            
            <!-- 時間 -->
            <span class="text-sm text-gray-500 dark:text-gray-400">
              · {{ formatTime(reply.createdAt) }}
            </span>
            
            <!-- 如果是回覆某人 -->
            <span v-if="reply.parentReplyId && parentAuthor" class="text-sm text-gray-500 dark:text-gray-400">
              回覆 @{{ parentAuthor }}
            </span>
          </div>
          
          <!-- 操作按鈕 -->
          <div class="reply-actions">
            <button 
              @click="toggleReplyForm"
              class="action-btn"
              title="回覆"
            >
              <svg class="w-4 h-4" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M3 10h10a8 8 0 018 8v2M3 10l6 6m-6-6l6-6" />
              </svg>
              <span class="ml-1">回覆</span>
            </button>
          </div>
        </div>
        
        <!-- 回覆內容（可折疊） -->
        <div v-show="!isCollapsed" class="reply-body">
          <!-- 文字內容 -->
          <div class="prose prose-sm max-w-none dark:prose-invert mt-2">
            <RichTextDisplay :content="reply.content" />
          </div>
          
          <!-- 內嵌回覆表單 -->
          <div v-if="showReplyForm" class="inline-reply-form">
            <ReplyInput
              :post-id="postId"
              :parent-reply-id="reply.id"
              :parent-author="reply.authorName"
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
              @reply-added="$emit('reply-added', $event)"
            />
          </div>
        </div>
        
        <!-- 折疊時顯示的摘要 -->
        <div v-if="isCollapsed && hasChildren" class="collapsed-summary">
          <span class="text-sm text-gray-500 dark:text-gray-400">
            已折疊 {{ countDescendants(reply) }} 則回覆
          </span>
        </div>
      </div>
    </div>
  </div>
</template>

<script setup>
import { ref, computed } from 'vue'
import RichTextDisplay from './RichTextDisplay.vue'
import ReplyInput from './ReplyInput.vue'

const props = defineProps({
  reply: {
    type: Object,
    required: true
  },
  postId: {
    type: [Number, String],
    required: true
  },
  depth: {
    type: Number,
    default: 0
  },
  allReplies: {
    type: Array,
    default: () => []
  }
})

const emit = defineEmits(['reply-added'])

// 最大顯示深度
const maxDepth = 5

// 狀態
const isCollapsed = ref(false)
const showReplyForm = ref(false)

// 計算屬性
const hasChildren = computed(() => {
  return props.reply.children && props.reply.children.length > 0
})

// 找出父回覆的作者名稱
const parentAuthor = computed(() => {
  if (!props.reply.parentReplyId) return null
  const parent = props.allReplies.find(r => r.id === props.reply.parentReplyId)
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
    reply.children.forEach(child => {
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
</script>

<style scoped>
.reply-thread {
  position: relative;
}

/* 深度樣式 */
.depth-1 { padding-left: 1.5rem; }
.depth-2 { padding-left: 3rem; }
.depth-3 { padding-left: 4.5rem; }
.depth-4 { padding-left: 6rem; }
.depth-5 { padding-left: 7.5rem; }

/* 手機版減少縮排 */
@media (max-width: 640px) {
  .depth-1 { padding-left: 0.75rem; }
  .depth-2 { padding-left: 1.5rem; }
  .depth-3 { padding-left: 2.25rem; }
  .depth-4 { padding-left: 3rem; }
  .depth-5 { padding-left: 3.75rem; }
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
  background: linear-gradient(180deg, 
    transparent 0%, 
    #e5e7eb 1rem, 
    #e5e7eb calc(100% - 1rem), 
    transparent 100%
  );
}

:root.dark .reply-connector {
  background: linear-gradient(180deg, 
    transparent 0%, 
    #374151 1rem, 
    #374151 calc(100% - 1rem), 
    transparent 100%
  );
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
</style>