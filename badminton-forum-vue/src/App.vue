<script setup>
import { RouterView, RouterLink, useRouter } from 'vue-router'
import { useAuthStore } from './stores/auth'

const router = useRouter()
const authStore = useAuthStore()

const handleLogout = () => {
  authStore.logout()
  router.push('/')
}
</script>

<template>
  <div id="app">
    <nav class="navbar">
      <div class="nav-container">
        <RouterLink to="/" class="brand">
          🏸 羽毛球論壇
        </RouterLink>
        
        <div class="nav-links">
          <RouterLink to="/categories">版塊</RouterLink>
          <RouterLink to="/search">搜尋</RouterLink>
          
          <template v-if="authStore.isAuthenticated">
            <RouterLink to="/new-post">發表文章</RouterLink>
            <RouterLink :to="`/profile/${authStore.user.username}`">
              {{ authStore.user.username }}
            </RouterLink>
            <RouterLink to="/settings">設置</RouterLink>
            <RouterLink v-if="authStore.user?.isAdmin" to="/admin">管理</RouterLink>
            <button @click="handleLogout" class="logout-btn">
              登出
            </button>
          </template>
          
          <template v-else>
            <RouterLink to="/login">登入</RouterLink>
            <RouterLink to="/register">註冊</RouterLink>
          </template>
        </div>
      </div>
    </nav>
    
    <main class="main-content">
      <RouterView />
    </main>
    
    <footer class="footer">
      <p>&copy; 2024 羽毛球論壇. All rights reserved.</p>
    </footer>
  </div>
</template>

<style>
* {
  margin: 0;
  padding: 0;
  box-sizing: border-box;
}

body {
  font-family: -apple-system, BlinkMacSystemFont, 'Segoe UI', Roboto, Oxygen, Ubuntu, Cantarell, sans-serif;
  background-color: #f5f5f5;
  color: #333;
}

#app {
  min-height: 100vh;
  display: flex;
  flex-direction: column;
}

.navbar {
  background-color: #2c3e50;
  color: white;
  padding: 1rem 0;
  box-shadow: 0 2px 4px rgba(0,0,0,0.1);
  position: fixed;
  top: 0;
  left: 0;
  right: 0;
  z-index: 1000;
}

.nav-container {
  max-width: 1200px;
  margin: 0 auto;
  padding: 0 1rem;
  display: flex;
  justify-content: space-between;
  align-items: center;
}

.brand {
  font-size: 1.5rem;
  font-weight: bold;
  color: white;
  text-decoration: none;
}

.nav-links {
  display: flex;
  gap: 1.5rem;
  align-items: center;
  flex-shrink: 0;
}

.nav-links a {
  color: white;
  text-decoration: none;
  transition: opacity 0.3s;
}

.nav-links a:hover {
  opacity: 0.8;
}

.nav-links a.router-link-active {
  text-decoration: underline;
}

.logout-btn {
  background: none;
  border: 1px solid white;
  color: white;
  padding: 0.5rem 1rem;
  border-radius: 4px;
  cursor: pointer;
  transition: all 0.3s;
}

.logout-btn:hover {
  background-color: white;
  color: #2c3e50;
}

.main-content {
  flex: 1;
  max-width: 1200px;
  margin: 2rem auto;
  padding: 0 1rem;
  width: 100%;
  margin-top: calc(60px + 2rem);
}

.footer {
  background-color: #34495e;
  color: white;
  text-align: center;
  padding: 2rem 0;
  margin-top: auto;
}

/* 響應式設計 */
@media (max-width: 768px) {
  .nav-container {
    flex-wrap: wrap;
    padding: 0.5rem 1rem;
  }
  
  .brand {
    font-size: 1.2rem;
  }
  
  .nav-links {
    gap: 0.8rem;
    font-size: 0.9rem;
    flex-wrap: wrap;
    justify-content: flex-end;
  }
  
  .logout-btn {
    padding: 0.3rem 0.8rem;
    font-size: 0.9rem;
  }
  
  .main-content {
    margin-top: calc(80px + 1rem);
  }
}

@media (max-width: 480px) {
  .nav-container {
    flex-direction: column;
    gap: 0.5rem;
  }
  
  .nav-links {
    width: 100%;
    justify-content: center;
  }
  
  .main-content {
    margin-top: calc(100px + 1rem);
    padding: 0 0.5rem;
  }
}
</style>