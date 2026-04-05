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
    <editable title="ERP Company" v-model="details.erpCompany" :state="state" />
    <editable
      title="Contract Number"
      v-model="details.contractNumber"
      :state="state"
    />
    <editable title="UOM" v-model="details.uom" :state="state" />
    <editable title="Item" v-model="details.item" :state="state" />
    <editable title="Load Type" v-model="details.loadType" :state="state" />
    <editable title="Supplier" v-model="details.supplier" :state="state" />
    <editable title="Origin Port" v-model="details.originPort" :state="state" />
    <editable
      title="Ship To Location"
      v-model="details.shipToLocation"
      :state="state"
    />
    <editable
      title="Bill Of Lading Quantity"
      v-model="details.billOfLadingQuantity"
      :state="state"
    />
    <editable title="Total" v-model="details.total" :state="state" />
    <editable
      title="Payment Terms"
      v-model="details.paymentTerms"
      :state="state"
    />
    <editable
      title="Purchase Price"
      v-model="details.purchasePrice"
      :state="state"
    />
    <editable
      title="Method Of Transport"
      v-model="details.methodOfTransport"
      :state="state"
    />
    <editable title="Vessel" v-model="details.vessel" :state="state" />
    <editable
      title="Shipping Period To"
      v-model="details.shippingPeriodTo"
      :state="state"
    />
    <editable
      title="Estimated Time Of Arrival"
      v-model="details.estimatedTimeOfArrival"
      :state="state"
    />
    <editable
      title="Allocated Quantity"
      v-model="details.allocatedQuantity"
      :state="state"
    />
    <editable title="Territory" v-model="details.territory" :state="state" />
    <editable
      title="Created Date"
      v-model="details.createdDate"
      :state="state"
    />
    <editable title="Created By" v-model="details.createdBy" :state="state" />
    <editable title="Accpac Port" v-model="details.accpacPort" :state="state" />
    <editable
      title="Accpac Destination"
      v-model="details.accpacDestination"
      :state="state"
    />
    <editable
      title="Accpac User"
      v-model="details.assignedUser"
      :state="state"
    />
  </details-view>
</template>


<script>
import { ContractsApi } from "../plugins/api-classes";
import Editable from "../components/Editable.vue";
import DetailsView from "../components/DetailsView.vue";
export default {
  inject: ["authHeaders"],
  async mounted() {
    this.api = new ContractsApi(await this.authHeaders());
    if (this.creating) {
      this.state.edit = true;
    } else {
      this.details = await this.api.get(this.$route.params.id);
      this.state.loaded = this.details?.id != 0;
    }
    this.state.initialized = true;
  },
  components: {
    Editable,
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
        errors: [],
      },
      details: {
        id: 0,
      },
      api: null,
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
      if (this.details.contractNumber) return true;
      this.state.errors.push({
        field: "Contract Number",
        message: "is required",
      });
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
    toggle() {
      if (this.creating) this.$router.back();
      this.state.edit = !this.state.edit;
    },
  },
};
</script>
