<template>
  <div class="category-view">
    <h1>{{ category.name }}</h1>
    <p>{{ category.description }}</p>
    
    <div class="posts">
      <div v-for="post in posts" :key="post.id" class="post-item">
        <RouterLink :to="`/post/${post.id}`">
          <h3>{{ post.title }}</h3>
        </RouterLink>
        <div class="post-meta">
          <span>作者: {{ post.authorName }}</span>
          <span>回覆: {{ post.replyCount }}</span>
          <span>瀏覽: {{ post.viewCount }}</span>
        </div>
      </div>
    </div>
  </div>
</template>

<script setup>
import { ref, onMounted } from 'vue'
import { useRoute, RouterLink } from 'vue-router'
import { categoriesApi } from '../api/categories'

const route = useRoute()

const category = ref({
  name: '載入中...',
  description: ''
})

const posts = ref([])
const loading = ref(true)

onMounted(async () => {
  const categoryId = route.params.id
  
  try {
    // Fetch category info
    const categoryResponse = await categoriesApi.getCategory(categoryId)
    category.value = categoryResponse.data
    
    // Fetch posts
    const postsResponse = await categoriesApi.getCategoryPosts(categoryId)
    posts.value = postsResponse.data
  } catch (error) {
    console.error('Failed to fetch category data:', error)
  } finally {
    loading.value = false
  }
})
</script>

<style scoped>
.category-view {
  padding: 2rem;
}

.posts {
  margin-top: 2rem;
}

.post-item {
  background: white;
  padding: 1.5rem;
  margin-bottom: 1rem;
  border-radius: 8px;
  box-shadow: 0 2px 4px rgba(0,0,0,0.1);
}

.post-item h3 {
  margin: 0 0 0.5rem 0;
  color: #2c3e50;
}

.post-meta {
  color: #666;
  font-size: 0.9rem;
  display: flex;
  gap: 1rem;
}
</style>