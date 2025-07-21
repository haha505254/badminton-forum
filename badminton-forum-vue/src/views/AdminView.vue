<template>
  <div class="flex h-[calc(100vh-4rem)]">
    <!-- Sidebar -->
    <AdminSidebar 
      :active-tab="activeTab" 
      @change-tab="activeTab = $event"
    />
    
    <!-- Main Content -->
    <div class="flex-1 overflow-y-auto bg-gray-50 dark:bg-gray-900">
      <div class="p-8">
        <!-- Overview Tab -->
        <div v-if="activeTab === 'overview'" class="space-y-6">
          <h1 class="text-3xl font-bold text-gray-900 dark:text-white">總覽</h1>
          
          <!-- Stats Grid -->
          <div class="grid grid-cols-1 gap-4 md:grid-cols-2 lg:grid-cols-4">
            <StatsCard 
              title="總使用者數" 
              :total="stats.totalUsers" 
              rate="+5.2%" 
              :levelUp="true"
            >
              <template #icon>
                <svg class="fill-primary-600 dark:fill-primary-400" width="22" height="22" viewBox="0 0 24 24">
                  <path d="M16 11c1.66 0 2.99-1.34 2.99-3S17.66 5 16 5c-1.66 0-3 1.34-3 3s1.34 3 3 3zm-8 0c1.66 0 2.99-1.34 2.99-3S9.66 5 8 5C6.34 5 5 6.34 5 8s1.34 3 3 3zm0 2c-2.33 0-7 1.17-7 3.5V19h14v-2.5c0-2.33-4.67-3.5-7-3.5zm8 0c-.29 0-.62.02-.97.05 1.16.84 1.97 1.97 1.97 3.45V19h6v-2.5c0-2.33-4.67-3.5-7-3.5z"/>
                </svg>
              </template>
            </StatsCard>
            
            <StatsCard 
              title="總文章數" 
              :total="stats.totalPosts" 
              rate="+12.5%" 
              :levelUp="true"
            >
              <template #icon>
                <svg class="fill-primary-600 dark:fill-primary-400" width="22" height="22" viewBox="0 0 24 24">
                  <path d="M19 3H5c-1.1 0-2 .9-2 2v14c0 1.1.9 2 2 2h14c1.1 0 2-.9 2-2V5c0-1.1-.9-2-2-2zM9 17H7v-7h2v7zm4 0h-2V7h2v10zm4 0h-2v-4h2v4z"/>
                </svg>
              </template>
            </StatsCard>
            
            <StatsCard 
              title="今日新帖" 
              :total="stats.todayPosts"
            >
              <template #icon>
                <svg class="fill-primary-600 dark:fill-primary-400" width="22" height="22" viewBox="0 0 24 24">
                  <path d="M19 3h-1V1h-2v2H8V1H6v2H5c-1.11 0-1.99.9-1.99 2L3 19c0 1.1.89 2 2 2h14c1.1 0 2-.9 2-2V5c0-1.1-.9-2-2-2zm0 16H5V8h14v11zM7 10h5v5H7z"/>
                </svg>
              </template>
            </StatsCard>
            
            <StatsCard 
              title="待處理檢舉" 
              :total="stats.pendingReports"
              rate="-15%" 
              :levelUp="false"
            >
              <template #icon>
                <svg class="fill-primary-600 dark:fill-primary-400" width="22" height="22" viewBox="0 0 24 24">
                  <path d="M1 21h22L12 2 1 21zm12-3h-2v-2h2v2zm0-4h-2v-4h2v4z"/>
                </svg>
              </template>
            </StatsCard>
          </div>
          
          <!-- Recent Activity -->
          <div class="grid grid-cols-1 lg:grid-cols-2 gap-6">
            <div class="card-dark">
              <h3 class="text-xl font-semibold text-gray-900 dark:text-white mb-4">最新註冊用戶</h3>
              <div class="space-y-3">
                <div v-for="user in recentUsers" :key="user.id" class="flex items-center justify-between">
                  <div class="flex items-center space-x-3">
                    <div class="w-10 h-10 bg-primary-100 dark:bg-primary-900 rounded-full flex items-center justify-center">
                      <span class="text-sm font-medium text-primary-600 dark:text-primary-400">
                        {{ user.username.charAt(0).toUpperCase() }}
                      </span>
                    </div>
                    <div>
                      <p class="font-medium text-gray-900 dark:text-white">{{ user.username }}</p>
                      <p class="text-sm text-gray-500 dark:text-gray-400">{{ formatDate(user.createdAt) }}</p>
                    </div>
                  </div>
                </div>
              </div>
            </div>
            
            <div class="card-dark">
              <h3 class="text-xl font-semibold text-gray-900 dark:text-white mb-4">熱門文章</h3>
              <div class="space-y-3">
                <div v-for="post in popularPosts" :key="post.id" class="border-b border-gray-200 dark:border-gray-700 pb-3 last:border-0">
                  <RouterLink :to="`/post/${post.id}`" class="text-primary-600 hover:text-primary-700 dark:text-primary-400 font-medium">
                    {{ post.title }}
                  </RouterLink>
                  <div class="flex items-center space-x-4 mt-1 text-sm text-gray-500 dark:text-gray-400">
                    <span>{{ post.viewCount }} 瀏覽</span>
                    <span>{{ post.replyCount }} 回覆</span>
                  </div>
                </div>
              </div>
            </div>
          </div>
        </div>
        
        <!-- Users Tab -->
        <div v-else-if="activeTab === 'users'" class="space-y-6">
          <div class="flex justify-between items-center">
            <h1 class="text-3xl font-bold text-gray-900 dark:text-white">使用者管理</h1>
            <div class="flex space-x-4">
              <input 
                type="text" 
                v-model="userSearch" 
                placeholder="搜尋使用者..."
                class="form-input"
              />
            </div>
          </div>
          
          <div class="card-dark overflow-hidden">
            <div class="overflow-x-auto">
              <DataTable
                title=""
                :headers="userHeaders"
                :items="filteredUsers"
              >
                <template #row="{ item }">
                  <div class="col-span-1">{{ item.id }}</div>
                  <div class="col-span-2">
                    <div class="flex items-center space-x-2">
                      <div class="w-8 h-8 bg-primary-100 dark:bg-primary-900 rounded-full flex items-center justify-center">
                        <span class="text-xs font-medium text-primary-600 dark:text-primary-400">
                          {{ item.username.charAt(0).toUpperCase() }}
                        </span>
                      </div>
                      <span class="font-medium">{{ item.username }}</span>
                    </div>
                  </div>
                  <div class="col-span-2 text-sm">{{ item.email }}</div>
                  <div class="col-span-1 text-sm">{{ formatDate(item.createdAt) }}</div>
                  <div class="col-span-1">
                    <span :class="[
                      'inline-flex px-2 py-1 text-xs font-medium rounded-full',
                      item.isActive 
                        ? 'bg-green-100 text-green-800 dark:bg-green-900 dark:text-green-200'
                        : 'bg-red-100 text-red-800 dark:bg-red-900 dark:text-red-200'
                    ]">
                      {{ item.isActive ? '正常' : '停用' }}
                    </span>
                  </div>
                  <div class="col-span-1 flex space-x-2">
                    <button 
                      @click="toggleUserActive(item)"
                      :class="[
                        'px-3 py-1 text-sm rounded',
                        item.isActive ? 'btn-outline' : 'btn-primary'
                      ]"
                    >
                      {{ item.isActive ? '停用' : '啟用' }}
                    </button>
                  </div>
                </template>
              </DataTable>
            </div>
          </div>
        </div>
        
        <!-- Categories Tab -->
        <div v-else-if="activeTab === 'categories'" class="space-y-6">
          <div class="flex justify-between items-center">
            <h1 class="text-3xl font-bold text-gray-900 dark:text-white">版塊管理</h1>
            <button @click="showCategoryModal = true" class="btn-primary">
              新增版塊
            </button>
          </div>
          
          <div class="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-3 gap-6">
            <div 
              v-for="category in categories" 
              :key="category.id"
              class="card-dark hover:shadow-lg transition-shadow"
            >
              <div class="flex items-start justify-between mb-4">
                <div class="text-3xl">{{ category.icon }}</div>
                <div class="flex space-x-2">
                  <button @click="editCategory(category)" class="text-primary-600 hover:text-primary-700">
                    <svg class="w-5 h-5" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                      <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M11 5H6a2 2 0 00-2 2v11a2 2 0 002 2h11a2 2 0 002-2v-5m-1.414-9.414a2 2 0 112.828 2.828L11.828 15H9v-2.828l8.586-8.586z"/>
                    </svg>
                  </button>
                  <button 
                    @click="deleteCategory(category)" 
                    :disabled="category.postCount > 0"
                    class="text-red-600 hover:text-red-700 disabled:opacity-50"
                  >
                    <svg class="w-5 h-5" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                      <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M19 7l-.867 12.142A2 2 0 0116.138 21H7.862a2 2 0 01-1.995-1.858L5 7m5 4v6m4-6v6m1-10V4a1 1 0 00-1-1h-4a1 1 0 00-1 1v3M4 7h16"/>
                    </svg>
                  </button>
                </div>
              </div>
              <h3 class="text-xl font-semibold text-gray-900 dark:text-white mb-2">
                {{ category.name }}
              </h3>
              <p class="text-gray-600 dark:text-gray-300 mb-4">
                {{ category.description }}
              </p>
              <div class="text-sm text-gray-500 dark:text-gray-400">
                {{ category.postCount }} 篇文章
              </div>
            </div>
          </div>
        </div>
        
        <!-- Posts Tab -->
        <div v-else-if="activeTab === 'posts'" class="space-y-6">
          <div class="flex justify-between items-center">
            <h1 class="text-3xl font-bold text-gray-900 dark:text-white">文章管理</h1>
            <div class="flex space-x-4">
              <select v-model="postFilter" class="form-input">
                <option value="all">全部文章</option>
                <option value="pinned">置頂文章</option>
                <option value="locked">鎖定文章</option>
              </select>
            </div>
          </div>
          
          <!-- Posts implementation similar to above but with Tailwind classes -->
        </div>
      </div>
    </div>
  </div>
