<template>
  <div class="admin-dashboard">
    <h1>管理員後台</h1>
    
    <div class="admin-tabs">
      <button
        @click="activeTab = 'users'"
        :class="{ active: activeTab === 'users' }"
        class="tab-button"
      >
        使用者管理
      </button>
      <button
        @click="activeTab = 'categories'"
        :class="{ active: activeTab === 'categories' }"
        class="tab-button"
      >
        版塊管理
      </button>
      <button
        @click="activeTab = 'posts'"
        :class="{ active: activeTab === 'posts' }"
        class="tab-button"
      >
        文章管理
      </button>
    </div>
    
    <div class="tab-content">
      <!-- 使用者管理 -->
      <div v-show="activeTab === 'users'" class="users-management">
        <h2>使用者管理</h2>
        
        <div v-if="loading" class="loading">載入中...</div>
        
        <table v-else class="admin-table">
          <thead>
            <tr>
              <th>ID</th>
              <th>使用者名稱</th>
              <th>註冊時間</th>
              <th>最後登入</th>
              <th>文章數</th>
              <th>回覆數</th>
              <th>狀態</th>
              <th>權限</th>
              <th>操作</th>
            </tr>
          </thead>
          <tbody>
            <tr v-for="user in users" :key="user.id">
              <td>{{ user.id }}</td>
              <td>{{ user.username }}</td>
              <td>{{ formatDate(user.createdAt) }}</td>
              <td>{{ user.lastLoginAt ? formatDate(user.lastLoginAt) : '從未登入' }}</td>
              <td>{{ user.postCount }}</td>
              <td>{{ user.replyCount }}</td>
              <td>
                <span :class="['status', user.isActive ? 'active' : 'inactive']">
                  {{ user.isActive ? '正常' : '停用' }}
                </span>
              </td>
              <td>
                <span :class="['role', user.isAdmin ? 'admin' : 'user']">
                  {{ user.isAdmin ? '管理員' : '一般使用者' }}
                </span>
              </td>
              <td class="actions">
                <button
                  @click="toggleUserActive(user)"
                  :class="user.isActive ? 'btn-danger' : 'btn-success'"
                  class="action-btn"
                >
                  {{ user.isActive ? '停用' : '啟用' }}
                </button>
                <button
                  @click="toggleUserAdmin(user)"
                  :disabled="user.id === currentUserId"
                  class="action-btn"
                >
                  {{ user.isAdmin ? '移除管理員' : '設為管理員' }}
                </button>
              </td>
            </tr>
          </tbody>
        </table>
        
        <div v-if="usersTotalPages > 1" class="pagination">
          <button
            @click="loadUsers(usersCurrentPage - 1)"
            :disabled="usersCurrentPage === 1"
            class="page-button"
          >
            上一頁
          </button>
          <span class="page-info">
            第 {{ usersCurrentPage }} / {{ usersTotalPages }} 頁
          </span>
          <button
            @click="loadUsers(usersCurrentPage + 1)"
            :disabled="usersCurrentPage === usersTotalPages"
            class="page-button"
          >
            下一頁
          </button>
        </div>
      </div>
      
      <!-- 版塊管理 -->
      <div v-show="activeTab === 'categories'" class="categories-management">
        <h2>版塊管理</h2>
        
        <button @click="showCreateCategory = true" class="btn-primary">
          新增版塊
        </button>
        
        <div v-if="loading" class="loading">載入中...</div>
        
        <table v-else class="admin-table">
          <thead>
            <tr>
              <th>ID</th>
              <th>圖示</th>
              <th>名稱</th>
              <th>描述</th>
              <th>排序</th>
              <th>文章數</th>
              <th>操作</th>
            </tr>
          </thead>
          <tbody>
            <tr v-for="category in categories" :key="category.id">
              <td>{{ category.id }}</td>
              <td>{{ category.icon || '無' }}</td>
              <td>{{ category.name }}</td>
              <td>{{ category.description || '無描述' }}</td>
              <td>{{ category.displayOrder }}</td>
              <td>{{ category.postCount }}</td>
              <td class="actions">
                <button @click="editCategory(category)" class="action-btn">
                  編輯
                </button>
                <button
                  @click="deleteCategory(category)"
                  :disabled="category.postCount > 0"
                  class="action-btn btn-danger"
                >
                  刪除
                </button>
              </td>
            </tr>
          </tbody>
        </table>
        
        <!-- 新增/編輯版塊對話框 -->
        <div v-if="showCreateCategory || editingCategory" class="modal">
          <div class="modal-content">
            <h3>{{ editingCategory ? '編輯版塊' : '新增版塊' }}</h3>
            <form @submit.prevent="saveCategory">
              <div class="form-group">
                <label>名稱</label>
                <input v-model="categoryForm.name" type="text" required />
              </div>
              <div class="form-group">
                <label>描述</label>
                <textarea v-model="categoryForm.description" rows="3"></textarea>
              </div>
              <div class="form-group">
                <label>圖示</label>
                <input v-model="categoryForm.icon" type="text" />
              </div>
              <div class="form-group">
                <label>排序</label>
                <input v-model.number="categoryForm.displayOrder" type="number" required />
              </div>
              <div class="form-actions">
                <button type="submit" class="btn-primary">儲存</button>
                <button type="button" @click="cancelCategoryEdit" class="btn-secondary">
                  取消
                </button>
              </div>
            </form>
          </div>
        </div>
      </div>
      
      <!-- 文章管理 -->
      <div v-show="activeTab === 'posts'" class="posts-management">
        <h2>文章管理</h2>
        
        <div v-if="loading" class="loading">載入中...</div>
        
        <table v-else class="admin-table">
          <thead>
            <tr>
              <th>ID</th>
              <th>標題</th>
              <th>作者</th>
              <th>版塊</th>
              <th>瀏覽</th>
              <th>讚</th>
              <th>回覆</th>
              <th>狀態</th>
              <th>發布時間</th>
              <th>操作</th>
            </tr>
          </thead>
          <tbody>
            <tr v-for="post in posts" :key="post.id">
              <td>{{ post.id }}</td>
              <td class="post-title">
                <router-link :to="`/post/${post.id}`" target="_blank">
                  {{ post.title }}
                </router-link>
              </td>
              <td>{{ post.authorName }}</td>
              <td>{{ post.categoryName }}</td>
              <td>{{ post.viewCount }}</td>
              <td>{{ post.likeCount }}</td>
              <td>{{ post.replyCount }}</td>
              <td>
                <span v-if="post.isPinned" class="badge pinned">置頂</span>
                <span v-if="post.isLocked" class="badge locked">鎖定</span>
              </td>
              <td>{{ formatDate(post.createdAt) }}</td>
              <td class="actions">
                <button @click="togglePin(post)" class="action-btn">
                  {{ post.isPinned ? '取消置頂' : '置頂' }}
                </button>
                <button @click="toggleLock(post)" class="action-btn">
                  {{ post.isLocked ? '解鎖' : '鎖定' }}
                </button>
                <button @click="movePost(post)" class="action-btn">
                  移動
                </button>
                <button @click="deletePost(post)" class="action-btn btn-danger">
                  刪除
                </button>
              </td>
            </tr>
          </tbody>
        </table>
        
        <div v-if="postsTotalPages > 1" class="pagination">
          <button
            @click="loadPosts(postsCurrentPage - 1)"
            :disabled="postsCurrentPage === 1"
            class="page-button"
          >
            上一頁
          </button>
          <span class="page-info">
            第 {{ postsCurrentPage }} / {{ postsTotalPages }} 頁
          </span>
          <button
            @click="loadPosts(postsCurrentPage + 1)"
            :disabled="postsCurrentPage === postsTotalPages"
            class="page-button"
          >
            下一頁
          </button>
        </div>
        
        <!-- 移動文章對話框 -->
        <div v-if="movingPost" class="modal">
          <div class="modal-content">
            <h3>移動文章</h3>
            <p>將「{{ movingPost.title }}」移動到：</p>
            <select v-model="targetCategoryId" required>
              <option value="">請選擇版塊</option>
              <option
                v-for="category in categories"
                :key="category.id"
                :value="category.id"
                :disabled="category.id === movingPost.categoryId"
              >
                {{ category.name }}
              </option>
            </select>
            <div class="form-actions">
              <button @click="confirmMovePost" :disabled="!targetCategoryId" class="btn-primary">
                確定移動
              </button>
              <button @click="movingPost = null" class="btn-secondary">
                取消
              </button>
            </div>
          </div>
        </div>
      </div>
    </div>
  </div>
