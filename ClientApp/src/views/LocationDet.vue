<template>
  <details-view
    :state="state"
    :create="create"
    :update="update"
    :toggle="toggle"
    :start="start"
    :finish="finish"
    :doDelete="doDelete"
  >
    <template v-slot:actions v-if="state.edit == false">
      <w-button @click.prevent="toggleBranch" class="mr-2" type="outline"
        >Add Branch</w-button
      >
      <w-button @click.prevent="toggleContract" class="mr-2" type="outline"
        >Select Contract</w-button
      >
    </template>
    <editable title="Name" v-model="details.name" :state="state" />
    <selectable
      title="Country"
      v-model="details.country"
      :state="state"
      :options="countries"
      displayKey="name"
    />
    <selectable
      title="Unit"
      v-model="details.unit"
      :state="state"
      :options="units"
      displayKey="name"
    />
    <checkable title="Is Customer" v-model="details.isClient" :state="state" />
    <editable title="Contacts" v-model="details.contacts" :state="state" />
    <editable title="Website" v-model="details.website" :state="state" />
    <editable
      title="Ticket Prefix"
      v-model="details.ticketPrefix"
      :state="state"
      type="number"
    />
    <dl v-if="state.edit == true">
      <dt>
        <h3 class="font-bold text-gray-900">Logo</h3>
      </dt>
      <dd>
        <input
          type="file"
          ref="file"
          name="logo"
          id="logo"
          v-on:change="onChangeFileUpload($event.target.files)"
        />
      </dd>
    </dl>
    <div class="col-span-2 mt-4" v-show="details.branches?.length > 0">
      <h2 class="mb-2">Branches</h2>
      <data-table
        :headings="headings"
        :rows="details.branches"
        :links="details.id + '/branch'"
      />
    </div>
    <modal :showing="showBranch" @close="toggleBranch">
      <div class="py-3 flex flex-col items-center">
        <h2>Add Branch</h2>
        <form @submit.prevent="createBranch">
          <ul>
            <editable title="Name" v-model="branch.name" :state="branchState" />
            <editable
              title="Phone Number"
              v-model="branch.phoneNumber"
              :state="branchState"
            />
          </ul>
          <w-button class="mt-2">Submit</w-button>
        </form>
      </div>
    </modal>
    <modal :showing="showContract" @close="toggleContract">
      <div class="py-3 flex flex-col items-center">
        <h2>Select Default Contract</h2>
        <form @submit.prevent="selectContract">
          <ul>
            <selectable
              :state="state"
              :options="contracts"
              title="Contract"
              v-model="contract"
              displayKey="contractNumber"
            />
          </ul>
          <w-button class="mt-2">Submit</w-button>
        </form>
      </div>
    </modal>
  </details-view>
