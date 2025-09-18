<template>
  <div class="post-view">
    <!-- Loading State -->
    <div v-if="loading" class="space-y-4">
      <div class="card-dark animate-pulse">
        <div class="h-8 bg-gray-300 dark:bg-gray-600 rounded w-3/4 mb-4"></div>
        <div class="flex space-x-4 mb-4">
          <div class="h-4 bg-gray-300 dark:bg-gray-600 rounded w-24"></div>
          <div class="h-4 bg-gray-300 dark:bg-gray-600 rounded w-32"></div>
          <div class="h-4 bg-gray-300 dark:bg-gray-600 rounded w-20"></div>
        </div>
        <div class="space-y-2">
          <div class="h-4 bg-gray-300 dark:bg-gray-600 rounded"></div>
          <div class="h-4 bg-gray-300 dark:bg-gray-600 rounded"></div>
          <div class="h-4 bg-gray-300 dark:bg-gray-600 rounded w-5/6"></div>
        </div>
      </div>
    </div>

    <!-- Post Not Found (404) -->
    <div v-else-if="!post && !loading" class="card-dark text-center py-12">
      <h1 class="text-2xl font-bold text-gray-500 dark:text-gray-400 mb-4">文章不存在</h1>
      <p class="text-gray-400 dark:text-gray-500 mb-6">此文章可能已被刪除或不存在</p>
      <RouterLink to="/" class="btn-primary">返回首頁</RouterLink>
    </div>

    <!-- Deleted Post with Replies (Preserve Thread) -->
    <div v-else-if="post && post.isDeleted && !isAuthor" class="space-y-6">
      <div class="card-dark">
        <div class="text-center py-8">
          <h1 class="text-2xl font-bold text-gray-500 dark:text-gray-400 mb-2">{{ post.title }}</h1>
          <p class="text-gray-400 dark:text-gray-500 text-lg mb-2">[此文章已被作者刪除]</p>
          <p v-if="post.deletedAt" class="text-sm text-gray-400 dark:text-gray-500">
            刪除時間：{{ formatDate(post.deletedAt) }}
          </p>
          <div v-if="post.replyCount > 0" class="mt-6 p-4 bg-blue-50 dark:bg-blue-900/20 rounded-lg">
            <p class="text-sm text-blue-600 dark:text-blue-400">
              💬 雖然原文已刪除，但下方仍有 {{ post.replyCount }} 則討論內容保留供參考
            </p>
          </div>
        </div>
      </div>
    </div>

    <!-- Author Viewing Own Deleted Post -->
    <div v-else-if="post && post.isDeleted && isAuthor" class="space-y-6">
      <div class="bg-yellow-100 dark:bg-yellow-900/30 border border-yellow-300 dark:border-yellow-700 p-4 rounded-lg">
        <p class="text-yellow-800 dark:text-yellow-200 flex items-center">
          <svg class="w-5 h-5 mr-2" fill="currentColor" viewBox="0 0 20 20">
            <path fill-rule="evenodd" d="M8.257 3.099c.765-1.36 2.722-1.36 3.486 0l5.58 9.92c.75 1.334-.213 2.98-1.742 2.98H4.42c-1.53 0-2.493-1.646-1.743-2.98l5.58-9.92zM11 13a1 1 0 11-2 0 1 1 0 012 0zm-1-8a1 1 0 00-1 1v3a1 1 0 002 0V6a1 1 0 00-1-1z" clip-rule="evenodd" />
          </svg>
          此文章已被您刪除，其他用戶無法看到原始內容
          <span v-if="post.replyCount > 0">，但保留了 {{ post.replyCount }} 則回覆</span>
        </p>
      </div>
      <article class="card-dark mb-6">
        <!-- Show full content for author -->
        <div class="border-b border-gray-200 dark:border-gray-700 pb-4 mb-6">
          <h1 class="text-3xl font-bold text-gray-500 dark:text-gray-400">
            {{ post.title }} <span class="text-sm font-normal">[已刪除]</span>
          </h1>
        </div>
        <div class="prose prose-lg max-w-none dark:prose-invert opacity-75">
          <RichTextDisplay 
            :content="post.content" 
            display-context="post"
            :default-expanded="true"
          />
        </div>
      </article>
    </div>

    <!-- Normal Post Content -->
    <article v-else-if="post" class="card-dark mb-6">
      <!-- Post Header -->
      <div class="border-b border-gray-200 dark:border-gray-700 pb-4 mb-6">
        <div class="flex items-start justify-between mb-4">
          <h1 class="text-3xl font-bold" :class="post.isDeleted ? 'text-gray-500 dark:text-gray-400 line-through' : 'text-gray-900 dark:text-white'">
            {{ post.title }}
          </h1>
          
          <!-- Edit/Delete Buttons (only for post author) -->
          <div v-if="isAuthor && !post.isDeleted" class="flex gap-2">
            <RouterLink 
              :to="`/posts/${post.id}/edit`"
              class="btn-primary flex items-center gap-2"
            >
              <svg class="w-4 h-4" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M11 5H6a2 2 0 00-2 2v11a2 2 0 002 2h11a2 2 0 002-2v-5m-1.414-9.414a2 2 0 112.828 2.828L11.828 15H9v-2.828l8.586-8.586z" />
              </svg>
              編輯文章
            </RouterLink>
            <button
              @click="deletePost"
              class="btn-danger flex items-center gap-2"
            >
              <svg class="w-4 h-4" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M19 7l-.867 12.142A2 2 0 0116.138 21H7.862a2 2 0 01-1.995-1.858L5 7m5 4v6m4-6v6m1-10V4a1 1 0 00-1-1h-4a1 1 0 00-1 1v3M4 7h16" />
              </svg>
              刪除文章
            </button>
          </div>
        </div>
        
        <!-- Post Meta -->
        <div class="flex flex-wrap items-center gap-4 text-sm text-gray-600 dark:text-gray-400">
          <div class="flex items-center">
            <svg class="w-4 h-4 mr-1" fill="none" stroke="currentColor" viewBox="0 0 24 24">
              <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M16 7a4 4 0 11-8 0 4 4 0 018 0zM12 14a7 7 0 00-7 7h14a7 7 0 00-7-7z" />
            </svg>
            <span>{{ post.authorName }}</span>
          </div>
          
          <div class="flex items-center">
            <svg class="w-4 h-4 mr-1" fill="none" stroke="currentColor" viewBox="0 0 24 24">
              <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M8 7V3m8 4V3m-9 8h10M5 21h14a2 2 0 002-2V7a2 2 0 00-2-2H5a2 2 0 00-2 2v12a2 2 0 002 2z" />
            </svg>
            <span>{{ formatDate(post.createdAt) }}</span>
          </div>
          
          <div class="flex items-center">
            <svg class="w-4 h-4 mr-1" fill="none" stroke="currentColor" viewBox="0 0 24 24">
              <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M15 12a3 3 0 11-6 0 3 3 0 016 0z" />
              <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M2.458 12C3.732 7.943 7.523 5 12 5c4.478 0 8.268 2.943 9.542 7-1.274 4.057-5.064 7-9.542 7-4.477 0-8.268-2.943-9.542-7z" />
            </svg>
            <span>{{ post.viewCount }} 次瀏覽</span>
          </div>
        </div>
      </div>
      
      <!-- Post Content -->
      <div v-if="post.isDeleted" class="text-center py-8">
        <p class="text-gray-500 dark:text-gray-400 text-lg italic">
          [此文章已被作者刪除]
        </p>
        <p v-if="post.deletedAt" class="text-sm text-gray-400 dark:text-gray-500 mt-2">
          刪除時間：{{ formatDate(post.deletedAt) }}
        </p>
      </div>
      <div v-else class="prose prose-lg max-w-none dark:prose-invert">
        <RichTextDisplay 
          :content="post.content" 
          display-context="post"
          :default-expanded="true"
        />
      </div>
    </article>
    
    <!-- Replies Section -->
    <section class="card-dark mb-6">
      <h2 class="text-2xl font-bold text-gray-900 dark:text-white mb-6 flex items-center">
        <svg class="w-6 h-6 mr-2" fill="none" stroke="currentColor" viewBox="0 0 24 24">
          <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M8 12h.01M12 12h.01M16 12h.01M21 12c0 4.418-4.03 8-9 8a9.863 9.863 0 01-4.255-.949L3 20l1.395-3.72C3.512 15.042 3 13.574 3 12c0-4.418 4.03-8 9-8s9 3.582 9 8z" />
        </svg>
        回覆 ({{ replies.length }})
      </h2>
      
      <!-- Reply List (Nested) -->
      <div v-if="replyTree.length > 0" class="space-y-2">
        <ReplyThread
          v-for="reply in replyTree"
          :key="reply.id"
          :reply="reply"
          :post-id="post.id"
          :all-replies="replies"
          @reply-added="handleReplyAdded"
        />
      </div>
      
      <!-- Empty State -->
      <div v-else class="text-center py-8">
        <svg class="mx-auto h-12 w-12 text-gray-400" fill="none" viewBox="0 0 24 24" stroke="currentColor">
          <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M8 12h.01M12 12h.01M16 12h.01M21 12c0 4.418-4.03 8-9 8a9.863 9.863 0 01-4.255-.949L3 20l1.395-3.72C3.512 15.042 3 13.574 3 12c0-4.418 4.03-8 9-8s9 3.582 9 8z" />
        </svg>
        <p class="mt-2 text-sm text-gray-500 dark:text-gray-400">
          還沒有任何回覆，成為第一個回覆的人吧！
        </p>
      </div>
    </section>
    
    <!-- Reply Form or Disabled Message -->
    <section v-if="post && post.isLocked && !post.isDeleted" class="card-dark">
      <div class="text-center py-6 text-gray-500 dark:text-gray-400">
        <svg class="w-12 h-12 mx-auto mb-3" fill="currentColor" viewBox="0 0 20 20">
          <path fill-rule="evenodd" d="M5 9V7a5 5 0 0110 0v2a2 2 0 012 2v5a2 2 0 01-2 2H5a2 2 0 01-2-2v-5a2 2 0 012-2zm8-2v2H7V7a3 3 0 016 0z" clip-rule="evenodd" />
        </svg>
        <p class="text-lg font-medium">此文章已被鎖定</p>
        <p class="text-sm mt-1">無法發表新的回覆</p>
      </div>
    </section>
    
    <!-- Reply Form -->
    <section v-else-if="authStore.isAuthenticated && post && !post.isDeleted && !post.isLocked" class="card-dark">
      <h3 class="text-xl font-bold text-gray-900 dark:text-white mb-4">
        發表回覆
      </h3>
      
      <div class="space-y-4">
        <!-- 文字編輯器（始終顯示） -->
        <RichTextEditor 
          v-model="newReply" 
          placeholder="寫下您的回覆..." 
          class="min-h-[150px]"
        />
        
        <!-- 戰術圖工具列 -->
        <div class="editor-toolbar-custom">
          <button
            type="button"
            @click="toggleDiagramMode"
            class="diagram-btn"
            :class="{ active: showDiagram }"
          >
            🏸 {{ showDiagram ? '隱藏戰術圖' : '添加戰術圖' }}
          </button>
          <button
            v-if="post.content.includes('badminton-diagram-placeholder')"
            type="button"
            @click="loadOriginalDiagram"
            class="diagram-btn"
          >
            📋 引用原文戰術圖
          </button>
          <span v-if="hasDiagram" class="diagram-indicator">
            ✓ 已添加戰術圖
          </span>
        </div>
        
        <!-- 戰術圖編輯器（可選顯示） -->
        <div v-if="showDiagram" class="diagram-editor-section">
          <div class="diagram-editor-header">
            <span class="diagram-editor-title">編輯戰術圖</span>
            <button 
              @click="clearDiagram"
              class="clear-diagram-btn"
              title="清除戰術圖"
            >
              清除
            </button>
          </div>
          <BadmintonCourtDiagram
            v-model="diagramData"
          />
        </div>
        
        <div class="flex justify-end">
          <button 
            @click="submitReply" 
            :disabled="submitting || (!newReply.trim() && !hasDiagram)" 
            class="btn-primary"
          >
            <span v-if="submitting" class="flex items-center">
              <svg class="animate-spin -ml-1 mr-3 h-5 w-5 text-white" fill="none" viewBox="0 0 24 24">
                <circle class="opacity-25" cx="12" cy="12" r="10" stroke="currentColor" stroke-width="4"></circle>
                <path class="opacity-75" fill="currentColor" d="M4 12a8 8 0 018-8V0C5.373 0 0 5.373 0 12h4zm2 5.291A7.962 7.962 0 014 12H0c0 3.042 1.135 5.824 3 7.938l3-2.647z"></path>
              </svg>
              發表中...
            </span>
            <span v-else>發表回覆</span>
          </button>
        </div>
      </div>
    </section>
    
    <!-- Login Prompt -->
    <div v-else class="card-dark text-center">
      <svg class="mx-auto h-12 w-12 text-gray-400" fill="none" viewBox="0 0 24 24" stroke="currentColor">
        <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M11 16l-4-4m0 0l4-4m-4 4h14m-5 4v1a3 3 0 01-3 3H6a3 3 0 01-3-3V7a3 3 0 013-3h7a3 3 0 013 3v1" />
      </svg>
      <h3 class="mt-2 text-sm font-medium text-gray-900 dark:text-white">
        登入後即可回覆
      </h3>
      <p class="mt-1 text-sm text-gray-500 dark:text-gray-400">
        加入討論，分享您的觀點
      </p>
      <div class="mt-6">
        <RouterLink to="/login" class="btn-primary">
          立即登入
        </RouterLink>
      </div>
    </div>
  </div>
