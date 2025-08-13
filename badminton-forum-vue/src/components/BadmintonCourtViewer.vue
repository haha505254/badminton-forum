<template>
  <div class="badminton-court-viewer">
    <h4 v-if="data.description" class="diagram-title">{{ data.description }}</h4>
    
    <div class="canvas-container">
      <v-stage :config="stageConfig">
        <v-layer>
          <!-- 羽球場地背景 -->
          <v-rect
            :config="{
              x: 0,
              y: 0,
              width: canvasWidth,
              height: canvasHeight,
              fill: '#4a7c59'
            }"
          />
          
          <!-- 場地線條（與編輯器相同） -->
          <!-- 1-4. 外部框架（雙打場地） -->
          <!-- 底部端線 -->
          <v-line
            :config="{
              points: [offsetX, offsetY + courtHeight, offsetX + courtWidth, offsetY + courtHeight],
              stroke: 'white',
              strokeWidth: 3
            }"
          />
          <!-- 頂部端線 -->
          <v-line
            :config="{
              points: [offsetX, offsetY, offsetX + courtWidth, offsetY],
              stroke: 'white',
              strokeWidth: 3
            }"
          />
          <!-- 左側雙打邊線 -->
          <v-line
            :config="{
              points: [offsetX, offsetY, offsetX, offsetY + courtHeight],
              stroke: 'white',
              strokeWidth: 3
            }"
          />
          <!-- 右側雙打邊線 -->
          <v-line
            :config="{
              points: [offsetX + courtWidth, offsetY, offsetX + courtWidth, offsetY + courtHeight],
              stroke: 'white',
              strokeWidth: 3
            }"
          />
          
          <!-- 5-6. 單打邊線 -->
          <!-- 左側單打邊線 -->
          <v-line
            :config="{
              points: [offsetX + singlesLineLeft, offsetY, offsetX + singlesLineLeft, offsetY + courtHeight],
              stroke: 'white',
              strokeWidth: 2
            }"
          />
          <!-- 右側單打邊線 -->
          <v-line
            :config="{
              points: [offsetX + singlesLineRight, offsetY, offsetX + singlesLineRight, offsetY + courtHeight],
              stroke: 'white',
              strokeWidth: 2
            }"
          />
          
          <!-- 7-8. 前發球線 -->
          <!-- 下半場前發球線 -->
          <v-line
            :config="{
              points: [offsetX, offsetY + courtHeight - frontServiceLine1, offsetX + courtWidth, offsetY + courtHeight - frontServiceLine1],
              stroke: 'white',
              strokeWidth: 2
            }"
          />
          <!-- 上半場前發球線 -->
          <v-line
            :config="{
              points: [offsetX, offsetY + courtHeight - frontServiceLine2, offsetX + courtWidth, offsetY + courtHeight - frontServiceLine2],
              stroke: 'white',
              strokeWidth: 2
            }"
          />
          
          <!-- 9-10. 雙打後發球線 -->
          <!-- 下半場雙打後發球線 -->
          <v-line
            :config="{
              points: [offsetX, offsetY + courtHeight - doubleServiceLine1, offsetX + courtWidth, offsetY + courtHeight - doubleServiceLine1],
              stroke: 'white',
              strokeWidth: 2
            }"
          />
          <!-- 上半場雙打後發球線 -->
          <v-line
            :config="{
              points: [offsetX, offsetY + courtHeight - doubleServiceLine2, offsetX + courtWidth, offsetY + courtHeight - doubleServiceLine2],
              stroke: 'white',
              strokeWidth: 2
            }"
          />
          
          <!-- 11-12. 中線 -->
          <!-- 下半場中線 -->
          <v-line
            :config="{
              points: [offsetX + centerLineX, offsetY + courtHeight, offsetX + centerLineX, offsetY + courtHeight - frontServiceLine1],
              stroke: 'white',
              strokeWidth: 2
            }"
          />
          <!-- 上半場中線 -->
          <v-line
            :config="{
              points: [offsetX + centerLineX, offsetY + courtHeight - frontServiceLine2, offsetX + centerLineX, offsetY],
              stroke: 'white',
              strokeWidth: 2
            }"
          />
          
          <!-- 網子 -->
          <v-rect
            :config="{
              x: 0,
              y: offsetY + courtHeight - netY - 2,
              width: canvasWidth,
              height: 4,
              fill: '#333',
              opacity: 0.8
            }"
          />

          <!-- 移動箭頭 -->
          <v-arrow
            v-for="(arrow, index) in data.arrows"
            :key="`arrow-${index}`"
            :config="{
              points: [arrow.from.x, arrow.from.y, arrow.to.x, arrow.to.y],
              pointerLength: arrow.type === 'shuttle' ? 20 : 15,
              pointerWidth: arrow.type === 'shuttle' ? 20 : 15,
              fill: arrow.type === 'shuttle' ? '#FFD700' : '#4ecdc4',
              stroke: arrow.type === 'shuttle' ? '#FFD700' : '#4ecdc4',
              strokeWidth: arrow.type === 'shuttle' ? 4 : 3,
              dash: arrow.type === 'shuttle' ? [8, 4] : []
            }"
          />

          <!-- 羽球位置 -->
          <v-group v-if="data.shuttle">
            <v-circle
              :config="{
                x: data.shuttle.x,
                y: data.shuttle.y,
                radius: 8,
                fill: 'white',
                stroke: '#333',
                strokeWidth: 2
              }"
            />
            <v-text
              :config="{
                x: data.shuttle.x - 12,
                y: data.shuttle.y - 20,
                text: '🏸',
                fontSize: 20
              }"
            />
          </v-group>

          <!-- 球員位置 -->
          <v-group
            v-for="player in data.players"
            :key="player.id"
            :config="{
              x: player.x,
              y: player.y,
              draggable: false
            }"
          >
            <v-circle
              :config="{
                x: 0,
                y: 0,
                radius: 18,
                fill: player.team === 'A' ? '#3498db' : '#e74c3c',
                stroke: 'white',
                strokeWidth: 2
              }"
            />
            <v-text
              :config="{
                x: getTextXOffset(player.label),
                y: -7,
                text: player.label,
                fontSize: player.label.length > 2 ? 13 : 14,
                fill: 'white',
                fontStyle: 'bold'
              }"
            />
          </v-group>
          
          <!-- 文字標註 -->
          <v-group
            v-for="annotation in data.textAnnotations || []"
            :key="annotation.id"
            :config="{
              x: annotation.x,
              y: annotation.y,
              draggable: false
            }"
          >
            <!-- 文字（無背景） -->
            <v-text
              :config="{
                x: 0,
                y: 0,
                text: annotation.text,
                fontSize: 18,
                fill: 'black',
                fontStyle: 'normal'
              }"
            />
          </v-group>
        </v-layer>
      </v-stage>
    </div>

    <div class="legend">
      <div class="legend-item">
        <span class="legend-color team-a"></span>
        <span>我方</span>
      </div>
      <div class="legend-item">
        <span class="legend-color team-b"></span>
        <span>對手</span>
      </div>
      <div class="legend-item" v-if="data.shuttle">
        <span>🏸</span>
        <span>羽球位置</span>
      </div>
      <div class="legend-item" v-if="hasPlayerArrows">
        <span class="legend-arrow player-arrow"></span>
        <span>人員移動</span>
      </div>
      <div class="legend-item" v-if="hasShuttleArrows">
        <span class="legend-arrow shuttle-arrow"></span>
        <span>球路軌跡</span>
      </div>
    </div>
  </div>
