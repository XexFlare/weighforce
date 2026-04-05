<template>
  <div>
    <h2>Users</h2>
    <button
      @click="toggleModal()"
      class="ml-2 px-6 py-2 bg-accent text-white rounded hover:bg-primary"
    >
      New User
    </button>
    <div
      class="
        min-w-max
        flex
        justify-center
        overflow-y-auto
        scrollbar-w-2
        scrollbar-track-gray-lighter
        scrollbar-thumb-rounded
        scrollbar-thumb-gray
        scrolling-touch
      "
    >
      <table class="text-left table-collapse w-full">
        <thead class="before:border">
          <tr class="bg-gray-100 border-b border-t border-border-gray-900">
            <th class="td pl-2">Name</th>
            <th class="td pr-6">Email</th>
            <th class="td pr-6">Roles</th>
            <th class="td pr-6">Locations</th>
          </tr>
        </thead>
        <tbody class="align-baseline">
          <tr
            v-for="user in filtered()"
            v-bind:key="user.id"
            :class="isAdmin(user) ? 'bg-green-100' : 'bg-blue-100'"
          >
            <td
              class="
                py-2
                px-2
                font-mono
                text-sm text-blue-700
                whitespace-nowrap
                border-t border-gray-200
              "
            >
              {{ user.name }}
            </td>
            <td
              class="
                py-2
                font-mono
                text-sm text-blue-700
                whitespace-nowrap
                border-t border-gray-200
                flex
                justify-between
                align-center
              "
            >
              <span>{{ user.email }}</span>
              <button
                @click="toggleReset(user.email)"
                class="
                  rounded-full
                  p-1
                  mx-1
                  text-white
                  bg-primary
                  hover:bg-accent
                  transition
                  duration-200
                  ease-in-out
                "
              >
                <svg
                  xmlns="http://www.w3.org/2000/svg"
                  class="h-3 w-3"
                  viewBox="0 0 20 20"
                  fill="currentColor"
                >
                  <path
                    d="M10 2a5 5 0 00-5 5v2a2 2 0 00-2 2v5a2 2 0 002 2h10a2 2 0 002-2v-5a2 2 0 00-2-2H7V7a3 3 0 015.905-.75 1 1 0 001.937-.5A5.002 5.002 0 0010 2z"
                  />
                </svg>
              </button>
            </td>
            <td
              class="
                py-2
                font-mono
                text-sm text-blue-700
                border-t border-gray-200
                max-w-md
              "
            >
              <pill
                v-for="role in user.roles"
                v-bind:key="role"
                :name="role"
                :email="user.email"
                :remove="removeRole"
              />
              <button
                @click="toggleRole(user.email)"
                class="
                  px-1
                  text-white
                  bg-primary
                  hover:bg-accent
                  rounded-full
                  transition
                  duration-200
                  ease-in-out
                "
              >
                +
              </button>
              <ul
                v-if="roler == user.email"
                class="dropdown-menu absolute text-white pt-1 z-20"
              >
                <li
                  class=""
                  v-for="role in roles.filter(
                    (role) => !user.roles.includes(role)
                  )"
                  v-bind:key="role"
                >
                  <button class="dropbutton" @click="selectRole(role)">
                    {{ role }}
                  </button>
                </li>
              </ul>
            </td>
            <td
              class="
                py-2
                font-mono
                text-sm text-blue-700
                border-t border-gray-200
                max-w-md
              "
            >
              <pill
                v-for="location in user.locations"
                v-bind:key="location"
                :name="location"
                :email="user.email"
                :remove="removeLocation"
              />
              <button
                @click="toggleLocal(user.email)"
                class="
                  px-1
                  text-white
                  bg-primary
                  hover:bg-accent
                  rounded-full
                  transition
                  duration-200
                  ease-in-out
                "
              >
                +
              </button>
              <ul
                v-if="locator == user.email"
                class="dropdown-menu absolute text-white pt-1 z-20"
              >
                <li
                  v-for="location in locations.filter(
                    (location) => !user.locations.includes(location)
                  )"
                  v-bind:key="location"
                >
                  <button class="dropbutton" @click="selectLocation(location)">
                    {{ location }}
                  </button>
                </li>
              </ul>
            </td>
          </tr>
        </tbody>
      </table>
    </div>
    <Modal :showing="resetter != ''" @close="toggleReset('')" :showClose="true">
      <div class="py-3 flex flex-col items-center">
        <h2>Reset Password</h2>
        <form @submit.prevent="resetPassword">
          <div class="flex mt-4">
            <label for="password" class="w-48">Password</label>
            <input
              v-model="reset.password"
              class="w-40 bg-gray-200"
              type="password"
              name="password"
              autofocus="true"
            />
          </div>
          <div class="flex mt-4">
            <label for="repeat" class="w-48">Repeat Password</label>
            <input
              v-model="reset.repeat"
              type="password"
              name="repeat"
              class="w-40 bg-gray-200"
            />
          </div>
          <div class="text-red-600 p-4" v-if="reset.message">
            {{ reset.message }}
          </div>
          <button
            type="submit"
            class="
              w-40
              bg-primary
              hover:bg-accent
              p-1
              rounded
              text-white
              mt-2
              block
            "
          >
            Reset Password
          </button>
        </form>
      </div>
    </Modal>
    <Modal :showing="show" @close="toggleModal" :showClose="true">
      <div class="py-3 flex flex-col items-center">
        <h2>Add User</h2>
        <div class="flex mt-4">
          <label for="email" class="w-24">Email</label>
          <input
            v-model="user.email"
            type="email"
            name="email"
            id="email"
            class="w-40 bg-gray-200"
          />
        </div>
        <div class="flex mt-4">
          <label for="name" class="w-24">Name</label>
          <input
            v-model="user.name"
            type="text"
            name="name"
            id="name"
            class="w-40 bg-gray-200"
          />
        </div>
        <div class="flex mt-4">
          <label for="password" class="w-24">Password</label>
          <input
            v-model="user.password"
            type="password"
            name="password"
            id="password"
            class="w-40 bg-gray-200"
          />
        </div>
        <button
          @click="addUser()"
          class="
            w-40
            bg-primary
            hover:bg-accent
            p-1
            rounded
            text-white
            mt-2
            block
          "
        >
          Create User
        </button>
      </div>
    </Modal>
  </div>
