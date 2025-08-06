<template>
  <div class="google-signin-wrapper">
    <!-- 自定義按鈕 -->
    <button
      @click="handleGoogleSignIn"
      class="google-signin-button"
      :disabled="loading"
      v-if="!useOfficialButton"
    >
      <svg class="google-icon" viewBox="0 0 24 24">
        <path fill="#4285F4" d="M22.56 12.25c0-.78-.07-1.53-.2-2.25H12v4.26h5.92c-.26 1.37-1.04 2.53-2.21 3.31v2.77h3.57c2.08-1.92 3.28-4.74 3.28-8.09z"/>
        <path fill="#34A853" d="M12 23c2.97 0 5.46-.98 7.28-2.66l-3.57-2.77c-.98.66-2.23 1.06-3.71 1.06-2.86 0-5.29-1.93-6.16-4.53H2.18v2.84C3.99 20.53 7.7 23 12 23z"/>
        <path fill="#FBBC05" d="M5.84 14.09c-.22-.66-.35-1.36-.35-2.09s.13-1.43.35-2.09V7.07H2.18C1.43 8.55 1 10.22 1 12s.43 3.45 1.18 4.93l2.85-2.22.81-.62z"/>
        <path fill="#EA4335" d="M12 5.38c1.62 0 3.06.56 4.21 1.64l3.15-3.15C17.45 2.09 14.97 1 12 1 7.7 1 3.99 3.47 2.18 7.07l3.66 2.84c.87-2.6 3.3-4.53 6.16-4.53z"/>
      </svg>
      <span v-if="!loading">{{ buttonText }}</span>
      <span v-else>處理中...</span>
    </button>
    
    <!-- Google 官方按鈕容器 -->
    <div id="google-signin-button" v-if="useOfficialButton"></div>
  </div>
</template>

<script setup>
import { ref, onMounted } from 'vue'
import { useAuthStore } from '@/stores/auth'
import { useRouter } from 'vue-router'

const props = defineProps({
  buttonText: {
    type: String,
    default: '使用 Google 登入'
  },
  mode: {
    type: String,
    default: 'login' // 'login' or 'register'
  },
  useOfficialButton: {
    type: Boolean,
    default: false // 預設使用自定義按鈕
  }
})

const emit = defineEmits(['success', 'error'])

const authStore = useAuthStore()
const router = useRouter()
const loading = ref(false)

let googleInitialized = false

const initializeGoogleSignIn = () => {
  if (googleInitialized || !window.google) return

  const clientId = import.meta.env.VITE_GOOGLE_CLIENT_ID
  
  if (!clientId || clientId === 'YOUR_GOOGLE_CLIENT_ID.apps.googleusercontent.com') {
    console.error('Google Client ID is not properly configured')
    emit('error', 'Google 登入服務未正確設定')
    return
  }

  try {
    // 初始化 Google Identity Services
    window.google.accounts.id.initialize({
      client_id: clientId,
      callback: handleCredentialResponse,
      auto_select: false,
      cancel_on_tap_outside: true,
    })

    // 如果使用官方按鈕，嘗試渲染它
    if (props.useOfficialButton) {
      try {
        window.google.accounts.id.renderButton(
          document.getElementById('google-signin-button'),
          {
            theme: 'outline',
            size: 'large',
            width: '100%',
            text: props.mode === 'register' ? 'signup_with' : 'signin_with',
            locale: 'zh-TW'
          }
        )
      } catch (error) {
        console.warn('Could not render official button:', error)
      }
    }
    // 不再嘗試渲染隱藏按鈕，因為會因為 403 錯誤失敗
    // 改為在點擊時動態創建

    googleInitialized = true
    console.log('Google Sign-In initialized successfully')
  } catch (error) {
    console.error('Failed to initialize Google Sign-In:', error)
    emit('error', 'Google 登入服務初始化失敗')
  }
}

const handleGoogleSignIn = () => {
  if (!window.google || !googleInitialized) {
    console.error('Google Sign-In SDK not properly initialized')
    emit('error', 'Google 登入服務未就緒，請重新整理頁面')
    return
  }

  loading.value = true

  // 方案 1：嘗試使用 prompt（可能會被瀏覽器阻擋）
  try {
    window.google.accounts.id.disableAutoSelect()
    
    window.google.accounts.id.prompt((notification) => {
      console.log('Prompt notification:', notification)
      
      if (notification.isNotDisplayed() || notification.isSkippedMoment()) {
        console.log('Prompt failed, trying alternative method...')
        // 方案 2：創建臨時按鈕並觸發
        tryAlternativeSignIn()
      } else {
        // Prompt 成功顯示，等待使用者操作
        loading.value = false
      }
    })
  } catch (error) {
    console.error('Prompt error:', error)
    // 如果 prompt 失敗，嘗試備用方案
    tryAlternativeSignIn()
  }
}

