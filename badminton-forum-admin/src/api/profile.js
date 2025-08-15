import api from './index'

export const profileApi = {
  // 獲取用戶資料 (透過 ID)
  getProfileById(userId) {
    return api.get(`/profile/by-id/${userId}`)
  },

  // 獲取用戶資料 (透過 username - 保留以相容舊版)
  getProfile(username) {
    return api.get(`/profile/${username}`)
  },

  // 獲取用戶文章 (透過 ID)
  getUserPostsById(userId, page = 1, pageSize = 20) {
    return api.get(`/profile/by-id/${userId}/posts`, {
      params: { page, pageSize }
    })
  },

  // 獲取用戶文章 (透過 username - 保留以相容舊版)
  getUserPosts(username, page = 1, pageSize = 20) {
    return api.get(`/profile/${username}/posts`, {
      params: { page, pageSize }
    })
  },

  // 更新個人資料
  updateProfile(data) {
    return api.put('/profile', data)
  },

  // 修改密碼
  changePassword(data) {
    return api.post('/profile/change-password', data)
  },

  // 上傳頭像
  uploadAvatar(file) {
    const formData = new FormData()
    formData.append('file', file)
    return api.post('/profile/upload-avatar', formData, {
      headers: {
        'Content-Type': 'multipart/form-data'
      }
    })
  }
}