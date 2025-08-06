<template>
  <div class="badminton-court-diagram">
    <div class="toolbar">
      <button @click="mode = 'player'" :class="{ active: mode === 'player' }">
        👤 球員位置
      </button>
      <button @click="mode = 'shuttle'" :class="{ active: mode === 'shuttle' }">
        🏸 羽球位置
      </button>
      <button @click="mode = 'arrow'" :class="{ active: mode === 'arrow' }">
        ➡️ 移動路徑
      </button>
      <button @click="clearDiagram" class="clear-btn">
        🗑️ 清除
      </button>
      <button @click="loadTemplate('defense')" class="template-btn">
        📋 防守站位
      </button>
      <button @click="loadTemplate('attack')" class="template-btn">
        📋 進攻站位
      </button>
    </div>

    <div class="canvas-container">
      <v-stage
        ref="stage"
        :config="stageConfig"
        @mousedown="handleMouseDown"
        @mousemove="handleMouseMove"
        @mouseup="handleMouseUp"
      >
        <v-layer>
          <!-- 羽球場地背景 -->
          <v-rect
            :config="{
              x: 0,
              y: 0,
              width: courtWidth,
              height: courtHeight,
              fill: '#4a7c59',
              stroke: 'white',
              strokeWidth: 2
            }"
          />
          
          <!-- 場地線條 -->
          <!-- 外框線 -->
          <v-line
            :config="{
              points: [margin, margin, courtWidth - margin, margin],
              stroke: 'white',
              strokeWidth: 3
            }"
          />
          <v-line
            :config="{
              points: [margin, courtHeight - margin, courtWidth - margin, courtHeight - margin],
              stroke: 'white',
              strokeWidth: 3
            }"
          />
          <v-line
            :config="{
              points: [margin, margin, margin, courtHeight - margin],
              stroke: 'white',
              strokeWidth: 3
            }"
          />
          <v-line
            :config="{
              points: [courtWidth - margin, margin, courtWidth - margin, courtHeight - margin],
              stroke: 'white',
              strokeWidth: 3
            }"
          />
          
          <!-- 中線 -->
          <v-line
            :config="{
              points: [courtWidth / 2, margin, courtWidth / 2, courtHeight - margin],
              stroke: 'white',
              strokeWidth: 2,
              dash: [10, 5]
            }"
          />
          
          <!-- 發球線 -->
          <v-line
            :config="{
              points: [margin, serviceLineY, courtWidth - margin, serviceLineY],
              stroke: 'white',
              strokeWidth: 2
            }"
          />
          <v-line
            :config="{
              points: [margin, courtHeight - serviceLineY, courtWidth - margin, courtHeight - serviceLineY],
              stroke: 'white',
              strokeWidth: 2
            }"
          />
          
          <!-- 網子 -->
          <v-rect
            :config="{
              x: 0,
              y: courtHeight / 2 - 2,
              width: courtWidth,
              height: 4,
              fill: '#333',
              opacity: 0.8
            }"
          />

          <!-- 移動箭頭 -->
          <v-arrow
            v-for="(arrow, index) in arrows"
            :key="`arrow-${index}`"
            :config="{
              points: [arrow.from.x, arrow.from.y, arrow.to.x, arrow.to.y],
              pointerLength: 15,
              pointerWidth: 15,
              fill: arrow.type === 'attack' ? '#ff6b6b' : '#4ecdc4',
              stroke: arrow.type === 'attack' ? '#ff6b6b' : '#4ecdc4',
              strokeWidth: 3
            }"
          />

          <!-- 羽球位置 -->
          <v-group v-if="shuttlePosition">
            <v-circle
              :config="{
                x: shuttlePosition.x,
                y: shuttlePosition.y,
                radius: 8,
                fill: 'white',
                stroke: '#333',
                strokeWidth: 2
              }"
            />
            <v-text
              :config="{
                x: shuttlePosition.x - 12,
                y: shuttlePosition.y - 20,
                text: '🏸',
                fontSize: 20
              }"
            />
          </v-group>

          <!-- 球員位置 -->
          <v-group
            v-for="player in players"
            :key="player.id"
            :config="{
              x: player.x,
              y: player.y,
              draggable: true
            }"
            @dragend="handlePlayerDragEnd($event, player)"
          >
            <v-circle
              :config="{
                x: 0,
                y: 0,
                radius: 20,
                fill: player.team === 'A' ? '#3498db' : '#e74c3c',
                stroke: 'white',
                strokeWidth: 2
              }"
            />
            <v-text
              :config="{
                x: -10,
                y: -8,
                text: player.label,
                fontSize: 16,
                fill: 'white',
                fontStyle: 'bold'
              }"
            />
          </v-group>

          <!-- 繪製中的箭頭 -->
          <v-arrow
            v-if="drawingArrow"
            :config="{
              points: [drawingArrow.from.x, drawingArrow.from.y, drawingArrow.to.x, drawingArrow.to.y],
              pointerLength: 15,
              pointerWidth: 15,
              fill: '#999',
              stroke: '#999',
              strokeWidth: 2,
              dash: [5, 5]
            }"
          />
        </v-layer>
      </v-stage>
    </div>

    <div class="description-input">
      <label for="description">戰術說明：</label>
      <input
        id="description"
        v-model="description"
        type="text"
        placeholder="輸入戰術說明（例如：雙打防守站位）"
      />
    </div>
  </div>
