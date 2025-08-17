<template>
  <div class="badminton-court-diagram">
    <div class="mode-selector">
      <button type="button" :class="{ active: gameMode === 'singles' }" @click="gameMode = 'singles'">
        🎾 單打模式
      </button>
      <button type="button" :class="{ active: gameMode === 'doubles' }" @click="gameMode = 'doubles'">
        👥 雙打模式
      </button>
    </div>

    <div class="toolbar">
      <button type="button" :class="{ active: mode === 'player' }" @click="mode = 'player'">👤 球員位置</button>
      <button type="button" :class="{ active: mode === 'shuttle' }" @click="mode = 'shuttle'">🏸 羽球位置</button>
      <button type="button" :class="{ active: mode === 'playerArrow' }" @click="mode = 'playerArrow'">
        🏃 人員移動
      </button>
      <button type="button" :class="{ active: mode === 'shuttleArrow' }" @click="mode = 'shuttleArrow'">
        🏸➡️ 球路軌跡
      </button>
      <button type="button" :class="{ active: mode === 'text' }" @click="mode = 'text'">📝 文字標註</button>
      <button type="button" :class="{ active: mode === 'eraser', 'eraser-btn': true }" @click="mode = 'eraser'">
        🧹 橡皮擦
      </button>
      <button type="button" class="undo-btn" :disabled="!canUndo" @click="undo">↶ 復原</button>
      <button type="button" class="redo-btn" :disabled="!canRedo" @click="redo">↷ 重做</button>
      <button type="button" class="clear-btn" @click="clearDiagram">🗑️ 清除</button>
    </div>

    <div class="canvas-container">
      <VStage
        ref="stage"
        :config="stageConfig"
        @mousedown="handleMouseDown"
        @mousemove="handleMouseMove"
        @mouseup="handleMouseUp"
      >
        <VLayer>
          <!-- 羽球場地背景 -->
          <VRect
            :config="{
              x: 0,
              y: 0,
              width: canvasWidth,
              height: canvasHeight,
              fill: '#4a7c59',
            }"
          />

          <!-- 場地線條（根據標準規格繪製） -->
          <!-- 轉換座標：Canvas Y軸從上到下，需要反轉 -->

          <!-- 1-4. 外部框架（雙打場地） -->
          <!-- 底部端線 -->
          <VLine
            :config="{
              points: [offsetX, offsetY + courtHeight, offsetX + courtWidth, offsetY + courtHeight],
              stroke: 'white',
              strokeWidth: 3,
            }"
          />
          <!-- 頂部端線 -->
          <VLine
            :config="{
              points: [offsetX, offsetY, offsetX + courtWidth, offsetY],
              stroke: 'white',
              strokeWidth: 3,
            }"
          />
          <!-- 左側雙打邊線 -->
          <VLine
            :config="{
              points: [offsetX, offsetY, offsetX, offsetY + courtHeight],
              stroke: 'white',
              strokeWidth: 3,
            }"
          />
          <!-- 右側雙打邊線 -->
          <VLine
            :config="{
              points: [offsetX + courtWidth, offsetY, offsetX + courtWidth, offsetY + courtHeight],
              stroke: 'white',
              strokeWidth: 3,
            }"
          />

          <!-- 5-6. 單打邊線 -->
          <!-- 左側單打邊線 (x=0.46m) -->
          <VLine
            :config="{
              points: [offsetX + singlesLineLeft, offsetY, offsetX + singlesLineLeft, offsetY + courtHeight],
              stroke: 'white',
              strokeWidth: 2,
            }"
          />
          <!-- 右側單打邊線 (x=5.64m) -->
          <VLine
            :config="{
              points: [offsetX + singlesLineRight, offsetY, offsetX + singlesLineRight, offsetY + courtHeight],
              stroke: 'white',
              strokeWidth: 2,
            }"
          />

          <!-- 7-8. 前發球線 -->
          <!-- 下半場前發球線 (y=4.72m) -->
          <VLine
            :config="{
              points: [
                offsetX,
                offsetY + courtHeight - frontServiceLine1,
                offsetX + courtWidth,
                offsetY + courtHeight - frontServiceLine1,
              ],
              stroke: 'white',
              strokeWidth: 2,
            }"
          />
          <!-- 上半場前發球線 (y=8.68m) -->
          <VLine
            :config="{
              points: [
                offsetX,
                offsetY + courtHeight - frontServiceLine2,
                offsetX + courtWidth,
                offsetY + courtHeight - frontServiceLine2,
              ],
              stroke: 'white',
              strokeWidth: 2,
            }"
          />

          <!-- 9-10. 雙打後發球線 -->
          <!-- 下半場雙打後發球線 (y=0.76m) -->
          <VLine
            :config="{
              points: [
                offsetX,
                offsetY + courtHeight - doubleServiceLine1,
                offsetX + courtWidth,
                offsetY + courtHeight - doubleServiceLine1,
              ],
              stroke: 'white',
              strokeWidth: 2,
            }"
          />
          <!-- 上半場雙打後發球線 (y=12.64m) -->
          <VLine
            :config="{
              points: [
                offsetX,
                offsetY + courtHeight - doubleServiceLine2,
                offsetX + courtWidth,
                offsetY + courtHeight - doubleServiceLine2,
              ],
              stroke: 'white',
              strokeWidth: 2,
            }"
          />

          <!-- 11-12. 中線 -->
          <!-- 下半場中線：從前發球線到端線 -->
          <VLine
            :config="{
              points: [
                offsetX + centerLineX,
                offsetY + courtHeight,
                offsetX + centerLineX,
                offsetY + courtHeight - frontServiceLine1,
              ],
              stroke: 'white',
              strokeWidth: 2,
            }"
          />
          <!-- 上半場中線：從前發球線到端線 -->
          <VLine
            :config="{
              points: [
                offsetX + centerLineX,
                offsetY + courtHeight - frontServiceLine2,
                offsetX + centerLineX,
                offsetY,
              ],
              stroke: 'white',
              strokeWidth: 2,
            }"
          />

          <!-- 網子 (y=6.7m) -->
          <VRect
            :config="{
              x: 0,
              y: offsetY + courtHeight - netY - 2,
              width: canvasWidth,
              height: 4,
              fill: '#333',
              opacity: 0.8,
            }"
          />

          <!-- 移動箭頭 -->
          <VArrow
            v-for="(arrow, index) in arrows"
            :key="`arrow-${index}`"
            :config="{
              points: [arrow.from.x, arrow.from.y, arrow.to.x, arrow.to.y],
              pointerLength: arrow.type === 'shuttle' ? 20 : 15,
              pointerWidth: arrow.type === 'shuttle' ? 20 : 15,
              fill: arrow.type === 'shuttle' ? '#FFD700' : '#4ecdc4',
              stroke: arrow.type === 'shuttle' ? '#FFD700' : '#4ecdc4',
              strokeWidth: arrow.type === 'shuttle' ? 4 : 3,
              dash: arrow.type === 'shuttle' ? [8, 4] : [],
              hitStrokeWidth: 20,
            }"
            @click="handleArrowClick(index)"
          />

          <!-- 羽球位置 -->
          <VGroup v-if="shuttlePosition" @click="handleShuttleClick">
            <VCircle
              :config="{
                x: shuttlePosition.x,
                y: shuttlePosition.y,
                radius: 8,
                fill: 'white',
                stroke: '#333',
                strokeWidth: 2,
              }"
            />
            <VText
              :config="{
                x: shuttlePosition.x - 12,
                y: shuttlePosition.y - 20,
                text: '🏸',
                fontSize: 20,
              }"
            />
          </VGroup>

          <!-- 球員位置 -->
          <VGroup
            v-for="player in players"
            :key="player.id"
            :config="{
              x: player.x,
              y: player.y,
              draggable: mode !== 'eraser',
            }"
            @dragend="handlePlayerDragEnd($event, player)"
            @click="handlePlayerClick(player)"
          >
            <VCircle
              :config="{
                x: 0,
                y: 0,
                radius: 18,
                fill: player.team === 'A' ? '#3498db' : '#e74c3c',
                stroke: 'white',
                strokeWidth: 2,
              }"
            />
            <VText
              :config="{
                x: getTextXOffset(player.label),
                y: -7,
                text: player.label,
                fontSize: player.label.length > 2 ? 13 : 14,
                fill: 'white',
                fontStyle: 'bold',
              }"
            />
          </VGroup>

          <!-- 繪製中的箭頭 -->
          <VArrow
            v-if="drawingArrow"
            :config="{
              points: [drawingArrow.from.x, drawingArrow.from.y, drawingArrow.to.x, drawingArrow.to.y],
              pointerLength: drawingArrow.type === 'shuttle' ? 20 : 15,
              pointerWidth: drawingArrow.type === 'shuttle' ? 20 : 15,
              fill: drawingArrow.type === 'shuttle' ? '#FFD700' : '#4ecdc4',
              stroke: drawingArrow.type === 'shuttle' ? '#FFD700' : '#4ecdc4',
              strokeWidth: drawingArrow.type === 'shuttle' ? 4 : 3,
              dash: drawingArrow.type === 'shuttle' ? [8, 4] : [5, 5],
              opacity: 0.7,
            }"
          />

          <!-- 文字標註 -->
          <VGroup
            v-for="annotation in textAnnotations"
            :key="annotation.id"
            :config="{
              x: annotation.x,
              y: annotation.y,
              draggable: mode === 'text' && editingTextId !== annotation.id,
            }"
            @dragend="handleTextDragEnd($event, annotation)"
            @click="handleTextClick(annotation, $event)"
          >
            <!-- 文字（無背景） -->
            <VText
              :config="{
                x: 0,
                y: 0,
                text: annotation.text,
                fontSize: 18,
                fill: 'black',
                fontStyle: 'normal',
              }"
            />
          </VGroup>
        </VLayer>
      </VStage>
    </div>

    <!-- 文字編輯彈窗 -->
    <div v-if="editingTextId" class="text-edit-modal">
      <input
        ref="textInput"
        v-model="tempText"
        placeholder="輸入標註文字"
        @keyup.enter="saveTextEdit"
        @keyup.esc="cancelTextEdit"
      />
      <div class="text-edit-buttons">
        <button type="button" class="save-btn" @click="saveTextEdit">確定</button>
        <button type="button" class="delete-btn" @click="deleteText">刪除</button>
        <button type="button" class="cancel-btn" @click="cancelTextEdit">取消</button>
      </div>
    </div>

    <div class="description-input">
      <label for="description">戰術說明：</label>
      <input id="description" v-model="description" type="text" placeholder="輸入戰術說明（例如：雙打防守站位）" />
    </div>
  </div>