</template>

<script setup>
import { ref, onMounted, watch, computed } from 'vue'
import { useRoute, RouterLink } from 'vue-router'
import { useAuthStore } from '../stores/auth'
import { postsApi } from '../api/posts'
import { repliesApi } from '../api/replies'
import RichTextEditor from '../components/RichTextEditor.vue'
import RichTextDisplay from '../components/RichTextDisplay.vue'
import BadmintonCourtDiagram from '../components/BadmintonCourtDiagram.vue'
import ReplyThread from '../components/ReplyThread.vue'

const route = useRoute()
const authStore = useAuthStore()

const post = ref({
  id: null,
  title: '',
  authorId: null,
  authorName: '',
  content: '',
  createdAt: new Date(),
  viewCount: 0
})

// 檢查當前使用者是否為文章作者
const isAuthor = computed(() => {
  return authStore.isAuthenticated && 
         authStore.user?.id && 
         post.value.authorId === authStore.user.id
})

// 刪除文章
const deletePost = async () => {
  if (!confirm('確定要刪除這篇文章嗎？刪除後無法復原，但回覆會保留。')) {
    return
  }
  
  try {
    await postsApi.deletePost(post.value.id)
    // 重新載入文章以顯示刪除狀態
    const postResponse = await postsApi.getPost(post.value.id)
    post.value = postResponse.data
  } catch (error) {
    console.error('Failed to delete post:', error)
    alert('刪除文章失敗')
  }
}

