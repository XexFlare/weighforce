<template>
  <div>
    <ul class="flex">
      <li v-for="tab in tabs" :key="tab">
        <button
          class="nav-link bg-accent text-white p-2 hover:bg-primary"
          :class="{ 'bg-primary': tab === selected }"
          @click="setTab(tab)"
        >
          {{ tab }}
        </button>
      </li>
    </ul>
    <slot></slot>
  </div>
</template>

<script>
export default {
  props: {
    tabs: {
      type: Array,
      required: true,
    },
    selected: {
      type: String,
      required: true,
    },
  },
  emits: ["setSelected"],
  methods: {
    setTab(tab) {
      console.log("Emitting ", tab);
      this.$emit("setSelected", tab);
    },
  },
};
</script>

  <style scoped>
</style>