const tryAlternativeSignIn = () => {
  try {
    // 創建臨時容器
    const tempContainer = document.createElement('div')
    tempContainer.id = 'temp-google-button'
    tempContainer.style.position = 'absolute'
    tempContainer.style.left = '-9999px'
    tempContainer.style.top = '-9999px'
    document.body.appendChild(tempContainer)

    // 渲染臨時按鈕
    window.google.accounts.id.renderButton(
      tempContainer,
      {
        theme: 'outline',
        size: 'large',
        click_listener: () => {
          console.log('Temporary button clicked')
        }
      }
    )

    // 短暫延遲後嘗試點擊
    setTimeout(() => {
      const tempButton = tempContainer.querySelector('div[role="button"]') || 
                         tempContainer.querySelector('button') ||
                         tempContainer.querySelector('iframe')
      
      if (tempButton) {
        console.log('Found temporary button, clicking...')
        if (tempButton.tagName === 'IFRAME') {
          // 如果是 iframe，可能需要不同的處理方式
          emit('error', '請在 Google Cloud Console 中確認已正確設定 http://localhost:5173 為授權來源')
        } else {
          tempButton.click()
        }
      } else {
        console.error('Could not find button in temporary container')
        emit('error', 'Google 登入服務暫時無法使用，請確認授權設定')
      }
      
      // 清理臨時容器
      setTimeout(() => {
        if (document.body.contains(tempContainer)) {
          document.body.removeChild(tempContainer)
        }
      }, 1000)
      
      loading.value = false
    }, 100)
  } catch (error) {
    console.error('Alternative sign-in error:', error)
    emit('error', 'Google 登入失敗，請稍後再試')
    loading.value = false
  }
}

const handleCredentialResponse = async (response) => {
  console.log('Received credential response')
  loading.value = true
  
  try {
    // response.credential 是 ID Token
    if (!response.credential) {
      throw new Error('未收到有效的認證資訊')
    }

    // 發送 ID token 到後端
    const result = await authStore.googleLogin(response.credential)
    
    if (result.success) {
      console.log('Google login successful')
      emit('success', result)
      // 登入成功後導向首頁
      router.push('/')
    } else {
      console.error('Google login failed:', result.message)
      emit('error', result.message || '登入失敗')
    }
  } catch (error) {
    console.error('Google login error:', error)
    emit('error', error.message || 'Google 登入失敗')
  } finally {
    loading.value = false
  }
}

const loadGoogleSignInScript = () => {
  return new Promise((resolve, reject) => {
    // 檢查是否已經載入
    if (window.google && window.google.accounts) {
      resolve()
      return
    }

    // 檢查是否已經有 script 標籤
    const existingScript = document.querySelector('script[src="https://accounts.google.com/gsi/client"]')
    if (existingScript) {
      existingScript.addEventListener('load', resolve)
      existingScript.addEventListener('error', reject)
      return
    }

    // 創建新的 script 標籤
    const script = document.createElement('script')
    script.src = 'https://accounts.google.com/gsi/client'
    script.async = true
    script.defer = true
    script.onload = () => {
      // 給 Google SDK 一點時間初始化
      setTimeout(() => {
        if (window.google && window.google.accounts) {
          initializeGoogleSignIn()
          resolve()
        } else {
          reject(new Error('Google SDK loaded but not available'))
        }
      }, 100)
    }
    script.onerror = () => {
      reject(new Error('Failed to load Google Sign-In SDK'))
    }
    document.head.appendChild(script)
  })
}

onMounted(async () => {
  try {
    await loadGoogleSignInScript()
  } catch (error) {
    console.error('Failed to load Google Sign-In SDK:', error)
    emit('error', 'Google 登入服務載入失敗')
  }
})
</script>

<style scoped>
.google-signin-wrapper {
  margin: 1rem 0;
}

.google-signin-button {
  display: flex;
  align-items: center;
  justify-content: center;
  gap: 0.75rem;
  width: 100%;
  padding: 0.75rem 1rem;
  font-size: 1rem;
  font-weight: 500;
  color: #3c4043;
  background-color: #fff;
  border: 1px solid #dadce0;
  border-radius: 0.5rem;
  cursor: pointer;
  transition: all 0.2s ease;
}

.google-signin-button:hover:not(:disabled) {
  background-color: #f8f9fa;
  border-color: #d2d3d4;
  box-shadow: 0 1px 2px 0 rgba(60, 64, 67, 0.3);
}

.google-signin-button:disabled {
  opacity: 0.6;
  cursor: not-allowed;
}

.google-icon {
  width: 20px;
  height: 20px;
}

/* 確保官方按鈕寬度正確 */
#google-signin-button {
  width: 100%;
}

#google-signin-button > div {
  width: 100% !important;
}

@media (max-width: 768px) {
  .google-signin-button {
    font-size: 0.9rem;
    padding: 0.625rem 0.875rem;
  }
}
</style>