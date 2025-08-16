<template>
  <div class="replies-page">
    <div class="flex flex-col md:flex-row gap-4 justify-between items-start mb-4">
      <h1 class="text-3xl font-bold">回覆管理</h1>
      <div class="flex gap-2">
        <VaButton v-if="selectedReplies.length > 0" preset="danger" :disabled="isLoading" @click="handleBatchDelete">
          批量刪除 ({{ selectedReplies.length }})
        </VaButton>
        <VaButton preset="secondary" :disabled="isLoading" @click="loadReplies"> 重新整理 </VaButton>
      </div>
    </div>

    <!-- 搜尋和篩選 -->
    <VaCard class="mb-4">
      <VaCardContent>
        <div class="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-4 gap-4">
          <VaInput
            v-model="filters.search"
            label="搜尋內容"
            placeholder="輸入關鍵字搜尋..."
            clearable
            @clear="handleSearch"
            @enter="handleSearch"
          />
          <VaInput
            v-model="filters.author"
            label="作者"
            placeholder="輸入作者名稱..."
            clearable
            @clear="handleSearch"
            @enter="handleSearch"
          />
          <VaDateInput
            v-model="filters.startDate"
            label="開始日期"
            clearable
            @clear="handleSearch"
            @update:modelValue="handleSearch"
          />
          <VaDateInput
            v-model="filters.endDate"
            label="結束日期"
            clearable
            @clear="handleSearch"
            @update:modelValue="handleSearch"
          />
        </div>
        <div class="flex justify-end mt-4">
          <VaButton preset="primary" :disabled="isLoading" @click="handleSearch"> 搜尋 </VaButton>
        </div>
      </VaCardContent>
    </VaCard>

    <!-- 回覆列表 -->
    <VaCard>
      <VaCardContent>
        <VaDataTable
          v-model:selected-items="selectedReplies"
          :items="replies"
          :columns="columns"
          :loading="isLoading"
          selectable
          select-mode="multiple"
          striped
          sticky-header
          :pagination="pagination"
          @update:pagination="updatePagination"
        >
          <template #cell(content)="{ value }">
            <div class="max-w-xs truncate" :title="stripHtml(value)">
              {{ stripHtml(value).substring(0, 50) }}{{ stripHtml(value).length > 50 ? '...' : '' }}
            </div>
          </template>

          <template #cell(replyTarget)="{ rowData }">
            <div v-if="!rowData.parentReplyId" class="text-gray-500">直接回覆</div>
            <div v-else>
              <span class="text-primary">@{{ rowData.parentReplyAuthorName }}</span>
              <VaBadge v-if="rowData.parentReplyIsDeleted" color="danger" size="small" class="ml-1"> 已刪除 </VaBadge>
            </div>
          </template>

          <template #cell(authorName)="{ value, rowData }">
            <RouterLink :to="`/users?id=${rowData.authorId}`" class="text-primary hover:underline">
              {{ value }}
            </RouterLink>
          </template>

          <template #cell(postTitle)="{ value, rowData }">
            <a
              :href="`http://localhost:5173/posts/${rowData.postId}`"
              target="_blank"
              class="text-primary hover:underline"
            >
              {{ value }}
            </a>
          </template>

          <template #cell(createdAt)="{ value }">
            {{ formatDate(value) }}
          </template>

          <template #cell(isDeleted)="{ rowData }">
            <VaBadge :color="rowData.isDeleted ? 'danger' : 'success'">
              {{ rowData.isDeleted ? '已刪除' : '正常' }}
            </VaBadge>
          </template>

          <template #cell(actions)="{ rowData }">
            <div class="flex gap-2">
              <VaButton preset="plain" size="small" @click="viewReply(rowData)"> 查看 </VaButton>
              <VaButton
                preset="plain"
                size="small"
                color="danger"
                :disabled="rowData.isDeleted"
                @click="deleteReply(rowData)"
              >
                刪除
              </VaButton>
            </div>
          </template>
        </VaDataTable>
      </VaCardContent>
    </VaCard>

    <!-- 查看回覆對話框 -->
    <VaModal v-model="showViewModal" title="查看回覆內容" size="large" :close-button="true">
      <div v-if="currentReply" class="space-y-4">
        <!-- 父回覆資訊（如果有） -->
        <div v-if="currentReply.parentReplyId" class="border-l-4 border-warning pl-4 mb-4">
          <h4 class="font-bold mb-2 text-warning">回覆對象</h4>
          <div class="space-y-2">
            <div>
              <strong>作者：</strong>
              <span>{{ currentReply.parentReplyAuthorName }}</span>
              <VaBadge v-if="currentReply.parentReplyIsDeleted" color="danger" size="small" class="ml-2">
                已刪除
              </VaBadge>
            </div>
            <div v-if="currentReply.parentReplyDeletedAt">
              <strong>刪除時間：</strong>{{ formatDate(currentReply.parentReplyDeletedAt) }}
            </div>
            <div>
              <strong>內容：</strong>
              <div
                class="mt-2 p-3 rounded"
                :class="
                  currentReply.parentReplyIsDeleted
                    ? 'bg-red-50 dark:bg-red-900/20 opacity-75'
                    : 'bg-gray-50 dark:bg-gray-800'
                "
              >
                <div v-if="currentReply.parentReplyIsDeleted" class="text-sm text-danger mb-2">
                  [此內容已被用戶刪除，保留供管理審查]
                </div>
                <div v-html="currentReply.parentReplyContent"></div>
              </div>
            </div>
          </div>
        </div>

        <!-- 當前回覆資訊 -->
        <div class="border-l-4 border-primary pl-4">
          <h4 class="font-bold mb-2 text-primary">當前回覆</h4>
          <div class="space-y-2">
            <div><strong>作者：</strong>{{ currentReply.authorName }}</div>
            <div>
              <strong>所屬文章：</strong>
              <a
                :href="`http://localhost:5173/posts/${currentReply.postId}`"
                target="_blank"
                class="text-primary hover:underline"
              >
                {{ currentReply.postTitle }}
              </a>
            </div>
            <div><strong>發表時間：</strong>{{ formatDate(currentReply.createdAt) }}</div>
            <div v-if="currentReply.updatedAt"><strong>更新時間：</strong>{{ formatDate(currentReply.updatedAt) }}</div>
            <div>
              <strong>狀態：</strong>
              <VaBadge :color="currentReply.isDeleted ? 'danger' : 'success'">
                {{ currentReply.isDeleted ? '已刪除' : '正常' }}
              </VaBadge>
            </div>
            <div v-if="currentReply.deletedAt"><strong>刪除時間：</strong>{{ formatDate(currentReply.deletedAt) }}</div>
            <div>
              <strong>內容：</strong>
              <div
                class="mt-2 p-4 rounded"
                :class="
                  currentReply.isDeleted ? 'bg-red-50 dark:bg-red-900/20 opacity-75' : 'bg-gray-100 dark:bg-gray-800'
                "
              >
                <div v-if="currentReply.isDeleted" class="text-sm text-danger mb-2">
                  [此內容已被刪除，保留供管理審查]
                </div>
                <div v-html="currentReply.content"></div>
              </div>
            </div>
          </div>
        </div>
      </div>
      <template #footer>
        <VaButton preset="secondary" @click="showViewModal = false"> 關閉 </VaButton>
      </template>
    </VaModal>
  </div>