</template>

<script setup>
import { ref, computed, watch, nextTick } from 'vue'

const props = defineProps({
  modelValue: {
    type: Object,
    default: () => ({
      players: [],
      shuttle: null,
      arrows: [],
      description: '',
    }),
  },
})

const emit = defineEmits(['update:modelValue'])

// 場地尺寸設定 (根據標準規格，但調整比例讓視覺更平衡)
// 座標系統：原點(0,0)在場地左下角，X軸向右，Y軸向上
const scale = 50 // 像素/公尺，放大比例讓場地更大
const widthScale = 1.5 // 寬度放大倍數，讓場地看起來不那麼瘦長
const courtWidth = 6.1 * scale * widthScale // 實際場地寬度(雙打)，視覺上放寬
const courtHeight = 13.4 * scale // 實際場地長度

// 畫布尺寸（留一些邊界）
const canvasWidth = courtWidth + 80
const canvasHeight = courtHeight + 80
const offsetX = 40 // 畫布邊界偏移
const offsetY = 40 // 畫布邊界偏移

// 關鍵座標（根據規格文件，X座標需要乘以寬度縮放）
const netY = 6.7 * scale // 球網位置
const frontServiceLine1 = 4.72 * scale // 下半場前發球線 (6.7 - 1.98)
const frontServiceLine2 = 8.68 * scale // 上半場前發球線 (6.7 + 1.98)
const doubleServiceLine1 = 0.76 * scale // 下半場雙打後發球線
const doubleServiceLine2 = 12.64 * scale // 上半場雙打後發球線
const centerLineX = 3.05 * scale * widthScale // 中線X座標
const singlesLineLeft = 0.46 * scale * widthScale // 左側單打邊線
const singlesLineRight = 5.64 * scale * widthScale // 右側單打邊線

