<template>
  <div class="dashboard">
    <h1 class="page-title">羽球論壇管理儀表板</h1>

    <!-- 統計卡片 -->
    <div class="row">
      <div class="flex xs12 sm6 md3">
        <VaCard class="mb-4">
          <VaCardContent>
            <div class="flex items-center">
              <VaIcon name="group" size="large" color="primary" class="mr-4" />
              <div>
                <p class="text-secondary text-xs">總用戶數</p>
                <h2 class="text-2xl font-bold">{{ stats.users }}</h2>
                <p class="text-success text-xs">
                  <VaIcon name="trending_up" size="small" />
                  本月新增 {{ stats.newUsers }}
                </p>
              </div>
            </div>
          </VaCardContent>
        </VaCard>
      </div>

      <div class="flex xs12 sm6 md3">
        <VaCard class="mb-4">
          <VaCardContent>
            <div class="flex items-center">
              <VaIcon name="article" size="large" color="info" class="mr-4" />
              <div>
                <p class="text-secondary text-xs">總貼文數</p>
                <h2 class="text-2xl font-bold">{{ stats.posts }}</h2>
                <p class="text-success text-xs">
                  <VaIcon name="trending_up" size="small" />
                  今日新增 {{ stats.todayPosts }}
                </p>
              </div>
            </div>
          </VaCardContent>
        </VaCard>
      </div>

      <div class="flex xs12 sm6 md3">
        <VaCard class="mb-4">
          <VaCardContent>
            <div class="flex items-center">
              <VaIcon name="comment" size="large" color="warning" class="mr-4" />
              <div>
                <p class="text-secondary text-xs">總回覆數</p>
                <h2 class="text-2xl font-bold">{{ stats.replies }}</h2>
                <p class="text-success text-xs">
                  <VaIcon name="trending_up" size="small" />
                  今日新增 {{ stats.todayReplies }}
                </p>
              </div>
            </div>
          </VaCardContent>
        </VaCard>
      </div>

      <div class="flex xs12 sm6 md3">
        <VaCard class="mb-4">
          <VaCardContent>
            <div class="flex items-center">
              <VaIcon name="visibility" size="large" color="success" class="mr-4" />
              <div>
                <p class="text-secondary text-xs">總瀏覽量</p>
                <h2 class="text-2xl font-bold">{{ stats.views }}</h2>
                <p class="text-success text-xs">
                  <VaIcon name="trending_up" size="small" />
                  今日瀏覽 {{ stats.todayViews }}
                </p>
              </div>
            </div>
          </VaCardContent>
        </VaCard>
      </div>
    </div>

    <!-- 圖表區域 -->
    <div class="row">
      <div class="flex xs12 md8">
        <VaCard class="mb-4">
          <VaCardTitle>
            <h3>每日活動趨勢</h3>
          </VaCardTitle>
          <VaCardContent>
            <VaChart :data="chartData" type="line" :options="chartOptions" style="height: 300px" />
          </VaCardContent>
        </VaCard>
      </div>

      <div class="flex xs12 md4">
        <VaCard class="mb-4">
          <VaCardTitle>
            <h3>分類文章分布</h3>
          </VaCardTitle>
          <VaCardContent>
            <VaChart :data="pieChartData" type="doughnut" :options="pieChartOptions" style="height: 300px" />
          </VaCardContent>
        </VaCard>
      </div>
    </div>

    <!-- 最新活動 -->
    <div class="row">
      <div class="flex xs12 md6">
        <VaCard>
          <VaCardTitle>
            <h3>最新貼文</h3>
          </VaCardTitle>
          <VaCardContent>
            <VaList>
              <VaListItem v-for="post in recentPosts" :key="post.id">
                <VaListItemSection avatar>
                  <VaAvatar :src="post.author?.avatar" :fallback-text="post.author?.username?.[0]" size="small" />
                </VaListItemSection>
                <VaListItemSection>
                  <VaListItemLabel>
                    {{ post.title }}
                  </VaListItemLabel>
                  <VaListItemLabel caption>
                    {{ post.author?.username }} · {{ formatTime(post.createdAt) }}
                  </VaListItemLabel>
                </VaListItemSection>
                <VaListItemSection icon>
                  <VaBadge :text="post.category?.name" color="info" />
                </VaListItemSection>
              </VaListItem>
            </VaList>
          </VaCardContent>
        </VaCard>
      </div>

      <div class="flex xs12 md6">
        <VaCard>
          <VaCardTitle>
            <h3>最新用戶</h3>
          </VaCardTitle>
          <VaCardContent>
            <VaList>
              <VaListItem v-for="user in recentUsers" :key="user.id">
                <VaListItemSection avatar>
                  <VaAvatar :src="user.avatar" :fallback-text="user.username?.[0]" size="small" />
                </VaListItemSection>
                <VaListItemSection>
                  <VaListItemLabel>
                    {{ user.username }}
                  </VaListItemLabel>
                  <VaListItemLabel caption> {{ user.email }} · {{ formatTime(user.createdAt) }} </VaListItemLabel>
                </VaListItemSection>
                <VaListItemSection icon>
                  <VaBadge
                    :text="user.provider || 'Local'"
                    :color="user.provider === 'Google' ? 'warning' : 'success'"
                  />
                </VaListItemSection>
              </VaListItem>
            </VaList>
          </VaCardContent>
        </VaCard>
      </div>
    </div>
  </div>
