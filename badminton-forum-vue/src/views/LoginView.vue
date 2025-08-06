<template>
  <div class="min-h-[calc(100vh-200px)] flex items-center justify-center px-4 sm:px-6 lg:px-8">
    <div class="max-w-md w-full">
      <!-- Login Card -->
      <div class="card-dark">
        <!-- Header -->
        <div class="text-center mb-8">
          <div class="mx-auto h-12 w-12 text-primary-600 dark:text-primary-400">
            <svg fill="none" viewBox="0 0 24 24" stroke="currentColor">
              <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M16 7a4 4 0 11-8 0 4 4 0 018 0zM12 14a7 7 0 00-7 7h14a7 7 0 00-7-7z" />
            </svg>
          </div>
          <h2 class="mt-4 text-2xl font-bold text-gray-900 dark:text-white">
            登入您的帳戶
          </h2>
          <p class="mt-2 text-sm text-gray-600 dark:text-gray-400">
            歡迎回到羽毛球論壇
          </p>
        </div>
        
        <!-- Error Message -->
        <div v-if="error" class="mb-6 bg-red-50 dark:bg-red-900/20 border border-red-200 dark:border-red-800 text-red-600 dark:text-red-400 px-4 py-3 rounded-md text-sm">
          {{ error }}
        </div>
        
        <!-- Google Sign In -->
        <div class="mb-6">
          <GoogleSignInButton 
            button-text="使用 Google 登入"
            mode="login"
            @success="handleGoogleSuccess"
            @error="handleGoogleError"
          />
          
          <!-- Divider -->
          <div class="relative my-6">
            <div class="absolute inset-0 flex items-center">
              <div class="w-full border-t border-gray-300 dark:border-gray-600"></div>
            </div>
            <div class="relative flex justify-center text-sm">
              <span class="px-2 bg-white dark:bg-gray-800 text-gray-500">或使用電子郵件登入</span>
            </div>
          </div>
        </div>
        
        <!-- Login Form -->
        <form @submit.prevent="handleLogin" class="space-y-6">
          <div>
            <label for="email" class="form-label">
              電子郵件
            </label>
            <input
              id="email"
              v-model="formData.email"
              type="email"
              required
              class="form-input"
              placeholder="請輸入電子郵件"
            />
          </div>
          
          <div>
            <label for="password" class="form-label">
              密碼
            </label>
            <input
              id="password"
              v-model="formData.password"
              type="password"
              required
              class="form-input"
              placeholder="請輸入密碼"
            />
          </div>
          
          <div class="flex items-center justify-between">
            <div class="flex items-center">
              <input
                id="remember-me"
                type="checkbox"
                class="h-4 w-4 text-primary-600 focus:ring-primary-500 border-gray-300 rounded"
              />
              <label for="remember-me" class="ml-2 block text-sm text-gray-700 dark:text-gray-300">
                記住我
              </label>
            </div>
            
            <div class="text-sm">
              <RouterLink to="/forgot-password" class="font-medium text-primary-600 hover:text-primary-500 dark:text-primary-400">
                忘記密碼？
              </RouterLink>
            </div>
          </div>
          
          <div>
            <button
              type="submit"
              class="w-full btn-primary"
              :disabled="loading"
            >
              <span v-if="loading" class="flex items-center justify-center">
                <svg class="animate-spin -ml-1 mr-3 h-5 w-5 text-white" fill="none" viewBox="0 0 24 24">
                  <circle class="opacity-25" cx="12" cy="12" r="10" stroke="currentColor" stroke-width="4"></circle>
                  <path class="opacity-75" fill="currentColor" d="M4 12a8 8 0 018-8V0C5.373 0 0 5.373 0 12h4zm2 5.291A7.962 7.962 0 014 12H0c0 3.042 1.135 5.824 3 7.938l3-2.647z"></path>
                </svg>
                登入中...
              </span>
              <span v-else>登入</span>
            </button>
          </div>
          
          <div class="text-center text-sm text-gray-600 dark:text-gray-400">
            還沒有帳號？
            <RouterLink to="/register" class="font-medium text-primary-600 hover:text-primary-500 dark:text-primary-400">
              立即註冊
            </RouterLink>
          </div>
        </form>
      </div>
    </div>
  </div>
</template>

<script setup>
import { ref, reactive } from 'vue'
import { useRouter, RouterLink } from 'vue-router'
import { useAuthStore } from '../stores/auth'
import GoogleSignInButton from '@/components/GoogleSignInButton.vue'

const router = useRouter()
const authStore = useAuthStore()

const formData = reactive({
  email: '',
  password: ''
})

const loading = ref(false)
const error = ref('')

const handleLogin = async () => {
  loading.value = true
  error.value = ''
  
  const result = await authStore.login(formData)
  
  if (result.success) {
    const redirect = router.currentRoute.value.query.redirect || '/'
    router.push(redirect)
  } else {
    error.value = result.message
  }
  
  loading.value = false
}

const handleGoogleSuccess = (result) => {
  const redirect = router.currentRoute.value.query.redirect || '/'
  router.push(redirect)
}

const handleGoogleError = (errorMessage) => {
  error.value = errorMessage
}
</script>