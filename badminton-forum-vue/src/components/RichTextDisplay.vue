<template>
  <div class="rich-text-display">
    <!-- 如果內容包含戰術圖，則進行特殊處理 -->
    <div v-if="hasDiagram">
      <div v-for="(segment, index) in contentSegments" :key="index">
        <div v-if="segment.type === 'html'" v-html="segment.content"></div>
        <BadmintonDiagramWrapper 
          v-else-if="segment.type === 'diagram'" 
          :data="segment.data"
          :context="displayContext"
          :default-expanded="defaultExpanded"
        />
      </div>
    </div>
    <!-- 否則直接顯示HTML內容 -->
    <div v-else v-html="sanitizedContent"></div>
  </div>
</template>

<script setup>
import { computed } from 'vue'
import DOMPurify from 'dompurify'
import BadmintonDiagramWrapper from './BadmintonDiagramWrapper.vue'

const props = defineProps({
  content: {
    type: String,
    required: true
  },
  displayContext: {
    type: String,
    default: 'reply' // 'post' | 'reply'
  },
  defaultExpanded: {
    type: Boolean,
    default: false
  }
})

// 檢查是否包含戰術圖
const hasDiagram = computed(() => {
  return props.content.includes('badminton-diagram-placeholder')
})

// 解析內容，分離HTML和戰術圖（支援任意層級的 placeholder）
const contentSegments = computed(() => {
  if (!hasDiagram.value) return []

  const parser = new DOMParser()
  const doc = parser.parseFromString(props.content, 'text/html')
  const body = doc.body

  // 收集佔位元素，並以順序替換為唯一標記
  const placeholders = Array.from(body.querySelectorAll('.badminton-diagram-placeholder'))
  const diagramList = []
  placeholders.forEach((el, index) => {
    try {
      const dataAttr = el.getAttribute('data-diagram') || '{}'
      const diagramData = JSON.parse(dataAttr)
      diagramList.push(diagramData)
    } catch (e) {
      console.error('Failed to parse diagram data:', e)
      diagramList.push(null)
    }
    const token = `[[DIAGRAM_PLACEHOLDER_${index}]]`
    const tokenNode = doc.createTextNode(token)
    el.parentNode?.replaceChild(tokenNode, el)
  })

  // 將替換後的 HTML 依標記拆分為段落
  const combinedHtml = body.innerHTML
  const segments = []
  const parts = combinedHtml.split(/\[\[DIAGRAM_PLACEHOLDER_(\d+)\]\]/g)
  // parts 結構: [html, index, html, index, html, ...]
  for (let i = 0; i < parts.length; i++) {
    if (i % 2 === 0) {
      const htmlChunk = parts[i]
      if (htmlChunk) {
        segments.push({ type: 'html', content: DOMPurify.sanitize(htmlChunk) })
      }
    } else {
      const idx = Number(parts[i])
      const data = diagramList[idx]
      if (data) {
        segments.push({ type: 'diagram', data })
      }
    }
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

/* 萬一 fallback 走原始 HTML 渲染，避免顯示 placeholder 文字 */
:deep(.badminton-diagram-placeholder) { display: none; }

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