</template>

<script setup>
import { ref, computed, onMounted } from 'vue'
import { useRouter, RouterLink } from 'vue-router'
import { useAuthStore } from '../stores/auth'
import { adminApi } from '../api/admin'
import AdminSidebar from '../components/ui/AdminSidebar.vue'
import StatsCard from '../components/ui/StatsCard.vue'
import DataTable from '../components/ui/DataTable.vue'

const router = useRouter()
const authStore = useAuthStore()

// Check admin permission
if (!authStore.user?.isAdmin) {
  router.push('/')
}

// State
const activeTab = ref('overview')
const loading = ref(false)

// Overview stats
const stats = ref({
  totalUsers: '1,523',
  totalPosts: '3,847',
  todayPosts: '47',
  pendingReports: '5'
})

const recentUsers = ref([
  { id: 1, username: '新用戶1', createdAt: new Date() },
  { id: 2, username: '新用戶2', createdAt: new Date() },
  { id: 3, username: '新用戶3', createdAt: new Date() }
])

const popularPosts = ref([
  { id: 1, title: '如何提升反手技術？', viewCount: 1523, replyCount: 45 },
  { id: 2, title: 'YONEX 新款球拍評測', viewCount: 1234, replyCount: 32 },
  { id: 3, title: '2024年羽球規則更新', viewCount: 987, replyCount: 28 }
])

