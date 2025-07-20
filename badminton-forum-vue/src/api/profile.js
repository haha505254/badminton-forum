import api from './index'

export const profileApi = {
  // 獲取用戶資料
  getProfile(username) {
    return api.get(`/profile/${username}`)
  },

  // 獲取用戶文章
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
    return api.post('/profile/change-REMOVED', data)
  }
}