<template>
  <li>
    <div class="flex-auto">
      <h3 class="font-bold text-gray-900">{{ title }}</h3>
      <div v-if="state.edit">
        <select
          v-if="state.edit"
          :value="
            modelValue != null
              ? valueKey
                ? modelValue[valueKey]
                : modelValue.id
              : ''
          "
          @input="
            $emit(
              'update:modelValue',
              options.find((v) =>
                valueKey ? v[valueKey] : v.id == $event.target.value
              )
            )
          "
          class="
            w-60
            bg-gray-100
            p-1
            mb-1
            bordered
            border-accent border-2
            rounded
          "
        >
          <option
            v-for="option in options"
            :key="valueKey ? option[valueKey] : option.id"
            :value="valueKey ? option[valueKey] : option.id"
            :selected="
              modelValue != null
                ? valueKey
                  ? modelValue[valueKey] == option[valueKey]
                  : modelValue.id == option.id
                : false
            "
          >
            {{
              displayKey
                ? option[displayKey] ?? "_"
                : displayFormat
                ? displayFormat(option)
                : option?.name ?? "_"
            }}
          </option>
        </select>
        <div v-if="error" class="text-red-400">
          The {{ title }} field {{ error.message }}
        </div>
        <div v-if="serverError" class="text-red-400">
          {{ serverError.message }}
        </div>
      </div>
      <p v-else class="mb-1">
        {{
          modelValue != null
            ? displayKey
              ? modelValue[displayKey] ?? "_"
              : modelValue?.name ?? "_"
            : "_"
        }}
      </p>
    </div>
  </li>
</template>
<script>
export default {
  emits: ["update:modelValue"],
  props: [
    "title",
    "modelValue",
    "options",
    "state",
    "valueKey",
    "displayKey",
    "displayFormat",
  ],
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