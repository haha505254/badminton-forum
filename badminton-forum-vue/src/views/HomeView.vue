<template>
  <div>
    <!-- Hero Section -->
    <div class="text-center mb-12">
      <h1 class="text-4xl md:text-5xl font-bold text-gray-900 dark:text-white mb-4">
        🎉 CI/CD 自動部署成功！🎉
      </h1>
      <p class="text-xl text-gray-600 dark:text-gray-300">
        這段文字是透過 GitHub Actions 自動部署上來的！
      </p>
    </div>

    <!-- Stats Cards -->
    <div class="grid grid-cols-1 gap-4 md:grid-cols-2 md:gap-6 xl:grid-cols-4 2xl:gap-7.5 mb-8">
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
        title="活躍用戶" 
        :total="stats.activeUsers" 
        rate="+8.2%" 
        :levelUp="true"
      >
        <template #icon>
          <svg class="fill-primary-600 dark:fill-primary-400" width="22" height="22" viewBox="0 0 24 24">
            <path d="M16 11c1.66 0 2.99-1.34 2.99-3S17.66 5 16 5c-1.66 0-3 1.34-3 3s1.34 3 3 3zm-8 0c1.66 0 2.99-1.34 2.99-3S9.66 5 8 5C6.34 5 5 6.34 5 8s1.34 3 3 3zm0 2c-2.33 0-7 1.17-7 3.5V19h14v-2.5c0-2.33-4.67-3.5-7-3.5zm8 0c-.29 0-.62.02-.97.05 1.16.84 1.97 1.97 1.97 3.45V19h6v-2.5c0-2.33-4.67-3.5-7-3.5z"/>
          </svg>
        </template>
      </StatsCard>
      
      <StatsCard 
        title="今日新帖" 
        :total="stats.todayPosts" 
        rate="-2.1%" 
        :levelUp="false"
      >
        <template #icon>
          <svg class="fill-primary-600 dark:fill-primary-400" width="22" height="22" viewBox="0 0 24 24">
            <path d="M19 3h-1V1h-2v2H8V1H6v2H5c-1.11 0-1.99.9-1.99 2L3 19c0 1.1.89 2 2 2h14c1.1 0 2-.9 2-2V5c0-1.1-.9-2-2-2zm0 16H5V8h14v11zM7 10h5v5H7z"/>
          </svg>
        </template>
      </StatsCard>
      
      <StatsCard 
        title="在線用戶" 
        :total="stats.onlineUsers"
      >
        <template #icon>
          <svg class="fill-primary-600 dark:fill-primary-400" width="22" height="22" viewBox="0 0 24 24">
            <path d="M12 2C6.48 2 2 6.48 2 12s4.48 10 10 10 10-4.48 10-10S17.52 2 12 2zm-2 15l-5-5 1.41-1.41L10 14.17l7.59-7.59L19 8l-9 9z"/>
          </svg>
        </template>
      </StatsCard>
    </div>
    
    <!-- Features Grid -->
    <div class="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-4 gap-6 mb-8">
      <div class="card-dark hover:shadow-lg transition-shadow cursor-pointer">
        <div class="flex items-center mb-4">
          <span class="text-3xl mr-3">🏸</span>
          <h3 class="text-xl font-semibold text-gray-900 dark:text-white">技術討論</h3>
        </div>
        <p class="text-gray-600 dark:text-gray-300">
          分享和學習羽毛球技術，提升您的球技
        </p>
      </div>
      
      <div class="card-dark hover:shadow-lg transition-shadow cursor-pointer">
        <div class="flex items-center mb-4">
          <span class="text-3xl mr-3">🎾</span>
          <h3 class="text-xl font-semibold text-gray-900 dark:text-white">裝備推薦</h3>
        </div>
        <p class="text-gray-600 dark:text-gray-300">
          了解最新的球拍、球鞋等裝備資訊
        </p>
      </div>
      
      <div class="card-dark hover:shadow-lg transition-shadow cursor-pointer">
        <div class="flex items-center mb-4">
          <span class="text-3xl mr-3">📅</span>
          <h3 class="text-xl font-semibold text-gray-900 dark:text-white">活動公告</h3>
        </div>
        <p class="text-gray-600 dark:text-gray-300">
          參加比賽和活動，認識更多球友
        </p>
      </div>
      
      <div class="card-dark hover:shadow-lg transition-shadow cursor-pointer">
        <div class="flex items-center mb-4">
          <span class="text-3xl mr-3">👥</span>
          <h3 class="text-xl font-semibold text-gray-900 dark:text-white">球友交流</h3>
        </div>
        <p class="text-gray-600 dark:text-gray-300">
          尋找志同道合的球友，一起打球
        </p>
      </div>
    </div>

    <!-- Latest Posts Section -->
    <div class="grid grid-cols-1 lg:grid-cols-3 gap-6">
      <!-- Latest Posts Table -->
      <div class="lg:col-span-2">
        <DataTable 
          title="最新文章" 
          :headers="postHeaders" 
          :items="latestPosts"
        >
          <template #row="{ item }">
            <div class="col-span-3">
              <RouterLink 
                :to="`/post/${item.id}`" 
                class="text-primary-600 hover:text-primary-700 dark:text-primary-400"
              >
                {{ item.title }}
              </RouterLink>
            </div>
            <div class="col-span-2 text-sm text-gray-600 dark:text-gray-400">
              {{ item.author }}
            </div>
            <div class="col-span-1 text-sm text-gray-600 dark:text-gray-400 text-center">
              {{ item.replies }}
            </div>
          </template>
        </DataTable>
      </div>
      
      <!-- Quick Actions -->
      <div>
        <div class="card-dark">
          <h3 class="text-xl font-semibold text-gray-900 dark:text-white mb-6">
            快速操作
          </h3>
          <div class="space-y-4">
            <RouterLink to="/categories" class="btn-primary w-full text-center">
              瀏覽論壇版塊
            </RouterLink>
            <RouterLink to="/new-post" class="btn-secondary w-full text-center">
              發表新文章
            </RouterLink>
            <RouterLink to="/search" class="btn-outline w-full text-center">
              搜尋文章
            </RouterLink>
          </div>
        </div>
      </div>
    </div>
  </div>
</template>

<script setup>
import { ref, onMounted } from 'vue'
import { RouterLink } from 'vue-router'
import StatsCard from '../components/ui/StatsCard.vue'
import DataTable from '../components/ui/DataTable.vue'

// Mock data - 實際應該從 API 獲取
const stats = ref({
  totalPosts: '1,245',
  activeUsers: '523',
  todayPosts: '28',
  onlineUsers: '87'
})

const postHeaders = [
  { text: '標題', value: 'title', class: 'col-span-3' },
  { text: '作者', value: 'author', class: 'col-span-2' },
  { text: '回覆', value: 'replies', class: 'col-span-1 text-center' }
]

const latestPosts = ref([
  { id: 1, title: '新手請教：正手高遠球總是打不遠怎麼辦？', author: '小明', replies: 12 },
  { id: 2, title: '分享：YONEX ARC11 PRO 使用心得', author: '球拍達人', replies: 8 },
  { id: 3, title: '週末約打球 - 台北大安運動中心', author: '愛打球', replies: 5 },
  { id: 4, title: '2024年全國羽球錦標賽報名開始', author: '管理員', replies: 23 },
  { id: 5, title: '請問大家都用什麼羽球？', author: '新手上路', replies: 15 }
])

onMounted(() => {
  // TODO: 從 API 獲取實際數據
})
</script>