</template>

<script setup>
import { ref, onMounted, computed } from 'vue'
import { useRouter } from 'vue-router'
import { useAuthStore } from '@/stores/auth'
import { adminApi } from '@/api/admin'

const router = useRouter()
const authStore = useAuthStore()

// 檢查管理員權限
if (!authStore.user?.isAdmin) {
  router.push('/')
}

const currentUserId = computed(() => authStore.user?.id)

// 共用狀態
const activeTab = ref('users')
const loading = ref(false)

// 使用者管理
const users = ref([])
const usersCurrentPage = ref(1)
const usersTotalPages = ref(0)

// 版塊管理
const categories = ref([])
const showCreateCategory = ref(false)
const editingCategory = ref(null)
const categoryForm = ref({
  name: '',
  description: '',
  icon: '',
  displayOrder: 0
})

// 文章管理
const posts = ref([])
const postsCurrentPage = ref(1)
const postsTotalPages = ref(0)
const movingPost = ref(null)
const targetCategoryId = ref('')

// 載入使用者
const loadUsers = async (page = 1) => {
  loading.value = true
  try {
    const response = await adminApi.getUsers(page)
    users.value = response.data
    usersCurrentPage.value = page
    usersTotalPages.value = parseInt(response.headers['x-total-pages'] || '0')
  } catch (error) {
    console.error('載入使用者失敗:', error)
  } finally {
    loading.value = false
  }
}

