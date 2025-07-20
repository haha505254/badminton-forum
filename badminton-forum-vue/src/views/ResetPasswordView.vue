<template>
  <div class="reset-password-container">
    <form @submit.prevent="handleSubmit" class="reset-password-form">
      <h2>重置密碼</h2>
      
      <div v-if="successMessage" class="success-message">
        {{ successMessage }}
        <p class="redirect-message">
          即將跳轉到登入頁面...
        </p>
      </div>
      
      <div v-if="error" class="error-message">
        {{ error }}
      </div>
      
      <div v-if="!successMessage">
        <div class="form-group">
          <label for="password">新密碼</label>
          <input
            id="password"
            v-model="formData.newPassword"
            type="password"
            required
            placeholder="請輸入新密碼（至少6個字元）"
            minlength="6"
          />
        </div>
        
        <div class="form-group">
          <label for="confirmPassword">確認新密碼</label>
          <input
            id="confirmPassword"
            v-model="formData.confirmPassword"
            type="password"
            required
            placeholder="請再次輸入新密碼"
          />
        </div>
        
        <button type="submit" class="submit-btn" :disabled="loading">
          {{ loading ? '重置中...' : '重置密碼' }}
        </button>
      </div>
      
      <p class="back-link">
        <RouterLink to="/login">返回登入</RouterLink>
      </p>
    </form>
  </div>
</template>

<script setup>
import { ref, reactive, onMounted } from 'vue'
import { useRouter, useRoute, RouterLink } from 'vue-router'
import axios from 'axios'

const router = useRouter()
const route = useRoute()

const formData = reactive({
  newPassword: '',
  confirmPassword: ''
})

const loading = ref(false)
const error = ref('')
const successMessage = ref('')
const token = ref('')

const API_URL = import.meta.env.VITE_API_URL || 'http://localhost:5246/api'

onMounted(() => {
  token.value = route.query.token || ''
  if (!token.value) {
    error.value = '無效的重置連結'
  }
})

const handleSubmit = async () => {
  if (formData.newPassword !== formData.confirmPassword) {
    error.value = '兩次輸入的密碼不一致'
    return
  }
  
  loading.value = true
  error.value = ''
  
  try {
    const response = await axios.post(`${API_URL}/auth/reset-password`, {
      token: token.value,
      newPassword: formData.newPassword,
      confirmPassword: formData.confirmPassword
    })
    
    successMessage.value = response.data.message
    
    // 3秒後跳轉到登入頁面
    setTimeout(() => {
      router.push('/login')
    }, 3000)
  } catch (err) {
    error.value = err.response?.data?.message || '重置失敗，請稍後再試'
  } finally {
    loading.value = false
  }
}
</script>

<style scoped>
.reset-password-container {
  display: flex;
  justify-content: center;
  align-items: center;
  min-height: calc(100vh - 200px);
}

.reset-password-form {
  background: white;
  padding: 2rem;
  border-radius: 8px;
  box-shadow: 0 2px 8px rgba(0,0,0,0.1);
  width: 100%;
  max-width: 400px;
}

h2 {
  text-align: center;
  color: #2c3e50;
  margin-bottom: 1.5rem;
}

.success-message {
  background-color: #dff0d8;
  color: #3c763d;
  padding: 1rem;
  border-radius: 4px;
  margin-bottom: 1rem;
  text-align: center;
}

.redirect-message {
  margin-top: 0.5rem;
  font-size: 0.875rem;
  color: #666;
}

.error-message {
  background-color: #fee;
  color: #c33;
  padding: 0.75rem;
  border-radius: 4px;
  margin-bottom: 1rem;
  text-align: center;
}

.form-group {
  margin-bottom: 1.5rem;
}

label {
  display: block;
  margin-bottom: 0.5rem;
  color: #555;
  font-weight: 500;
}

input {
  width: 100%;
  padding: 0.75rem;
  border: 1px solid #ddd;
  border-radius: 4px;
  font-size: 1rem;
  transition: border-color 0.3s;
}

input:focus {
  outline: none;
  border-color: #3498db;
}

.submit-btn {
  width: 100%;
  padding: 0.75rem;
  background-color: #3498db;
  color: white;
  border: none;
  border-radius: 4px;
  font-size: 1rem;
  cursor: pointer;
  transition: background-color 0.3s;
}

.submit-btn:hover:not(:disabled) {
  background-color: #2980b9;
}

.submit-btn:disabled {
  background-color: #95a5a6;
  cursor: not-allowed;
}

.back-link {
  text-align: center;
  margin-top: 1rem;
  color: #666;
}

.back-link a {
  color: #3498db;
  text-decoration: none;
}

.back-link a:hover {
  text-decoration: underline;
}
</style>