const stageConfig = {
  width: canvasWidth,
  height: canvasHeight,
}

// 狀態管理
const gameMode = ref('doubles') // 'singles' 或 'doubles'
const mode = ref('player')
const players = ref([])
const shuttlePosition = ref(null)
const arrows = ref([])
const description = ref('')
const drawingArrow = ref(null)
const arrowStartPoint = ref(null)
const textAnnotations = ref([]) // 文字標註
const editingTextId = ref(null) // 正在編輯的文字ID
const editingTextObject = ref(null) // 正在編輯的文字物件（可能是新增或既有）
const tempText = ref('') // 臨時文字內容
const textInput = ref(null) // 文字輸入框ref
let nextTextId = 1 // 文字ID計數器

// 操作歷史記錄
const history = ref([])
const historyIndex = ref(-1)
const maxHistorySize = 50 // 最大歷史記錄數

// 計算屬性
const canUndo = computed(() => historyIndex.value >= 0)
const canRedo = computed(() => historyIndex.value < history.value.length - 1)

// 初始化球員位置
const initPlayers = () => {
  const centerX = offsetX + centerLineX
  const leftX = offsetX + courtWidth * 0.35
  const rightX = offsetX + courtWidth * 0.65
  // 注意：Canvas Y軸從上到下，所以底部是較大的Y值
  const bottomY = offsetY + courtHeight * 0.75 // 我方場地
  const topY = offsetY + courtHeight * 0.25 // 對方場地

  if (gameMode.value === 'singles') {
    players.value = [
      { id: 1, team: 'A', x: centerX, y: bottomY, label: '我' },
      { id: 2, team: 'B', x: centerX, y: topY, label: 'O' },
    ]
  } else {
    players.value = [
      { id: 1, team: 'A', x: leftX, y: bottomY, label: '我' },
      { id: 2, team: 'A', x: rightX, y: bottomY, label: 'P' },
      { id: 3, team: 'B', x: leftX, y: topY, label: 'O1' },
      { id: 4, team: 'B', x: rightX, y: topY, label: 'O2' },
    ]
  }
}

