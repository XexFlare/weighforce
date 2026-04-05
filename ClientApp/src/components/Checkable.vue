<template>
  <li>
    <div class="flex-auto">
      <h3 class="font-bold text-gray-900">{{ title }}</h3>
      <div v-if="state.edit">
        <input
          v-if="state.edit"
          type="checkbox"
          :checked="modelValue == 'true' || modelValue"
          @input="$emit('update:modelValue', $event.target.checked)"
          :disabled="state.loading"
          class="
            p-1
          "
        />
        <div v-if="error" class="text-red-400">
          The {{ title }} field {{ error.message }}
        </div>
        <div v-if="serverError" class="text-red-400">
          {{ serverError.message }}
        </div>
      </div>
      <p v-else class="h-4">{{ modelValue == true ? "☑️" : "🔲"}}</p>
    </div>
  </li>
</template>
<script>
export default {
  emits: ["update:modelValue"],
  props: ["title", "modelValue", "state"],
  computed: {
    serverError() {
      return this.state.errors.find((e) => e.field == this.title && e.server);
    },
    error() {
      return this.state.errors.find((e) => e.field == this.title && !e.server);
    },
  },
};
</script>