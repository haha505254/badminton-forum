<template>
  <div class="post-view">
    <article class="post">
      <h1>{{ post.title }}</h1>
      <div class="post-meta">
        <span>作者: {{ post.authorName }}</span>
        <span>發表於: {{ formatDate(post.createdAt) }}</span>
        <span>瀏覽: {{ post.viewCount }}</span>
      </div>
      <div class="post-content" v-html="post.content"></div>
    </article>
    
    <section class="replies">
      <h2>回覆 ({{ replies.length }})</h2>
      <div v-for="reply in replies" :key="reply.id" class="reply-item">
        <div class="reply-header">
          <strong>{{ reply.authorName }}</strong>
          <span>{{ formatDate(reply.createdAt) }}</span>
        </div>
        <div class="reply-content">{{ reply.content }}</div>
      </div>
    </section>
    
    <section v-if="authStore.isAuthenticated" class="reply-form">
      <h3>發表回覆</h3>
      <textarea v-model="newReply" placeholder="寫下您的回覆..."></textarea>
      <button @click="submitReply" :disabled="submitting">
        {{ submitting ? '發表中...' : '發表' }}
      </button>
    </section>
  </div>
</template>

<script setup>
import { ref, onMounted } from 'vue'
import { useRoute } from 'vue-router'
import { useAuthStore } from '../stores/auth'
import { postsApi } from '../api/posts'
import { repliesApi } from '../api/replies'

const route = useRoute()
const authStore = useAuthStore()

const post = ref({
  title: '載入中...',
  authorName: '',
  content: '',
  createdAt: new Date(),
  viewCount: 0
})

const replies = ref([])
const newReply = ref('')
const loading = ref(true)
const submitting = ref(false)

const formatDate = (date) => {
  return new Date(date).toLocaleString('zh-TW')
}

const submitReply = async () => {
  if (!newReply.value.trim()) return
  
  submitting.value = true
  try {
    const response = await repliesApi.createReply(post.value.id, {
      content: newReply.value
    })
    replies.value.push(response.data)
    newReply.value = ''
  } catch (error) {
    console.error('Failed to submit reply:', error)
    alert('發表回覆失敗')
  } finally {
    submitting.value = false
  }
}

onMounted(async () => {
  const postId = route.params.id
  
  try {
    // Fetch post data
    const postResponse = await postsApi.getPost(postId)
    post.value = postResponse.data
    
    // Fetch replies
    const repliesResponse = await repliesApi.getReplies(postId)
    replies.value = repliesResponse.data
  } catch (error) {
    console.error('Failed to fetch post data:', error)
  } finally {
    loading.value = false
  }
})
</script>

<style scoped>
.post-view {
  padding: 2rem;
  max-width: 800px;
  margin: 0 auto;
}

.post {
  background: white;
  padding: 2rem;
  border-radius: 8px;
  box-shadow: 0 2px 4px rgba(0,0,0,0.1);
  margin-bottom: 2rem;
}

.post h1 {
  color: #2c3e50;
  margin-bottom: 1rem;
}

.post-meta {
  color: #666;
  font-size: 0.9rem;
  margin-bottom: 1.5rem;
  display: flex;
  gap: 1rem;
}

.post-content {
  line-height: 1.6;
}

.replies {
  background: white;
  padding: 2rem;
  border-radius: 8px;
  box-shadow: 0 2px 4px rgba(0,0,0,0.1);
  margin-bottom: 2rem;
}

.reply-item {
  padding: 1rem 0;
  border-bottom: 1px solid #eee;
}

.reply-item:last-child {
  border-bottom: none;
}

.reply-header {
  display: flex;
  justify-content: space-between;
  margin-bottom: 0.5rem;
  color: #666;
  font-size: 0.9rem;
}

.reply-form {
  background: white;
  padding: 2rem;
  border-radius: 8px;
  box-shadow: 0 2px 4px rgba(0,0,0,0.1);
}

.reply-form textarea {
  width: 100%;
  min-height: 100px;
  padding: 0.75rem;
  border: 1px solid #ddd;
  border-radius: 4px;
  margin-bottom: 1rem;
  font-family: inherit;
}

.reply-form button {
  background-color: #3498db;
  color: white;
  padding: 0.75rem 1.5rem;
  border: none;
  border-radius: 4px;
  cursor: pointer;
}

.reply-form button:hover {
  background-color: #2980b9;
}
</style>