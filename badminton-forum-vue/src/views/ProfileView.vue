<template>
  <div class="profile">
    <div v-if="loading" class="loading">載入中...</div>
    <div v-else-if="error" class="error-message">
      {{ error }}
    </div>
    <template v-else>
      <div class="profile-header">
        <img :src="user.avatar || '/default-avatar.png'" alt="Avatar" class="avatar">
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
            <RouterLink :to="`/post/${post.id}`">
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
import { ref, onMounted } from 'vue'
import { useRoute, RouterLink } from 'vue-router'
import { profileApi } from '../api/profile'

const route = useRoute()

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

const formatDate = (date) => {
  return new Date(date).toLocaleDateString('zh-TW')
}

onMounted(async () => {
  const userId = route.params.id
  
  try {
    // Fetch user profile by ID
    const profileResponse = await profileApi.getProfileById(userId)
    user.value = profileResponse.data
    
    // Fetch user posts (still using username for now)
    const postsResponse = await profileApi.getUserPosts(user.value.username)
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
  max-width: 800px;
  margin: 0 auto;
  padding: 2rem;
}

.profile-header {
  background: white;
  padding: 2rem;
  border-radius: 8px;
  box-shadow: 0 2px 4px rgba(0,0,0,0.1);
  display: flex;
  gap: 2rem;
  margin-bottom: 2rem;
}

.avatar {
  width: 120px;
  height: 120px;
  border-radius: 50%;
  object-fit: cover;
  background-color: #e0e0e0;
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