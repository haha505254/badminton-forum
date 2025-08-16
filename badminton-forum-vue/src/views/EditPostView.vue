<template>
  <div class="edit-post">
    <div class="card-dark">
      <!-- Header -->
      <div class="flex items-center justify-between mb-6">
        <h1 class="text-2xl font-bold text-gray-900 dark:text-white">編輯文章</h1>
        <RouterLink :to="`/posts/${postId}`" class="btn-outline">返回文章</RouterLink>
      </div>

      <!-- Loading State -->
      <div v-if="loading" class="space-y-4">
        <div class="animate-pulse">
          <div class="h-10 bg-gray-300 dark:bg-gray-600 rounded mb-4"></div>
          <div class="h-10 bg-gray-300 dark:bg-gray-600 rounded mb-4"></div>
          <div class="h-40 bg-gray-300 dark:bg-gray-600 rounded"></div>
        </div>
      </div>

      <!-- Edit Form -->
      <form v-else @submit.prevent="submitPost" class="space-y-6">
        <!-- Category (Read-only) -->
        <div>
          <label class="block text-sm font-medium text-gray-700 dark:text-gray-300 mb-1">版塊</label>
          <div class="w-full rounded-md border border-gray-300 dark:border-gray-700 bg-gray-100 dark:bg-gray-700 px-3 py-2 text-gray-900 dark:text-gray-100">
            {{ formData.categoryName }}
          </div>
          <p class="mt-1 text-xs text-gray-500 dark:text-gray-400">版塊無法變更</p>
        </div>

        <!-- Title -->
        <div>
          <div class="flex items-center justify-between mb-1">
            <label for="title" class="block text-sm font-medium text-gray-700 dark:text-gray-300">文章標題</label>
            <span class="text-xs text-gray-500 dark:text-gray-400">{{ titleLength }}/{{ titleMaxLen }}</span>
          </div>
          <input
            id="title"
            v-model="formData.title"
            type="text"
            :maxlength="titleMaxLen"
            required
            placeholder="請輸入文章標題（最多 200 字）"
            class="w-full rounded-md border border-gray-300 dark:border-gray-700 bg-white dark:bg-gray-800 px-3 py-2 text-gray-900 dark:text-gray-100 focus:outline-none focus:ring-2 focus:ring-primary-500"
          />
        </div>

        <!-- Content -->
        <div>
          <div class="flex items-center justify-between mb-2">
            <label for="content" class="block text-sm font-medium text-gray-700 dark:text-gray-300">文章內容</label>
            <div class="flex items-center gap-2">
              <button v-if="!hasDiagram" type="button" @click="openDiagramModal" class="btn-outline">
                🏸 插入戰術圖
              </button>
            </div>
          </div>

          <RichTextEditor
            ref="editorRef"
            v-model="formData.content"
            placeholder="請輸入文章內容..."
            @edit-diagram="handleEditDiagram"
          />
          <p class="mt-2 text-xs text-gray-500 dark:text-gray-400">請遵守社群守則，理性交流。</p>

          <!-- Diagram Modal -->
          <div v-if="showDiagramModal" class="modal-overlay" @keydown.esc="closeDiagramModal" tabindex="-1">
            <div class="modal-card" role="dialog" aria-modal="true">
              <div class="flex items-center justify-between mb-3">
                <h3 class="text-lg font-semibold text-gray-900 dark:text-white">{{ isEditingDiagram ? '編輯戰術圖' : '插入戰術圖' }}</h3>
                <button type="button" class="btn-outline" @click="closeDiagramModal">關閉</button>
              </div>
              <p class="text-sm text-gray-600 dark:text-gray-400 mb-3">在下方編輯戰術示意，確認後將插入到文章內容中。</p>
              <div class="modal-body">
                <div class="diagram-wrapper">
                  <BadmintonCourtDiagram v-model="diagramDraft" />
                </div>
              </div>
              <div class="flex items-center justify-end gap-3 mt-4">
                <button type="button" class="btn-outline" @click="closeDiagramModal">取消</button>
                <button type="button" class="btn-primary" @click="confirmDiagram">{{ isEditingDiagram ? '更新至文章' : '插入到文章' }}</button>
              </div>
            </div>
          </div>
        </div>

        <!-- Actions -->
        <div class="flex items-center justify-between pt-2">
          <div class="text-sm text-gray-500 dark:text-gray-400">
            上次更新：{{ formData.updatedAt ? formatDate(formData.updatedAt) : '從未更新' }}
          </div>
          <div class="flex items-center gap-3">
            <RouterLink :to="`/posts/${postId}`" class="btn-outline">取消</RouterLink>
            <button type="submit" class="btn-primary" :disabled="!canSubmit">
              <span v-if="submitting" class="flex items-center">
                <svg class="animate-spin -ml-1 mr-2 h-5 w-5 text-white" fill="none" viewBox="0 0 24 24">
                  <circle class="opacity-25" cx="12" cy="12" r="10" stroke="currentColor" stroke-width="4"></circle>
                  <path class="opacity-75" fill="currentColor" d="M4 12a8 8 0 018-8V0C5.373 0 0 5.373 0 12h4zm2 5.291A7.962 7.962 0 014 12H0c0 3.042 1.135 5.824 3 7.938l3-2.647z"></path>
                </svg>
                更新中...
              </span>
              <span v-else>更新文章</span>
            </button>
          </div>
        </div>
      </form>
    </div>
  </div>
</template>

<script setup>
import { ref, reactive, onMounted, computed } from 'vue'
import { useRouter, useRoute, RouterLink } from 'vue-router'
import { useAuthStore } from '../stores/auth'
import { postsApi } from '../api/posts'
import RichTextEditor from '../components/RichTextEditor.vue'
import BadmintonCourtDiagram from '../components/BadmintonCourtDiagram.vue'