// 保存當前狀態到歷史
const saveToHistory = () => {
  // 刪除當前索引之後的所有歷史
  if (historyIndex.value < history.value.length - 1) {
    history.value = history.value.slice(0, historyIndex.value + 1)
  }

  // 保存當前狀態
  const currentState = {
    players: JSON.parse(JSON.stringify(players.value)),
    shuttlePosition: shuttlePosition.value ? { ...shuttlePosition.value } : null,
    arrows: JSON.parse(JSON.stringify(arrows.value)),
    textAnnotations: JSON.parse(JSON.stringify(textAnnotations.value)),
    description: description.value,
  }

  history.value.push(currentState)

  // 限制歷史大小
  if (history.value.length > maxHistorySize) {
    history.value.shift()
  } else {
    historyIndex.value++
  }
}

// 撤銷操作
const undo = () => {
  if (!canUndo.value) return

  // 如果是第一次撤銷，先保存當前狀態
  if (historyIndex.value === history.value.length - 1) {
    saveToHistory()
    historyIndex.value--
  }

  historyIndex.value--

  if (historyIndex.value >= 0) {
    const state = history.value[historyIndex.value]
    players.value = JSON.parse(JSON.stringify(state.players))
    shuttlePosition.value = state.shuttlePosition ? { ...state.shuttlePosition } : null
    arrows.value = JSON.parse(JSON.stringify(state.arrows))
    textAnnotations.value = state.textAnnotations ? JSON.parse(JSON.stringify(state.textAnnotations)) : []
    description.value = state.description
  } else {
    // 回復到初始狀態
    initPlayers()
    shuttlePosition.value = null
    arrows.value = []
    textAnnotations.value = []
    description.value = ''
  }

  emitUpdate()
}