</template>

<script setup lang="ts">
import { ref, onMounted } from 'vue'
import { useModal, useToast } from 'vuestic-ui'
import adminApi from '../../api/admin'

interface Reply {
  id: number
  content: string
  authorId: number
  authorName: string
  postId: number
  postTitle: string
  createdAt: string
  updatedAt?: string
  isDeleted: boolean
  deletedAt?: string
  // Parent reply information
  parentReplyId?: number
  parentReplyContent?: string
  parentReplyAuthorName?: string
  parentReplyAuthorId?: number
  parentReplyIsDeleted?: boolean
  parentReplyDeletedAt?: string
}

const { init: notify } = useToast()
const { confirm } = useModal()

const replies = ref<Reply[]>([])
const selectedReplies = ref<Reply[]>([])
const isLoading = ref(false)
const showViewModal = ref(false)
const currentReply = ref<Reply | null>(null)

// 篩選條件
const filters = ref({
  search: '',
  author: '',
  startDate: null as Date | null,
  endDate: null as Date | null,
})

// 分頁
const pagination = ref({
  page: 1,
  perPage: 20,
  total: 0,
})

// 表格欄位
const columns = [
  { key: 'id', label: 'ID', sortable: true },
  { key: 'content', label: '內容' },
  { key: 'replyTarget', label: '回覆對象' },
  { key: 'authorName', label: '作者', sortable: true },
  { key: 'postTitle', label: '所屬文章' },
  { key: 'createdAt', label: '發表時間', sortable: true },
  { key: 'isDeleted', label: '狀態', sortable: true },
  { key: 'actions', label: '操作', width: 120 },
]

