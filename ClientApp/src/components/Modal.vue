<template>
  <Transition name="fade">
    <div
      v-if="showing"
      class="
        fixed
        inset-0
        w-full
        h-screen
        flex
        items-start
        justify-center
        lg:items-center
        bg
        z-50
      "
      @click.self="closeIfShown"
    >
      <div
        class="
          relative
          max-h-screen
          w-full
          bg-white
          shadow-lg
          rounded-lg
          flex
          overflow-hidden
          opacity-100
        "
        :class="fw ? '' : xl ? 'max-w-3xl' : 'max-w-2xl'"
      >
        <button
          v-if="showClose"
          aria-label="close"
          class="absolute top-0 right-0 text-xl text-gray-500 my-2 mx-4"
          @click.prevent="close"
        >
          ×
        </button>
        <div class="overflow-auto max-h-screen w-full">
          <slot />
        </div>
      </div>
    </div>
  </Transition>
</template>
<style scoped>
.bg {
  background-color: rgba(0, 0, 0, 0.75);
}
</style>
<script>
export default {
  emits: ["close"],
  props: {
    xl: false,
    fw: false,
    showing: {
      required: true,
      type: Boolean,
    },
    showClose: {
      type: Boolean,
      default: true,
    },
  },
  watch: {
    showing(value) {
      if (value) {
        return document.querySelector("body").classList.add("overflow-hidden");
      }
      return document.querySelector("body").classList.remove("overflow-hidden");
    },
  },
  methods: {
    close() {
      this.$emit("close");
    },
    closeIfShown() {
      if (this.showClose) {
        this.close();
      }
    },
  },
  mounted: function () {
      document.addEventListener("keydown", (e) => {
        if (e.keyCode == 27 && this.showClose && this.showing) {
          this.close();
        }
      });
  },
};
</script>

<style>
.fade-enter-active,
.fade-leave-active {
  transition: all 0.6s;
}
.fade-enter,
.fade-leave-to {
  opacity: 0;
}
</style>