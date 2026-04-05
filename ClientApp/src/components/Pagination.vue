<template>
  <span>
    <w-button
      @click="prevPage()"
      type="nbg"
      class="mr-2"
      :disabled="!canGoPrevPage"
      :class="
        !canGoPrevPage
          ? 'bg-gray-300 cursor-not-allowed'
          : 'bg-accent hover:bg-primary'
      "
      >Prev</w-button
    >
    <w-button
      @click="nextPage()"
      type="nbg"
      :class="
        !canGoNextPage
          ? 'bg-gray-300 cursor-not-allowed'
          : 'bg-accent hover:bg-primary'
      "
      :disabled="!canGoNextPage"
      >Next</w-button
    >
  </span>
</template>
<script>
import { mapGetters } from "vuex";
import WButton from "./WButton.vue";
export default {
  mounted() {
    this.$store.dispatch("dispatches/resetPage");
  },
  components: { WButton },
  computed: mapGetters("dispatches", ["canGoPrevPage", "canGoNextPage"]),
  methods: {
    prevPage() {
      this.$store.dispatch("dispatches/prevPage");
    },
    nextPage() {
      this.$store.dispatch("dispatches/nextPage");
    },
  },
};
</script>