// 重做操作
const redo = () => {
  if (!canRedo.value) return

  historyIndex.value++
  const state = history.value[historyIndex.value]

  players.value = JSON.parse(JSON.stringify(state.players))
  shuttlePosition.value = state.shuttlePosition ? { ...state.shuttlePosition } : null
  arrows.value = JSON.parse(JSON.stringify(state.arrows))
  textAnnotations.value = state.textAnnotations ? JSON.parse(JSON.stringify(state.textAnnotations)) : []
  description.value = state.description

  emitUpdate()
}

// 載入模板 - 預留功能，未來可能啟用
// const loadTemplate = (templateType) => {
//   // 先保存當前狀態
//   saveToHistory()
//
//   const centerX = offsetX + centerLineX
//   const leftX = offsetX + courtWidth * 0.3
//   const rightX = offsetX + courtWidth * 0.7
//   const frontY = offsetY + courtHeight * 0.65
//   const backY = offsetY + courtHeight * 0.8
//   const oppFrontY = offsetY + courtHeight * 0.35
//   const oppBackY = offsetY + courtHeight * 0.2
//
//   if (templateType === 'defense') {
//     // 雙打防守站位
//     players.value = [
//       { id: 1, team: 'A', x: leftX, y: backY, label: '我' },
//       { id: 2, team: 'A', x: rightX, y: backY, label: 'P' },
//       { id: 3, team: 'B', x: leftX, y: oppBackY, label: 'O1' },
//       { id: 4, team: 'B', x: rightX, y: oppBackY, label: 'O2' },
//     ]
//     description.value = '雙打防守站位 - 左右並排站位'
//   } else if (templateType === 'attack') {
//     // 雙打進攻站位
//     players.value = [
//       { id: 1, team: 'A', x: centerX, y: frontY, label: '我' },
//       { id: 2, team: 'A', x: centerX, y: backY, label: 'P' },
//       { id: 3, team: 'B', x: centerX, y: oppBackY, label: 'O1' },
//       { id: 4, team: 'B', x: centerX, y: oppFrontY, label: 'O2' },
//     ]
//     description.value = '雙打進攻站位 - 前後站位'
//   }
//
//   // 保存載入模板後的狀態
//   saveToHistory()
//
//   emitUpdate()
// }

// 清除圖表
const clearDiagram = () => {
  // 先保存當前狀態到歷史
  saveToHistory()

  initPlayers()
  shuttlePosition.value = null
  arrows.value = []
  textAnnotations.value = []
  description.value = ''
  drawingArrow.value = null
  arrowStartPoint.value = null
  editingTextId.value = null
  editingTextObject.value = null
  tempText.value = ''

  // 保存清除後的狀態
  saveToHistory()

  emitUpdate()
}

// 處理滑鼠事件
const handleMouseDown = (e) => {
  const pos = e.target.getStage().getPointerPosition()

  // 橡皮擦模式下不創建新物件
  if (mode.value === 'eraser') {
    return
  }

  // 檢查是否點擊到了場地背景（而非其他物件）
  // 如果點擊的是其他物件（如文字標註），則不處理
  const clickedOnStage =
    e.target === e.target.getStage() || e.target.className === 'Rect' || e.target.className === 'Line'

  if (!clickedOnStage) {
    return // 點擊到了其他物件，不創建新物件
  }

  if (mode.value === 'shuttle') {
    // 保存當前狀態
    saveToHistory()

    shuttlePosition.value = { x: pos.x, y: pos.y }

    // 保存新狀態
    saveToHistory()

    emitUpdate()
  } else if (mode.value === 'text') {
    // 添加文字標註 - 建立暫時的標註物件
    const newText = {
      id: `text-${nextTextId++}`,
      x: pos.x,
      y: pos.y,
      text: '',
      isNew: true, // 標記為新增的
    }

    // 立即開始編輯（但還不加入陣列）
    startEditingText(newText)
  } else if (mode.value === 'playerArrow' || mode.value === 'shuttleArrow') {
    arrowStartPoint.value = { x: pos.x, y: pos.y }
    drawingArrow.value = {
      from: { x: pos.x, y: pos.y },
      to: { x: pos.x, y: pos.y },
      type: mode.value === 'shuttleArrow' ? 'shuttle' : 'player',
    }
  }
}

