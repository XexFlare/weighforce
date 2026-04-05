<template>
  <div class="bg-primary w-screen fixed top-0 left-0 shadow-lg z-20 p-2">
    <nav v-if="isAuthenticated.value" x-data="{show:false}" 
       class="flex items-center justify-between flex-wrap">
      <div class="flex items-center shrink-0 text-white mr-6">
        <div class="flex items-center justify-between h-16">
          <div class="flex items-center">
            <div class="
                flex
                px-2
                py-1
                self-center
                text-sm
                font-medium
                text-gray-400
                antialiased
                focus:outline-none focus:bg-blue-100
              ">
              <img src="../assets/logo.png" class="main-logo " />
              <img src="../assets/meridian.jpg" class="logo px-2" />
              <img :src="logo.value" class="logo px-2" />
            </div>
            <div class="md:block">
              <div class="flex items-center ml-3"></div>
            </div>
          </div>
        </div>
      </div>
      <div class="block md:hidden">
        <button @click="show = !show"
          class="flex items-center px-3 py-2 border rounded text-gray-100 border-gray-200 hover:text-white hover:border-white">
          <svg class="fill-current h-3 w-3" viewBox="0 0 20 20" xmlns="http://www.w3.org/2000/svg">
            <title>Menu</title>
            <path d="M0 3h20v2H0V3zm0 6h20v2H0V9zm0 6h20v2H0v-2z" />
          </svg>
        </button>
      </div>
      <div
      
      :class="{ 'hidden': !show, 'w-full block grow md:flex md:justify-end md:w-auto': true }">
        <div class="" v-if="locations">
          <div class="flex justify-end">
            <div>
              <div class="dropdown block z-30" v-if="office">
                <button class="nav-link inline-flex">
                  <span class="mr-1">{{ office.name }}</span>
                  <svg class="fill-current h-4 w-4" xmlns="http://www.w3.org/2000/svg" viewBox="0 0 20 20">
                    <path d="M9.293 12.95l.707.707L15.657 8l-1.414-1.414L10 10.828 5.757 6.586 4.343 8z" />
                  </svg>
                </button>
                <ul class="dropdown-menu absolute hidden text-white pt-1 z-20">
                  <li class="" v-for="office in locations" v-bind:key="office.id">
                    <button class="dropbutton" @click="selectOffice(office)">
                      {{ office.name }}
                    </button>
                  </li>
                </ul>
              </div>
            </div>
            <router-link class="nav-link" to="/settings/user">{{
              userName
            }}</router-link>
          </div>
          <div class='mt-2 flex-grow basis-[100%] items-center lg:mt-0 lg:!flex lg:basis-auto justify-end'>
            <ul class="list-style-none mr-auto flex flex-col pl-0 lg:mt-1 lg:flex-row" @click.away="show = false">
              <nav-link link="/">Menu</nav-link>
              <nav-link v-if="inRoles(['operator'])" link="/transit">Transit</nav-link>
              <nav-link v-if="inRoles(['operator'])" link="/receivals">Receivals</nav-link>
              <nav-link link="/trucks">Vehicle Database</nav-link>
              <nav-link v-if="inRoles(['manager'])" link="/reports">Reports</nav-link>
              <nav-link link="/settings">Settings</nav-link>
            </ul>
          </div>
        </div>
      </div>
    </nav>
    <header v-else>
      <img src="../assets/login.jpg" alt="login image" class="w-screen" />
      <div class="w-full py-2 bg-primary text-gray-300 flex justify-end">
        <router-link class="nav-link" to="/authentication/login">Login</router-link>
      </div>
    </header>
  </div>
</template>

<script>
import NavLink from "./NavLink.vue";

export default {
  components: {
    NavLink,
  },
  inject: ["authHeaders", "getOfficeId", "user", "isAuthenticated", "logo"],
  async mounted() {
    if (this.isAuthenticated.value) {
      this.userName = this.user.value.name;
      this.locations = this.user.value.locations;
      const id = this.getOfficeId();
      if (this.locations.length > 0) {
        this.office =
          id != null
            ? this.locations.find((o) => o.id == id) ?? this.locations[0]
            : this.locations[0];
      }
    }
  },
  data() {
    return {
      userName: "",
      locations: [],
      office: null,
      show: false,
    };
  },
  methods: {
    selectOffice(office) {
      this.office = office;
      localStorage.setItem("officeId", office.id);
      this.$router.go(0);
    },
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
<style scoped>
.logo {
  @apply hidden lg:block;
  height: 50px;
}
.main-logo {
  height: 50px;
}

.router-link-active,
.nav-link:hover {
  @apply bg-accent text-white;
}

.nav-link:focus {
  @apply outline-none bg-accent;
}

@media (min-width: 764px) {
  nav:after {
    content: "";
    position: absolute;
    left: 0;
    top: 60px;
    z-index: -1;
    border-style: solid;
    border-width: 45px 0px 0px 100vw;
    border-color: #4d7d77 transparent transparent transparent;
  }
}

.dropdown:hover .dropdown-menu {
  display: block;
}

.dropbutton {
  @apply bg-primary py-2 px-4 block whitespace-nowrap w-max;
}

.dropbutton:hover {
  @apply bg-accent;
}

.dropbutton:first-of-type {
  @apply rounded-t;
}

.dropbutton:last-of-type {
  @apply rounded-b;
}
</style>