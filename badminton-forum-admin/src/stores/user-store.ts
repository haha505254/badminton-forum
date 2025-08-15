import { defineStore } from 'pinia'
import { useAuthStore } from './auth'

export const useUserStore = defineStore('user', {
  state: () => {
    return {
      is2FAEnabled: false,
    }
  },

  getters: {
    userName(): string {
      const authStore = useAuthStore()
      return authStore.user?.username || authStore.user?.email?.split('@')[0] || '用戶'
    },
    
    email(): string {
      const authStore = useAuthStore()
      return authStore.user?.email || ''
    },
    
    memberSince(): string {
      const authStore = useAuthStore()
      if (authStore.user?.createdAt) {
        const date = new Date(authStore.user.createdAt)
        return date.toLocaleDateString('en-US', {
          month: 'numeric',
          day: 'numeric',
          year: 'numeric'
        })
      }
      return new Date().toLocaleDateString('en-US')
    },
    
    pfp(): string {
      const authStore = useAuthStore()
      return authStore.user?.avatar || `https://ui-avatars.com/api/?name=${this.userName}&background=random`
    }
  },

  actions: {
    toggle2FA() {
      this.is2FAEnabled = !this.is2FAEnabled
    },

    async changeUserName(userName: string) {
      const authStore = useAuthStore()
      // Update local user data
      if (authStore.user) {
        authStore.user.username = userName
        localStorage.setItem('user', JSON.stringify(authStore.user))
      }
      // TODO: Call API to update username on server
    },
  },
})