// Users management
const users = ref([])
const userSearch = ref('')
const userHeaders = [
  { text: 'ID', value: 'id', class: 'col-span-1' },
  { text: '使用者名稱', value: 'username', class: 'col-span-2' },
  { text: '電子郵件', value: 'email', class: 'col-span-2' },
  { text: '註冊日期', value: 'createdAt', class: 'col-span-1' },
  { text: '狀態', value: 'status', class: 'col-span-1' },
  { text: '操作', value: 'actions', class: 'col-span-1' }
]

const filteredUsers = computed(() => {
  if (!userSearch.value) return users.value
  return users.value.filter(user => 
    user.username.toLowerCase().includes(userSearch.value.toLowerCase()) ||
    user.email.toLowerCase().includes(userSearch.value.toLowerCase())
  )
})

// Categories management
const categories = ref([])
const showCategoryModal = ref(false)
const editingCategory = ref(null)

// Posts management
const posts = ref([])
const postFilter = ref('all')

// Methods
const formatDate = (date) => {
  return new Date(date).toLocaleDateString('zh-TW')
}

const toggleUserActive = async (user) => {
  try {
    const response = await adminApi.toggleUserActive(user.id)
    user.isActive = response.data.isActive
  } catch (error) {
    console.error('Failed to toggle user status:', error)
  }
}

const editCategory = (category) => {
  editingCategory.value = category
  showCategoryModal.value = true
}

const deleteCategory = async (category) => {
  if (category.postCount > 0) {
    alert('無法刪除含有文章的版塊')
    return
  }
  
  if (!confirm(`確定要刪除版塊「${category.name}」嗎？`)) return
  
  try {
    await adminApi.deleteCategory(category.id)
    await loadCategories()
  } catch (error) {
    console.error('Failed to delete category:', error)
  }
}

// Load data
const loadUsers = async () => {
  try {
    const response = await adminApi.getUsers()
    users.value = response.data
  } catch (error) {
    console.error('Failed to load users:', error)
  }
}

const loadCategories = async () => {
  try {
    const response = await adminApi.getCategories()
    categories.value = response.data
  } catch (error) {
    console.error('Failed to load categories:', error)
  }
}

onMounted(() => {
  loadUsers()
  loadCategories()
})
</script>