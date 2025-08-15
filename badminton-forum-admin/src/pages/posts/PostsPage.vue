<template>
  <div class="posts-page">
    <div class="row align-center mb-4">
      <h1 class="flex">貼文管理</h1>
      <VaInput
        v-model="filter"
        placeholder="搜尋貼文..."
        class="mr-3"
        style="max-width: 300px"
      >
        <template #prepend>
          <VaIcon name="search" />
        </template>
      </VaInput>
    </div>

    <VaCard>
      <VaCardContent>
        <div class="table-wrapper">
          <VaDataTable
            :items="posts"
            :columns="columns"
            :loading="loading"
            :pagination="pagination"
            @update:pagination="updatePagination"
          >
            <template #cell(title)="{ rowData }">
              <div class="max-w-xs truncate">
                {{ rowData.title }}
              </div>
            </template>

            <template #cell(author)="{ rowData }">
              <div class="flex items-center">
                <VaAvatar
                  :src="rowData.author?.avatar"
                  :fallback-text="rowData.author?.username?.[0]"
                  size="small"
                  class="mr-2"
                />
                {{ rowData.author?.username }}
              </div>
            </template>

            <template #cell(category)="{ rowData }">
              <VaBadge :text="rowData.category?.name" color="info" />
            </template>

            <template #cell(status)="{ rowData }">
              <div class="flex gap-1">
                <VaBadge v-if="rowData.isPinned" text="置頂" color="warning" />
                <VaBadge v-if="rowData.isLocked" text="鎖定" color="danger" />
                <VaBadge v-if="rowData.isDeleted" text="已刪除" color="secondary" />
              </div>
            </template>

            <template #cell(createdAt)="{ rowData }">
              {{ formatDate(rowData.createdAt) }}
            </template>

            <template #cell(actions)="{ rowData }">
              <div class="flex gap-2">
                <VaButton
                  preset="plain"
                  icon="visibility"
                  @click="viewPost(rowData)"
                />
                <VaButton
                  v-if="!rowData.isPinned"
                  preset="plain"
                  icon="push_pin"
                  color="warning"
                  @click="togglePin(rowData)"
                />
                <VaButton
                  v-else
                  preset="plain"
                  icon="push_pin"
                  color="secondary"
                  @click="togglePin(rowData)"
                />
                <VaButton
                  v-if="!rowData.isLocked"
                  preset="plain"
                  icon="lock_open"
                  color="info"
                  @click="toggleLock(rowData)"
                />
                <VaButton
                  v-else
                  preset="plain"
                  icon="lock"
                  color="danger"
                  @click="toggleLock(rowData)"
                />
                <VaButton
                  v-if="!rowData.isDeleted"
                  preset="plain"
                  icon="delete"
                  color="danger"
                  @click="deletePost(rowData)"
                />
              </div>
            </template>
          </VaDataTable>
        </div>
      </VaCardContent>
    </VaCard>
  </div>
</template>

<script setup lang="ts">
import { ref, onMounted, computed } from 'vue'
import { useToast } from 'vuestic-ui'
import adminApi from '../../api/admin'

const { init: notify } = useToast()

const posts = ref([])
const loading = ref(false)
const filter = ref('')
const currentPage = ref(1)
const perPage = ref(20)
const total = ref(0)

const columns = [
  { key: 'title', label: '標題', sortable: true },
  { key: 'author', label: '作者', sortable: true },
  { key: 'category', label: '分類', sortable: true },
  { key: 'viewCount', label: '瀏覽數', sortable: true },
  { key: 'status', label: '狀態' },
  { key: 'createdAt', label: '發布時間', sortable: true },
  { key: 'actions', label: '操作', width: '150px' }
]

const pagination = computed(() => ({
  page: currentPage.value,
  perPage: perPage.value,
  total: total.value
}))

const filteredPosts = computed(() => {
  if (!filter.value) return posts.value
  const searchTerm = filter.value.toLowerCase()
  return posts.value.filter(post => 
    post.title?.toLowerCase().includes(searchTerm) ||
    post.author?.username?.toLowerCase().includes(searchTerm) ||
    post.category?.name?.toLowerCase().includes(searchTerm)
  )
})

async function fetchPosts() {
  loading.value = true
  try {
    const response = await adminApi.getPosts(currentPage.value, perPage.value)
    posts.value = response.data
    total.value = parseInt(response.headers['x-total-count'] || '0')
  } catch (error) {
    notify({
      message: '載入貼文失敗',
      color: 'danger'
    })
  } finally {
    loading.value = false
  }
}

async function togglePin(post) {
  try {
    await adminApi.togglePostPin(post.id)
    post.isPinned = !post.isPinned
    notify({
      message: post.isPinned ? '已置頂貼文' : '已取消置頂',
      color: 'success'
    })
  } catch (error) {
    notify({
      message: '操作失敗',
      color: 'danger'
    })
  }
}

async function toggleLock(post) {
  try {
    await adminApi.togglePostLock(post.id)
    post.isLocked = !post.isLocked
    notify({
      message: post.isLocked ? '已鎖定貼文' : '已解鎖貼文',
      color: 'success'
    })
  } catch (error) {
    notify({
      message: '操作失敗',
      color: 'danger'
    })
  }
}

async function deletePost(post) {
  if (!confirm('確定要刪除這篇貼文嗎？')) return
  
  try {
    await adminApi.deletePost(post.id)
    post.isDeleted = true
    notify({
      message: '貼文已刪除',
      color: 'success'
    })
  } catch (error) {
    notify({
      message: '刪除失敗',
      color: 'danger'
    })
  }
}

function viewPost(post) {
  window.open(`http://localhost:5173/posts/${post.id}`, '_blank')
}

function updatePagination(newPagination) {
  currentPage.value = newPagination.page
  perPage.value = newPagination.perPage
  fetchPosts()
}

function formatDate(dateString) {
  return new Date(dateString).toLocaleDateString('zh-TW', {
    year: 'numeric',
    month: '2-digit',
    day: '2-digit',
    hour: '2-digit',
    minute: '2-digit'
  })
}

onMounted(() => {
  fetchPosts()
})
</script>

<style scoped>
.posts-page {
  padding: 1.5rem;
}

.table-wrapper {
  overflow-x: auto;
}
</style>