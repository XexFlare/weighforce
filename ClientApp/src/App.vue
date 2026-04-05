<template>
  <div v-if="ready">
    <Nav />
    <div
      class="px-4 mx-auto mb-8"
      :class="isAuthenticated ? 'mt-28' : 'mt-4'"
    >
      <router-view/>
    </div>
  </div>
</template>
<script>
import { provide, computed } from "vue";
import { mapActions } from "vuex";

import Nav from "./components/Nav.vue";
export default {
  name: "App",
  provide() {
    return {
      authHeaders: this.authHeaders,
      getOfficeId: this.getOfficeId,
      user: computed(() => this.user),
      logo: computed(() => this.logo),
      currentLocation: computed(() => this.currentLocation),
      isAuthenticated: computed(() => this.isAuthenticated),
      authStringHeader: this.authStringHeader,
    };
  },
  async mounted() {
    this.auth = this.$auth;
    const [isAuthenticated, user] = await Promise.all([
      await this.$auth.isAuthenticated(),
      await this.$auth.getUser(),
    ]);
    if (isAuthenticated) {
      let token = await this.auth.getAccessToken();
      this.$store.commit("auth/setAccessToken", token);
      this.user = await fetch(
        `${import.meta.env.VITE_API}/api/users/${user.name}`,
        await this.authHeaders()
      )
        .then((data) => data.json())
        .then((data) => {
          if (data.status == 404) {
            return null;
          }
          return data;
        })
        .catch((error) => console.error("Unable to get user.", error));
    }
    if (this.user != null) this.isAuthenticated = isAuthenticated;
    if (isAuthenticated && this.getOfficeId() != null) {
      this.$store.commit("auth/setOfficeId", this.getOfficeId());
    }
    if (
      isAuthenticated &&
      this.user.locations.length > 0 &&
      (this.getOfficeId() == null
      || this.user.locations.find((o) => o.id == this.getOfficeId()) == null)
    ) {
      this.$store.commit("auth/setOfficeId", this.user.locations[0].id);
      localStorage.setItem("officeId", this.user.locations[0].id);
    } else if(this.user?.locations?.length == 0) {
      this.$store.commit("auth/setOfficeId", null);
      localStorage.setItem("officeId", null);
    }
    this.ready = true;
  },
  data() {
    return {
      auth: {},
      isAuthenticated: false,
      user: {},
      ready: false,
    };
  },
  components: {
    Nav,
  },
  computed: {
    logo() {
      return this.currentLocation != null ? import.meta.env.VITE_API + this.currentLocation?.logo ?? null : null
    },
    currentLocation() {
      return this.user?.locations.find((o) => o.id == this.getOfficeId())
    }
  },
  methods: {
    ...mapActions("auth", ["setOfficeId", "setAccessToken"]),
    
    async authHeaders() {
      const token = await this.auth.getAccessToken();
      const officeId = this.getOfficeId();
      return {
        headers: !token
          ? {}
          : {
              Authorization: `Bearer ${token}`,
              OfficeId: officeId,
            },
      };
    },
    async authStringHeader() {
      const token = await this.auth.getAccessToken();
      return { Authorization: `Bearer ${token}` };
    },
    getOfficeId() {
      return localStorage.getItem("officeId");
    },
  },
};
</script>
