// 板塊數據配置
export const categoryData = [
  {
    id: 1,
    name: '綜合討論區',
    icon: '💬',
    description: '羽毛球相關的一般討論',
    slug: 'general'
  },
  {
    id: 2,
    name: '技術交流區',
    icon: '🏸',
    description: '技術分享與教學討論',
    slug: 'technique'
  },
  {
    id: 3,
    name: '裝備討論區',
    icon: '🎾',
    description: '球拍、球鞋、裝備評測與推薦',
    slug: 'equipment'
  },
  {
    id: 4,
    name: '賽事專區',
    icon: '🏆',
    description: '國內外賽事討論與轉播',
    slug: 'tournament'
  },
  {
    id: 5,
    name: '地區球友會',
    icon: '📍',
    description: '各地區球友交流與約球',
    slug: 'local'
  },
  {
    id: 6,
    name: '羽球新聞',
    icon: '📰',
    description: '國內外羽球新聞資訊',
    slug: 'news'
  }
]

// 根據 ID 獲取板塊
export const getCategoryById = (id) => {
  return categoryData.find(cat => cat.id === parseInt(id))
}

// 根據 slug 獲取板塊
export const getCategoryBySlug = (slug) => {
  return categoryData.find(cat => cat.slug === slug)
}