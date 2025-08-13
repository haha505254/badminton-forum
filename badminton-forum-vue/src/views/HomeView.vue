<template>
  <div>
    <!-- Hero Section -->
    <div class="text-center mb-10">
      <h1 class="text-4xl md:text-5xl font-bold tracking-tight text-gray-900 dark:text-white mb-3">
        歡迎來到羽毛球論壇
      </h1>
      <p class="text-lg md:text-xl text-gray-600 dark:text-gray-300">
        分享您的羽毛球經驗，與球友交流技術！
      </p>
    </div>

    <!-- Categories Section -->
    <div class="mb-12">
      <h2 class="text-2xl font-bold text-gray-900 dark:text-white mb-6">論壇板塊</h2>
      <div class="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-3 gap-5">
        <RouterLink
          v-for="category in categories"
          :key="category.id"
          :to="`/category/${category.id}`"
          class="card-dark hover:shadow-xl transition-all duration-300 group block"
        >
          <div class="flex items-start space-x-4">
            <div class="flex-shrink-0 text-3xl">{{ category.icon }}</div>
            <div class="flex-1">
              <h3 class="text-lg font-semibold text-gray-900 dark:text-white group-hover:text-primary-600 dark:group-hover:text-primary-400 mb-1">
                {{ category.name }}
              </h3>
              <p class="text-sm text-gray-600 dark:text-gray-300">
                {{ category.description }}
              </p>
              <div class="mt-2 text-xs text-gray-500 dark:text-gray-400">
                {{ category.postCount || 0 }} 篇文章
              </div>
            </div>
          </div>
        </RouterLink>
      </div>
    </div>
    
    <!-- Latest Posts Section (簡化版) -->
    <div class="card-dark">
      <div class="flex items-center justify-between mb-4">
        <h2 class="text-2xl font-bold text-gray-900 dark:text-white">最新文章</h2>
        <RouterLink to="/search" class="text-sm text-primary-600 dark:text-primary-400 hover:underline">更多</RouterLink>
      </div>
      <div v-if="loadingPosts" class="space-y-3">
        <div class="h-6 bg-gray-200 dark:bg-gray-700 rounded animate-pulse"></div>
        <div class="h-6 bg-gray-200 dark:bg-gray-700 rounded animate-pulse"></div>
        <div class="h-6 bg-gray-200 dark:bg-gray-700 rounded animate-pulse"></div>
      </div>
      <ul v-else class="divide-y divide-gray-200 dark:divide-gray-700">
        <li v-for="item in latestPosts" :key="item.id" class="py-3">
          <div class="flex items-center justify-between">
            <RouterLink 
              :to="`/post/${item.id}`" 
              class="text-base font-medium text-primary-600 hover:text-primary-700 dark:text-primary-400"
            >
              {{ item.title }}
            </RouterLink>
            <span class="text-xs text-gray-500 dark:text-gray-400">回覆 {{ item.replyCount }}</span>
          </div>
          <div class="text-sm text-gray-600 dark:text-gray-400 mt-1">
            {{ item.authorName }} · {{ formatDate(item.createdAt) }}
          </div>
        </li>
      </ul>
    </div>
  </div>
</template>

<script setup>
import { ref, onMounted } from 'vue'
import { RouterLink } from 'vue-router'
import { categoryData } from '../data/categories'
import { categoriesApi } from '../api/categories'
import { postsApi } from '../api/posts'

// 板塊數據
const categories = ref([])
const latestPosts = ref([])
const loadingPosts = ref(true)

const formatDate = (date) => {
  return new Date(date).toLocaleString('zh-TW', {
    year: 'numeric',
    month: '2-digit',
    day: '2-digit',
    hour: '2-digit',
    minute: '2-digit'
  })
}

onMounted(async () => {
  // 加載板塊數據
  try {
    const response = await categoriesApi.getCategories()
    // 將 API 數據與本地配置數據合併
    categories.value = response.data.map(apiCategory => {
      const localCategory = categoryData.find(c => c.id === apiCategory.id)
      return {
        ...apiCategory,
        icon: localCategory?.icon || '📁',
        description: localCategory?.description || apiCategory.description
      }
    })
  } catch (error) {
    // 如果 API 失敗，使用本地數據
    console.error('Failed to fetch categories:', error)
    categories.value = categoryData.map(cat => ({
      ...cat,
      postCount: 0
    }))
  }

  // 加載最新文章
  try {
    const { data, headers } = await postsApi.getPosts(1, 10)
    latestPosts.value = data
  } catch (error) {
    console.error('Failed to fetch latest posts:', error)
    latestPosts.value = []
  } finally {
    loadingPosts.value = false
  }
})
</script>