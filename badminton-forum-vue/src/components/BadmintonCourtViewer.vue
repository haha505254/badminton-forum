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
            v-for="(arrow, index) in data.arrows"
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
        </v-layer>
      </v-stage>
    </div>

    <div class="legend">
      <div class="legend-item">
        <span class="legend-color team-a"></span>
        <span>A隊</span>
      </div>
      <div class="legend-item">
        <span class="legend-color team-b"></span>
        <span>B隊</span>
      </div>
      <div class="legend-item" v-if="data.shuttle">
        <span>🏸</span>
        <span>羽球位置</span>
      </div>
      <div class="legend-item" v-if="data.arrows?.length > 0">
        <span class="legend-arrow"></span>
        <span>移動路徑</span>
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
  }
})

// 場地尺寸設定
const courtWidth = 600
const courtHeight = 800
const margin = 40
const serviceLineY = 200

const stageConfig = {
  width: courtWidth,
  height: courtHeight
}
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
  background: #ff6b6b;
  position: relative;
}

.legend-arrow::after {
  content: '';
  position: absolute;
  right: 0;
  top: -3px;
  width: 0;
  height: 0;
  border-left: 8px solid #ff6b6b;
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