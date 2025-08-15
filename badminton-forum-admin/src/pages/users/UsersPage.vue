<template>
  <div class="users-page">
    <div class="row align-center mb-4">
      <h1 class="flex">用戶管理</h1>
      <VaInput
        v-model="filter"
        placeholder="搜尋用戶..."
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
            :items="filteredUsers"
            :columns="columns"
            :loading="loading"
            :pagination="pagination"
            @update:pagination="updatePagination"
          >
            <template #cell(user)="{ rowData }">
              <div class="flex items-center">
                <VaAvatar
                  :src="rowData.avatar"
                  :fallback-text="rowData.username?.[0]"
                  size="small"
                  class="mr-2"
                />
                <div>
                  <div class="font-semibold">{{ rowData.username }}</div>
                  <div class="text-xs text-gray-500">{{ rowData.email }}</div>
                </div>
              </div>
            </template>

            <template #cell(role)="{ rowData }">
              <VaBadge 
                :text="rowData.isAdmin ? '管理員' : '一般用戶'"
                :color="rowData.isAdmin ? 'danger' : 'info'"
              />
            </template>

            <template #cell(status)="{ rowData }">
              <VaBadge 
                :text="rowData.isActive ? '啟用' : '停用'"
                :color="rowData.isActive ? 'success' : 'secondary'"
              />
            </template>

            <template #cell(provider)="{ rowData }">
              <div class="flex items-center">
                <VaIcon 
                  v-if="rowData.provider === 'Google'"
                  name="mdi-google"
                  class="mr-1"
                  color="primary"
                />
                {{ rowData.provider || 'Local' }}
              </div>
            </template>

            <template #cell(createdAt)="{ rowData }">
              {{ formatDate(rowData.createdAt) }}
            </template>

            <template #cell(lastLoginAt)="{ rowData }">
              {{ rowData.lastLoginAt ? formatDate(rowData.lastLoginAt) : '從未登入' }}
            </template>

            <template #cell(actions)="{ rowData }">
              <div class="flex gap-2">
                <VaButton
                  v-if="rowData.isActive"
                  preset="plain"
                  icon="block"
                  color="warning"
                  @click="toggleUserActive(rowData)"
                  title="停用用戶"
                />
                <VaButton
                  v-else
                  preset="plain"
                  icon="check_circle"
                  color="success"
                  @click="toggleUserActive(rowData)"
                  title="啟用用戶"
                />
                
                <VaButton
                  v-if="!rowData.isAdmin"
                  preset="plain"
                  icon="admin_panel_settings"
                  color="primary"
                  @click="toggleUserAdmin(rowData)"
                  title="設為管理員"
                />
                <VaButton
                  v-else
                  preset="plain"
                  icon="remove_moderator"
                  color="secondary"
                  @click="toggleUserAdmin(rowData)"
                  title="移除管理員"
                />
                
                <VaButton
                  preset="plain"
                  icon="visibility"
                  @click="viewUserProfile(rowData)"
                  title="查看個人資料"
                />
              </div>
            </template>
          </VaDataTable>
        </div>
      </VaCardContent>
    </VaCard>

    <!-- 用戶統計卡片 -->
    <div class="row mt-4">
      <div class="flex xs12 md3">
        <VaCard>
          <VaCardContent>
            <div class="text-center">
              <div class="text-3xl font-bold text-primary">{{ stats.totalUsers }}</div>
              <div class="text-sm text-secondary mt-2">總用戶數</div>
            </div>
          </VaCardContent>
        </VaCard>
      </div>
      
      <div class="flex xs12 md3">
        <VaCard>
          <VaCardContent>
            <div class="text-center">
              <div class="text-3xl font-bold text-success">{{ stats.activeUsers }}</div>
              <div class="text-sm text-secondary mt-2">活躍用戶</div>
            </div>
          </VaCardContent>
        </VaCard>
      </div>
      
      <div class="flex xs12 md3">
        <VaCard>
          <VaCardContent>
            <div class="text-center">
              <div class="text-3xl font-bold text-danger">{{ stats.adminUsers }}</div>
              <div class="text-sm text-secondary mt-2">管理員</div>
            </div>
          </VaCardContent>
        </VaCard>
      </div>
      
      <div class="flex xs12 md3">
        <VaCard>
          <VaCardContent>
            <div class="text-center">
              <div class="text-3xl font-bold text-info">{{ stats.googleUsers }}</div>
              <div class="text-sm text-secondary mt-2">Google 用戶</div>
            </div>
          </VaCardContent>
        </VaCard>
      </div>
    </div>
  </div>
</template>

<script setup lang="ts">
import { ref, onMounted, computed } from 'vue'
import { useToast } from 'vuestic-ui'
import adminApi from '../../api/admin'

const { init: notify } = useToast()

const users = ref([])
const loading = ref(false)
const filter = ref('')
const currentPage = ref(1)
const perPage = ref(20)
const total = ref(0)

const columns = [
  { key: 'user', label: '用戶', sortable: true },
  { key: 'role', label: '角色', width: '100px' },
  { key: 'status', label: '狀態', width: '100px' },
  { key: 'provider', label: '登入方式', width: '120px' },
  { key: 'createdAt', label: '註冊時間', sortable: true },
  { key: 'lastLoginAt', label: '最後登入', sortable: true },
  { key: 'actions', label: '操作', width: '150px' }
]

const pagination = computed(() => ({
  page: currentPage.value,
  perPage: perPage.value,
  total: total.value
}))

const filteredUsers = computed(() => {
  if (!filter.value) return users.value
  const searchTerm = filter.value.toLowerCase()
  return users.value.filter(user => 
    user.username?.toLowerCase().includes(searchTerm) ||
    user.email?.toLowerCase().includes(searchTerm)
  )
})

const stats = computed(() => ({
  totalUsers: users.value.length,
  activeUsers: users.value.filter(u => u.isActive).length,
  adminUsers: users.value.filter(u => u.isAdmin).length,
  googleUsers: users.value.filter(u => u.provider === 'Google').length
}))

async function fetchUsers() {
  loading.value = true
  try {
    const response = await adminApi.getUsers(currentPage.value, perPage.value)
    users.value = response.data
    total.value = parseInt(response.headers['x-total-count'] || '0')
  } catch (error) {
    notify({
      message: '載入用戶失敗',
      color: 'danger'
    })
  } finally {
    loading.value = false
  }
}

async function toggleUserActive(user) {
  try {
    await adminApi.toggleUserActive(user.id)
    user.isActive = !user.isActive
    notify({
      message: user.isActive ? '用戶已啟用' : '用戶已停用',
      color: 'success'
    })
  } catch (error) {
    notify({
      message: '操作失敗',
      color: 'danger'
    })
  }
}

async function toggleUserAdmin(user) {
  const action = user.isAdmin ? '移除管理員權限' : '授予管理員權限'
  if (!confirm(`確定要${action}嗎？`)) return
  
  try {
    await adminApi.toggleUserAdmin(user.id)
    user.isAdmin = !user.isAdmin
    notify({
      message: `已${action}`,
      color: 'success'
    })
  } catch (error) {
    notify({
      message: '操作失敗',
      color: 'danger'
    })
  }
}

function viewUserProfile(user) {
  window.open(`http://localhost:5173/profile/${user.id}`, '_blank')
}

function updatePagination(newPagination) {
  currentPage.value = newPagination.page
  perPage.value = newPagination.perPage
  fetchUsers()
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
  fetchUsers()
})
</script>

<style scoped>
.users-page {
  padding: 1.5rem;
}

.table-wrapper {
  overflow-x: auto;
}
</style>