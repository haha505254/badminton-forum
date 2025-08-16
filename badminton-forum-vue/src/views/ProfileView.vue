<template>
  <div class="profile">
    <div v-if="loading" class="loading">載入中...</div>
    <div v-else-if="error" class="error-message">
      {{ error }}
    </div>
    <template v-else>
      <div class="profile-header">
        <div class="avatar-container">
          <img 
            v-if="getAvatarUrl() && !avatarLoadError" 
            :src="getAvatarUrl()" 
            alt="Avatar" 
            class="avatar"
            @error="handleAvatarError"
          >
          <div v-else class="avatar avatar-placeholder">
            <span>{{ user.username.charAt(0).toUpperCase() }}</span>
          </div>
          <div v-if="isCurrentUser" class="avatar-overlay" @click="triggerFileInput">
            <span class="upload-text">{{ (getAvatarUrl() && !avatarLoadError) ? '更換頭像' : '上傳頭像' }}</span>
          </div>
          <input
            ref="fileInput"
            type="file"
            accept="image/*"
            style="display: none"
            @change="handleFileSelect"
          >
        </div>
        <div class="profile-info">
          <h1>{{ user.username }}</h1>
          <p>{{ user.email }}</p>
          <p class="bio">{{ user.bio || '這個用戶還沒有填寫簡介' }}</p>
          <div class="badminton-info">
            <span v-if="user.playingStyle" class="info-item">
              <strong>打球風格:</strong> {{ user.playingStyle }}
            </span>
            <span v-if="user.yearsOfExperience" class="info-item">
              <strong>球齡:</strong> {{ user.yearsOfExperience }} 年
            </span>
          </div>
          <p v-if="user.signature" class="signature">{{ user.signature }}</p>
          <p class="join-date">加入於 {{ formatDate(user.createdAt) }}</p>
        </div>
      </div>
      
      <div class="profile-content">
        <h2>最近的文章</h2>
        <div v-if="posts.length === 0" class="empty-state">
          還沒有發表任何文章
        </div>
        <div v-else class="post-list">
          <div v-for="post in posts" :key="post.id" class="post-item">
            <RouterLink :to="`/posts/${post.id}`">
              <h3>{{ post.title }}</h3>
            </RouterLink>
            <div class="post-meta">
              <span>{{ formatDate(post.createdAt) }}</span>
              <span>回覆: {{ post.replyCount }}</span>
            </div>
          </div>
        </div>
      </div>
    </template>
  </div>
</template>

<script setup>
import { ref, onMounted, computed } from 'vue'
import { useRoute, RouterLink } from 'vue-router'
import { profileApi } from '../api/profile'
import { useAuthStore } from '../stores/auth'

const route = useRoute()
const authStore = useAuthStore()

const user = ref({
  username: '載入中...',
  email: '',
  bio: '',
  avatar: '',
  createdAt: new Date()
})

const posts = ref([])
const loading = ref(true)
const error = ref('')
const fileInput = ref(null)
const avatarUrl = ref('')
const uploadingAvatar = ref(false)
const avatarLoadError = ref(false)

const isCurrentUser = computed(() => {
  return authStore.user && authStore.user.id === user.value.id
})

const formatDate = (date) => {
  return new Date(date).toLocaleDateString('zh-TW')
}

const getAvatarUrl = () => {
  if (avatarUrl.value) {
    return avatarUrl.value
  }
  if (user.value.avatar) {
    // 如果是相對路徑，加上 API 基礎 URL
    if (user.value.avatar.startsWith('/')) {
      const apiBaseUrl = import.meta.env.VITE_API_URL || 'http://localhost:5000'
      return apiBaseUrl.replace('/api', '') + user.value.avatar
    }
    return user.value.avatar
  }
  return ''
}

const handleAvatarError = () => {
  avatarLoadError.value = true
  console.log('頭像載入失敗，顯示占位符')
}

const triggerFileInput = () => {
  fileInput.value.click()
}

const handleFileSelect = async (event) => {
  const file = event.target.files[0]
  if (!file) return

  // 檢查檔案大小
  if (file.size > 5 * 1024 * 1024) {
    alert('檔案大小不能超過 5MB')
    return
  }

  // 檢查檔案類型
  const allowedTypes = ['image/jpeg', 'image/png', 'image/gif']
  if (!allowedTypes.includes(file.type)) {
    alert('只能上傳 JPG、PNG 或 GIF 格式的圖片')
    return
  }

  // 預覽圖片
  const reader = new FileReader()
  reader.onload = (e) => {
    avatarUrl.value = e.target.result
  }
  reader.readAsDataURL(file)

  // 上傳圖片
  uploadingAvatar.value = true
  try {
    const response = await profileApi.uploadAvatar(file)
    user.value.avatar = response.data.avatarUrl
    avatarLoadError.value = false // 重置錯誤狀態
    // 更新 authStore 中的用戶資料
    if (isCurrentUser.value) {
      authStore.user.avatar = response.data.avatarUrl
    }
  } catch (err) {
    console.error('上傳頭像失敗:', err)
    alert('上傳頭像失敗，請稍後再試')
    avatarUrl.value = '' // 重置預覽
  } finally {
    uploadingAvatar.value = false
  }
}