const handleMouseMove = (e) => {
  if ((mode.value === 'playerArrow' || mode.value === 'shuttleArrow') && drawingArrow.value && arrowStartPoint.value) {
    const pos = e.target.getStage().getPointerPosition()
    drawingArrow.value = {
      from: arrowStartPoint.value,
      to: { x: pos.x, y: pos.y },
      type: mode.value === 'shuttleArrow' ? 'shuttle' : 'player',
    }
  }
}

const handleMouseUp = (e) => {
  if ((mode.value === 'playerArrow' || mode.value === 'shuttleArrow') && drawingArrow.value && arrowStartPoint.value) {
    const pos = e.target.getStage().getPointerPosition()

    // 只有當拖曳距離超過最小值時才創建箭頭
    const distance = Math.sqrt(
      Math.pow(pos.x - arrowStartPoint.value.x, 2) + Math.pow(pos.y - arrowStartPoint.value.y, 2),
    )

    if (distance > 20) {
      // 保存當前狀態
      saveToHistory()

      arrows.value.push({
        from: { ...arrowStartPoint.value },
        to: { x: pos.x, y: pos.y },
        type: mode.value === 'shuttleArrow' ? 'shuttle' : 'player',
      })

      // 保存新狀態
      saveToHistory()

      emitUpdate()
    }

    drawingArrow.value = null
    arrowStartPoint.value = null
  }
}

// 處理球員拖曳
const handlePlayerDragEnd = (e, player) => {
  // 保存拖曳前的狀態
  saveToHistory()

  player.x = e.target.x()
  player.y = e.target.y()

  // 保存拖曳後的狀態
  saveToHistory()

  emitUpdate()
}

// 處理文字拖曳
const handleTextDragEnd = (e, annotation) => {
  // 只有在文字模式下才能拖曳
  if (mode.value !== 'text') return

  saveToHistory()

  // 更新文字標註的位置
  const index = textAnnotations.value.findIndex((a) => a.id === annotation.id)
  if (index !== -1) {
    textAnnotations.value[index].x = e.target.x()
    textAnnotations.value[index].y = e.target.y()
  }

  emitUpdate()
}

// 開始編輯文字
const startEditingText = (annotation) => {
  editingTextId.value = annotation.id
  editingTextObject.value = annotation // 保存正在編輯的物件
  tempText.value = annotation.text || ''

  // 等待DOM更新後自動聚焦
  nextTick(() => {
    if (textInput.value) {
      textInput.value.focus()
      textInput.value.select()
    }
  })
}

// 保存文字編輯
const saveTextEdit = () => {
  if (!tempText.value.trim()) {
    // 如果沒有輸入文字，且是新增的，就不做任何事
    if (editingTextObject.value?.isNew) {
      editingTextId.value = null
      editingTextObject.value = null
      tempText.value = ''
      return
    }
    // 如果是既有的文字，刪除它
    deleteText()
    return
  }

  // 檢查是新增還是編輯
  if (editingTextObject.value?.isNew) {
    // 新增文字
    saveToHistory()
    const newAnnotation = {
      id: editingTextObject.value.id,
      x: editingTextObject.value.x,
      y: editingTextObject.value.y,
      text: tempText.value.trim(),
    }
    textAnnotations.value.push(newAnnotation)
    emitUpdate()
  } else {
    // 編輯既有文字
    const annotation = textAnnotations.value.find((a) => a.id === editingTextId.value)
    if (annotation) {
      saveToHistory()
      annotation.text = tempText.value.trim()
      emitUpdate()
    }
  }

  editingTextId.value = null
  editingTextObject.value = null
  tempText.value = ''
}

