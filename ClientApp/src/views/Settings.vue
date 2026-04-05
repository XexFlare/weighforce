<template>
  <div style="width: 1200px; margin: 0 auto 0 auto" class="flex">
    <div
      class="
        fixed
        z-40
        inset-0
        flex-none
        h-full
        w-full
        md:static
        md:h-auto
        md:overflow-y-visible
        md:pt-0
        md:w-48
        lg:w-52
        md:block
      "
    >
      <div
        class="
          h-full
          overflow-y-auto
          scrolling-touch
          md:h-auto
          md:block
          md:relative
          md:sticky
          md:bg-transparent
          overflow-hidden
          md:top-18
          bg-white
          mr-24
          md:mr-0
        "
      >
        <ul class="bt-menu">
          <li>
            <router-link
              class="text-primary hover:text-accent"
              to="/settings/user"
              >User Profile</router-link
            >
          </li>
          <li v-if="inRoles(['admin'])">
            <router-link
              class="text-primary hover:text-accent"
              to="/settings/users"
              >Users &amp; Roles</router-link
            >
          </li>
          <li v-if="inRoles(['manager']) && isServer">
            <router-link
              class="text-primary hover:text-accent"
              to="/settings/mailing-list"
              >Mailing List</router-link
            >
          </li>
          <li v-if="inRoles(['upload'])">
            <router-link
              class="text-primary hover:text-accent"
              to="/settings/upload"
              >OSR Report Upload</router-link
            >
          </li>
          <li v-if="inRoles(['upload'])">
            <router-link
              class="text-primary hover:text-accent"
              to="/settings/send"
              >Send Reports</router-link
            >
          </li>
          <li v-if="inRoles(['manager'])">
            <router-link
              class="text-primary hover:text-accent"
              to="/settings/contracts"
              >Contracts</router-link
            >
          </li>
          <li v-if="inRoles(['manager', 'dispatch'])">
            <router-link
              class="text-primary hover:text-accent"
              to="/settings/transport-instructions"
              >Transport Instructions</router-link
            >
          </li>
          <li v-if="inRoles(['manager', 'dispatch'])">
            <router-link
              class="text-primary hover:text-accent"
              to="/settings/transporters"
              >Transporters</router-link
            >
          </li>
          <li v-if="inRoles(['manager', 'dispatch'])">
            <router-link
              class="text-primary hover:text-accent"
              to="/settings/products"
              >Products</router-link
            >
          </li>
          <li v-if="inRoles(['manager','dispatch'])">
            <router-link
              class="text-primary hover:text-accent"
              to="/settings/locations"
              >Locations</router-link
            >
          </li>
          <li v-if="inRoles(['manager'])">
            <router-link
              class="text-primary hover:text-accent"
              to="/settings/units"
              >Units</router-link
            >
          </li>
          <li v-if="inRoles(['manager'])">
            <router-link
              class="text-primary hover:text-accent"
              to="/settings/countries"
              >Countries</router-link
            >
          </li>
          <li v-if="inRoles(['sync', 'admin'])">
            <router-link
              class="text-primary hover:text-accent"
              to="/settings/sync"
              >Sync</router-link
            >
          </li>
        </ul>
      </div>
    </div>
    <div
      class="
        content
        min-w-0
        w-full
        flex-auto
        lg:static
        lg:max-h-full
        lg:overflow-visible
      "
    >
      <router-view />
    </div>
  </div>
</template>

<style scoped>
.bt-menu li {
  @apply flex items-center;
}
.bt-menu li a {
  @apply w-full p-2 mb-1 mr-8 rounded transition duration-200 ease-in-out;
}
.bt-menu li a:hover {
  @apply bg-accent text-white hover:bg-primary;
}
</style>
<script>
import User from "./User.vue";
export default {
  inject: ["isAuthenticated", "user"],
  components: {
    User,
  },
  methods: {
    inRoles(roleList) {
      if (roleList == null) return true;
      const roles = roleList.map((role) => role.toLowerCase());
      return this.user.value.roles.some((role) =>
        roles.includes(role.toLowerCase())
      );
    },
  },
  computed: {
    isServer: () => import.meta.env.VITE_SERVER == "true",
  },
};
</script>
