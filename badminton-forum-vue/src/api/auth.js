import axios from 'axios'

const API_URL = import.meta.env.VITE_API_URL || 'http://localhost:5246/api'

export const authApi = {
  // 登入
  login: (data) => {
    return axios.post(`${API_URL}/auth/login`, data)
  },

  // 註冊
  register: (data) => {
    return axios.post(`${API_URL}/auth/register`, data)
  },

  // 忘記密碼
  forgotPassword: (email) => {
    return axios.post(`${API_URL}/auth/forgot-REMOVED`, { email })
  },

  // 重置密碼
  resetPassword: (data) => {
    return axios.post(`${API_URL}/auth/reset-REMOVED`, data)
  }
}

export default authApi