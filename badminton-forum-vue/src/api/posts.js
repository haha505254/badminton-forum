import api from './index'

export const postsApi = {
  // 獲取文章列表
  getPosts(page = 1, pageSize = 20) {
    return api.get('/posts', {
      params: { page, pageSize }
    })
  },

  // 獲取單篇文章
  getPost(id) {
    return api.get(`/posts/${id}`)
  },

  // 創建文章
  createPost(data) {
    return api.post('/posts', data)
  },

  // 更新文章
  updatePost(id, data) {
    return api.put(`/posts/${id}`, data)
  },

  // 刪除文章
  deletePost(id) {
    return api.delete(`/posts/${id}`)
  },

  // 點讚文章
  likePost(id) {
    return api.post(`/posts/${id}/like`)
  }
}