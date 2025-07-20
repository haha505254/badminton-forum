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
        <RichTextEditor
          v-model="formData.content"
          placeholder="請輸入文章內容..."
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
import { ref, reactive, onMounted } from 'vue'
import { useRouter, RouterLink } from 'vue-router'
import { categoriesApi } from '../api/categories'
import { postsApi } from '../api/posts'
import RichTextEditor from '../components/RichTextEditor.vue'

const router = useRouter()

const formData = reactive({
  categoryId: '',
  title: '',
  content: ''
})

const loading = ref(false)
const categories = ref([])

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
  max-width: 800px;
  margin: 0 auto;
  padding: 2rem;
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
</style>