// 切換使用者狀態
const toggleUserActive = async (user) => {
  if (user.id === currentUserId.value) {
    alert('無法停用自己的帳號')
    return
  }
  
  try {
    const response = await adminApi.toggleUserActive(user.id)
    user.isActive = response.data.isActive
  } catch (error) {
    console.error('切換使用者狀態失敗:', error)
    alert(error.response?.data?.message || '操作失敗')
  }
}

// 切換管理員權限
const toggleUserAdmin = async (user) => {
  if (user.id === currentUserId.value) {
    return
  }
  
  try {
    const response = await adminApi.toggleUserAdmin(user.id)
    user.isAdmin = response.data.isAdmin
  } catch (error) {
    console.error('切換管理員權限失敗:', error)
    alert(error.response?.data?.message || '操作失敗')
  }
}

// 載入版塊
const loadCategories = async () => {
  loading.value = true
  try {
    const response = await adminApi.getCategories()
    categories.value = response.data
  } catch (error) {
    console.error('載入版塊失敗:', error)
  } finally {
    loading.value = false
  }
}

// 編輯版塊
const editCategory = (category) => {
  editingCategory.value = category
  categoryForm.value = {
    name: category.name,
    description: category.description || '',
    icon: category.icon || '',
    displayOrder: category.displayOrder
  }
}

// 儲存版塊
const saveCategory = async () => {
  try {
    if (editingCategory.value) {
      await adminApi.updateCategory(editingCategory.value.id, categoryForm.value)
    } else {
      await adminApi.createCategory(categoryForm.value)
    }
    await loadCategories()
    cancelCategoryEdit()
  } catch (error) {
    console.error('儲存版塊失敗:', error)
    alert('儲存失敗')
  }
}

