<template>
  <div style="width: 1000px; margin: 0 auto 0 auto">
    <h2>Locations</h2>
    <ul>
      <li v-for="office in myOffices" v-bind:key="office.id">
        {{ office.name }}
      </li>
    </ul>
    <br />
    <p v-if="updateSuccess" class="text-primary">Password has been updated</p>
    <w-button @click="toggleUpdate()" class="mb-2" type="secondary"
      >Update Password</w-button
    ><br />
    <w-button @click="logout()">Logout</w-button>
    <modal :showing="showUpdate" @close="toggleUpdate" :showClose="true">
      <form @submit.prevent="updatePassword" class="m-8 p-2">
        <ul class="grid grid-cols-2 gap-2">
          <editable
            title="Current Password"
            v-model="user.currentPassword"
            type="password"
            :state="state"
          />
          <div>
            <editable
              title="New Password"
              v-model="user.password"
              type="password"
              :state="state"
            />
            <editable
              title="Repeat Password"
              v-model="user.repeatPassword"
              type="password"
              :state="state"
            />
          </div>
        </ul>
        <div v-if="updateError" class="text-red-400 mt-2">
          {{ updateError }}
        </div>
        <div class="m-4 flex justify-end">
          <w-button type="submit" class="mr-2">Update</w-button>
          <w-button @click="toggleUpdate()" type="secondary">Cancel</w-button>
        </div>
      </form>
    </modal>
  </div>
</template>

<script>
import Editable from "../components/Editable.vue";
import Modal from "../components/Modal.vue";
import WButton from "../components/WButton.vue";
import { UserApi } from "../plugins/api-classes";
export default {
  components: {
    WButton,
    Modal,
    Editable,
  },
  inject: ["isAuthenticated", "authHeaders"],
  async mounted() {
    this.myOffices = await fetch(
      `${import.meta.env.VITE_API}/api/offices/user`,
      await this.authHeaders()
    ).then((data) => data.json());
  },
  data() {
    return {
      myOffices: [],
      newOffice: null,
      updateSuccess: false,
      updateError: null,
      state: {
        edit: true,
        errors: [],
        loading: false,
      },
      user: {
        password: "",
        currentPassword: "",
      },
      showUpdate: false,
    };
  },
  methods: {
    async updatePassword() {
      const api = new UserApi(await this.authHeaders());
      if (this.user.password != this.user.repeatPassword) {
        this.updateError = "Repeat Password does not match";
        return;
      }
      const res = await api.update("me", this.user);
      if (res.detail) {
        this.updateError = res.detail;
      } else {
        this.toggleUpdate();
        this.updateSuccess = true;
      }
      console.log(res);
    },
    toggleUpdate() {
      this.showUpdate = !this.showUpdate;
    },
    async logout() {
      const state = { returnUrl: `${import.meta.env.VITE_APP}` };
      if (this.isAuthenticated.value) {
        const result = await this.$auth.signOut(state);
        switch (result.status) {
          case AuthenticationResultStatus.Redirect:
            break;
          case AuthenticationResultStatus.Success:
            await this.navigateToReturnUrl(returnUrl);
            break;
          case AuthenticationResultStatus.Fail:
            this.setState({ message: result.message });
            break;
          default:
            throw new Error("Invalid authentication result status.");
        }
      } else {
        this.setState({ message: "You successfully logged out!" });
      }
    },
  },
};
</script>