</template>
<script>
import {
  ContractsApi,
  CountriesApi,
  LocationsApi,
  UnitsApi,
} from "../plugins/api-classes";
import Editable from "../components/Editable.vue";
import Selectable from "../components/Selectable.vue";
import DetailsView from "../components/DetailsView.vue";
import WButton from "../components/WButton.vue";
import Modal from "../components/Modal.vue";
import { BaseService } from "../plugins/base-service";
import DataTable from "../components/DataTable.vue";
import axios from "axios";
import Checkable from '../components/Checkable.vue';
export default {
  inject: ["authHeaders", "authStringHeader"],
  async mounted() {
    const headers = await this.authHeaders();
    this.api = new LocationsApi(headers);
    this.countriesApi = new CountriesApi(headers);
    this.unitsApi = new UnitsApi(headers);
    if (this.creating) {
      await this.getRelated();
      this.state.edit = true;
    } else {
      this.details = await this.api.get(this.$route.params.id);
      this.state.loaded = this.details.id ?? false;
    }
    this.state.initialized = true;
  },
  components: {
    Editable,
    Selectable,
    DetailsView,
    WButton,
    Modal,
    DataTable,
    Checkable,
  },
  computed: {
    creating() {
      return this.$route.params.id == null;
    },
  },
  data() {
    return {
      state: {
        edit: false,
        loading: false,
        loaded: false,
        initialized: false,
        related: false,
        errors: [],
      },
      branchState: {
        edit: true,
        loading: false,
        loaded: true,
        initialized: true,
        related: false,
        errors: [],
      },
      details: {
        id: 0,
      },
      files: new FormData(),
      branch: {},
      contract: {},
      contracts: [],
      units: [],
      api: null,
      countries: null,
      showBranch: false,
      showContract: false,
      headings: [
        {
          title: "Name",
          value: "name",
        },
        {
          title: "Phone Number",
          value: "phoneNumber",
        },
      ],
    };
  },
  methods: {
    onChangeFileUpload(fileList) {
      this.files.set("file", fileList[0], fileList[0].name);
    },
    start() {
      this.state.loading = true;
    },
    finish() {
      this.state.loading = false;
    },
    validate() {
      this.state.errors = [];
      if (this.details.name && this.details.country) return true;
      if (!this.details.name)
        this.state.errors.push({ field: "Name", message: "is required" });
      if (!this.details.country)
        this.state.errors.push({ field: "Country", message: "is required" });
      return false;
    },
    async create() {
      if (!this.validate()) return false;
      this.details.ticketPrefix = parseInt(this.details.ticketPrefix);
      this.files.set("details", JSON.stringify(this.details));
      var res = await axios
        .post(`${import.meta.env.VITE_API}/api/offices`, this.files, {
          headers: {
            "Content-Type": "multipart/form-data",
            ...(await this.authStringHeader()),
          },
        })
        .then((res) => true)
        .catch((e) => {
          if (e.response.status == 400) {
            this.state.errors = [];
            this.state.errors.push({
              field: "Name",
              message: "" + e.response.data.detail,
              server: true,
            });
            return false;
          }
        });
      return res;

      // return true;
    },
    async update() {
      if (!this.validate()) return false;
      this.details.ticketPrefix = parseInt(this.details.ticketPrefix);
      this.files.set("details", JSON.stringify(this.details));
      var res = axios
        .put(
          `${import.meta.env.VITE_API}/api/offices/${this.$route.params.id}`,
          this.files,
          {
            headers: {
              "Content-Type": "multipart/form-data",
              ...(await this.authStringHeader()),
            },
          }
        )
        .then((res) => true)
        .catch((e) => {
          if (e.response.status == 400) {
            this.state.errors = [];
            this.state.errors.push({
              field: "Name",
              message: "" + e.response.data.detail,
              server: true,
            });
            return false;
          }
        });
      return res;
    },
    async doDelete() {
      await this.api.delete(this.$route.params.id);
      this.$router.back();
    },
    async closeTPI() {
      await this.api.delete(this.$route.params.id);
      this.details.closed = true;
    },
    async getRelated() {
      this.state.loading = true;
      this.countries = await this.countriesApi.getAll();
      this.units = await this.unitsApi.getAll();
      this.state.loading = false;
      this.state.related = true;
    },
    async toggleContract() {
      if (this.showContract == true) {
        this.state.edit = false;
        this.showContract = false;
      }
      else {
        const headers = await this.authHeaders();
        const contractsApi = new ContractsApi(headers);
        this.state.loading = true;
        this.showContract = true;
        this.contracts = await contractsApi.getAll();
        this.state.loading = false;
        this.state.related = true;
        this.state.edit = true;
      }
    },
    toggleBranch() {
      this.showBranch = !this.showBranch;
    },
    async createBranch() {
      const headers = await this.authHeaders();
      const api = new BaseService("offices/branch", headers);
      const res = await api.create({
        name: this.branch.name,
        phoneNumber: this.branch.phoneNumber,
        office: { id: this.details.id },
      });
      this.details.branches = [...this.details.branches, res];
      this.toggleBranch();
    },
    async selectContract() {
      const headers = await this.authHeaders();
      const api = new BaseService("meta", headers);
      await api.create({
        name: "PrefContract:" + this.$route.params.id,
        value: this.contract.id.toString(),
      });
      this.toggleContract()
    },
    async toggle() {
      if (this.creating) this.$router.back();
      else {
        if (!this.state.related) {
          await this.getRelated();
        }
        this.state.edit = !this.state.edit;
      }
    },
  },
};
</script>