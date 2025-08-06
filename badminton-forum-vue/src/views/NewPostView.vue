<template>
  <div class="new-post">
    <h1>發表新文章</h1>
    
    <form @submit.prevent="submitPost">
      <div class="form-group">
        <label for="category">選擇版塊</label>
        <select id="category" v-model.number="formData.categoryId" required>
          <option value="">請選擇版塊</option>
          <option v-for="category in categories" :key="category.id" :value="category.id">
            {{ category.name }}
          </option>
        </select>
      </div>
      
      <div class="form-group">
        <label for="title">文章標題</label>
        <input
          id="title"
          v-model="formData.title"
          type="text"
          required
          placeholder="請輸入文章標題"
        />
      </div>
      
      <div class="form-group">
        <label for="content">文章內容</label>
        <div class="editor-toolbar-custom">
          <button
            type="button"
            @click="toggleDiagramMode"
            class="diagram-btn"
            :class="{ active: showDiagram }"
          >
            🏸 插入戰術圖
          </button>
        </div>
        <RichTextEditor
          v-if="!showDiagram"
          v-model="formData.content"
          placeholder="請輸入文章內容..."
        />
        <BadmintonCourtDiagram
          v-else
          v-model="diagramData"
        />
      </div>
      
      <div class="form-actions">
        <button type="submit" :disabled="loading">
          {{ loading ? '發表中...' : '發表文章' }}
        </button>
        <RouterLink to="/" class="cancel-btn">取消</RouterLink>
      </div>
    </form>
  </div>
</template>

<script setup>
import { ref, reactive, onMounted, watch } from 'vue'
import { useRouter, RouterLink } from 'vue-router'
import { categoriesApi } from '../api/categories'
import { postsApi } from '../api/posts'
import RichTextEditor from '../components/RichTextEditor.vue'
import BadmintonCourtDiagram from '../components/BadmintonCourtDiagram.vue'

const router = useRouter()

const formData = reactive({
  categoryId: '',
  title: '',
  content: ''
})

const loading = ref(false)
const categories = ref([])
const showDiagram = ref(false)
const diagramData = ref({
  players: [],
  shuttle: null,
  arrows: [],
  description: ''
})

const toggleDiagramMode = () => {
  showDiagram.value = !showDiagram.value
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
    if (!formData.content.includes('badminton-diagram-placeholder')) {
      formData.content += diagramHtml
    } else {
      // 更新現有的戰術圖
      formData.content = formData.content.replace(
        /<div class="badminton-diagram-placeholder".*?<\/div>/s,
        diagramHtml
      )
    }
  }
}, { deep: true })

const submitPost = async () => {
  loading.value = true
  
  try {
    const response = await postsApi.createPost(formData)
    router.push(`/post/${response.data.id}`)
  } catch (error) {
    console.error('Failed to create post:', error)
    alert('發表文章失敗，請重試')
  } finally {
    loading.value = false
  }
}

onMounted(async () => {
  try {
    const response = await categoriesApi.getCategories()
    categories.value = response.data
  } catch (error) {
    console.error('Failed to fetch categories:', error)
  }
})
</script>

<style scoped>
.new-post {
  /* 預設不設置最小寬度，保持響應式 */
  width: 100%;
}

/* 只在大螢幕上設定最小寬度，確保桌面版寬度一致 */
@media (min-width: 1024px) {
  .new-post {
    min-width: 1088px;
  }
}

h1 {
  color: #2c3e50;
  margin-bottom: 2rem;
}

form {
  background: white;
  padding: 2rem;
  border-radius: 8px;
  box-shadow: 0 2px 4px rgba(0,0,0,0.1);
  max-width: 800px;
  margin: 0 auto;
}

.form-group {
  margin-bottom: 1.5rem;
}

label {
  display: block;
  margin-bottom: 0.5rem;
  color: #555;
  font-weight: 500;
}

input,
select {
  width: 100%;
  padding: 0.75rem;
  border: 1px solid #ddd;
  border-radius: 4px;
  font-size: 1rem;
  font-family: inherit;
}

input:focus,
select:focus {
  outline: none;
  border-color: #3498db;
}

.form-actions {
  display: flex;
  gap: 1rem;
  justify-content: flex-end;
}

button[type="submit"] {
  background-color: #3498db;
  color: white;
  padding: 0.75rem 2rem;
  border: none;
  border-radius: 4px;
  font-size: 1rem;
  cursor: pointer;
  transition: background-color 0.3s;
}

button[type="submit"]:hover:not(:disabled) {
  background-color: #2980b9;
}

button[type="submit"]:disabled {
  background-color: #95a5a6;
  cursor: not-allowed;
}

.cancel-btn {
  background-color: #95a5a6;
  color: white;
  padding: 0.75rem 2rem;
  border-radius: 4px;
  text-decoration: none;
  display: inline-flex;
  align-items: center;
  transition: background-color 0.3s;
}

.cancel-btn:hover {
  background-color: #7f8c8d;
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