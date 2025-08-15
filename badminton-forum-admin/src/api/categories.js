import api from './index'

export const categoriesApi = {
  // 獲取所有分類
  getCategories() {
    return api.get('/categories')
  },

  // 獲取單個分類
  getCategory(id) {
    return api.get(`/categories/${id}`)
  },

  // 獲取分類下的文章
  getCategoryPosts(id, page = 1, pageSize = 20) {
    return api.get(`/categories/${id}/posts`, {
      params: { page, pageSize }
    })
  }
}