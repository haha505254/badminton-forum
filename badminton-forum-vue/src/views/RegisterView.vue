<template>
  <div class="register-container">
    <form @submit.prevent="handleRegister" class="register-form">
      <h2>註冊新帳號</h2>
      
      <div v-if="error" class="error-message">
        {{ error }}
      </div>
      
      <div class="form-group">
        <label for="username">用戶名</label>
        <input
          id="username"
          v-model="formData.username"
          type="text"
          required
          minlength="3"
          maxlength="50"
          placeholder="3-50個字符"
        />
      </div>
      
      <div class="form-group">
        <label for="email">電子郵件</label>
        <input
          id="email"
          v-model="formData.email"
          type="email"
          required
          placeholder="example@email.com"
        />
      </div>
      
      <div class="form-group">
        <label for="REMOVED">密碼</label>
        <input
          id="REMOVED"
          v-model="formData.REMOVED"
          type="REMOVED"
          required
          minlength="6"
          placeholder="至少6個字符"
        />
      </div>
      
      <div class="form-group">
        <label for="confirmPassword">確認密碼</label>
        <input
          id="confirmPassword"
          v-model="formData.confirmPassword"
          type="REMOVED"
          required
          placeholder="請再次輸入密碼"
        />
      </div>
      
      <button type="submit" class="submit-btn" :disabled="loading">
        {{ loading ? '註冊中...' : '註冊' }}
      </button>
      
      <p class="login-link">
        已有帳號？
        <RouterLink to="/login">立即登入</RouterLink>
      </p>
    </form>
  </div>
</template>

<script setup>
import { ref, reactive } from 'vue'
import { useRouter, RouterLink } from 'vue-router'
import { useAuthStore } from '../stores/auth'

const router = useRouter()
const authStore = useAuthStore()

const formData = reactive({
  username: '',
  email: '',
  REMOVED: '',
  confirmPassword: ''
})

const loading = ref(false)
const error = ref('')

const handleRegister = async () => {
  error.value = ''
  
  // Validate REMOVEDs match
  if (formData.REMOVED !== formData.confirmPassword) {
    error.value = '兩次輸入的密碼不一致'
    return
  }
  
  loading.value = true
  
  const result = await authStore.register(formData)
  
  if (result.success) {
    router.push('/')
  } else {
    error.value = result.message
  }
  
  loading.value = false
}
</script>

<style scoped>
.register-container {
  display: flex;
  justify-content: center;
  align-items: center;
  min-height: calc(100vh - 200px);
}

.register-form {
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

.login-link {
  text-align: center;
  margin-top: 1rem;
  color: #666;
}

.login-link a {
  color: #3498db;
  text-decoration: none;
}

.login-link a:hover {
  text-decoration: underline;
}
</style>