// 載入回覆列表
const loadReplies = async () => {
  isLoading.value = true
  try {
    const params: any = {
      page: pagination.value.page,
      pageSize: pagination.value.perPage,
    }

    if (filters.value.search) {
      params.search = filters.value.search
    }
    if (filters.value.author) {
      params.author = filters.value.author
    }
    if (filters.value.startDate) {
      params.startDate = filters.value.startDate.toISOString()
    }
    if (filters.value.endDate) {
      params.endDate = filters.value.endDate.toISOString()
    }

    const response = await adminApi.getReplies(params)
    replies.value = response.data
    pagination.value.total = parseInt(response.headers['x-total-count'] || '0')
  } catch (error) {
    console.error('載入回覆失敗:', error)
    notify({
      title: '錯誤',
      message: '載入回覆列表失敗',
      color: 'danger',
    })
  } finally {
    isLoading.value = false
  }
}

// 搜尋
const handleSearch = () => {
  pagination.value.page = 1
  loadReplies()
}

// 更新分頁
const updatePagination = (newPagination: any) => {
  pagination.value = newPagination
  loadReplies()
}

// 查看回覆
const viewReply = (reply: Reply) => {
  currentReply.value = reply
  showViewModal.value = true
}

// 刪除單個回覆
const deleteReply = async (reply: Reply) => {
  const agreed = await confirm({
    title: '確認刪除',
    message: `確定要刪除這個回覆嗎？`,
    okText: '確定',
    cancelText: '取消',
    size: 'small',
  })

  if (!agreed) return

  try {
    await adminApi.deleteReply(reply.id)
    notify({
      title: '成功',
      message: '回覆已刪除',
      color: 'success',
    })
    loadReplies()
  } catch (error) {
    console.error('刪除回覆失敗:', error)
    notify({
      title: '錯誤',
      message: '刪除回覆失敗',
      color: 'danger',
    })
  }
}

// 批量刪除
const handleBatchDelete = async () => {
  if (selectedReplies.value.length === 0) return

  const agreed = await confirm({
    title: '確認批量刪除',
    message: `確定要刪除選中的 ${selectedReplies.value.length} 個回覆嗎？`,
    okText: '確定',
    cancelText: '取消',
    size: 'small',
  })

  if (!agreed) return

  try {
    const ids = selectedReplies.value.map((r) => r.id)
    await adminApi.batchDeleteReplies(ids)
    notify({
      title: '成功',
      message: `已刪除 ${selectedReplies.value.length} 個回覆`,
      color: 'success',
    })
    selectedReplies.value = []
    loadReplies()
  } catch (error) {
    console.error('批量刪除失敗:', error)
    notify({
      title: '錯誤',
      message: '批量刪除失敗',
      color: 'danger',
    })
  }
}

// 格式化日期
const formatDate = (dateString: string) => {
  const date = new Date(dateString)
  return date.toLocaleString('zh-TW')
}

// 移除 HTML 標籤
const stripHtml = (html: string) => {
  const doc = new DOMParser().parseFromString(html, 'text/html')
  return doc.body.textContent || ''
}

onMounted(() => {
  loadReplies()
})
</script>

<style scoped>
.replies-page {
  padding: 1rem;
}
</style>
