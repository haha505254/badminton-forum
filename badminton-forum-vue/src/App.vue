<script setup>
import { ref } from 'vue'
import { RouterView, RouterLink, useRouter } from 'vue-router'
import { useAuthStore } from './stores/auth'

const router = useRouter()
const authStore = useAuthStore()
const mobileMenuOpen = ref(false)

const handleLogout = () => {
  authStore.logout()
  router.push('/')
}
</script>

<template>
  <div id="app" class="min-h-screen flex flex-col bg-gray-50 dark:bg-gray-900">
    <!-- Navigation Bar -->
    <nav class="sticky top-0 z-50 bg-white shadow-md dark:bg-gray-800">
      <div class="max-w-7xl mx-auto px-4 sm:px-6 lg:px-8">
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
            <RouterLink to="/categories" class="nav-link">版塊</RouterLink>
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
          <RouterLink to="/categories" class="mobile-nav-link">版塊</RouterLink>
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
    <main class="flex-1 max-w-7xl mx-auto px-4 sm:px-6 lg:px-8 py-8 w-full">
      <RouterView />
    </main>
    
    <!-- Footer -->
    <footer class="bg-gray-800 text-white py-8 mt-auto">
      <div class="max-w-7xl mx-auto px-4 sm:px-6 lg:px-8 text-center">
        <p>&copy; 2024 羽毛球論壇. All rights reserved.</p>
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