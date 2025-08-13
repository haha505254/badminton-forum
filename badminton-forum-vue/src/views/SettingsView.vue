<template>
  <div class="settings container-max">
    <h1 class="text-2xl font-bold text-gray-900 dark:text-white mb-6">帳戶設置</h1>

    <div class="grid grid-cols-1 lg:grid-cols-2 gap-6">
      <!-- Profile -->
      <div class="card-dark">
        <div class="flex items-center justify-between mb-4">
          <h2 class="text-xl font-semibold text-gray-900 dark:text-white">個人資料</h2>
        </div>
        <form @submit.prevent="updateProfile" class="space-y-4">
          <div>
            <label for="username" class="block text-sm font-medium text-gray-700 dark:text-gray-300 mb-1">用戶名</label>
            <input id="username" v-model="profile.username" type="text" disabled class="w-full rounded-md border border-gray-300 dark:border-gray-700 bg-gray-100 dark:bg-gray-800/50 px-3 py-2 text-gray-500 dark:text-gray-400" />
          </div>

          <div>
            <label for="email" class="block text-sm font-medium text-gray-700 dark:text-gray-300 mb-1">電子郵件</label>
            <input id="email" v-model="profile.email" type="email" required class="w-full rounded-md border border-gray-300 dark:border-gray-700 bg-white dark:bg-gray-800 px-3 py-2 text-gray-900 dark:text-gray-100 focus:outline-none focus:ring-2 focus:ring-primary-500" />
          </div>

          <div>
            <label for="bio" class="block text-sm font-medium text-gray-700 dark:text-gray-300 mb-1">個人簡介</label>
            <textarea id="bio" v-model="profile.bio" rows="3" placeholder="介紹一下自己..." class="w-full rounded-md border border-gray-300 dark:border-gray-700 bg-white dark:bg-gray-800 px-3 py-2 text-gray-900 dark:text-gray-100 focus:outline-none focus:ring-2 focus:ring-primary-500"></textarea>
          </div>

          <div class="grid grid-cols-1 md:grid-cols-2 gap-4">
            <div>
              <label for="playingStyle" class="block text-sm font-medium text-gray-700 dark:text-gray-300 mb-1">打球風格</label>
              <select id="playingStyle" v-model="profile.playingStyle" class="w-full rounded-md border border-gray-300 dark:border-gray-700 bg-white dark:bg-gray-800 px-3 py-2 text-gray-900 dark:text-gray-100 focus:outline-none focus:ring-2 focus:ring-primary-500">
                <option value="">請選擇</option>
                <option value="攻擊型">攻擊型</option>
                <option value="防守型">防守型</option>
                <option value="全能型">全能型</option>
              </select>
            </div>
            <div>
              <label for="yearsOfExperience" class="block text-sm font-medium text-gray-700 dark:text-gray-300 mb-1">球齡（年）</label>
              <input id="yearsOfExperience" v-model.number="profile.yearsOfExperience" type="number" min="0" max="100" placeholder="例如：5" class="w-full rounded-md border border-gray-300 dark:border-gray-700 bg-white dark:bg-gray-800 px-3 py-2 text-gray-900 dark:text-gray-100 focus:outline-none focus:ring-2 focus:ring-primary-500" />
            </div>
          </div>

          <div>
            <label for="signature" class="block text-sm font-medium text-gray-700 dark:text-gray-300 mb-1">簽名檔</label>
            <textarea id="signature" v-model="profile.signature" rows="2" placeholder="你的個性簽名..." class="w-full rounded-md border border-gray-300 dark:border-gray-700 bg-white dark:bg-gray-800 px-3 py-2 text-gray-900 dark:text-gray-100 focus:outline-none focus:ring-2 focus:ring-primary-500"></textarea>
          </div>

          <div class="flex items-center justify-end gap-3 pt-2">
            <button type="submit" class="btn-primary" :disabled="!canSaveProfile">
              <span v-if="loadingProfile" class="flex items-center">
                <svg class="animate-spin -ml-1 mr-2 h-5 w-5 text-white" fill="none" viewBox="0 0 24 24">
                  <circle class="opacity-25" cx="12" cy="12" r="10" stroke="currentColor" stroke-width="4"></circle>
                  <path class="opacity-75" fill="currentColor" d="M4 12a8 8 0 018-8V0C5.373 0 0 5.373 0 12h4zm2 5.291A7.962 7.962 0 014 12H0c0 3.042 1.135 5.824 3 7.938l3-2.647z"></path>
                </svg>
                保存中...
              </span>
              <span v-else>保存更改</span>
            </button>
          </div>
        </form>
      </div>

      <!-- Password -->
      <div class="card-dark">
        <div class="flex items-center justify-between mb-4">
          <h2 class="text-xl font-semibold text-gray-900 dark:text-white">修改密碼</h2>
        </div>
        <form @submit.prevent="changePassword" class="space-y-4">
          <div>
            <label for="currentPassword" class="block text-sm font-medium text-gray-700 dark:text-gray-300 mb-1">當前密碼</label>
            <input id="currentPassword" v-model="passwordForm.currentPassword" type="password" required class="w-full rounded-md border border-gray-300 dark:border-gray-700 bg-white dark:bg-gray-800 px-3 py-2 text-gray-900 dark:text-gray-100 focus:outline-none focus:ring-2 focus:ring-primary-500" />
          </div>
          <div>
            <label for="newPassword" class="block text-sm font-medium text-gray-700 dark:text-gray-300 mb-1">新密碼</label>
            <input id="newPassword" v-model="passwordForm.newPassword" type="password" required minlength="6" class="w-full rounded-md border border-gray-300 dark:border-gray-700 bg-white dark:bg-gray-800 px-3 py-2 text-gray-900 dark:text-gray-100 focus:outline-none focus:ring-2 focus:ring-primary-500" />
            <p class="text-xs text-gray-500 dark:text-gray-400 mt-1">至少 6 個字元</p>
          </div>
          <div>
            <label for="confirmPassword" class="block text-sm font-medium text-gray-700 dark:text-gray-300 mb-1">確認新密碼</label>
            <input id="confirmPassword" v-model="passwordForm.confirmPassword" type="password" required class="w-full rounded-md border border-gray-300 dark:border-gray-700 bg-white dark:bg-gray-800 px-3 py-2 text-gray-900 dark:text-gray-100 focus:outline-none focus:ring-2 focus:ring-primary-500" />
            <p v-if="passwordMismatch && passwordForm.confirmPassword" class="text-xs text-red-600 mt-1">新密碼與確認密碼不符</p>
          </div>
          <div class="flex items-center justify-end gap-3 pt-2">
            <button type="submit" class="btn-primary" :disabled="!canChangePassword">
              <span v-if="loadingPassword" class="flex items-center">
                <svg class="animate-spin -ml-1 mr-2 h-5 w-5 text-white" fill="none" viewBox="0 0 24 24">
                  <circle class="opacity-25" cx="12" cy="12" r="10" stroke="currentColor" stroke-width="4"></circle>
                  <path class="opacity-75" fill="currentColor" d="M4 12a8 8 0 018-8V0C5.373 0 0 5.373 0 12h4zm2 5.291A7.962 7.962 0 014 12H0c0 3.042 1.135 5.824 3 7.938l3-2.647z"></path>
                </svg>
                修改中...
              </span>
              <span v-else>修改密碼</span>
            </button>
          </div>
        </form>
      </div>
    </div>
  </div>
