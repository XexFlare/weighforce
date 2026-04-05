<template>
  <div class="flex items-center mb-6">
    <slot name="actions" />
    <div
      v-if="deletable && inRoles(['admin'])"
      @click="doDelete()"
      class="
        p-1
        rounded
        hover:bg-red-600
        hover:text-white
        transition
        mr-6
        cursor-default
        text-red-200
      "
    >
      Delete
    </div>
    <div v-if="state.edit" class="flex items-center">
      <input
        type="submit"
        value="Save"
        class="p-2 rounded transition bg-primary text-gray-200 mr-2"
        :class="
          state.loading
            ? 'bg-gray-200 hover:bg-gray-200'
            : 'hover:bg-accent hover:text-white'
        "
      />
      <div
        @click="toggle()"
        class="
          p-1
          hover:bg-accent
          rounded
          hover:text-white
          transition
          border-2 border-accent
          cursor-default
        "
      >
        Cancel
      </div>
    </div>
    <div
      v-else
      @click="toggle()"
      class="
        p-2
        hover:bg-accent
        rounded
        hover:text-white
        transition
        cursor-default
      "
    >
      Edit
    </div>
  </div>
</template>
<script>
export default {
  props: ["state", "toggle", "doDelete", "deletable"],
  inject: ["user"],
  methods: {
    inRoles(roleList) {
      if (roleList == null) return true;
      const roles = roleList.map((role) => role.toLowerCase());
      return this.user.value.roles.some((role) =>
        roles.includes(role.toLowerCase())
      );
    },
  },
};
</script>