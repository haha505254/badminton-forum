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

    <!-- Post Content -->
    <article v-else class="card-dark mb-6">
      <!-- Post Header -->
      <div class="border-b border-gray-200 dark:border-gray-700 pb-4 mb-6">
        <h1 class="text-3xl font-bold text-gray-900 dark:text-white mb-4">
          {{ post.title }}
        </h1>
        
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
      <div class="prose prose-lg max-w-none dark:prose-invert">
        <RichTextDisplay :content="post.content" />
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
      
      <!-- Reply List -->
      <div v-if="replies.length > 0" class="space-y-4">
        <div 
          v-for="reply in replies" 
          :key="reply.id" 
          class="border-l-4 border-primary-200 dark:border-primary-800 pl-4 py-4"
        >
          <!-- Reply Header -->
          <div class="flex items-center justify-between mb-3">
            <div class="flex items-center space-x-2">
              <div class="w-8 h-8 bg-primary-100 dark:bg-primary-900 rounded-full flex items-center justify-center">
                <span class="text-sm font-medium text-primary-600 dark:text-primary-400">
                  {{ reply.authorName.charAt(0).toUpperCase() }}
                </span>
              </div>
              <span class="font-medium text-gray-900 dark:text-white">
                {{ reply.authorName }}
              </span>
            </div>
            <span class="text-sm text-gray-500 dark:text-gray-400">
              {{ formatDate(reply.createdAt) }}
            </span>
          </div>
          
          <!-- Reply Content -->
          <div class="prose prose-sm max-w-none dark:prose-invert">
            <RichTextDisplay :content="reply.content" />
          </div>
        </div>
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
    
    <!-- Reply Form -->
    <section v-if="authStore.isAuthenticated" class="card-dark">
      <h3 class="text-xl font-bold text-gray-900 dark:text-white mb-4">
        發表回覆
      </h3>
      
      <div class="space-y-4">
        <!-- 戰術圖切換按鈕 -->
        <div class="editor-toolbar-custom">
          <button
            type="button"
            @click="toggleDiagramMode"
            class="diagram-btn"
            :class="{ active: showDiagram }"
          >
            🏸 插入戰術圖
          </button>
          <button
            v-if="post.content.includes('badminton-diagram-placeholder')"
            type="button"
            @click="loadOriginalDiagram"
            class="diagram-btn"
          >
            📋 引用原文戰術圖
          </button>
        </div>
        
        <RichTextEditor 
          v-if="!showDiagram"
          v-model="newReply" 
          placeholder="寫下您的回覆..." 
          class="min-h-[150px]"
        />
        <BadmintonCourtDiagram
          v-else
          v-model="diagramData"
        />
        
        <div class="flex justify-end">
          <button 
            @click="submitReply" 
            :disabled="submitting || !newReply.trim()" 
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
import { ref, onMounted, watch } from 'vue'
import { useRoute, RouterLink } from 'vue-router'
import { useAuthStore } from '../stores/auth'
import { postsApi } from '../api/posts'
import { repliesApi } from '../api/replies'
import RichTextEditor from '../components/RichTextEditor.vue'
import RichTextDisplay from '../components/RichTextDisplay.vue'
import BadmintonCourtDiagram from '../components/BadmintonCourtDiagram.vue'

const route = useRoute()
const authStore = useAuthStore()

const post = ref({
  title: '',
  authorName: '',
  content: '',
  createdAt: new Date(),
  viewCount: 0
})

const replies = ref([])
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

const toggleDiagramMode = () => {
  showDiagram.value = !showDiagram.value
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
      // 深拷貝原始資料
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

// 當戰術圖資料更新時，將其嵌入到內容中
watch(diagramData, (newData) => {
  if (showDiagram.value && newData) {
    // 將戰術圖資料以特殊格式嵌入到內容中
    const diagramHtml = `
      <div class="badminton-diagram-placeholder" data-diagram='${JSON.stringify(newData)}'>
        <p>[羽球戰術圖: ${newData.description || '戰術示意圖'}]</p>
      </div>
    `
    
    // 保留原有內容並添加戰術圖
    if (!newReply.value.includes('badminton-diagram-placeholder')) {
      newReply.value += diagramHtml
    } else {
      // 更新現有的戰術圖
      newReply.value = newReply.value.replace(
        /<div class="badminton-diagram-placeholder".*?<\/div>/s,
        diagramHtml
      )
    }
  }
}, { deep: true })

const submitReply = async () => {
  if (!newReply.value.trim()) return
  
  submitting.value = true
  try {
    const response = await repliesApi.createReply(post.value.id, {
      content: newReply.value
    })
    replies.value.push(response.data)
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
    
    // Fetch replies
    const repliesResponse = await repliesApi.getReplies(postId)
    replies.value = repliesResponse.data
  } catch (error) {
    console.error('Failed to fetch post data:', error)
  } finally {
    loading.value = false
  }
})
</script>

<style scoped>
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
  gap: 0.5rem;
  margin-bottom: 0.5rem;
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

.diagram-btn:hover {
  background: #f0f0f0;
}

.diagram-btn.active {
  background: #27ae60;
  color: white;
  border-color: #27ae60;
}
</style>