// 取消編輯
const cancelCategoryEdit = () => {
  showCreateCategory.value = false
  editingCategory.value = null
  categoryForm.value = {
    name: '',
    description: '',
    icon: '',
    displayOrder: 0
  }
}

// 刪除版塊
const deleteCategory = async (category) => {
  if (category.postCount > 0) {
    alert('無法刪除含有文章的版塊')
    return
  }
  
  if (!confirm(`確定要刪除版塊「${category.name}」嗎？`)) {
    return
  }
  
  try {
    await adminApi.deleteCategory(category.id)
    await loadCategories()
  } catch (error) {
    console.error('刪除版塊失敗:', error)
    alert(error.response?.data?.message || '刪除失敗')
  }
}

// 載入文章
const loadPosts = async (page = 1) => {
  loading.value = true
  try {
    const response = await adminApi.getPosts(page)
    posts.value = response.data
    postsCurrentPage.value = page
    postsTotalPages.value = parseInt(response.headers['x-total-pages'] || '0')
  } catch (error) {
    console.error('載入文章失敗:', error)
  } finally {
    loading.value = false
  }
}

// 切換置頂
const togglePin = async (post) => {
  try {
    const response = await adminApi.togglePostPin(post.id)
    post.isPinned = response.data.isPinned
  } catch (error) {
    console.error('切換置頂失敗:', error)
  }
}

// 切換鎖定
const toggleLock = async (post) => {
  try {
    const response = await adminApi.togglePostLock(post.id)
    post.isLocked = response.data.isLocked
  } catch (error) {
    console.error('切換鎖定失敗:', error)
  }
}

// 移動文章
const movePost = (post) => {
  movingPost.value = post
  targetCategoryId.value = ''
}

// 確認移動
const confirmMovePost = async () => {
  if (!targetCategoryId.value) return
  
  try {
    await adminApi.movePost(movingPost.value.id, targetCategoryId.value)
    movingPost.value = null
    await loadPosts(postsCurrentPage.value)
  } catch (error) {
    console.error('移動文章失敗:', error)
    alert('移動失敗')
  }
}

// 刪除文章
const deletePost = async (post) => {
  if (!confirm(`確定要刪除文章「${post.title}」嗎？`)) {
    return
  }
  
  try {
    await adminApi.deletePost(post.id)
    await loadPosts(postsCurrentPage.value)
  } catch (error) {
    console.error('刪除文章失敗:', error)
    alert('刪除失敗')
  }
}

// 格式化日期
const formatDate = (dateString) => {
  return new Date(dateString).toLocaleDateString('zh-TW')
}

// 初始載入
onMounted(() => {
  loadUsers()
  loadCategories()
  loadPosts()
})
</script>

<style scoped>
.admin-dashboard {
  max-width: 1400px;
  margin: 0 auto;
  padding: 20px;
  min-height: 100vh;
}

.admin-dashboard h1 {
  font-size: 32px;
  margin-bottom: 30px;
}

.admin-tabs {
  display: flex;
  gap: 10px;
  margin-bottom: 30px;
  border-bottom: 2px solid #e0e0e0;
}

.tab-button {
  padding: 12px 24px;
  background: none;
  border: none;
  font-size: 16px;
  cursor: pointer;
  color: #666;
  border-bottom: 3px solid transparent;
  transition: all 0.3s;
}

.tab-button:hover {
  color: #333;
}

.tab-button.active {
  color: #4CAF50;
  border-bottom-color: #4CAF50;
}

.tab-content {
  min-height: 600px;
  position: relative;
}

.tab-content h2 {
  font-size: 24px;
  margin-bottom: 20px;
}

.loading {
  text-align: center;
  padding: 40px;
  font-size: 18px;
  color: #666;
}

.admin-table {
  width: 100%;
  border-collapse: collapse;
  background: white;
  box-shadow: 0 1px 3px rgba(0, 0, 0, 0.1);
  table-layout: fixed;
}

