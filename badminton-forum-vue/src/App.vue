<script setup>
import { ref, onMounted } from 'vue'
import { RouterView, RouterLink, useRouter } from 'vue-router'
import { useAuthStore } from './stores/auth'
import { categoryData } from './data/categories'
import { categoriesApi } from './api/categories'

const router = useRouter()
const authStore = useAuthStore()
const mobileMenuOpen = ref(false)
const categoriesDropdownOpen = ref(false)
const categories = ref([])
const isDark = ref(false)

const loadTheme = () => {
  const stored = localStorage.getItem('theme')
  const preferDark = window.matchMedia && window.matchMedia('(prefers-color-scheme: dark)').matches
  const useDark = stored ? stored === 'dark' : preferDark
  isDark.value = useDark
  document.documentElement.classList.toggle('dark', useDark)
}

const toggleTheme = () => {
  isDark.value = !isDark.value
  localStorage.setItem('theme', isDark.value ? 'dark' : 'light')
  document.documentElement.classList.toggle('dark', isDark.value)
}

const handleLogout = () => {
  authStore.logout()
  router.push('/')
}

// 加載板塊數據
onMounted(async () => {
  loadTheme()
  try {
    const response = await categoriesApi.getCategories()
    categories.value = response.data.map(apiCategory => {
      const localCategory = categoryData.find(c => c.id === apiCategory.id)
      return {
        ...apiCategory,
        icon: localCategory?.icon || '📁',
        description: localCategory?.description || apiCategory.description
      }
    })
  } catch (error) {
    console.error('Failed to fetch categories:', error)
    categories.value = categoryData
  }
})
</script>

