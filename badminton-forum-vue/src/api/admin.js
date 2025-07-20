import api from './index'

export const adminApi = {
  // 使用者管理
  getUsers(page = 1, pageSize = 20) {
    return api.get('/admin/users', {
      params: { page, pageSize }
    })
  },
  
  toggleUserActive(userId) {
    return api.put(`/admin/users/${userId}/toggle-active`)
  },
  
  toggleUserAdmin(userId) {
    return api.put(`/admin/users/${userId}/toggle-admin`)
  },
  
  // 版塊管理
  getCategories() {
    return api.get('/admin/categories')
  },
  
  createCategory(data) {
    return api.post('/admin/categories', data)
  },
  
  updateCategory(id, data) {
    return api.put(`/admin/categories/${id}`, data)
  },
  
  deleteCategory(id) {
    return api.delete(`/admin/categories/${id}`)
  },
  
  // 文章管理
  getPosts(page = 1, pageSize = 20) {
    return api.get('/admin/posts', {
      params: { page, pageSize }
    })
  },
  
  deletePost(id) {
    return api.delete(`/admin/posts/${id}`)
  },
  
  movePost(id, targetCategoryId) {
    return api.put(`/admin/posts/${id}/move`, { targetCategoryId })
  },
  
  togglePostPin(id) {
    return api.put(`/admin/posts/${id}/toggle-pin`)
  },
  
  togglePostLock(id) {
    return api.put(`/admin/posts/${id}/toggle-lock`)
  }
}