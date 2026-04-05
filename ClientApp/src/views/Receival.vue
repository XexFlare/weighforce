<template>
  <details-view
    style="width: 700px"
    class="mx-auto"
    :state="state"
    :create="create"
    :update="update"
    :toggle="redirect"
    :start="start"
    :finish="finish"
  >
    <editable
      title="Number Plate"
      v-model="details.numberPlate"
      :state="state"
      autocomplete="address-line1"
    />
    <editable
      title="Ticket Number"
      v-model="details.tempTicketNumber"
      :state="state"
    />
    <div>
      <w-button @click.prevent="checkDetails()" type="button">Check</w-button>
    </div>
    <br />
    <span class="text-red-600">{{ message }}</span>
    <br />
    <selectable
      v-if="check"
      title="Contract"
      v-model="details.transportInstruction"
      :options="tpis"
      :state="state"
      :displayFormat="contractDisplay"
    />
    <selectable
      v-if="check"
      title="Transporter"
      v-model="details.transporter"
      :options="transporters"
      :state="state"
    />
    <div class="col-span-2 grid grid-cols-2 gap-4">
      <selectable
        v-if="check && prefContract != null"
        title="From"
        v-model="fromLocation"
        :state="state"
        :options="locations"
      />
      <div v-else></div>
      <editable
        v-if="details?.transporter?.name == 'Other'"
        title="Transporter Name"
        v-model="details.otherTransporter"
        :state="state"
      />
      <div v-else></div>
    </div>
    <editable
      v-if="check"
      title="Driver Name"
      v-model="details.driverName"
      :state="state"
      autocomplete="name"
    />

    <editable
      v-if="check"
      title="Phone Number"
      v-model="details.phoneNumber"
      autocomplete="tel"
      :state="state"
    />
    <editable
      v-if="check"
      title="Drivers License"
      v-model="details.passportNumber"
      :state="state"
      autocomplete="address-line2"
    />
    <editable
      v-if="check"
      title="Trailer Number"
      v-model="details.trailerNumber"
      :state="state"
      autocomplete="address-line3"
    />
  </details-view>
</template>
<script>
import DetailsView from "../components/DetailsView.vue";
import Editable from "../components/Editable.vue";
import Selectable from "../components/Selectable.vue";
import WButton from "../components/WButton.vue";
import { LocationsApi, TpisApi, Transporter } from "../plugins/api-classes";
import { BaseService } from "../plugins/base-service";
export default {
  components: { Selectable, DetailsView, Editable, WButton },
  async mounted() {
    const headers = await this.authHeaders();
    this.api = new BaseService("bookings/temp", headers);
    const transporterApi = new Transporter(headers);
    const tpisApi = new TpisApi(headers);
    const locationsApi = new LocationsApi(headers);
    const metaApi = new BaseService("meta", headers);
    await metaApi.get("PrefContract:" + this.getOfficeId()).then(response => this.prefContract = response != null ? response['value'] : null);
    if(this.prefContract != null){
      this.locations = await locationsApi.getAll()
    }
    this.transporters = await transporterApi.getAll();
    this.transporters = [{ name: "Other", id: 0 }, ...this.transporters];
    this.tpis = await tpisApi.get("inbound");
    this.state.initialized = true;
  },
  computed: {
    isGSL: () => import.meta.env.VITE_GSL_SITE == "true",
  },
  data() {
    return {
      tpis: [],
      transporters: [],
      locations: [],
      prefContract: null,
      fromLocation: null,
      state: {
        edit: true,
        loading: false,
        loaded: true,
        initialized: false,
        related: false,
        errors: [],
      },
      api: null,
      details: {
        id: 0,
      },
      check: false,
      message: "",
    };
  },
  inject: ["authStringHeader", "authHeaders","getOfficeId"],
  watch: {
    "details.numberPlate": function (numberPlate, prev) {
      if (prev != numberPlate) {
        this.check = false;
      }
    },
    "details.tempTicketNumber": function (tempTicketNumber, prev) {
      if (prev != tempTicketNumber) {
        this.check = false;
      }
    },
  },
  methods: {
    async checkDetails() {
      if (this.checkValidate()) {
        this.check = await this.api.get(
          `${this.details.numberPlate}?ticket=${this.details.tempTicketNumber}`
        );
        this.message = this.check ? "" : "This Truck is in Transit";
      }
    },
    contractDisplay(contract) {
      return contract?.fromLocation?.name
       ? `${contract?.product?.name ?? "_"}: 
      ${contract?.contract?.contractNumber ?? "_"}
      (from: ${contract?.fromLocation?.name ?? "_"})`
      : `${contract?.product?.name ?? "_"}`;
    },
    start() {
      this.state.loading = true;
    },
    finish() {
      this.state.loading = false;
    },
    update() {},
    redirect() {
      this.$router.push("/receivals");
    },
    checkValidate() {
      this.state.errors = [];
      if (this.details.numberPlate) return true;
      if (!this.details.numberPlate)
        this.state.errors.push({
          field: "Number Plate",
          message: "is required",
        });
      return false;
    },
    validate() {
      this.state.errors = [];
      if (
        this.details.transportInstruction &&
        this.details.driverName &&
        this.details.numberPlate &&
        this.details.transporter
      )
        return true;
      if (!this.details.transportInstruction)
        this.state.errors.push({ field: "Contract", message: "is required" });
      if (!this.details.transporter)
        this.state.errors.push({
          field: "Transporter",
          message: "is required",
        });
        if (this.prefContract && !this.details.transportInstruction?.toLocation)
        this.state.errors.push({
          field: "Destination",
          message: "is required",
        });
      if (!this.details.tempTicketNumber)
        this.state.errors.push({
          field: "Ticket Number",
          message: "is required",
        });
      if (!this.details.driverName)
        this.state.errors.push({
          field: "Driver Name",
          message: "is required",
        });
      if (!this.details.numberPlate)
        this.state.errors.push({
          field: "Number Plate",
          message: "is required",
        });
      return false;
    },
    async create() {
      if(this.details.transportInstruction && this.fromLocation != null) this.details.transportInstruction.fromLocation = this.fromLocation
      return this.validate() && (await this.api.create(this.details));
    },
  },
};
</script>