const replies = ref([])
const replyTree = ref([])
const newReply = ref('')
const loading = ref(true)
const submitting = ref(false)
const showDiagram = ref(false)
const diagramData = ref({
  players: [],
  shuttle: null,
  arrows: [],
  textAnnotations: [],
  description: ''
})

const formatDate = (date) => {
  return new Date(date).toLocaleString('zh-TW', {
    year: 'numeric',
    month: '2-digit',
    day: '2-digit',
    hour: '2-digit',
    minute: '2-digit'
  })
}

// 檢查是否有有效的戰術圖資料
const hasDiagram = computed(() => {
  return diagramData.value && (
    diagramData.value.players?.length > 0 ||
    diagramData.value.shuttle ||
    diagramData.value.arrows?.length > 0 ||
    diagramData.value.textAnnotations?.length > 0
  )
})

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

// 載入原文的戰術圖
const loadOriginalDiagram = () => {
  try {
    // 從原文中解析戰術圖資料
    const parser = new DOMParser()
    const doc = parser.parseFromString(post.value.content, 'text/html')
    const diagramElement = doc.querySelector('.badminton-diagram-placeholder')
    
    if (diagramElement) {
      const originalData = JSON.parse(diagramElement.getAttribute('data-diagram'))
      // 深拷貝原始資料（資料已經是相對座標格式，直接使用）
      diagramData.value = {
        players: [...(originalData.players || [])],
        shuttle: originalData.shuttle ? { ...originalData.shuttle } : null,
        arrows: [...(originalData.arrows || [])],
        textAnnotations: [...(originalData.textAnnotations || [])],
        description: originalData.description ? `回應：${originalData.description}` : '回應戰術圖'
      }
      showDiagram.value = true
    }
  } catch (error) {
    console.error('Failed to load original diagram:', error)
    alert('載入原文戰術圖失敗')
  }
}

