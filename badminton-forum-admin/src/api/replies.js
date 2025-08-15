import api from './index'

export const repliesApi = {
  // 獲取文章回覆
  getReplies(postId, page = 1, pageSize = 50) {
    return api.get(`/posts/${postId}/replies`, {
      params: { page, pageSize }
    })
  },

  // 創建回覆
  createReply(postId, data) {
    return api.post(`/posts/${postId}/replies`, data)
  },

  // 更新回覆
  updateReply(postId, replyId, content) {
    return api.put(`/posts/${postId}/replies/${replyId}`, { content })
  },

  // 刪除回覆
  deleteReply(postId, replyId) {
    return api.delete(`/posts/${postId}/replies/${replyId}`)
  },

  // 點讚回覆
  likeReply(postId, replyId) {
    return api.post(`/posts/${postId}/replies/${replyId}/like`)
  }
}