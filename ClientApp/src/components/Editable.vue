<template>
  <li>
    <div class="flex-auto">
      <h3 class="font-bold text-gray-900">{{ title }}</h3>
      <div v-if="state.edit">
        <input
          v-if="state.edit"
          :type="type ?? 'text'"
          :value="modelValue"
          @input="$emit('update:modelValue', $event.target.value)"
          :disabled="state.loading"
          :name="autocomplete"
          :autocomplete="autocomplete"
          class="
            w-60
            bg-gray-100
            p-1
            bordered
            border-accent border-2
            rounded
            shadow
          "
        />
        <div v-if="error" class="text-red-400">
          The {{ title }} field {{ error.message }}
        </div>
        <div v-if="serverError" class="text-red-400">
          {{ serverError.message }}
        </div>
      </div>
      <p v-else class="h-4">{{ modelValue }}</p>
    </div>
  </li>
</template>
<script>
export default {
  emits: ["update:modelValue"],
  props: ["title", "modelValue", "state", "type", "autocomplete"],
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