// 建立回覆樹狀結構
const buildReplyTree = (flatReplies) => {
  const replyMap = {}
  const rootReplies = []
  
  // 先建立所有回覆的映射
  flatReplies.forEach(reply => {
    replyMap[reply.id] = { ...reply, children: [] }
  })
  
  // 建立樹狀結構
  flatReplies.forEach(reply => {
    if (reply.parentReplyId && replyMap[reply.parentReplyId]) {
      replyMap[reply.parentReplyId].children.push(replyMap[reply.id])
    } else if (!reply.parentReplyId) {
      rootReplies.push(replyMap[reply.id])
    }
  })
  
  return rootReplies
}

// 處理新增回覆
const handleReplyAdded = async (newReplyData) => {
  // 重新載入回覆以獲取最新資料
  await loadReplies()
}

// 載入回覆
const loadReplies = async () => {
  try {
    const repliesResponse = await repliesApi.getReplies(post.value.id)
    replies.value = repliesResponse.data
    replyTree.value = buildReplyTree(replies.value)
  } catch (error) {
    console.error('Failed to fetch replies:', error)
  }
}

const submitReply = async () => {
  // 檢查是否有內容可以提交
  if (!newReply.value.trim() && !hasDiagram.value) return
  
  submitting.value = true
  try {
    // 組合最終內容
    let finalContent = newReply.value || ''
    
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
    
    const response = await repliesApi.createReply(post.value.id, {
      content: finalContent,
      parentReplyId: null // 頂層回覆
    })
    
    // 新增到回覆列表並重建樹狀結構
    replies.value.push(response.data)
    replyTree.value = buildReplyTree(replies.value)
    
    // 清空表單
    newReply.value = ''
    showDiagram.value = false
    diagramData.value = {
      players: [],
      shuttle: null,
      arrows: [],
      textAnnotations: [],
      description: ''
    }
  } catch (error) {
    console.error('Failed to submit reply:', error)
    alert('發表回覆失敗')
  } finally {
    submitting.value = false
  }
}