// 刪除文字
const deleteText = () => {
  // 如果是新增的文字，直接清除編輯狀態即可
  if (editingTextObject.value?.isNew) {
    editingTextId.value = null
    editingTextObject.value = null
    tempText.value = ''
    return
  }

  // 刪除既有的文字
  const index = textAnnotations.value.findIndex((a) => a.id === editingTextId.value)
  if (index !== -1) {
    saveToHistory()
    textAnnotations.value.splice(index, 1)
    emitUpdate()
  }

  editingTextId.value = null
  editingTextObject.value = null
  tempText.value = ''
}

// 取消文字編輯
const cancelTextEdit = () => {
  // 如果是新增的文字，取消時不加入陣列
  // 如果是編輯既有文字，取消時保持原樣
  editingTextId.value = null
  editingTextObject.value = null
  tempText.value = ''
}

// 計算文字 X 偏移量以達到置中對齊
const getTextXOffset = (label) => {
  // 根據不同的標籤計算偏移量
  if (label === '我') return -5
  if (label === 'P') return -5 // Partner
  if (label === 'O') return -6 // Opponent (單打)
  if (label === 'O1') return -8 // Opponent 1
  if (label === 'O2') return -8 // Opponent 2
  return -6 // 預設值
}

// 處理球員點擊（橡皮擦模式）
const handlePlayerClick = (player) => {
  if (mode.value === 'eraser') {
    saveToHistory()
    const index = players.value.findIndex((p) => p.id === player.id)
    if (index !== -1) {
      players.value.splice(index, 1)
    }
    emitUpdate()
  }
}

// 處理羽球點擊（橡皮擦模式）
const handleShuttleClick = () => {
  if (mode.value === 'eraser') {
    saveToHistory()
    shuttlePosition.value = null
    emitUpdate()
  }
}

// 處理箭頭點擊（橡皮擦模式）
const handleArrowClick = (index) => {
  if (mode.value === 'eraser') {
    saveToHistory()
    arrows.value.splice(index, 1)
    emitUpdate()
  }
}

// 處理文字點擊（橡皮擦模式或編輯）
const handleTextClick = (annotation, e) => {
  // 停止事件冒泡，避免觸發 handleMouseDown
  if (e && e.cancelBubble !== undefined) {
    e.cancelBubble = true
  }

  if (mode.value === 'eraser') {
    saveToHistory()
    const index = textAnnotations.value.findIndex((a) => a.id === annotation.id)
    if (index !== -1) {
      textAnnotations.value.splice(index, 1)
    }
    emitUpdate()
  } else if (mode.value === 'text') {
    // 在文字模式下點擊可以編輯
    startEditingText(annotation)
  }
}

// 座標轉換函數
// 絕對座標轉換為相對座標（0-1）
const absoluteToRelative = (x, y) => {
  return {
    x: (x - offsetX) / courtWidth,
    y: (y - offsetY) / courtHeight,
  }
}

// 相對座標轉換為絕對座標
const relativeToAbsolute = (x, y) => {
  return {
    x: x * courtWidth + offsetX,
    y: y * courtHeight + offsetY,
  }
}

// 發送更新事件
const emitUpdate = () => {
  // 將絕對座標轉換為相對座標再儲存
  const relativeData = {
    players: players.value.map((p) => ({
      ...p,
      ...absoluteToRelative(p.x, p.y),
    })),
    shuttle: shuttlePosition.value ? absoluteToRelative(shuttlePosition.value.x, shuttlePosition.value.y) : null,
    arrows: arrows.value.map((a) => ({
      ...a,
      from: absoluteToRelative(a.from.x, a.from.y),
      to: absoluteToRelative(a.to.x, a.to.y),
    })),
    textAnnotations: textAnnotations.value.map((t) => ({
      ...t,
      ...absoluteToRelative(t.x, t.y),
    })),
    description: description.value,
  }

  emit('update:modelValue', relativeData)
}

// 監聽 description 變化
let descriptionTimer = null
watch(description, (newVal, oldVal) => {
  // 使用 debounce 避免每次輸入都保存
  if (descriptionTimer) clearTimeout(descriptionTimer)

  descriptionTimer = setTimeout(() => {
    if (newVal !== oldVal) {
      saveToHistory()
    }
    emitUpdate()
  }, 500)
})

