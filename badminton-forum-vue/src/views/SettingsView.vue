<template>
  <div class="settings">
    <h1>帳戶設置</h1>
    
    <div class="settings-section">
      <h2>個人資料</h2>
      <form @submit.prevent="updateProfile">
        <div class="form-group">
          <label for="username">用戶名</label>
          <input
            id="username"
            v-model="profile.username"
            type="text"
            disabled
          />
        </div>
        
        <div class="form-group">
          <label for="email">電子郵件</label>
          <input
            id="email"
            v-model="profile.email"
            type="email"
            required
          />
        </div>
        
        <div class="form-group">
          <label for="bio">個人簡介</label>
          <textarea
            id="bio"
            v-model="profile.bio"
            placeholder="介紹一下自己..."
          ></textarea>
        </div>
        
        <div class="form-group">
          <label for="playingStyle">打球風格</label>
          <select id="playingStyle" v-model="profile.playingStyle">
            <option value="">請選擇</option>
            <option value="攻擊型">攻擊型</option>
            <option value="防守型">防守型</option>
            <option value="全能型">全能型</option>
          </select>
        </div>
        
        <div class="form-group">
          <label for="yearsOfExperience">球齡（年）</label>
          <input
            id="yearsOfExperience"
            v-model.number="profile.yearsOfExperience"
            type="number"
            min="0"
            max="100"
            placeholder="例如：5"
          />
        </div>
        
        <div class="form-group">
          <label for="signature">簽名檔</label>
          <textarea
            id="signature"
            v-model="profile.signature"
            placeholder="你的個性簽名..."
            rows="3"
          ></textarea>
        </div>
        
        <button type="submit" :disabled="loading">
          {{ loading ? '保存中...' : '保存更改' }}
        </button>
      </form>
    </div>
    
    <div class="settings-section">
      <h2>修改密碼</h2>
      <form @submit.prevent="changePassword">
        <div class="form-group">
          <label for="currentPassword">當前密碼</label>
          <input
            id="currentPassword"
            v-model="passwordForm.currentPassword"
            type="password"
            required
          />
        </div>
        
        <div class="form-group">
          <label for="newPassword">新密碼</label>
          <input
            id="newPassword"
            v-model="passwordForm.newPassword"
            type="password"
            required
            minlength="6"
          />
        </div>
        
        <div class="form-group">
          <label for="confirmPassword">確認新密碼</label>
          <input
            id="confirmPassword"
            v-model="passwordForm.confirmPassword"
            type="password"
            required
          />
        </div>
        
        <button type="submit" :disabled="loading">
          {{ loading ? '修改中...' : '修改密碼' }}
        </button>
      </form>
    </div>
  </div>
</template>

<script setup>
import { ref, reactive, onMounted } from 'vue'
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

const loading = ref(false)

const updateProfile = async () => {
  loading.value = true
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
    loading.value = false
  }
}

const changePassword = async () => {
  if (passwordForm.newPassword !== passwordForm.confirmPassword) {
    alert('新密碼與確認密碼不符！')
    return
  }
  
  loading.value = true
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
    loading.value = false
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
.settings {
  /* 預設不設置寬度限制，保持響應式 */
  width: 100%;
  padding: 2rem;
}

/* 只在大螢幕上設定最小寬度，確保桌面版寬度一致 */
@media (min-width: 1024px) {
  .settings {
    min-width: 1088px;
  }
}

h1 {
  color: #2c3e50;
  margin-bottom: 2rem;
}

.settings-section {
  background: white;
  padding: 2rem;
  border-radius: 8px;
  box-shadow: 0 2px 4px rgba(0,0,0,0.1);
  margin-bottom: 2rem;
  max-width: 600px;
  margin-left: auto;
  margin-right: auto;
}

.settings-section h2 {
  color: #2c3e50;
  margin-bottom: 1.5rem;
  font-size: 1.3rem;
}

.form-group {
  margin-bottom: 1.5rem;
}

label {
  display: block;
  margin-bottom: 0.5rem;
  color: #555;
  font-weight: 500;
}

input,
textarea {
  width: 100%;
  padding: 0.75rem;
  border: 1px solid #ddd;
  border-radius: 4px;
  font-size: 1rem;
  font-family: inherit;
}

textarea {
  min-height: 100px;
  resize: vertical;
}

input:focus,
textarea:focus {
  outline: none;
  border-color: #3498db;
}

input:disabled {
  background-color: #f5f5f5;
  cursor: not-allowed;
}

button {
  background-color: #3498db;
  color: white;
  padding: 0.75rem 2rem;
  border: none;
  border-radius: 4px;
  font-size: 1rem;
  cursor: pointer;
  transition: background-color 0.3s;
}

button:hover {
  background-color: #2980b9;
}
</style>