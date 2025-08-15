import api from './index'

export const authApi = {
  // 登入
  login: (data) => {
    return api.post('/auth/login', data)
  },

  // 註冊
  register: (data) => {
    return api.post('/auth/register', data)
  },

  // 忘記密碼
  forgotPassword: (email) => {
    return api.post('/auth/forgot-password', { email })
  },

  // 重置密碼
  resetPassword: (data) => {
    return api.post('/auth/reset-password', data)
  }
}

export default authApi