</template>

<script>
import Modal from "../components/Modal.vue";
import Pill from "../components/Pill.vue";
export default {
  components: {
    Modal,
    Pill,
  },
  inject: ["authHeaders", "authStringHeader"],
  async mounted() {
    var { users, roles, locations } = await fetch(
      `${import.meta.env.VITE_API}/api/users`,
      await this.authHeaders()
    ).then((data) => data.json());
    this.users = users.map((user) => {
      return {
        ...user,
        locations: user.locations.map((location) => location.name),
      };
    });
    this.roles = roles;
    this.locations = locations;
  },
  data() {
    return {
      show: false,
      users: [],
      roles: [],
      locations: [],
      user: {},
      roler: "",
      reset: {},
      resetter: "",
      locator: "",
      filter: "↕",
      searchTerm: "",
    };
  },
  methods: {
    async addUser() {
      fetch(`${import.meta.env.VITE_API}/api/users`, {
        method: "POST",
        headers: {
          Accept: "application/json",
          "Content-Type": "application/json",
          ...(await this.authStringHeader()),
        },
        body: JSON.stringify(this.user),
      })
        .then((res) => res.json())
        .then((res) => {
          this.users.push(res);
          this.user = {};
          this.show = false;
        })
        .catch((error) => console.error("Unable to update item.", error));
    },
    async resetPassword() {
      if (this.reset.password != this.reset.repeat) {
        this.reset.message = "Repeat Password does not match.";
        return;
      } else this.reset.message = "";
      fetch(`${import.meta.env.VITE_API}/api/users/${this.resetter}`, {
        method: "PUT",
        headers: {
          Accept: "application/json",
          "Content-Type": "application/json",
          ...(await this.authStringHeader()),
        },
        body: JSON.stringify({
          password: this.reset.password,
        }),
      })
        .then(() => {
          this.resetter = "";
          this.reset = {};
        })
        .catch((error) => console.error("Unable to update item.", error));
    },
    async selectLocation(location) {
      fetch(`${import.meta.env.VITE_API}/api/users/location`, {
        method: "POST",
        headers: {
          Accept: "application/json",
          "Content-Type": "application/json",
          ...(await this.authStringHeader()),
        },
        body: JSON.stringify({ email: this.locator, name: location }),
      })
        .then((res) => {
          const user = this.users.find((user) => user.email == this.locator);
          user.locations.push(location);
          this.locator = "";
        })
        .catch((error) => console.error("Unable to update item.", error));
    },
    async removeLocation(email, location) {
      fetch(`${import.meta.env.VITE_API}/api/users/location`, {
        method: "DELETE",
        headers: {
          Accept: "application/json",
          "Content-Type": "application/json",
          ...(await this.authStringHeader()),
        },
        body: JSON.stringify({ email: email, name: location }),
      })
        .then((res) => {
          const user = this.users.find((user) => user.email == email);
          user.locations.splice(user.locations.indexOf(location), 1);
        })
        .catch((error) => console.error("Unable to update item.", error));
    },
    async selectRole(role) {
      fetch(`${import.meta.env.VITE_API}/api/users/role`, {
        method: "POST",
        headers: {
          Accept: "application/json",
          "Content-Type": "application/json",
          ...(await this.authStringHeader()),
        },
        body: JSON.stringify({ email: this.roler, name: role }),
      })
        .then((res) => {
          const user = this.users.find((user) => user.email == this.roler);
          user.roles.push(role);
          this.roler = "";
        })
        .catch((error) => console.error("Unable to update item.", error));
    },
    async removeRole(email, role) {
      fetch(`${import.meta.env.VITE_API}/api/users/role`, {
        method: "DELETE",
        headers: {
          Accept: "application/json",
          "Content-Type": "application/json",
          ...(await this.authStringHeader()),
        },
        body: JSON.stringify({ email, name: role }),
      })
        .then((res) => {
          const user = this.users.find((user) => user.email == email);
          user.roles.splice(user.roles.indexOf(role), 1);
        })
        .catch((error) => console.error("Unable to update item.", error));
    },
    toggleReset(user) {
      if (this.resetter == user) this.resetter = "";
      else this.resetter = user;
    },
    toggleRole(user) {
      if (this.roler == user) this.roler = "";
      else this.roler = user;
    },
    toggleLocal(user) {
      if (this.locator == user) this.locator = "";
      else this.locator = user;
    },
    toggleModal() {
      this.show = !this.show;
    },
    isAdmin(user) {
      return user.roles.includes("admin");
    },
    filtered() {
      if (this.filter == "↕") {
        return this.users.filter(this.search);
      }
      if (this.filter == "⬆Disp") {
        return this.users.filter(this.search).filter((d) => this.isAdmin(d));
      }
      if (this.filter == "⬇Rec") {
        return this.users.filter(this.search).filter((d) => !this.isAdmin(d));
      }
    },
    search(d) {
      return this.searchTerm == ""
        ? true
        : d.name
            .replace(" ", "")
            .toLowerCase()
            .includes(this.searchTerm.toLowerCase().replace(" ", ""));
    },
  },
};
</script>

<style scoped>
.dropdown:hover .dropdown-menu {
  display: block;
}
.dropbutton {
  @apply bg-primary py-2 px-4 block whitespace-nowrap w-32;
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