</template>

<script setup>
import { computed } from 'vue'

const props = defineProps({
  data: {
    type: Object,
    required: true,
    default: () => ({
      players: [],
      shuttle: null,
      arrows: [],
      description: ''
    })
  },
  scale: {
    type: Number,
    default: 1
  }
})

// 場地尺寸設定 (與編輯器相同的規格，但支援縮放)
const baseScale = 50
const scale = baseScale * props.scale
const widthScale = 1.5
const courtWidth = 6.1 * scale * widthScale
const courtHeight = 13.4 * scale
const canvasWidth = courtWidth + 80 * props.scale
const canvasHeight = courtHeight + 80 * props.scale
const offsetX = 40 * props.scale
const offsetY = 40 * props.scale

// 關鍵座標
const netY = 6.7 * scale
const frontServiceLine1 = 4.72 * scale
const frontServiceLine2 = 8.68 * scale
const doubleServiceLine1 = 0.76 * scale
const doubleServiceLine2 = 12.64 * scale
const centerLineX = 3.05 * scale * widthScale
const singlesLineLeft = 0.46 * scale * widthScale
const singlesLineRight = 5.64 * scale * widthScale

const stageConfig = {
  width: canvasWidth,
  height: canvasHeight
}

// 計算文字 X 偏移量以達到置中對齊
const getTextXOffset = (label) => {
  // 根據不同的標籤計算偏移量
  if (label === '我') return -5
  if (label === '隊友') return -12
  if (label === '對手') return -12
  if (label === '對手1') return -16
  if (label === '對手2') return -16
  return -8 // 預設值
}