<template>
  <div id="app" class="min-h-screen flex flex-col bg-gray-50 dark:bg-gray-900">
    <!-- Navigation Bar -->
    <nav class="sticky top-0 z-50 backdrop-blur supports-[backdrop-filter]:bg-white/80 dark:supports-[backdrop-filter]:bg-gray-900/60 border-b border-gray-200/60 dark:border-gray-700/60">
      <div class="container-max">
        <div class="flex justify-between h-16">
          <!-- Logo -->
          <div class="flex items-center">
            <RouterLink to="/" class="flex items-center space-x-2 text-xl font-bold text-gray-900 dark:text-white">
              <span class="text-2xl">🏸</span>
              <span>羽毛球論壇</span>
            </RouterLink>
          </div>
          
          <!-- Navigation Links -->
          <div class="hidden md:flex items-center space-x-8">
            <!-- Theme Toggle -->
            <button @click="toggleTheme" class="mobile-menu-button" :aria-label="isDark ? '切換為淺色模式' : '切換為深色模式'">
              <svg v-if="!isDark" class="h-5 w-5" viewBox="0 0 24 24" fill="none" stroke="currentColor">
                <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M21 12.79A9 9 0 1111.21 3 7 7 0 0021 12.79z" />
              </svg>
              <svg v-else class="h-5 w-5" viewBox="0 0 24 24" fill="none" stroke="currentColor">
                <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M12 3v2m0 14v2m9-9h-2M5 12H3m15.364 6.364l-1.414-1.414M8.05 8.05L6.636 6.636m10.728 0l-1.414 1.414M8.05 15.95l-1.414 1.414" />
              </svg>
            </button>
            <!-- Categories Dropdown -->
            <div class="relative" @mouseenter="categoriesDropdownOpen = true" @mouseleave="categoriesDropdownOpen = false">
              <RouterLink to="/categories" class="nav-link flex items-center">
                版塊
                <svg class="ml-1 w-4 h-4" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                  <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M19 9l-7 7-7-7" />
                </svg>
              </RouterLink>
              
              <!-- Dropdown Menu -->
              <div v-if="categoriesDropdownOpen" class="absolute left-0 mt-2 w-64 bg-white dark:bg-gray-800 rounded-lg shadow-lg py-2 z-50">
                <RouterLink
                  v-for="category in categories"
                  :key="category.id"
                  :to="`/category/${category.id}`"
                  class="flex items-center px-4 py-3 hover:bg-gray-100 dark:hover:bg-gray-700 transition-colors"
                  @click="categoriesDropdownOpen = false"
                >
                  <span class="text-xl mr-3">{{ category.icon }}</span>
                  <div>
                    <div class="text-sm font-medium text-gray-900 dark:text-white">{{ category.name }}</div>
                    <div class="text-xs text-gray-500 dark:text-gray-400">{{ category.description }}</div>
                  </div>
                </RouterLink>
              </div>
            </div>
            
            <RouterLink to="/search" class="nav-link">搜尋</RouterLink>
            
            <template v-if="authStore.isAuthenticated">
              <RouterLink to="/new-post" class="nav-link">發表文章</RouterLink>
              <RouterLink :to="`/profile/${authStore.user.id}`" class="nav-link">
                {{ authStore.user.username }}
              </RouterLink>
              <RouterLink to="/settings" class="nav-link">設置</RouterLink>
              <RouterLink v-if="authStore.user?.isAdmin" to="/admin" class="nav-link text-primary-600">
                管理
              </RouterLink>
              <button @click="handleLogout" class="btn-outline">
                登出
              </button>
            </template>
            
            <template v-else>
              <RouterLink to="/login" class="nav-link">登入</RouterLink>
              <RouterLink to="/register" class="btn-primary">註冊</RouterLink>
            </template>
          </div>
          
          <!-- Mobile menu button -->
          <div class="md:hidden flex items-center">
            <button @click="mobileMenuOpen = !mobileMenuOpen" class="mobile-menu-button">
              <svg class="h-6 w-6" fill="none" viewBox="0 0 24 24" stroke="currentColor">
                <path v-if="!mobileMenuOpen" stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M4 6h16M4 12h16M4 18h16" />
                <path v-else stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M6 18L18 6M6 6l12 12" />
              </svg>
            </button>
          </div>
        </div>
      </div>
      
      <!-- Mobile menu -->
      <div v-if="mobileMenuOpen" class="md:hidden">
        <div class="px-2 pt-2 pb-3 space-y-1 sm:px-3">
          <!-- Categories with Submenu -->
          <div>
            <button @click="categoriesDropdownOpen = !categoriesDropdownOpen" class="mobile-nav-link w-full flex items-center justify-between">
              <span>版塊</span>
              <svg class="w-4 h-4 transition-transform" :class="{ 'rotate-180': categoriesDropdownOpen }" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M19 9l-7 7-7-7" />
              </svg>
            </button>
            
            <!-- Categories Submenu -->
            <div v-if="categoriesDropdownOpen" class="pl-4 mt-2 space-y-1">
              <RouterLink
                v-for="category in categories"
                :key="category.id"
                :to="`/category/${category.id}`"
                class="mobile-nav-link text-sm flex items-center"
                @click="mobileMenuOpen = false"
              >
                <span class="mr-2">{{ category.icon }}</span>
                {{ category.name }}
              </RouterLink>
            </div>
          </div>
          
          <RouterLink to="/search" class="mobile-nav-link">搜尋</RouterLink>
          
          <template v-if="authStore.isAuthenticated">
            <RouterLink to="/new-post" class="mobile-nav-link">發表文章</RouterLink>
            <RouterLink :to="`/profile/${authStore.user.id}`" class="mobile-nav-link">
              {{ authStore.user.username }}
            </RouterLink>
            <RouterLink to="/settings" class="mobile-nav-link">設置</RouterLink>
            <RouterLink v-if="authStore.user?.isAdmin" to="/admin" class="mobile-nav-link text-primary-600">
              管理
            </RouterLink>
            <button @click="handleLogout" class="mobile-nav-link text-left w-full">
              登出
            </button>
          </template>
          
          <template v-else>
            <RouterLink to="/login" class="mobile-nav-link">登入</RouterLink>
            <RouterLink to="/register" class="mobile-nav-link">註冊</RouterLink>
          </template>
        </div>
      </div>
    </nav>
    
    <!-- Main Content -->
    <main class="flex-1 container-max py-8 w-full">
      <RouterView />
    </main>
    
    <!-- Footer -->
    <footer class="mt-auto border-t border-gray-200 dark:border-gray-700">
      <div class="container-max py-8 text-center text-sm text-gray-600 dark:text-gray-400">
        <p>&copy; 2024 羽毛球論壇</p>
      </div>
    </footer>
  </div>
</template>

<style scoped>
.nav-link {
  @apply text-gray-700 hover:text-primary-600 px-3 py-2 rounded-md text-sm font-medium transition-colors dark:text-gray-200 dark:hover:text-primary-400;
}

.nav-link.router-link-active {
  @apply text-primary-600 dark:text-primary-400;
}

.mobile-menu-button {
  @apply inline-flex items-center justify-center p-2 rounded-md text-gray-700 hover:text-primary-600 hover:bg-gray-100 focus:outline-none focus:ring-2 focus:ring-inset focus:ring-primary-500 dark:text-gray-200 dark:hover:text-primary-400 dark:hover:bg-gray-700;
}

.mobile-nav-link {
  @apply block px-3 py-2 rounded-md text-base font-medium text-gray-700 hover:text-primary-600 hover:bg-gray-50 dark:text-gray-200 dark:hover:text-primary-400 dark:hover:bg-gray-700;
}

.mobile-nav-link.router-link-active {
  @apply bg-primary-50 text-primary-600 dark:bg-primary-900/20 dark:text-primary-400;
}
</style>