onMounted(async () => {
  const postId = route.params.id
  
  try {
    // Fetch post data
    const postResponse = await postsApi.getPost(postId)
    post.value = postResponse.data
    
    // Fetch replies and build tree
    await loadReplies()
  } catch (error) {
    console.error('Failed to fetch post data:', error)
  } finally {
    loading.value = false
  }
})
</script>

<style scoped>
.btn-danger {
  @apply px-4 py-2 bg-red-600 text-white rounded-lg hover:bg-red-700 transition-colors disabled:opacity-50 disabled:cursor-not-allowed;
}

.post-view {
  /* 預設不設置最小寬度，保持響應式 */
  width: 100%;
}

/* 只在大螢幕上設定最小寬度，確保桌面版寬度一致 */
@media (min-width: 1024px) {
  .post-view {
    min-width: 1088px;
  }
}

/* 限制內容區域寬度 */
.card-dark {
  max-width: 64rem; /* 1024px */
  margin-left: auto;
  margin-right: auto;
}

.editor-toolbar-custom {
  display: flex;
  align-items: center;
  gap: 0.75rem;
  padding-top: 0.75rem;
  border-top: 1px solid #e5e7eb;
}

:root.dark .editor-toolbar-custom {
  border-color: #374151;
}

.diagram-btn {
  padding: 0.5rem 1rem;
  background: white;
  border: 1px solid #ddd;
  border-radius: 4px;
  cursor: pointer;
  transition: all 0.3s;
  font-size: 0.95rem;
}

:root.dark .diagram-btn {
  background: #374151;
  border-color: #4b5563;
  color: #e5e7eb;
}

.diagram-btn:hover {
  background: #f0f0f0;
}

:root.dark .diagram-btn:hover {
  background: #4b5563;
}

.diagram-btn.active {
  background: #27ae60;
  color: white;
  border-color: #27ae60;
}

.diagram-indicator {
  font-size: 0.875rem;
  color: #10b981;
  display: flex;
  align-items: center;
  gap: 0.25rem;
}

:root.dark .diagram-indicator {
  color: #34d399;
}

/* 戰術圖編輯區 */
.diagram-editor-section {
  margin-top: 0.75rem;
  padding: 1rem;
  background: #f9fafb;
  border: 1px solid #e5e7eb;
  border-radius: 0.5rem;
}

:root.dark .diagram-editor-section {
  background: #111827;
  border-color: #374151;
}

.diagram-editor-header {
  display: flex;
  justify-content: space-between;
  align-items: center;
  margin-bottom: 0.75rem;
}

.diagram-editor-title {
  font-weight: 500;
  color: #374151;
}

:root.dark .diagram-editor-title {
  color: #d1d5db;
}

.clear-diagram-btn {
  padding: 0.25rem 0.75rem;
  background: white;
  color: #ef4444;
  border: 1px solid #fca5a5;
  border-radius: 0.25rem;
  font-size: 0.875rem;
  cursor: pointer;
  transition: all 0.2s;
}

:root.dark .clear-diagram-btn {
  background: #7f1d1d;
  color: #fca5a5;
  border-color: #991b1b;
}

.clear-diagram-btn:hover {
  background: #fee2e2;
}

:root.dark .clear-diagram-btn:hover {
  background: #991b1b;
}
</style>