// 計算屬性：是否有人員移動箭頭
const hasPlayerArrows = computed(() => {
  return props.data.arrows?.some(arrow => arrow.type === 'player') || false
})

// 計算屬性：是否有球路軌跡箭頭
const hasShuttleArrows = computed(() => {
  return props.data.arrows?.some(arrow => arrow.type === 'shuttle') || false
})
</script>

<style scoped>
.badminton-court-viewer {
  background: white;
  border-radius: 8px;
  padding: 1rem;
  margin: 1rem 0;
  box-shadow: 0 2px 4px rgba(0, 0, 0, 0.1);
}

.diagram-title {
  color: #2c3e50;
  margin: 0 0 1rem 0;
  font-size: 1.1rem;
  font-weight: 500;
  text-align: center;
}

.canvas-container {
  border: 2px solid #ddd;
  border-radius: 4px;
  overflow: hidden;
  display: inline-block;
  width: 100%;
  max-width: 600px;
}

.legend {
  margin-top: 1rem;
  display: flex;
  gap: 1.5rem;
  flex-wrap: wrap;
  justify-content: center;
  font-size: 0.9rem;
  color: #666;
}

.legend-item {
  display: flex;
  align-items: center;
  gap: 0.5rem;
}

.legend-color {
  width: 20px;
  height: 20px;
  border-radius: 50%;
  border: 2px solid white;
  box-shadow: 0 1px 2px rgba(0, 0, 0, 0.2);
}

.legend-color.team-a {
  background: #3498db;
}

.legend-color.team-b {
  background: #e74c3c;
}

.legend-arrow {
  width: 30px;
  height: 2px;
  position: relative;
}

.legend-arrow.player-arrow {
  background: #4ecdc4;
}

.legend-arrow.player-arrow::after {
  content: '';
  position: absolute;
  right: 0;
  top: -3px;
  width: 0;
  height: 0;
  border-left: 8px solid #4ecdc4;
  border-top: 4px solid transparent;
  border-bottom: 4px solid transparent;
}

.legend-arrow.shuttle-arrow {
  background: linear-gradient(90deg, #FFD700 0%, #FFD700 25%, transparent 25%, transparent 50%, #FFD700 50%, #FFD700 75%, transparent 75%);
  background-size: 8px 100%;
}

.legend-arrow.shuttle-arrow::after {
  content: '';
  position: absolute;
  right: 0;
  top: -3px;
  width: 0;
  height: 0;
  border-left: 8px solid #FFD700;
  border-top: 4px solid transparent;
  border-bottom: 4px solid transparent;
}

@media (max-width: 768px) {
  .canvas-container {
    overflow-x: auto;
  }
  
  .legend {
    font-size: 0.8rem;
  }
}
</style>