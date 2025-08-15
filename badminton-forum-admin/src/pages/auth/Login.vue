<template>
  <VaForm ref="form" @submit.prevent="submit">
    <h1 class="font-semibold text-4xl mb-4">羽球論壇管理後台</h1>
    <p class="text-base mb-4 leading-5">
      請使用管理員帳號登入
    </p>
    
    <VaInput
      v-model="formData.email"
      :rules="[validators.required, validators.email]"
      label="Email"
      type="email"
      class="mb-4"
    />
    
    <VaInput
      v-model="formData.password"
      :rules="[validators.required]"
      label="密碼"
      type="password"
      class="mb-4"
    />
    
    <div class="flex justify-between items-center mb-4">
      <VaCheckbox
        v-model="formData.rememberMe"
        label="記住我"
        class="mb-0"
      />
      <RouterLink
        :to="{ name: 'recover-password' }"
        class="text-primary hover:underline"
      >
        忘記密碼？
      </RouterLink>
    </div>
    
    <VaButton
      type="submit"
      :loading="loading"
      :disabled="loading"
      class="w-full mb-4"
    >
      登入
    </VaButton>
    
    <VaDivider orientation="center" class="mb-4">
      <span class="px-2">或</span>
    </VaDivider>
    
    <VaButton
      preset="secondary"
      class="w-full mb-2"
      @click="googleLogin"
      :disabled="loading"
    >
      <VaIcon name="mdi-google" class="mr-2" />
      使用 Google 登入
    </VaButton>
    
    <p class="text-center mt-4">
      還沒有帳號？
      <RouterLink
        :to="{ name: 'signup' }"
        class="text-primary hover:underline"
      >
        註冊新帳號
      </RouterLink>
    </p>
  </VaForm>
</template>

<script setup lang="ts">
import { ref } from 'vue'
import { useRouter } from 'vue-router'
import { useToast } from 'vuestic-ui'
import { useAuthStore } from '../../stores/auth'

const router = useRouter()
const authStore = useAuthStore()
const { init: notify } = useToast()

const form = ref()
const loading = ref(false)

const formData = ref({
  email: '',
  password: '',
  rememberMe: false
})

const validators = {
  required: (value: string) => !!value || '此欄位為必填',
  email: (value: string) => {
    const pattern = /^[^\s@]+@[^\s@]+\.[^\s@]+$/
    return pattern.test(value) || '請輸入有效的 Email'
  }
}

async function submit() {
  const isValid = form.value?.validate()
  if (!isValid) return
  
  loading.value = true
  try {
    const result = await authStore.login({
      email: formData.value.email,
      password: formData.value.password
    })
    
    if (result.success) {
      // 檢查是否為管理員
      if (!authStore.user?.isAdmin) {
        notify({
          message: '您沒有管理員權限',
          color: 'danger'
        })
        authStore.logout()
        return
      }
      
      notify({
        message: '登入成功',
        color: 'success'
      })
      router.push({ name: 'dashboard' })
    } else {
      notify({
        message: result.message || '登入失敗',
        color: 'danger'
      })
    }
  } catch (error) {
    notify({
      message: '登入時發生錯誤',
      color: 'danger'
    })
  } finally {
    loading.value = false
  }
}

async function googleLogin() {
  // 載入 Google Sign-In SDK
  if (!window.google) {
    notify({
      message: 'Google 登入服務載入中，請稍後再試',
      color: 'warning'
    })
    return
  }
  
  loading.value = true
  try {
    // 初始化 Google Sign-In
    window.google.accounts.id.initialize({
      client_id: import.meta.env.VITE_GOOGLE_CLIENT_ID,
      callback: async (response) => {
        const result = await authStore.googleLogin(response.credential)
        
        if (result.success) {
          // 檢查是否為管理員
          if (!authStore.user?.isAdmin) {
            notify({
              message: '您沒有管理員權限',
              color: 'danger'
            })
            authStore.logout()
            return
          }
          
          notify({
            message: '登入成功',
            color: 'success'
          })
          router.push({ name: 'dashboard' })
        } else {
          notify({
            message: result.message || 'Google 登入失敗',
            color: 'danger'
          })
        }
      }
    })
    
    // 顯示 Google 登入按鈕
    window.google.accounts.id.prompt()
  } catch (error) {
    notify({
      message: 'Google 登入失敗',
      color: 'danger'
    })
  } finally {
    loading.value = false
  }
}

// 載入 Google Sign-In SDK
function loadGoogleSDK() {
  const script = document.createElement('script')
  script.src = 'https://accounts.google.com/gsi/client'
  script.async = true
  script.defer = true
  document.head.appendChild(script)
}

// 組件掛載時載入 SDK
loadGoogleSDK()
</script>