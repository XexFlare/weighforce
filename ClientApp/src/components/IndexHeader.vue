<template>
  <div class="flex justify-between items-center mb-6">
    <h2 class="mt-0">{{ title ?? $route.name }}</h2>
    <router-link
      v-if="to"
      :to="to"
      class="
        p-2
        rounded
        transition
        bg-primary
        text-gray-200
        mr-2
        hover:bg-accent
        hover:text-white
      "
    >
      Add New
    </router-link>
  </div>
</template>

<script>
export default {
  props: ["title", "to"],
};
</script>