onMounted(async () => {
  const userId = route.params.id
  
  try {
    // Fetch user profile by ID
    const profileResponse = await profileApi.getProfileById(userId)
    user.value = profileResponse.data
    avatarLoadError.value = false // 重置錯誤狀態
    
    // Fetch user posts (still using username for now)
    const postsResponse = await profileApi.getUserPostsById(userId)
    posts.value = postsResponse.data
  } catch (err) {
    console.error('Failed to fetch profile data:', err)
    if (err.response?.status === 404) {
      error.value = '用戶不存在'
    } else {
      error.value = '載入失敗，請稍後再試'
    }
  } finally {
    loading.value = false
  }
})
</script>

<style scoped>
.profile {
  /* 預設不設置寬度限制，保持響應式 */
  width: 100%;
  padding: 2rem;
}

/* 只在大螢幕上設定最小寬度，確保桌面版寬度一致 */
@media (min-width: 1024px) {
  .profile {
    min-width: 1088px;
  }
}

.profile-header {
  background: white;
  padding: 2rem;
  border-radius: 8px;
  box-shadow: 0 2px 4px rgba(0,0,0,0.1);
  display: flex;
  gap: 2rem;
  margin-bottom: 2rem;
  max-width: 800px;
  margin-left: auto;
  margin-right: auto;
}

.avatar-container {
  position: relative;
  width: 120px;
  height: 120px;
  cursor: pointer;
}

.avatar {
  width: 120px;
  height: 120px;
  border-radius: 50%;
  object-fit: cover;
  background-color: #e0e0e0;
}

.avatar-overlay {
  position: absolute;
  top: 0;
  left: 0;
  width: 100%;
  height: 100%;
  border-radius: 50%;
  background-color: rgba(0, 0, 0, 0.7);
  display: flex;
  align-items: center;
  justify-content: center;
  opacity: 0;
  transition: opacity 0.3s ease;
}

.avatar-container:hover .avatar-overlay {
  opacity: 1;
}

.upload-text {
  color: white;
  font-size: 14px;
  font-weight: 500;
}

.avatar-placeholder {
  display: flex;
  align-items: center;
  justify-content: center;
  background-color: #4CAF50;
  color: white;
  font-size: 48px;
  font-weight: bold;
}

.profile-info h1 {
  color: #2c3e50;
  margin-bottom: 0.5rem;
}

.bio {
  color: #666;
  margin: 1rem 0;
}

.join-date {
  color: #999;
  font-size: 0.9rem;
}

.badminton-info {
  display: flex;
  gap: 1.5rem;
  margin: 1rem 0;
}

.info-item {
  background-color: #f0f0f0;
  padding: 0.5rem 1rem;
  border-radius: 20px;
  font-size: 0.9rem;
  color: #555;
}

.info-item strong {
  color: #333;
}

.signature {
  font-style: italic;
  color: #666;
  margin: 1rem 0;
  padding: 1rem;
  background-color: #f9f9f9;
  border-left: 3px solid #4CAF50;
}

.profile-content {
  background: white;
  padding: 2rem;
  border-radius: 8px;
  box-shadow: 0 2px 4px rgba(0,0,0,0.1);
  max-width: 800px;
  margin-left: auto;
  margin-right: auto;
}

.profile-content h2 {
  color: #2c3e50;
  margin-bottom: 1.5rem;
}

.empty-state {
  text-align: center;
  color: #999;
  padding: 2rem;
}

.post-list {
  display: flex;
  flex-direction: column;
  gap: 1rem;
}

.post-item {
  padding: 1rem;
  border: 1px solid #eee;
  border-radius: 4px;
  transition: background-color 0.3s;
}

.post-item:hover {
  background-color: #f5f5f5;
}

.post-item h3 {
  color: #2c3e50;
  margin-bottom: 0.5rem;
}

.post-meta {
  color: #666;
  font-size: 0.9rem;
  display: flex;
  gap: 1rem;
}

.loading {
  text-align: center;
  padding: 4rem;
  font-size: 1.2rem;
  color: #666;
}

.error-message {
  background-color: #fee;
  color: #c33;
  padding: 2rem;
  border-radius: 8px;
  text-align: center;
  margin: 2rem auto;
  max-width: 600px;
}
</style>