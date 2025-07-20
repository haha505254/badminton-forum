<template>
  <div class="home">
    <h1>歡迎來到羽毛球論壇</h1>
    <p>分享您的羽毛球經驗，與球友交流技術！</p>
    
    <div class="features">
      <div class="feature-card">
        <h3>🏸 技術討論</h3>
        <p>分享和學習羽毛球技術，提升您的球技</p>
      </div>
      
      <div class="feature-card">
        <h3>🎾 裝備推薦</h3>
        <p>了解最新的球拍、球鞋等裝備資訊</p>
      </div>
      
      <div class="feature-card">
        <h3>📅 活動公告</h3>
        <p>參加比賽和活動，認識更多球友</p>
      </div>
      
      <div class="feature-card">
        <h3>👥 球友交流</h3>
        <p>尋找志同道合的球友，一起打球</p>
      </div>
    </div>
    
    <div class="cta">
      <RouterLink to="/categories" class="btn-primary">
        瀏覽論壇版塊
      </RouterLink>
    </div>
  </div>
</template>

<script setup>
import { RouterLink } from 'vue-router'
</script>

<style scoped>
.home {
  text-align: center;
  padding: 2rem;
}

h1 {
  font-size: 2.5rem;
  margin-bottom: 1rem;
  color: #2c3e50;
}

.features {
  display: grid;
  grid-template-columns: repeat(auto-fit, minmax(250px, 1fr));
  gap: 2rem;
  margin: 3rem 0;
}

.feature-card {
  background: white;
  padding: 2rem;
  border-radius: 8px;
  box-shadow: 0 2px 8px rgba(0,0,0,0.1);
  transition: transform 0.3s;
}

.feature-card:hover {
  transform: translateY(-5px);
}

.feature-card h3 {
  color: #2c3e50;
  margin-bottom: 1rem;
}

.cta {
  margin-top: 3rem;
}

.btn-primary {
  display: inline-block;
  background-color: #3498db;
  color: white;
  padding: 1rem 2rem;
  border-radius: 4px;
  text-decoration: none;
  font-size: 1.1rem;
  transition: background-color 0.3s;
}

.btn-primary:hover {
  background-color: #2980b9;
}
</style>