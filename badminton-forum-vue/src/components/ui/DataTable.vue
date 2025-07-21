<template>
  <div class="rounded-sm border border-gray-200 bg-white shadow-default dark:border-gray-700 dark:bg-gray-800">
    <div class="px-4 py-6 md:px-6 xl:px-7.5">
      <h4 class="text-xl font-semibold text-gray-900 dark:text-white">
        {{ title }}
      </h4>
    </div>

    <div class="grid grid-cols-6 border-t border-gray-200 px-4 py-4.5 dark:border-gray-700 sm:grid-cols-8 md:px-6 2xl:px-7.5">
      <div v-for="(header, index) in headers" :key="index" :class="header.class || 'col-span-1'">
        <h5 class="text-sm font-medium uppercase text-gray-600 dark:text-gray-400">
          {{ header.text }}
        </h5>
      </div>
    </div>

    <div 
      v-for="(item, index) in items" 
      :key="index"
      class="grid grid-cols-6 border-t border-gray-200 px-4 py-4.5 dark:border-gray-700 sm:grid-cols-8 md:px-6 2xl:px-7.5"
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
</template>

<script setup>
defineProps({
  title: {
    type: String,
    required: true
  },
  headers: {
    type: Array,
    required: true
  },
  items: {
    type: Array,
    required: true
  }
})
</script>