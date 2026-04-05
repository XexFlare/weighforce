<template>
  <button class="px-2 py-1" :class="typeClass">
    <slot />
  </button>
</template>
<script>
export default {
  props: ["type"],
  computed: {
    typeClass() {
      switch (this.type) {
        case "danger":
          return "p-1 rounded text-red-200 hover:bg-red-600 hover:text-white transition mr-6";
        case "outline":
          return "rounded hover:bg-accent hover:text-white border-2 border-accent";
        case "secondary":
          return "bg-gray-200 text-accent rounded hover:bg-primary hover:text-white";
        case "nbg":
          return "text-white rounded";
        case "primary":
        default:
          return "bg-accent text-white rounded hover:bg-primary";
      }
    },
  },
};
</script>