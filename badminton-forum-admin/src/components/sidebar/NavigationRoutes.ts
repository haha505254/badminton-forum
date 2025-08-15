export interface INavigationRoute {
  name: string
  displayName: string
  meta: { icon: string }
  children?: INavigationRoute[]
}

export default {
  root: {
    name: '/',
    displayName: 'navigationRoutes.home',
  },
  routes: [
    {
      name: 'dashboard',
      displayName: '儀表板',
      meta: {
        icon: 'vuestic-iconset-dashboard',
      },
    },
    {
      name: 'users',
      displayName: '用戶管理',
      meta: {
        icon: 'group',
      },
    },
    {
      name: 'posts',
      displayName: '貼文管理',
      meta: {
        icon: 'article',
      },
    },
    {
      name: 'categories',
      displayName: '分類管理',
      meta: {
        icon: 'folder',
      },
    },
    {
      name: 'replies',
      displayName: '回覆管理',
      meta: {
        icon: 'comment',
      },
    },
    {
      name: 'preferences',
      displayName: '個人設定',
      meta: {
        icon: 'manage_accounts',
      },
    },
    {
      name: 'settings',
      displayName: '系統設定',
      meta: {
        icon: 'settings',
      },
    },
  ] as INavigationRoute[],
}
