<template>
  <div class="lg:w-screen">

  <details-view
    class="lg:w-1/2 mx-auto"
    :state="state"
    :create="create"
    :update="update"
    :toggle="redirect"
    :start="start"
    :finish="finish"
  >
    <selectable
      title="Contract"
      v-model="tpi"
      :options="tpis"
      :state="state"
      :displayFormat="contractDisplay"
    />
    <selectable
      title="Transporter"
      v-model="details.transporter"
      :options="transporters"
      :state="state"
    />
    <div class="col-span-2 grid grid-cols-2 gap-4">
      <selectable
        v-if="prefContract != null"
        title="Destination"
        v-model="toLocation"
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
    <div class="col-span-2" v-if="branches.length > 0">
      <selectable
        title="Branch"
        v-model="details.branch"
        :options="branches"
        :state="state"
      />
    </div>
    <editable
      title="Driver Name"
      v-model="details.driverName"
      :state="state"
      autocomplete="name"
    />
    <editable
      title="Number Plate"
      v-model="details.numberPlate"
      :state="state"
      autocomplete="address-line1"
    />
    <editable
      title="Phone Number"
      v-model="details.phoneNumber"
      :state="state"
      autocomplete="tel"
    />
    <editable
      title="Drivers License"
      v-model="details.passportNumber"
      :state="state"
      autocomplete="address-line2"
    />
    <editable
      title="Trailer Number"
      v-model="details.trailerNumber"
      :state="state"
      autocomplete="address-line3"
    />
    <editable
      title="LPO number"
      v-model="details.lpo"
      :state="state"
    />
    <editable
      title="Description"
      v-model="details.description"
      :state="state"
    />
    <editable
      v-if="isGSL"
      title="Loading Authority Number"
      v-model="details.loadingAuthorityNumber"
      :state="state"
    />
  </details-view>
</div>

</template>
<script>
import DetailsView from "../components/DetailsView.vue";
import Editable from "../components/Editable.vue";
import Selectable from "../components/Selectable.vue";
import { BaseService } from "../plugins/base-service";
import {
  Booking,
  LocationsApi,
  TpisApi,
  Transporter,
} from "../plugins/api-classes";
export default {
  components: { Selectable, DetailsView, Editable },
  async mounted() {
    const headers = await this.authHeaders();
    this.api = new Booking(headers);
    const transporterApi = new Transporter(headers);
    const tpisApi = new TpisApi(headers);
    this.locationsApi = new LocationsApi(headers);
    const metaApi = new BaseService("meta", headers);
    await metaApi.get("PrefContract:" + this.getOfficeId()).then(response => this.prefContract = response != null ? response['value'] : null);
    if(this.prefContract != null){
      this.locations = await this.locationsApi.getAll()
    }
    this.transporters = await transporterApi.getAll();
    this.transporters = [{name: "Other", id: 0}, ...this.transporters]
    this.tpis = await tpisApi.get("local");
    this.state.initialized = true;
  },
  computed: {
    isGSL: () => import.meta.env.VITE_GSL_SITE == "true",
  },
  data() {
    return {
      tpis: [],
      tpi: null,
      toLocation: null,
      transporters: [],
      prefContract: null,
      state: {
        edit: true,
        loading: false,
        loaded: true,
        initialized: false,
        related: false,
        errors: [],
      },
      api: null,
      locationsApi: null,
      branches: [],
      locations: [],
      details: {
        id: 0,
      },
    };
  },
  inject: ["authStringHeader", "authHeaders","getOfficeId"],
  watch: {
    async tpi(tpi, tpi2) {
      const location = tpi?.toLocation;
      this.details.transportInstruction = tpi;
      this.branches = [];
      if (location?.id != null && location?.id != 0) {
        const res = await this.locationsApi.get(location.id);
        this.branches = res.branches ?? [];
      } else this.branches = [];
    },
  },
  methods: {
    contractDisplay(contract) {
      return contract?.toLocation?.name
       ? `${contract?.product?.name ?? "_"}: 
      ${contract?.contract?.contractNumber ?? "_"}
      (to: ${contract?.toLocation?.name ?? "_"})`
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
      this.$router.push("/transit");
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
      if(this.details.transportInstruction && this.toLocation != null) this.details.transportInstruction.toLocation = this.toLocation
      return this.validate() && (await this.api.create(this.details));
    },
  },
};
</script>