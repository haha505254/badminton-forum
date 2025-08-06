<template>
  <div class="search-view">
    <div class="search-header">
      <h1>搜尋文章</h1>
      <div class="search-box">
        <input
          v-model="searchKeyword"
          type="text"
          placeholder="輸入關鍵字搜尋文章標題或內容..."
          @keyup.enter="performSearch"
          class="search-input"
        />
        <button @click="performSearch" class="search-button">
          搜尋
        </button>
      </div>
    </div>

    <div v-if="searchPerformed" class="search-results">
      <div v-if="loading" class="loading">搜尋中...</div>
      
      <div v-else-if="posts.length === 0" class="no-results">
        找不到包含「{{ lastSearchKeyword }}」的文章
      </div>
      
      <div v-else>
        <div class="results-info">
          找到 {{ totalCount }} 篇包含「{{ lastSearchKeyword }}」的文章
        </div>
        
        <div class="posts-list">
          <div v-for="post in posts" :key="post.id" class="post-item">
            <div class="post-header">
              <router-link :to="`/post/${post.id}`" class="post-title">
                {{ post.title }}
              </router-link>
              <span class="post-category">{{ post.categoryName }}</span>
            </div>
            
            <div class="post-content">
              {{ truncateContent(post.content) }}
            </div>
            
            <div class="post-meta">
              <span class="author">{{ post.authorName }}</span>
              <span class="time">{{ formatDate(post.createdAt) }}</span>
              <span class="stats">
                <span class="views">瀏覽 {{ post.viewCount }}</span>
                <span class="replies">回覆 {{ post.replyCount }}</span>
                <span class="likes">讚 {{ post.likeCount }}</span>
              </span>
            </div>
          </div>
        </div>
        
        <div v-if="totalPages > 1" class="pagination">
          <button
            @click="changePage(currentPage - 1)"
            :disabled="currentPage === 1"
            class="page-button"
          >
            上一頁
          </button>
          
          <span class="page-info">
            第 {{ currentPage }} / {{ totalPages }} 頁
          </span>
          
          <button
            @click="changePage(currentPage + 1)"
            :disabled="currentPage === totalPages"
            class="page-button"
          >
            下一頁
          </button>
        </div>
      </div>
    </div>
  </div>
</template>

<script setup>
import { ref } from 'vue'
import { postsApi } from '../api/posts'

const searchKeyword = ref('')
const lastSearchKeyword = ref('')
const posts = ref([])
const loading = ref(false)
const searchPerformed = ref(false)
const currentPage = ref(1)
const totalCount = ref(0)
const totalPages = ref(0)
const pageSize = 20

const performSearch = async () => {
  if (!searchKeyword.value.trim()) {
    return
  }
  
  loading.value = true
  searchPerformed.value = true
  lastSearchKeyword.value = searchKeyword.value
  
  try {
    const response = await postsApi.searchPosts(searchKeyword.value, currentPage.value, pageSize)
    posts.value = response.data
    
    totalCount.value = parseInt(response.headers['x-total-count'] || '0')
    totalPages.value = parseInt(response.headers['x-total-pages'] || '0')
  } catch (error) {
    console.error('搜尋失敗:', error)
    posts.value = []
  } finally {
    loading.value = false
  }
}

const changePage = async (page) => {
  if (page < 1 || page > totalPages.value) return
  
  currentPage.value = page
  await performSearch()
}

const truncateContent = (content) => {
  const maxLength = 150
  if (content.length <= maxLength) return content
  return content.substring(0, maxLength) + '...'
}

const formatDate = (dateString) => {
  const date = new Date(dateString)
  const now = new Date()
  const diffTime = Math.abs(now - date)
  const diffDays = Math.floor(diffTime / (1000 * 60 * 60 * 24))
  
  if (diffDays === 0) {
    const diffHours = Math.floor(diffTime / (1000 * 60 * 60))
    if (diffHours === 0) {
      const diffMinutes = Math.floor(diffTime / (1000 * 60))
      return `${diffMinutes} 分鐘前`
    }
    return `${diffHours} 小時前`
  } else if (diffDays === 1) {
    return '昨天'
  } else if (diffDays < 7) {
    return `${diffDays} 天前`
  } else {
    return date.toLocaleDateString('zh-TW')
  }
}
</script>

<style scoped>
.search-view {
  /* 預設不設置最小寬度，保持響應式 */
  width: 100%;
}

/* 只在大螢幕上設定最小寬度，確保桌面版寬度一致 */
@media (min-width: 1024px) {
  .search-view {
    min-width: 1088px;
  }
}

.search-header {
  margin-bottom: 30px;
}

.search-header h1 {
  font-size: 28px;
  margin-bottom: 20px;
  color: #333;
}

.search-box {
  display: flex;
  gap: 10px;
}

.search-input {
  flex: 1;
  padding: 12px 16px;
  font-size: 16px;
  border: 2px solid #ddd;
  border-radius: 8px;
  transition: border-color 0.3s;
}

.search-input:focus {
  outline: none;
  border-color: #4CAF50;
}

.search-button {
  padding: 12px 30px;
  background-color: #4CAF50;
  color: white;
  border: none;
  border-radius: 8px;
  font-size: 16px;
  cursor: pointer;
  transition: background-color 0.3s;
}

.search-button:hover {
  background-color: #45a049;
}

.search-results {
  margin-top: 30px;
}

.loading {
  text-align: center;
  font-size: 18px;
  color: #666;
  padding: 40px;
}

.no-results {
  text-align: center;
  font-size: 18px;
  color: #666;
  padding: 40px;
}

.results-info {
  font-size: 16px;
  color: #666;
  margin-bottom: 20px;
}

.posts-list {
  display: flex;
  flex-direction: column;
  gap: 20px;
}

.post-item {
  background-color: #f9f9f9;
  border: 1px solid #e0e0e0;
  border-radius: 8px;
  padding: 20px;
  transition: box-shadow 0.3s;
}

.post-item:hover {
  box-shadow: 0 2px 8px rgba(0, 0, 0, 0.1);
}

.post-header {
  display: flex;
  justify-content: space-between;
  align-items: center;
  margin-bottom: 10px;
}

.post-title {
  font-size: 20px;
  font-weight: bold;
  color: #333;
  text-decoration: none;
  flex: 1;
}

.post-title:hover {
  color: #4CAF50;
}

.post-category {
  background-color: #e3f2fd;
  color: #1976d2;
  padding: 4px 12px;
  border-radius: 16px;
  font-size: 14px;
}

.post-content {
  color: #666;
  line-height: 1.6;
  margin-bottom: 15px;
}

.post-meta {
  display: flex;
  justify-content: space-between;
  align-items: center;
  font-size: 14px;
  color: #999;
}

.post-meta .author {
  font-weight: 500;
  color: #666;
}

.stats {
  display: flex;
  gap: 15px;
}

.pagination {
  display: flex;
  justify-content: center;
  align-items: center;
  gap: 20px;
  margin-top: 40px;
}

.page-button {
  padding: 8px 16px;
  background-color: #f0f0f0;
  border: 1px solid #ddd;
  border-radius: 4px;
  cursor: pointer;
  transition: background-color 0.3s;
}

.page-button:hover:not(:disabled) {
  background-color: #e0e0e0;
}

.page-button:disabled {
  opacity: 0.5;
  cursor: not-allowed;
}

.page-info {
  font-size: 16px;
  color: #666;
}
</style>