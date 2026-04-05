<template>
  <div class="flex justify-between">
    <div class="flex items-center mb-6">
      <back />
      <h2 class="mt-0">{{ title }}</h2>
    </div>
    <actions
      :toggle="toggle"
      :save="save"
      :state="state"
      :doDelete="doDelete"
      :deletable="deletable"
    >
      <template v-slot:actions>
        <slot name="actions" />
      </template>
    </actions>
  </div>
</template>

<script>
import Back from "./Back.vue";
import Actions from "./Actions.vue";
export default {
  props: ["title", "toggle", "save", "state", "doDelete", "deletable"],
  components: {
    Back,
    Actions,
  },
};
</script>