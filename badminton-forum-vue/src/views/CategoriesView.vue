<template>
  <div>
    <!-- Page Header -->
    <div class="mb-8">
      <h1 class="text-3xl font-bold text-gray-900 dark:text-white">論壇版塊</h1>
      <p class="mt-2 text-gray-600 dark:text-gray-300">選擇您感興趣的版塊，參與討論</p>
    </div>
    
    <!-- Loading State -->
    <div v-if="loading" class="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-3 gap-6">
      <div v-for="i in 6" :key="i" class="animate-pulse">
        <div class="card-dark">
          <div class="h-12 w-12 bg-gray-300 dark:bg-gray-600 rounded-full mb-4"></div>
          <div class="h-6 bg-gray-300 dark:bg-gray-600 rounded mb-2"></div>
          <div class="h-4 bg-gray-300 dark:bg-gray-600 rounded w-3/4"></div>
        </div>
      </div>
    </div>
    
    <!-- Categories Grid -->
    <div v-else class="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-3 gap-6">
      <div
        v-for="category in categories"
        :key="category.id"
        @click="goToCategory(category.id)"
        class="card-dark hover:shadow-xl transition-all duration-300 cursor-pointer group"
      >
        <!-- Category Icon -->
        <div class="flex items-center justify-center w-16 h-16 mb-4 text-3xl bg-primary-50 dark:bg-primary-900/20 rounded-full group-hover:scale-110 transition-transform">
          {{ category.icon }}
        </div>
        
        <!-- Category Info -->
        <h3 class="text-xl font-semibold text-gray-900 dark:text-white mb-2 group-hover:text-primary-600 dark:group-hover:text-primary-400">
          {{ category.name }}
        </h3>
        <p class="text-gray-600 dark:text-gray-300 mb-4">
          {{ category.description }}
        </p>
        
        <!-- Category Stats -->
        <div class="flex items-center justify-between border-t border-gray-200 dark:border-gray-700 pt-4">
          <div class="flex items-center space-x-4 text-sm text-gray-500 dark:text-gray-400">
            <span class="flex items-center">
              <svg class="w-4 h-4 mr-1" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M9 12h6m-6 4h6m2 5H7a2 2 0 01-2-2V5a2 2 0 012-2h5.586a1 1 0 01.707.293l5.414 5.414a1 1 0 01.293.707V19a2 2 0 01-2 2z" />
              </svg>
              {{ category.postCount || 0 }} 篇文章
            </span>
          </div>
          <svg class="w-5 h-5 text-gray-400 group-hover:text-primary-600 dark:group-hover:text-primary-400 transition-colors" fill="none" stroke="currentColor" viewBox="0 0 24 24">
            <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M9 5l7 7-7 7" />
          </svg>
        </div>
      </div>
    </div>
    
    <!-- Empty State -->
    <div v-if="!loading && categories.length === 0" class="text-center py-12">
      <svg class="mx-auto h-12 w-12 text-gray-400" fill="none" viewBox="0 0 24 24" stroke="currentColor">
        <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M19 11H5m14 0a2 2 0 012 2v6a2 2 0 01-2 2H5a2 2 0 01-2-2v-6a2 2 0 012-2m14 0V9a2 2 0 00-2-2M5 11V9a2 2 0 012-2m0 0V5a2 2 0 012-2h6a2 2 0 012 2v2M7 7h10" />
      </svg>
      <h3 class="mt-2 text-sm font-medium text-gray-900 dark:text-white">暫無版塊</h3>
      <p class="mt-1 text-sm text-gray-500 dark:text-gray-400">目前還沒有任何論壇版塊。</p>
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