</template>

<script setup>
import { ref, reactive, onMounted, computed } from 'vue'
import { useAuthStore } from '../stores/auth'
import { profileApi } from '../api/profile'

const authStore = useAuthStore()

const profile = reactive({
  username: '',
  email: '',
  bio: '',
  playingStyle: '',
  yearsOfExperience: null,
  signature: ''
})

const passwordForm = reactive({
  currentPassword: '',
  newPassword: '',
  confirmPassword: ''
})

const loadingProfile = ref(false)
const loadingPassword = ref(false)

const canSaveProfile = computed(() => {
  const emailOk = !!profile.email && /.+@.+\..+/.test(profile.email)
  return emailOk && !loadingProfile.value
})

const passwordMismatch = computed(() => passwordForm.newPassword !== passwordForm.confirmPassword)
const canChangePassword = computed(() => {
  const lengthOk = (passwordForm.newPassword?.length || 0) >= 6
  const filled = !!passwordForm.currentPassword && !!passwordForm.newPassword && !!passwordForm.confirmPassword
  return filled && lengthOk && !passwordMismatch.value && !loadingPassword.value
})

const updateProfile = async () => {
  loadingProfile.value = true
  try {
    await profileApi.updateProfile({
      email: profile.email,
      bio: profile.bio,
      playingStyle: profile.playingStyle || null,
      yearsOfExperience: profile.yearsOfExperience || null,
      signature: profile.signature || null
    })
    alert('個人資料已更新！')
    
    // Update auth store
    if (authStore.user) {
      authStore.user.email = profile.email
      authStore.user.bio = profile.bio
      authStore.user.playingStyle = profile.playingStyle
      authStore.user.yearsOfExperience = profile.yearsOfExperience
      authStore.user.signature = profile.signature
      localStorage.setItem('user', JSON.stringify(authStore.user))
    }
  } catch (error) {
    console.error('Failed to update profile:', error)
    alert('更新失敗，請重試')
  } finally {
    loadingProfile.value = false
  }
}

const changePassword = async () => {
  if (passwordMismatch.value) {
    alert('新密碼與確認密碼不符！')
    return
  }
  loadingPassword.value = true
  try {
    await profileApi.changePassword({
      currentPassword: passwordForm.currentPassword,
      newPassword: passwordForm.newPassword
    })
    alert('密碼已更改！')
    
    // Reset form
    passwordForm.currentPassword = ''
    passwordForm.newPassword = ''
    passwordForm.confirmPassword = ''
  } catch (error) {
    console.error('Failed to change password:', error)
    alert(error.response?.data?.message || '密碼更改失敗')
  } finally {
    loadingPassword.value = false
  }
}

onMounted(() => {
  // Load user data
  if (authStore.user) {
    profile.username = authStore.user.username
    profile.email = authStore.user.email
    profile.bio = authStore.user.bio || ''
    profile.playingStyle = authStore.user.playingStyle || ''
    profile.yearsOfExperience = authStore.user.yearsOfExperience || null
    profile.signature = authStore.user.signature || ''
  }
})
</script>

<style scoped>
.settings { width: 100%; }
</style>