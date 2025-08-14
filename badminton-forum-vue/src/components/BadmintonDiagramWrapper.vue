<template>
  <div class="diagram-wrapper" :data-context="context">
    <!-- 標題欄 -->
    <div class="diagram-header" @click="toggleExpanded">
      <div class="diagram-info">
        <span class="diagram-icon">🏸</span>
        <span class="diagram-title">戰術圖{{ title ? `：${title}` : '' }}</span>
        <span class="diagram-hint">{{ isExpanded ? '' : '（點擊展開）' }}</span>
      </div>
      <button 
        class="expand-btn"
        :aria-expanded="isExpanded"
        :title="isExpanded ? '收合戰術圖' : '展開戰術圖'"
      >
        <svg 
          class="expand-icon"
          :class="{ 'rotate-180': isExpanded }"
          width="16" 
          height="16" 
          viewBox="0 0 24 24" 
          fill="none" 
          stroke="currentColor"
        >
          <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M19 9l-7 7-7-7" />
        </svg>
      </button>
    </div>
    
    <!-- 內容區 -->
    <transition name="slide">
      <div v-if="isExpanded" class="diagram-content">
        <!-- 縮圖模式 -->
        <div v-if="!showFull" class="diagram-preview">
          <div class="preview-container" @click="showFull = true">
            <BadmintonCourtViewer 
              :data="data" 
              :scale="0.6"
              class="preview-diagram"
            />
            <div class="preview-overlay">
              <div class="overlay-content">
                <svg width="24" height="24" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                  <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M21 21l-6-6m2-5a7 7 0 11-14 0 7 7 0 0114 0zM10 7v3m0 0v3m0-3h3m-3 0H7" />
                </svg>
                <span>點擊查看完整尺寸</span>
              </div>
            </div>
          </div>
          <div class="preview-caption">
            預覽模式 - 點擊放大
          </div>
        </div>
        
        <!-- 完整模式 -->
        <div v-else class="diagram-full">
          <div class="full-controls">
            <button @click="showFull = false" class="minimize-btn">
              <svg width="16" height="16" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M20 14H4m6-6l-6 6 6 6" />
              </svg>
              返回縮圖
            </button>
          </div>
          <div class="full-container">
            <BadmintonCourtViewer 
              :data="data"
              :scale="1"
              class="full-diagram"
            />
          </div>
        </div>
      </div>
    </transition>
  </div>
</template>

<script setup>
import { ref, computed } from 'vue'
import BadmintonCourtViewer from './BadmintonCourtViewer.vue'

const props = defineProps({
  data: {
    type: Object,
    required: true
  },
  context: {
    type: String,
    default: 'reply' // 'post' | 'reply'
  },
  defaultExpanded: {
    type: Boolean,
    default: true  // 預設展開戰術圖
  }
})

// 狀態
const isExpanded = ref(props.defaultExpanded)
const showFull = ref(false)  // 預設顯示縮圖，不顯示完整尺寸

// 計算屬性
const title = computed(() => {
  return props.data?.description || '戰術示意圖'
})

// 方法
const toggleExpanded = () => {
  isExpanded.value = !isExpanded.value
  if (!isExpanded.value) {
    showFull.value = false // 收合時重置為縮圖模式
  }
}
</script>

<style scoped>
.diagram-wrapper {
  margin: 1rem 0;
  border: 1px solid #e5e7eb;
  border-radius: 0.5rem;
  overflow: hidden;
  background: white;
}

:root.dark .diagram-wrapper {
  border-color: #374151;
  background: #1f2937;
}

/* 標題欄 */
.diagram-header {
  display: flex;
  align-items: center;
  justify-content: space-between;
  padding: 0.5rem 0.875rem;
  background: #f9fafb;
  cursor: pointer;
  user-select: none;
  transition: background 0.2s;
}

:root.dark .diagram-header {
  background: #111827;
}

.diagram-header:hover {
  background: #f3f4f6;
}

:root.dark .diagram-header:hover {
  background: #1f2937;
}

.diagram-info {
  display: flex;
  align-items: center;
  gap: 0.375rem;
  flex: 1;
}

.diagram-icon {
  font-size: 1rem;
}

.diagram-title {
  font-weight: 500;
  font-size: 0.875rem;
  color: #111827;
}

