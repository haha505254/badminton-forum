<template>
  <div class="relative rounded-lg border border-gray-200 bg-white shadow-sm dark:border-gray-700 dark:bg-gray-800 overflow-hidden">
    <!-- Title (render only if non-empty) -->
    <div v-if="title && title.trim().length > 0" class="px-4 py-5 md:px-6 xl:px-7.5">
      <h4 class="text-xl font-semibold text-gray-900 dark:text-white">
        {{ title }}
      </h4>
    </div>

    <!-- Header row -->
    <div
      :class="[
        'grid grid-cols-6 border-t border-gray-200 px-4 dark:border-gray-700 sm:grid-cols-8 md:px-6 2xl:px-7.5',
        stickyHeader
          ? 'sticky top-0 z-10 py-3 bg-gray-50/80 dark:bg-gray-800/80 backdrop-blur supports-[backdrop-filter]:bg-gray-50/60 supports-[backdrop-filter]:dark:bg-gray-800/60'
          : 'py-4.5 bg-gray-50 dark:bg-gray-800'
      ]"
    >
      <div v-for="(header, index) in headers" :key="index" :class="header.class || 'col-span-1'">
        <h5 class="text-xs tracking-wide font-semibold uppercase text-gray-600 dark:text-gray-400">
          {{ header.text }}
        </h5>
      </div>
    </div>

    <!-- Loading skeleton -->
    <template v-if="loading">
      <div
        v-for="n in skeletonRows"
        :key="`skeleton-${n}`"
        :class="[
          'grid grid-cols-6 border-t border-gray-100 px-4 dark:border-gray-700 sm:grid-cols-8 md:px-6 2xl:px-7.5',
          dense ? 'py-3' : 'py-4.5'
        ]"
      >
        <div class="col-span-full">
          <div class="h-4 w-full rounded bg-gray-200/70 dark:bg-gray-700 animate-pulse"></div>
        </div>
      </div>
    </template>

    <!-- Rows -->
    <template v-else>
      <div v-if="items && items.length > 0">
        <div 
          v-for="(item, index) in items" 
          :key="index"
          :class="[
            'grid grid-cols-6 border-t px-4 sm:grid-cols-8 md:px-6 2xl:px-7.5 transition-colors',
            dense ? 'py-3' : 'py-4.5',
            'border-gray-100 dark:border-gray-700',
            zebra
              ? 'odd:bg-white even:bg-gray-50 dark:odd:bg-gray-800 dark:even:bg-gray-900/60 hover:bg-gray-50 dark:hover:bg-gray-800'
              : 'bg-white dark:bg-gray-800 hover:bg-gray-50 dark:hover:bg-gray-800'
          ]"
        >
          <slot name="row" :item="item" :index="index">
            <div v-for="(header, hIndex) in headers" :key="hIndex" :class="header.class || 'col-span-1'">
              <p class="text-sm text-gray-900 dark:text-white">
                {{ item[header.value] }}
              </p>
            </div>
          </slot>
        </div>
      </div>
      <div v-else class="border-t border-gray-100 px-4 py-10 text-center text-sm text-gray-500 dark:border-gray-700 dark:text-gray-400 md:px-6 2xl:px-7.5">
        {{ emptyText }}
      </div>
    </template>
  </div>
</template>

<script setup>
defineProps({
  title: {
    type: String,
    required: false,
    default: ''
  },
  headers: {
    type: Array,
    required: true
  },
  items: {
    type: Array,
    required: true
  },
  loading: {
    type: Boolean,
    default: false
  },
  emptyText: {
    type: String,
    default: '暫無資料'
  },
  zebra: {
    type: Boolean,
    default: true
  },
  stickyHeader: {
    type: Boolean,
    default: true
  },
  dense: {
    type: Boolean,
    default: false
  },
  skeletonRows: {
    type: Number,
    default: 5
  }
})
</script>