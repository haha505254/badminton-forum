<template>
  <div class="rounded-sm border border-gray-200 bg-white px-7.5 py-6 shadow-default dark:border-gray-700 dark:bg-gray-800">
    <div class="flex h-11.5 w-11.5 items-center justify-center rounded-full bg-primary-50 dark:bg-primary-900/20">
      <slot name="icon">
        <svg class="fill-primary-600 dark:fill-primary-400" width="22" height="22" viewBox="0 0 22 22">
          <path d="M11 1L14.09 7.26L21 8.27L16 13.14L17.18 20.02L11 16.77L4.82 20.02L6 13.14L1 8.27L7.91 7.26L11 1Z" />
        </svg>
      </slot>
    </div>

    <div class="mt-4 flex items-end justify-between">
      <div>
        <h4 class="text-title-md font-bold text-gray-900 dark:text-white">
          {{ total }}
        </h4>
        <span class="text-sm font-medium text-gray-600 dark:text-gray-400">{{ title }}</span>
      </div>

      <span 
        v-if="rate"
        class="flex items-center gap-1 text-sm font-medium"
        :class="levelUp ? 'text-green-600' : 'text-red-600'"
      >
        {{ rate }}
        <svg
          v-if="levelUp"
          class="fill-current"
          width="10"
          height="11"
          viewBox="0 0 10 11"
        >
          <path d="M4.35716 2.47737L0.908974 5.82987L5.0443e-07 4.94612L5 0.0848689L10 4.94612L9.09103 5.82987L5.64284 2.47737L5.64284 10.0849L4.35716 10.0849L4.35716 2.47737Z" />
        </svg>
        <svg
          v-else
          class="fill-current"
          width="10"
          height="11"
          viewBox="0 0 10 11"
        >
          <path d="M5.64284 7.69237L9.09102 4.33987L10 5.22362L5 10.0849L0 5.22362L0.908973 4.33987L4.35716 7.69237L4.35716 0.0848701L5.64284 0.0848701L5.64284 7.69237Z" />
        </svg>
      </span>
    </div>
  </div>
</template>

<script setup>
defineProps({
  title: {
    type: String,
    required: true
  },
  total: {
    type: [String, Number],
    required: true
  },
  rate: {
    type: String,
    default: null
  },
  levelUp: {
    type: Boolean,
    default: false
  }
})
</script>