// 監聽遊戲模式變化
watch(gameMode, (newMode) => {
  // 保存當前狀態
  saveToHistory()

  // 重新初始化球員
  initPlayers()

  // 清除箭頭和文字（可選）
  arrows.value = []
  textAnnotations.value = []
  shuttlePosition.value = null

  // 更新描述
  description.value = newMode === 'singles' ? '單打戰術圖' : '雙打戰術圖'

  // 保存新狀態
  saveToHistory()
  emitUpdate()
})

// 初始化
initPlayers()

// 如果有初始值，載入它（從相對座標轉換為絕對座標）
if (props.modelValue && props.modelValue.players?.length > 0) {
  // 轉換球員座標
  players.value = props.modelValue.players.map((p) => ({
    ...p,
    ...relativeToAbsolute(p.x, p.y),
  }))

  // 轉換羽球座標
  shuttlePosition.value = props.modelValue.shuttle
    ? relativeToAbsolute(props.modelValue.shuttle.x, props.modelValue.shuttle.y)
    : null

  // 轉換箭頭座標
  arrows.value = (props.modelValue.arrows || []).map((a) => ({
    ...a,
    from: relativeToAbsolute(a.from.x, a.from.y),
    to: relativeToAbsolute(a.to.x, a.to.y),
  }))

  // 轉換文字標註座標
  textAnnotations.value = (props.modelValue.textAnnotations || []).map((t) => ({
    ...t,
    ...relativeToAbsolute(t.x, t.y),
  }))

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

.mode-selector {
  display: flex;
  gap: 0.5rem;
  margin-bottom: 1rem;
  justify-content: center;
}

.mode-selector button {
  padding: 0.6rem 1.5rem;
  border: 2px solid #ddd;
  background: white;
  border-radius: 6px;
  cursor: pointer;
  transition: all 0.3s;
  font-size: 1rem;
  font-weight: 500;
}

.mode-selector button:hover {
  background: #f8f9fa;
}

.mode-selector button.active {
  background: #6c5ce7;
  color: white;
  border-color: #6c5ce7;
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

.toolbar .eraser-btn.active {
  background: #e67e22;
  color: white;
  border-color: #e67e22;
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

.toolbar .undo-btn,
.toolbar .redo-btn {
  background: #95a5a6;
  color: white;
  border-color: #95a5a6;
}

.toolbar .undo-btn:hover:not(:disabled),
.toolbar .redo-btn:hover:not(:disabled) {
  background: #7f8c8d;
}

.toolbar .undo-btn:disabled,
.toolbar .redo-btn:disabled {
  opacity: 0.5;
  cursor: not-allowed;
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

/* 文字編輯彈窗 */
.text-edit-modal {
  position: fixed;
  top: 50%;
  left: 50%;
  transform: translate(-50%, -50%);
  background: white;
  padding: 1rem;
  border-radius: 8px;
  box-shadow: 0 4px 12px rgba(0, 0, 0, 0.2);
  z-index: 1000;
}

.text-edit-modal input {
  width: 250px;
  padding: 0.5rem;
  border: 1px solid #ddd;
  border-radius: 4px;
  font-size: 1rem;
  margin-bottom: 0.5rem;
}

.text-edit-modal input:focus {
  outline: none;
  border-color: #3498db;
}

.text-edit-buttons {
  display: flex;
  gap: 0.5rem;
  justify-content: flex-end;
}

.text-edit-buttons button {
  padding: 0.4rem 0.8rem;
  border: none;
  border-radius: 4px;
  cursor: pointer;
  font-size: 0.9rem;
}

.save-btn {
  background: #3498db;
  color: white;
}

.save-btn:hover {
  background: #2980b9;
}

.delete-btn {
  background: #e74c3c;
  color: white;
}

.delete-btn:hover {
  background: #c0392b;
}

.cancel-btn {
  background: #95a5a6;
  color: white;
}

.cancel-btn:hover {
  background: #7f8c8d;
}
</style>
