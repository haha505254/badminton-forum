<template>
  <div class="categories">
    <h1>論壇版塊</h1>
    
    <div class="category-grid">
      <div
        v-for="category in categories"
        :key="category.id"
        class="category-card"
        @click="goToCategory(category.id)"
      >
        <div class="category-icon">{{ category.icon }}</div>
        <h3>{{ category.name }}</h3>
        <p>{{ category.description }}</p>
        <div class="category-stats">
          <span>{{ category.postCount || 0 }} 篇文章</span>
        </div>
      </div>
    </div>
  </div>
</template>

<script setup>
import { ref, onMounted } from 'vue'
import { useRouter } from 'vue-router'
import { categoriesApi } from '../api/categories'

const router = useRouter()
const categories = ref([])
const loading = ref(true)

const goToCategory = (categoryId) => {
  router.push(`/category/${categoryId}`)
}

onMounted(async () => {
  try {
    const response = await categoriesApi.getCategories()
    categories.value = response.data
  } catch (error) {
    console.error('Failed to fetch categories:', error)
  } finally {
    loading.value = false
  }
})
</script>

<style scoped>
.categories {
  padding: 2rem;
}

h1 {
  color: #2c3e50;
  margin-bottom: 2rem;
  text-align: center;
}

.category-grid {
  display: grid;
  grid-template-columns: repeat(auto-fill, minmax(300px, 1fr));
  gap: 2rem;
}

.category-card {
  background: white;
  padding: 2rem;
  border-radius: 8px;
  box-shadow: 0 2px 8px rgba(0,0,0,0.1);
  cursor: pointer;
  transition: all 0.3s;
  text-align: center;
}

.category-card:hover {
  transform: translateY(-5px);
  box-shadow: 0 4px 12px rgba(0,0,0,0.15);
}

.category-icon {
  font-size: 3rem;
  margin-bottom: 1rem;
}

.category-card h3 {
  color: #2c3e50;
  margin-bottom: 0.5rem;
}

.category-card p {
  color: #666;
  margin-bottom: 1rem;
}

.category-stats {
  color: #999;
  font-size: 0.9rem;
}
</style>