</template>

<script setup lang="ts">
import { ref, onMounted, computed } from 'vue'
import VaChart from '../../../components/va-charts/VaChart.vue'
import adminApi from '../../../api/admin'

// 註冊 Chart.js 必要模組
import {
  Chart as ChartJS,
  CategoryScale,
  LinearScale,
  PointElement,
  LineElement,
  ArcElement,
  Title,
  Tooltip,
  Legend,
  Filler,
} from 'chart.js'

ChartJS.register(CategoryScale, LinearScale, PointElement, LineElement, ArcElement, Title, Tooltip, Legend, Filler)

// 統計數據
const stats = ref({
  users: 0,
  newUsers: 0,
  posts: 0,
  todayPosts: 0,
  replies: 0,
  todayReplies: 0,
  views: 0,
  todayViews: 0,
})

// 最新數據
const recentPosts = ref([])
const recentUsers = ref([])

// 圖表數據
const chartData = computed(() => ({
  labels: ['週一', '週二', '週三', '週四', '週五', '週六', '週日'],
  datasets: [
    {
      label: '新貼文',
      data: [12, 19, 15, 25, 22, 30, 28],
      borderColor: '#4CAF50',
      backgroundColor: 'rgba(76, 175, 80, 0.1)',
      tension: 0.4,
    },
    {
      label: '新用戶',
      data: [5, 8, 3, 12, 7, 15, 10],
      borderColor: '#2196F3',
      backgroundColor: 'rgba(33, 150, 243, 0.1)',
      tension: 0.4,
    },
  ],
}))

const chartOptions = {
  responsive: true,
  maintainAspectRatio: false,
  plugins: {
    legend: {
      position: 'top',
    },
  },
  scales: {
    y: {
      beginAtZero: true,
    },
  },
}

const pieChartData = computed(() => ({
  labels: ['綜合討論區', '技術交流區', '裝備討論區', '賽事專區', '地區球友會'],
  datasets: [
    {
      data: [30, 25, 20, 15, 10],
      backgroundColor: ['#4CAF50', '#2196F3', '#FFC107', '#9C27B0', '#FF5722'],
    },
  ],
}))

const pieChartOptions = {
  responsive: true,
  maintainAspectRatio: false,
  plugins: {
    legend: {
      position: 'right',
    },
  },
}

// 載入數據
async function loadDashboardData() {
  try {
    // 載入用戶統計
    const usersResponse = await adminApi.getUsers(1, 10)
    stats.value.users = parseInt(usersResponse.headers['x-total-count'] || '0')
    recentUsers.value = usersResponse.data.slice(0, 5)

    // 載入貼文統計
    const postsResponse = await adminApi.getPosts(1, 10)
    stats.value.posts = parseInt(postsResponse.headers['x-total-count'] || '0')
    recentPosts.value = postsResponse.data.slice(0, 5)

    // 模擬其他統計數據
    stats.value.newUsers = Math.floor(Math.random() * 20) + 5
    stats.value.todayPosts = Math.floor(Math.random() * 10) + 1
    stats.value.replies = Math.floor(Math.random() * 1000) + 500
    stats.value.todayReplies = Math.floor(Math.random() * 50) + 10
    stats.value.views = Math.floor(Math.random() * 10000) + 5000
    stats.value.todayViews = Math.floor(Math.random() * 500) + 100
  } catch (error) {
    console.error('載入儀表板數據失敗:', error)
  }
}

// 格式化時間
function formatTime(dateString) {
  const date = new Date(dateString)
  const now = new Date()
  const diff = now - date

  if (diff < 60000) return '剛剛'
  if (diff < 3600000) return `${Math.floor(diff / 60000)} 分鐘前`
  if (diff < 86400000) return `${Math.floor(diff / 3600000)} 小時前`
  if (diff < 604800000) return `${Math.floor(diff / 86400000)} 天前`

  return date.toLocaleDateString('zh-TW')
}

onMounted(() => {
  loadDashboardData()
})
</script>

<style scoped>
.dashboard {
  padding: 1.5rem;
}

.page-title {
  font-size: 1.75rem;
  font-weight: 600;
  margin-bottom: 1.5rem;
}
</style>
