<template>
  <div class="forgot-password-container">
    <form @submit.prevent="handleSubmit" class="forgot-password-form">
      <h2>忘記密碼</h2>
      
      <div v-if="successMessage" class="success-message">
        {{ successMessage }}
        <p v-if="resetUrl" class="dev-only">
          開發測試用：<a :href="resetUrl" target="_blank">{{ resetUrl }}</a>
        </p>
      </div>
      
      <div v-if="error" class="error-message">
        {{ error }}
      </div>
      
      <div v-if="!successMessage">
        <p class="instructions">
          請輸入您註冊時使用的電子郵件地址，我們將發送密碼重置說明給您。
        </p>
        
        <div class="form-group">
          <label for="email">電子郵件</label>
          <input
            id="email"
            v-model="email"
            type="email"
            required
            placeholder="請輸入電子郵件"
          />
        </div>
        
        <button type="submit" class="submit-btn" :disabled="loading">
          {{ loading ? '發送中...' : '發送重置郵件' }}
        </button>
      </div>
      
      <p class="back-link">
        <RouterLink to="/login">返回登入</RouterLink>
      </p>
    </form>
  </div>
</template>

<script setup>
import { ref } from 'vue'
import { RouterLink } from 'vue-router'
import axios from 'axios'

const email = ref('')
const loading = ref(false)
const error = ref('')
const successMessage = ref('')
const resetUrl = ref('')

const API_URL = import.meta.env.VITE_API_URL || 'http://localhost:5246/api'

const handleSubmit = async () => {
  loading.value = true
  error.value = ''
  successMessage.value = ''
  
  try {
    const response = await axios.post(`${API_URL}/auth/forgot-password`, {
      email: email.value
    })
    
    successMessage.value = response.data.message
    
    // 開發環境下顯示重置連結
    if (response.data.resetUrl) {
      resetUrl.value = response.data.resetUrl
    }
  } catch (err) {
    error.value = err.response?.data?.message || '發送失敗，請稍後再試'
  } finally {
    loading.value = false
  }
}
</script>

<style scoped>
.forgot-password-container {
  display: flex;
  justify-content: center;
  align-items: center;
  min-height: calc(100vh - 200px);
}

.forgot-password-form {
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

.instructions {
  color: #666;
  margin-bottom: 1.5rem;
  text-align: center;
  line-height: 1.5;
}

.success-message {
  background-color: #dff0d8;
  color: #3c763d;
  padding: 1rem;
  border-radius: 4px;
  margin-bottom: 1rem;
  text-align: center;
}

.dev-only {
  margin-top: 1rem;
  font-size: 0.875rem;
  color: #666;
  word-break: break-all;
}

.dev-only a {
  color: #3498db;
  text-decoration: none;
}

.dev-only a:hover {
  text-decoration: underline;
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