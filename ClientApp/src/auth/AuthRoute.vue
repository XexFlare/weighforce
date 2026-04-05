<template>
  <div>
    <div v-if="ready">
      <component v-bind:is="component" v-if="inRoles()" />
      <h2 v-else>Not Authorized</h2>
    </div>
  </div>
</template>
<script>
export default {
  props: ["component", "roles"],
  inject: ["isAuthenticated", "user"],
  async mounted() {
    this.ready = true;
    const loginRoute = "/authentication/login";
    if (!this.isAuthenticated.value) {
      this.$router.push(loginRoute);
    }
  },
  methods: {
    inRoles() {
      if (this.roles == null) return true;
      const roles = this.roles.map((role) => role.toLowerCase());
      return this.user.value.roles.some((role) =>
        roles.includes(role.toLowerCase())
      );
    },
  },
  data() {
    return {
      authorized: true,
      ready: false,
    };
  },
};
</script>