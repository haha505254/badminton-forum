import { Node, mergeAttributes } from '@tiptap/core'
import { VueNodeViewRenderer } from '@tiptap/vue-3'
import BadmintonDiagramNode from './BadmintonDiagramNode.vue'

export default Node.create({
  name: 'badmintonDiagram',

  group: 'block',

  atom: true,

  addAttributes() {
    return {
      data: {
        default: null
      }
    }
  },

  parseHTML() {
    return [
      {
        tag: 'div[class="badminton-diagram-placeholder"]',
        getAttrs: dom => {
          const data = dom.getAttribute('data-diagram')
          return { data: data ? JSON.parse(data) : null }
        }
      }
    ]
  },

  renderHTML({ HTMLAttributes }) {
    // 確保 data 是正確序列化的 JSON 字串
    const diagramData = HTMLAttributes.data || {}
    const jsonString = JSON.stringify(diagramData)
    
    return ['div', mergeAttributes({ 
      class: 'badminton-diagram-placeholder',
      'data-diagram': jsonString
    }), [
      'p', {}, `[羽球戰術圖: ${diagramData.description || '戰術示意圖'}]`
    ]]
  },

  addNodeView() {
    return VueNodeViewRenderer(BadmintonDiagramNode)
  }
})