<template>
  <div>
    <h2>Mailing List</h2>
    <w-button @click="toggleAddToList" 
    class="ml-2 px-6 py-2 bg-accent text-white rounded hover:bg-primary"
    type="primary">Add To Mailing List</w-button>

    <div
      class="
        w-full
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
            <th class="td pr-6 max-w-40">Alerts</th>
          </tr>
        </thead>
        <tbody class="align-baseline">
          <tr v-for="user in mailingList" v-bind:key="user.id" class="bg-green-100">
            <td
              class="
                p-2
                font-mono
                text-sm text-blue-700
                whitespace-nowrap
                border-t border-gray-200
                w-40
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
                w-72
              "
            >
              {{ user.email }}
            </td>
            <td
              class="
                py-4
                font-mono
                text-sm text-blue-700
                border-t border-gray-200
                max-w-md
              "
            >
              <pill
                v-for="alert in user.alerts"
                v-bind:key="alert"
                :name="alert"
                :email="user.email"
                :remove="removeAlert"
              />
              <button
                @click="toggleAlert(user.email)"
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
            </td>
          </tr>
        </tbody>
      </table>
    </div>
    <modal :showing="showAddToList" @close="toggleAddToList">
      <ul class="list-none p-4">
        <editable
            title="Name"
            v-model="user.name"
            :state="state"
          />
        <editable
            title="Email"
            v-model="user.email"
            :state="state"
          />
        <selectable
          title="Alert"
          v-model="alert"
          :state="state"
          :options="alerts"
        />
        <selectable
          title="Location"
          v-model="location"
          :state="state"
          :options="locations"
        />
        <w-button :type="primary" @click="submitAlert()"> Submit </w-button>
      </ul>
    </modal>
  </div>
</template>

<script>
import Editable from "../components/Editable.vue";
import Modal from "../components/Modal.vue";
import Pill from "../components/Pill.vue";
import Selectable from "../components/Selectable.vue";
import WButton from "../components/WButton.vue";
import { LocationsApi } from "../plugins/api-classes";
export default {
  components: {
    Modal,
    Pill,
    Selectable,
    WButton,
    Editable
},
  inject: ["authHeaders", "authStringHeader"],
  async mounted() {
    this.mailingList = await fetch(
      `${import.meta.env.VITE_API}/api/users/mail`,
      await this.authHeaders()
    ).then((data) => data.json());
    let locationsApi = new LocationsApi(await this.authHeaders());
    this.locations = await locationsApi.getAll();
  },
  data() {
    return {
      mailingList: [],
      alerts: [
        {
          name: "Daily Summary",
          id: 1,
        },
        { name: "Weekly Summary", id: 2 },
        { name: "Shortage/Excess", id: 3 },
        { name: "Logistics", id: 4 },
        { name: "Trains", id: 5 },
      ],
      user: {},
      state: {
        edit: true,
        loading: false,
        errors: [],
      },
      alert: {},
      location: {},
      showAddToList: false,
      locations: [],
      alerter: "",
    };
  },
  methods: {
    async submitAlert() {
      this.user.office = {id: this.location.id, name: this.location.name }
      this.user.alert = this.alert.name
      console.log(JSON.stringify(this.user));
      fetch(`${import.meta.env.VITE_API}/api/users/mail`, {
        method: "POST",
        headers: {
          Accept: "application/json",
          "Content-Type": "application/json",
          ...(await this.authStringHeader()),
        },
        body: JSON.stringify(this.user),
      }).then((data) => {
        this.toggleAddToList();
        this.mailingList.push({
          ...this.user,
          alerts: [
            `${this.alert.name}-${this.location.name}`
          ]
        });
        this.alerter = "";
      });
    },
    toggleAddToList() {
      this.showAddToList = !this.showAddToList;
    },
    async selectAlert(alert) {
      fetch(`${import.meta.env.VITE_API}/api/users/mail`, {
        method: "POST",
        headers: {
          Accept: "application/json",
          "Content-Type": "application/json",
          ...(await this.authStringHeader()),
        },
        body: JSON.stringify({ email: this.alerter, name: alert }),
      })
        .then((res) => {
          const user = this.users.find((user) => user.email == this.alerter);
          user.alerts.push(alert);
          this.alerter = "";
        })
        .catch((error) => console.error("Unable to update item.", error));
    },
    async removeAlert(email, alert) {
      fetch(`${import.meta.env.VITE_API}/api/users/mail`, {
        method: "DELETE",
        headers: {
          Accept: "application/json",
          "Content-Type": "application/json",
          ...(await this.authStringHeader()),
        },
        body: JSON.stringify({ email, name: alert }),
      })
        .then((res) => {
          const user = this.users.find((user) => user.email == email);
          user.alerts.splice(user.alerts.indexOf(alert), 1);
        })
        .catch((error) => console.error("Unable to update item.", error));
    },
    toggleAlert(user) {
      if (this.alerter == user) this.alerter = "";
      else {
        this.alerter = user;
        this.toggleModal();
      }
    },
  },
};
</script>

<style scoped>
.dropdown:hover .dropdown-menu {
  display: block;
}
.dropbutton {
  @apply bg-primary py-2 px-4 block whitespace-nowrap w-48;
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