</template>

<script setup>
import { ref, computed, watch } from 'vue'

const props = defineProps({
  modelValue: {
    type: Object,
    default: () => ({
      players: [],
      shuttle: null,
      arrows: [],
      description: ''
    })
  }
})

const emit = defineEmits(['update:modelValue'])

// 場地尺寸設定
const courtWidth = 600
const courtHeight = 800
const margin = 40
const serviceLineY = 200

const stageConfig = {
  width: courtWidth,
  height: courtHeight
}

// 狀態管理
const mode = ref('player')
const players = ref([])
const shuttlePosition = ref(null)
const arrows = ref([])
const description = ref('')
const drawingArrow = ref(null)
const arrowStartPoint = ref(null)

// 初始化球員位置
const initPlayers = () => {
  players.value = [
    { id: 1, team: 'A', x: courtWidth / 4, y: courtHeight * 0.3, label: 'A1' },
    { id: 2, team: 'A', x: courtWidth * 0.75, y: courtHeight * 0.3, label: 'A2' },
    { id: 3, team: 'B', x: courtWidth / 4, y: courtHeight * 0.7, label: 'B1' },
    { id: 4, team: 'B', x: courtWidth * 0.75, y: courtHeight * 0.7, label: 'B2' }
  ]
}

// 載入模板
const loadTemplate = (templateType) => {
  if (templateType === 'defense') {
    // 防守站位
    players.value = [
      { id: 1, team: 'A', x: courtWidth * 0.3, y: courtHeight * 0.25, label: 'A1' },
      { id: 2, team: 'A', x: courtWidth * 0.7, y: courtHeight * 0.25, label: 'A2' },
      { id: 3, team: 'B', x: courtWidth * 0.3, y: courtHeight * 0.75, label: 'B1' },
      { id: 4, team: 'B', x: courtWidth * 0.7, y: courtHeight * 0.75, label: 'B2' }
    ]
    description.value = '雙打防守站位 - 左右並排站位'
  } else if (templateType === 'attack') {
    // 進攻站位
    players.value = [
      { id: 1, team: 'A', x: courtWidth / 2, y: courtHeight * 0.2, label: 'A1' },
      { id: 2, team: 'A', x: courtWidth / 2, y: courtHeight * 0.35, label: 'A2' },
      { id: 3, team: 'B', x: courtWidth / 2, y: courtHeight * 0.65, label: 'B1' },
      { id: 4, team: 'B', x: courtWidth / 2, y: courtHeight * 0.8, label: 'B2' }
    ]
    description.value = '雙打進攻站位 - 前後站位'
  }
  emitUpdate()
}

// 清除圖表
const clearDiagram = () => {
  initPlayers()
  shuttlePosition.value = null
  arrows.value = []
  description.value = ''
  drawingArrow.value = null
  arrowStartPoint.value = null
  emitUpdate()
}

