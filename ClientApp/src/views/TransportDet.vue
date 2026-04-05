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
    <template v-slot:actions>
      <div
        v-if="!state.edit"
        @click="closeTPI()"
        class="
          p-1
          rounded
          hover:bg-red-600
          hover:text-white
          transition
          mr-6
          cursor-default
        "
        :class="details.closed ? 'text-red-600 underline' : 'text-red-200'"
      >
        {{ details.closed ? "Closed" : "Close" }}
      </div>
    </template>
    <selectable
      title="Contract"
      v-model="details.contract"
      :state="state"
      :options="contracts"
      displayKey="contractNumber"
    />
    <selectable
      title="Product"
      v-model="details.product"
      :state="state"
      :options="products"
    />
    <selectable
      title="From"
      v-model="details.fromLocation"
      :state="state"
      :options="locations"
    />
    <selectable
      title="To"
      v-model="details.toLocation"
      :state="state"
      :options="locations"
    />
    <editable
      title="Kinetic Reference"
      v-model="details.kineticRef"
      :state="state"
    />
  </details-view>
</template>
<script>
import {
  ContractsApi,
  LocationsApi,
  ProductsApi,
  TpisApi,
} from "../plugins/api-classes";
import Editable from "../components/Editable.vue";
import Selectable from "../components/Selectable.vue";
import DetailsView from "../components/DetailsView.vue";
export default {
  inject: ["authHeaders"],
  async mounted() {
    const headers = await this.authHeaders();
    this.api = new TpisApi(headers);
    this.productsApi = new ProductsApi(headers);
    this.contractsApi = new ContractsApi(headers);
    this.locationsApi = new LocationsApi(headers);
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
      details: {
        id: 0,
      },
      api: null,
      products: null,
      contracts: null,
      locations: null,
      productsApi: null,
      contractsApi: null,
      locationsApi: null,
    };
  },
  methods: {
    start() {
      this.state.loading = true;
    },
    finish() {
      this.state.loading = false;
    },
    validate() {
      this.state.errors = [];

      if (
        this.details.contract &&
        this.details.product &&
        this.details.fromLocation &&
        this.details.toLocation &&
        this.details.fromLocation != this.details.toLocation
      )
        return true;
      if (this.details.fromLocation == this.details.toLocation)
        this.state.errors.push({
          field: "To",
          message: "cannot be the same location as From",
        });
      if (!this.details.contract)
        this.state.errors.push({ field: "Contract", message: "is required" });
      if (!this.details.product)
        this.state.errors.push({ field: "Product", message: "is required" });
      if (!this.details.fromLocation)
        this.state.errors.push({ field: "From", message: "is required" });
      if (!this.details.toLocation)
        this.state.errors.push({ field: "To", message: "is required" });
      return false;
    },
    async create() {
      return this.validate() && (await this.api.create(this.details));
    },
    async update() {
      return (
        this.validate() &&
        (await this.api.update(this.$route.params.id, this.details))
      );
    },
    async doDelete() {
      await this.api.delete(this.$route.params.id);
      this.$router.back();
    },
    async closeTPI() {
      await this.api.delete(this.$route.params.id + "/close");
      this.details.closed = !this.details.closed;
    },
    async getRelated() {
      this.state.loading = true;
      await Promise.all([
        (this.products = await this.productsApi.getAll()),
        (this.contracts = await this.contractsApi.getAll()),
        (this.locations = await this.locationsApi.getAll()),
      ]);
      this.state.loading = false;
      this.state.related = true;
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