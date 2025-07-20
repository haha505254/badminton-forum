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

const router = useRouter()

// Mock data - will be replaced with API call
const categories = ref([
  {
    id: 1,
    name: '技術討論',
    description: '分享和討論羽毛球技術',
    icon: '🏸',
    postCount: 156
  },
  {
    id: 2,
    name: '裝備推薦',
    description: '球拍、球鞋等裝備討論',
    icon: '🎾',
    postCount: 89
  },
  {
    id: 3,
    name: '活動公告',
    description: '比賽和活動信息',
    icon: '📅',
    postCount: 45
  },
  {
    id: 4,
    name: '球友交流',
    description: '尋找球友，組織活動',
    icon: '👥',
    postCount: 234
  }
])

const goToCategory = (categoryId) => {
  router.push(`/category/${categoryId}`)
}

onMounted(async () => {
  // TODO: Fetch categories from API
  // const response = await api.get('/categories')
  // categories.value = response.data
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