// 處理滑鼠事件
const handleMouseDown = (e) => {
  const pos = e.target.getStage().getPointerPosition()
  
  if (mode.value === 'shuttle') {
    shuttlePosition.value = { x: pos.x, y: pos.y }
    emitUpdate()
  } else if (mode.value === 'arrow') {
    arrowStartPoint.value = { x: pos.x, y: pos.y }
    drawingArrow.value = {
      from: { x: pos.x, y: pos.y },
      to: { x: pos.x, y: pos.y }
    }
  }
}

const handleMouseMove = (e) => {
  if (mode.value === 'arrow' && drawingArrow.value && arrowStartPoint.value) {
    const pos = e.target.getStage().getPointerPosition()
    drawingArrow.value = {
      from: arrowStartPoint.value,
      to: { x: pos.x, y: pos.y }
    }
  }
}

const handleMouseUp = (e) => {
  if (mode.value === 'arrow' && drawingArrow.value && arrowStartPoint.value) {
    const pos = e.target.getStage().getPointerPosition()
    
    // 只有當拖曳距離超過最小值時才創建箭頭
    const distance = Math.sqrt(
      Math.pow(pos.x - arrowStartPoint.value.x, 2) +
      Math.pow(pos.y - arrowStartPoint.value.y, 2)
    )
    
    if (distance > 20) {
      arrows.value.push({
        from: { ...arrowStartPoint.value },
        to: { x: pos.x, y: pos.y },
        type: 'attack' // 可以根據需要調整
      })
      emitUpdate()
    }
    
    drawingArrow.value = null
    arrowStartPoint.value = null
  }
}

// 處理球員拖曳
const handlePlayerDragEnd = (e, player) => {
  player.x = e.target.x()
  player.y = e.target.y()
  emitUpdate()
}

// 發送更新事件
const emitUpdate = () => {
  emit('update:modelValue', {
    players: players.value,
    shuttle: shuttlePosition.value,
    arrows: arrows.value,
    description: description.value
  })
}

// 監聽 description 變化
watch(description, () => {
  emitUpdate()
})

// 初始化
initPlayers()

// 如果有初始值，載入它
if (props.modelValue && props.modelValue.players?.length > 0) {
  players.value = [...props.modelValue.players]
  shuttlePosition.value = props.modelValue.shuttle
  arrows.value = [...(props.modelValue.arrows || [])]
  description.value = props.modelValue.description || ''
}
</script>

<style scoped>
.badminton-court-diagram {
  background: white;
  border-radius: 8px;
  padding: 1rem;
  margin: 1rem 0;
  box-shadow: 0 2px 4px rgba(0, 0, 0, 0.1);
}

.toolbar {
  display: flex;
  gap: 0.5rem;
  margin-bottom: 1rem;
  flex-wrap: wrap;
}

.toolbar button {
  padding: 0.5rem 1rem;
  border: 1px solid #ddd;
  background: white;
  border-radius: 4px;
  cursor: pointer;
  transition: all 0.3s;
}

.toolbar button:hover {
  background: #f0f0f0;
}

.toolbar button.active {
  background: #3498db;
  color: white;
  border-color: #3498db;
}

.toolbar .clear-btn {
  background: #e74c3c;
  color: white;
  border-color: #e74c3c;
}

.toolbar .clear-btn:hover {
  background: #c0392b;
}

.toolbar .template-btn {
  background: #27ae60;
  color: white;
  border-color: #27ae60;
}

.toolbar .template-btn:hover {
  background: #229954;
}

.canvas-container {
  border: 2px solid #ddd;
  border-radius: 4px;
  overflow: hidden;
  display: inline-block;
}

.description-input {
  margin-top: 1rem;
  display: flex;
  align-items: center;
  gap: 0.5rem;
}

.description-input label {
  font-weight: 500;
  color: #555;
}

.description-input input {
  flex: 1;
  padding: 0.5rem;
  border: 1px solid #ddd;
  border-radius: 4px;
  font-size: 1rem;
}

.description-input input:focus {
  outline: none;
  border-color: #3498db;
}
</style>