:root.dark .diagram-title {
  color: #f3f4f6;
}

.diagram-hint {
  font-size: 0.875rem;
  color: #6b7280;
}

:root.dark .diagram-hint {
  color: #9ca3af;
}

.expand-btn {
  padding: 0.25rem;
  background: transparent;
  border: none;
  cursor: pointer;
  color: #6b7280;
  transition: all 0.2s;
}

:root.dark .expand-btn {
  color: #9ca3af;
}

.expand-btn:hover {
  color: #111827;
}

:root.dark .expand-btn:hover {
  color: #f3f4f6;
}

.expand-icon {
  transition: transform 0.3s;
}

.rotate-180 {
  transform: rotate(180deg);
}

/* 內容區 */
.diagram-content {
  border-top: 1px solid #e5e7eb;
}

:root.dark .diagram-content {
  border-color: #374151;
}

/* 縮圖模式 */
.diagram-preview {
  padding: 1rem;
}

.preview-container {
  position: relative;
  display: inline-block;
  cursor: pointer;
  border-radius: 0.375rem;
  overflow: hidden;
  box-shadow: 0 1px 3px rgba(0, 0, 0, 0.1);
}

.preview-diagram {
  display: block;
  transition: transform 0.2s;
}

.preview-container:hover .preview-diagram {
  transform: scale(1.02);
}

.preview-overlay {
  position: absolute;
  inset: 0;
  background: rgba(0, 0, 0, 0.5);
  display: flex;
  align-items: center;
  justify-content: center;
  opacity: 0;
  transition: opacity 0.2s;
  pointer-events: none;
}

.preview-container:hover .preview-overlay {
  opacity: 1;
}

.overlay-content {
  display: flex;
  flex-direction: column;
  align-items: center;
  gap: 0.5rem;
  color: white;
  font-weight: 500;
}

.preview-caption {
  margin-top: 0.5rem;
  font-size: 0.75rem;
  color: #6b7280;
  text-align: center;
}

:root.dark .preview-caption {
  color: #9ca3af;
}

/* 完整模式 */
.diagram-full {
  padding: 1rem;
}

.full-controls {
  display: flex;
  justify-content: flex-end;
  margin-bottom: 0.5rem;
}

.minimize-btn {
  display: flex;
  align-items: center;
  gap: 0.25rem;
  padding: 0.375rem 0.75rem;
  background: white;
  border: 1px solid #e5e7eb;
  border-radius: 0.375rem;
  font-size: 0.875rem;
  color: #374151;
  cursor: pointer;
  transition: all 0.2s;
}

:root.dark .minimize-btn {
  background: #374151;
  border-color: #4b5563;
  color: #e5e7eb;
}

.minimize-btn:hover {
  background: #f3f4f6;
}

:root.dark .minimize-btn:hover {
  background: #4b5563;
}

.full-container {
  overflow-x: auto;
  padding: 0.5rem;
  background: #f9fafb;
  border-radius: 0.375rem;
}

:root.dark .full-container {
  background: #111827;
}

/* 動畫 */
.slide-enter-active,
.slide-leave-active {
  transition: all 0.3s ease;
  max-height: 1000px;
  overflow: hidden;
}

.slide-enter-from,
.slide-leave-to {
  max-height: 0;
  opacity: 0;
  padding: 0;
}

/* 回覆中的戰術圖使用更緊湊的樣式 */
.diagram-wrapper[data-context="reply"] .diagram-header {
  padding: 0.375rem 0.75rem;
  background: #f3f4f6;
}

:root.dark .diagram-wrapper[data-context="reply"] .diagram-header {
  background: #1f2937;
}

.diagram-wrapper[data-context="reply"] .diagram-icon {
  font-size: 0.875rem;
}

.diagram-wrapper[data-context="reply"] .diagram-title {
  font-size: 0.8125rem;
}

.diagram-wrapper[data-context="reply"] .diagram-hint {
  font-size: 0.75rem;
}

.diagram-wrapper[data-context="reply"] {
  margin: 0.75rem 0;
}

/* 響應式設計 */
@media (max-width: 640px) {
  .diagram-header {
    padding: 0.375rem 0.625rem;
  }
  
  .diagram-title {
    font-size: 0.875rem;
  }
  
  .diagram-preview,
  .diagram-full {
    padding: 0.75rem;
  }
}
</style>