<template>
  <div class="rich-text-display">
    <!-- 如果內容包含戰術圖，則進行特殊處理 -->
    <div v-if="hasDiagram">
      <div v-for="(segment, index) in contentSegments" :key="index">
        <div v-if="segment.type === 'html'" v-html="segment.content"></div>
        <BadmintonCourtViewer v-else-if="segment.type === 'diagram'" :data="segment.data" />
      </div>
    </div>
    <!-- 否則直接顯示HTML內容 -->
    <div v-else v-html="sanitizedContent"></div>
  </div>
</template>

<script setup>
import { computed } from 'vue'
import DOMPurify from 'dompurify'
import BadmintonCourtViewer from './BadmintonCourtViewer.vue'

const props = defineProps({
  content: {
    type: String,
    required: true
  }
})

// 檢查是否包含戰術圖
const hasDiagram = computed(() => {
  return props.content.includes('badminton-diagram-placeholder')
})

// 解析內容，分離HTML和戰術圖
const contentSegments = computed(() => {
  if (!hasDiagram.value) return []
  
  const segments = []
  const parser = new DOMParser()
  const doc = parser.parseFromString(props.content, 'text/html')
  const body = doc.body
  
  let currentHtml = ''
  
  for (const node of body.childNodes) {
    if (node.nodeType === Node.ELEMENT_NODE && 
        node.classList?.contains('badminton-diagram-placeholder')) {
      // 如果有累積的HTML，先推入
      if (currentHtml) {
        segments.push({
          type: 'html',
          content: DOMPurify.sanitize(currentHtml)
        })
        currentHtml = ''
      }
      
      // 解析戰術圖資料
      try {
        const diagramData = JSON.parse(node.getAttribute('data-diagram'))
        segments.push({
          type: 'diagram',
          data: diagramData
        })
      } catch (e) {
        console.error('Failed to parse diagram data:', e)
      }
    } else {
      // 累積HTML內容
      currentHtml += node.outerHTML || node.textContent || ''
    }
  }
  
  // 推入剩餘的HTML
  if (currentHtml) {
    segments.push({
      type: 'html',
      content: DOMPurify.sanitize(currentHtml)
    })
  }
  
  return segments
})

// 淨化HTML內容
const sanitizedContent = computed(() => {
  return DOMPurify.sanitize(props.content, {
    ALLOWED_TAGS: [
      'p', 'br', 'strong', 'em', 'u', 's', 'h1', 'h2', 'h3', 'h4', 'h5', 'h6',
      'ul', 'ol', 'li', 'blockquote', 'pre', 'code', 'a', 'img', 'hr', 'div', 'span'
    ],
    ALLOWED_ATTR: ['href', 'src', 'alt', 'title', 'class', 'style', 'target']
  })
})
</script>

<style scoped>
.rich-text-display {
  line-height: 1.6;
  color: #333;
}

:deep(p) {
  margin: 0 0 1em 0;
}

:deep(p:last-child) {
  margin-bottom: 0;
}

:deep(strong) {
  font-weight: bold;
}

:deep(em) {
  font-style: italic;
}

:deep(s) {
  text-decoration: line-through;
}

:deep(ul),
:deep(ol) {
  padding-left: 1.5em;
  margin: 0 0 1em 0;
}

:deep(ul) {
  list-style-type: disc;
}

:deep(ol) {
  list-style-type: decimal;
}

:deep(li) {
  margin: 0.25em 0;
}

:deep(blockquote) {
  padding-left: 1em;
  border-left: 3px solid #ddd;
  margin: 0 0 1em 0;
  color: #666;
  font-style: italic;
}

:deep(pre) {
  background-color: #f5f5f5;
  border: 1px solid #ddd;
  border-radius: 4px;
  padding: 0.75em 1em;
  margin: 0 0 1em 0;
  overflow-x: auto;
}

:deep(code) {
  background-color: #f5f5f5;
  padding: 0.2em 0.4em;
  border-radius: 3px;
  font-family: monospace;
  font-size: 0.9em;
}

:deep(pre code) {
  background: none;
  padding: 0;
}

:deep(hr) {
  border: none;
  border-top: 1px solid #ddd;
  margin: 1.5em 0;
}
</style>