const router = useRouter()
const route = useRoute()
const authStore = useAuthStore()

const postId = computed(() => route.params.id)

const formData = reactive({
  title: '',
  content: '',
  categoryId: null,
  categoryName: '',
  authorId: null,
  updatedAt: null
})

const loading = ref(true)
const submitting = ref(false)
const showDiagramModal = ref(false)
const isEditingDiagram = ref(false)
const diagramDraft = ref({ players: [], shuttle: null, arrows: [], textAnnotations: [], description: '' })
const editorRef = ref(null)
const currentDiagramNode = ref(null)

const titleMaxLen = 200
const titleLength = computed(() => formData.title?.length || 0)
const canSubmit = computed(() => {
  const hasTitle = formData.title.trim().length > 0 && formData.title.trim().length <= titleMaxLen
  const hasContent = formData.content && formData.content.replace(/<[^>]*>/g, '').trim().length > 0
  return hasTitle && hasContent && !loading.value && !submitting.value
})

// 從編輯器獲取戰術圖狀態
const hasDiagram = computed(() => editorRef.value?.hasDiagram || false)

const formatDate = (date) => {
  return new Date(date).toLocaleString('zh-TW', {
    year: 'numeric',
    month: '2-digit',
    day: '2-digit',
    hour: '2-digit',
    minute: '2-digit'
  })
}

const openDiagramModal = () => {
  diagramDraft.value = { players: [], shuttle: null, arrows: [], textAnnotations: [], description: '' }
  isEditingDiagram.value = false
  showDiagramModal.value = true
}

const handleEditDiagram = (eventData) => {
  currentDiagramNode.value = eventData
  // 深拷貝資料，避免直接修改原始資料
  diagramDraft.value = JSON.parse(JSON.stringify(eventData.data || { players: [], shuttle: null, arrows: [], textAnnotations: [], description: '' }))
  isEditingDiagram.value = true
  showDiagramModal.value = true
}

const closeDiagramModal = () => { 
  showDiagramModal.value = false
  currentDiagramNode.value = null
}

const confirmDiagram = () => {
  // 深拷貝資料，確保資料是獨立的
  const data = JSON.parse(JSON.stringify(diagramDraft.value || {}))
  
  if (currentDiagramNode.value) {
    // 使用 getPos 函數獲取當前位置並更新戰術圖
    const { getPos } = currentDiagramNode.value
    
    if (typeof getPos === 'function') {
      const pos = getPos()
      const success = editorRef.value?.updateDiagram(pos, data)
      
      if (!success) {
        alert('更新戰術圖失敗，請重試')
        return
      }
    } else {
      alert('無法更新戰術圖，請重試')
      return
    }
  } else {
    // 插入新的戰術圖
    editorRef.value?.insertDiagram(data)
  }
  
  showDiagramModal.value = false
  currentDiagramNode.value = null
}

const submitPost = async () => {
  if (!canSubmit.value) return
  submitting.value = true
  try {
    await postsApi.updatePost(postId.value, {
      title: formData.title.trim(),
      content: formData.content
    })
    router.push(`/posts/${postId.value}`)
  } catch (error) {
    console.error('Failed to update post:', error)
    if (error.response?.status === 403) {
      alert('您沒有權限編輯此文章')
    } else {
      alert('更新文章失敗，請重試')
    }
  } finally {
    submitting.value = false
  }
}

// 載入文章資料
const loadPost = async () => {
  loading.value = true
  try {
    const response = await postsApi.getPost(postId.value)
    const post = response.data
    
    // 檢查權限：只有作者可以編輯
    if (post.authorId !== authStore.user?.id) {
      alert('您沒有權限編輯此文章')
      router.push(`/posts/${postId.value}`)
      return
    }
    
    // 填入表單資料
    formData.title = post.title
    formData.content = post.content
    formData.categoryId = post.categoryId
    formData.categoryName = post.categoryName
    formData.authorId = post.authorId
    formData.updatedAt = post.updatedAt
  } catch (error) {
    console.error('Failed to fetch post:', error)
    alert('載入文章失敗')
    router.push(`/posts/${postId.value}`)
  } finally {
    loading.value = false
  }
}

onMounted(() => {
  if (!authStore.isAuthenticated) {
    router.push('/login')
    return
  }
  loadPost()
})
</script>

<style scoped>
.edit-post { width: 100%; }

.editor-toolbar-custom { display: flex; gap: 0.5rem; }

/* Simple modal styling */
.modal-overlay {
  position: fixed;
  inset: 0;
  background: rgba(0,0,0,0.5);
  display: flex;
  align-items: center;
  justify-content: center;
  z-index: 50;
  padding: 1rem;
}
.modal-card {
  background: var(--color-surface, #fff);
  color: inherit;
  width: min(960px, 100%);
  max-height: 90vh;
  border-radius: 12px;
  padding: 1rem 1rem 1.25rem;
  box-shadow: 0 10px 30px rgba(0,0,0,0.2);
}
.modal-body {
  border: 1px solid rgba(0,0,0,0.1);
  border-radius: 8px;
  padding: 0.75rem;
  background: #fff;
  max-height: 70vh;
  overflow: auto;
}

.diagram-wrapper {
  width: 100%;
  height: 60vh;
  min-height: 420px;
}

/* Vue 3 deep selector to style inner Konva/canvas container */
:deep(.diagram-wrapper) canvas,
:deep(.diagram-wrapper .konvajs-content) {
  max-width: 100% !important;
}

:root.dark .modal-card { background: #111827; }
:root.dark .modal-body { background: #0b1220; border-color: #1f2937; }
</style>