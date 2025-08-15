<template>
  <div class="categories-page">
    <div class="row align-center mb-4">
      <h1 class="flex">分類管理</h1>
      <VaButton @click="showCreateModal = true">
        <VaIcon name="add" class="mr-2" />
        新增分類
      </VaButton>
    </div>

    <VaCard>
      <VaCardContent>
        <VaDataTable
          :items="categories"
          :columns="columns"
          :loading="loading"
        >
          <template #cell(icon)="{ rowData }">
            <VaIcon :name="rowData.icon || 'folder'" size="large" />
          </template>

          <template #cell(postCount)="{ rowData }">
            <VaBadge :text="String(rowData.postCount || 0)" color="info" />
          </template>

          <template #cell(createdAt)="{ rowData }">
            {{ formatDate(rowData.createdAt) }}
          </template>

          <template #cell(actions)="{ rowData }">
            <div class="flex gap-2">
              <VaButton
                preset="plain"
                icon="edit"
                @click="editCategory(rowData)"
              />
              <VaButton
                preset="plain"
                icon="delete"
                color="danger"
                @click="deleteCategory(rowData)"
              />
            </div>
          </template>
        </VaDataTable>
      </VaCardContent>
    </VaCard>

    <!-- 創建/編輯分類 Modal -->
    <VaModal
      v-model="showModal"
      :title="editingCategory ? '編輯分類' : '新增分類'"
      size="medium"
    >
      <VaForm ref="formRef" tag="div">
        <VaInput
          v-model="categoryForm.name"
          label="分類名稱"
          placeholder="輸入分類名稱"
          :rules="[(v) => !!v || '請輸入分類名稱']"
          class="mb-4"
        />
        
        <VaTextarea
          v-model="categoryForm.description"
          label="分類描述"
          placeholder="輸入分類描述"
          :rules="[(v) => !!v || '請輸入分類描述']"
          class="mb-4"
        />
        
        <VaInput
          v-model="categoryForm.icon"
          label="圖標"
          placeholder="輸入圖標名稱 (Material Icons)"
          class="mb-4"
        >
          <template #prepend>
            <VaIcon :name="categoryForm.icon || 'folder'" />
          </template>
        </VaInput>
        
        <VaInput
          v-model.number="categoryForm.displayOrder"
          label="顯示順序"
          type="number"
          placeholder="輸入顯示順序"
          class="mb-4"
        />
      </VaForm>

      <template #footer>
        <VaButton preset="secondary" @click="cancelEdit">
          取消
        </VaButton>
        <VaButton @click="saveCategory">
          {{ editingCategory ? '更新' : '創建' }}
        </VaButton>
      </template>
    </VaModal>
  </div>
</template>

<script setup lang="ts">
import { ref, onMounted, computed } from 'vue'
import { useToast } from 'vuestic-ui'
import adminApi from '../../api/admin'

const { init: notify } = useToast()

const categories = ref([])
const loading = ref(false)
const showModal = ref(false)
const showCreateModal = computed({
  get: () => showModal.value,
  set: (val) => {
    if (val) {
      editingCategory.value = null
      resetForm()
    }
    showModal.value = val
  }
})

const editingCategory = ref(null)
const formRef = ref()

const categoryForm = ref({
  name: '',
  description: '',
  icon: 'folder',
  displayOrder: 0
})

const columns = [
  { key: 'displayOrder', label: '順序', sortable: true, width: '80px' },
  { key: 'icon', label: '圖標', width: '80px' },
  { key: 'name', label: '名稱', sortable: true },
  { key: 'description', label: '描述' },
  { key: 'postCount', label: '文章數', sortable: true, width: '100px' },
  { key: 'createdAt', label: '創建時間', sortable: true },
  { key: 'actions', label: '操作', width: '120px' }
]

async function fetchCategories() {
  loading.value = true
  try {
    const response = await adminApi.getCategories()
    categories.value = response.data
  } catch (error) {
    notify({
      message: '載入分類失敗',
      color: 'danger'
    })
  } finally {
    loading.value = false
  }
}

function editCategory(category) {
  editingCategory.value = category
  categoryForm.value = {
    name: category.name,
    description: category.description,
    icon: category.icon || 'folder',
    displayOrder: category.displayOrder || 0
  }
  showModal.value = true
}

async function saveCategory() {
  const isValid = formRef.value?.validate()
  if (!isValid) return

  try {
    if (editingCategory.value) {
      await adminApi.updateCategory(editingCategory.value.id, categoryForm.value)
      notify({
        message: '分類已更新',
        color: 'success'
      })
    } else {
      await adminApi.createCategory(categoryForm.value)
      notify({
        message: '分類已創建',
        color: 'success'
      })
    }
    
    showModal.value = false
    fetchCategories()
  } catch (error) {
    notify({
      message: '操作失敗',
      color: 'danger'
    })
  }
}

async function deleteCategory(category) {
  if (!confirm(`確定要刪除分類「${category.name}」嗎？`)) return
  
  try {
    await adminApi.deleteCategory(category.id)
    notify({
      message: '分類已刪除',
      color: 'success'
    })
    fetchCategories()
  } catch (error) {
    notify({
      message: '刪除失敗，可能還有文章在此分類下',
      color: 'danger'
    })
  }
}

function cancelEdit() {
  showModal.value = false
  resetForm()
}

function resetForm() {
  categoryForm.value = {
    name: '',
    description: '',
    icon: 'folder',
    displayOrder: 0
  }
  editingCategory.value = null
}

function formatDate(dateString) {
  return new Date(dateString).toLocaleDateString('zh-TW', {
    year: 'numeric',
    month: '2-digit',
    day: '2-digit'
  })
}

onMounted(() => {
  fetchCategories()
})
</script>

<style scoped>
.categories-page {
  padding: 1.5rem;
}
</style>