.admin-table th {
  background-color: #f5f5f5;
  padding: 12px;
  text-align: left;
  font-weight: 600;
  border-bottom: 2px solid #e0e0e0;
}

.admin-table th:nth-child(1) {
  width: 60px;
}

.admin-table th:nth-child(2) {
  width: 200px;
  max-width: 200px;
}

.admin-table td {
  padding: 12px;
  border-bottom: 1px solid #e0e0e0;
}

.admin-table td:nth-child(2) {
  overflow: hidden;
  text-overflow: ellipsis;
  white-space: nowrap;
  max-width: 200px;
}

.admin-table tr:hover {
  background-color: #f9f9f9;
}

.status {
  padding: 4px 8px;
  border-radius: 4px;
  font-size: 14px;
}

.status.active {
  background-color: #e8f5e9;
  color: #2e7d32;
}

.status.inactive {
  background-color: #ffebee;
  color: #c62828;
}

.role {
  padding: 4px 8px;
  border-radius: 4px;
  font-size: 14px;
}

.role.admin {
  background-color: #e3f2fd;
  color: #1976d2;
}

.role.user {
  background-color: #f5f5f5;
  color: #666;
}

.badge {
  padding: 2px 8px;
  border-radius: 12px;
  font-size: 12px;
  margin-right: 4px;
}

.badge.pinned {
  background-color: #fff3cd;
  color: #856404;
}

.badge.locked {
  background-color: #f8d7da;
  color: #721c24;
}

.actions {
  display: flex;
  gap: 8px;
}

.action-btn {
  padding: 6px 12px;
  border: 1px solid #ddd;
  background: white;
  border-radius: 4px;
  cursor: pointer;
  font-size: 14px;
  transition: all 0.3s;
}

.action-btn:hover {
  background-color: #f5f5f5;
}

.action-btn:disabled {
  opacity: 0.5;
  cursor: not-allowed;
}

.btn-primary {
  background-color: #4CAF50;
  color: white;
  border: none;
  padding: 10px 20px;
  border-radius: 4px;
  cursor: pointer;
  margin-bottom: 20px;
}

.btn-primary:hover {
  background-color: #45a049;
}

.btn-secondary {
  background-color: #666;
  color: white;
  border: none;
  padding: 10px 20px;
  border-radius: 4px;
  cursor: pointer;
}

.btn-secondary:hover {
  background-color: #555;
}

.btn-danger {
  background-color: #f44336;
  color: white;
}

.btn-danger:hover {
  background-color: #da190b;
}

.btn-success {
  background-color: #4CAF50;
  color: white;
}

.btn-success:hover {
  background-color: #45a049;
}

.pagination {
  display: flex;
  justify-content: center;
  align-items: center;
  gap: 20px;
  margin-top: 30px;
}

.page-button {
  padding: 8px 16px;
  background-color: #f0f0f0;
  border: 1px solid #ddd;
  border-radius: 4px;
  cursor: pointer;
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

.modal {
  position: fixed;
  top: 0;
  left: 0;
  right: 0;
  bottom: 0;
  background-color: rgba(0, 0, 0, 0.5);
  display: flex;
  align-items: center;
  justify-content: center;
  z-index: 1000;
}

.modal-content {
  background: white;
  padding: 30px;
  border-radius: 8px;
  max-width: 500px;
  width: 90%;
}

.modal-content h3 {
  margin-bottom: 20px;
}

.form-group {
  margin-bottom: 20px;
}

.form-group label {
  display: block;
  margin-bottom: 5px;
  font-weight: 600;
}

.form-group input,
.form-group textarea,
.form-group select {
  width: 100%;
  padding: 10px;
  border: 1px solid #ddd;
  border-radius: 4px;
  font-size: 16px;
}

.form-actions {
  display: flex;
  gap: 10px;
  justify-content: flex-end;
  margin-top: 20px;
}

.post-title a {
  color: #333;
  text-decoration: none;
}

.post-title a:hover {
  color: #4CAF50;
  text-decoration: underline;
}
</style>