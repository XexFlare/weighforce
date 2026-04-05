<template>
  <div style="width: 1000px; margin: 0 auto 0 auto">
    <h1>Shortage Report</h1>
    <div class="flex p-4 my-2 bg-gray-100 w-max rounded">
      <div class="">
        <label for="date" class="pr-2 font-bold">From: </label>
        <input type="date" :value="start" @input="changeStartDate($event)" />
      </div>
      <div class="">
        <label for="date" class="pr-2 font-bold">To: </label>
        <input type="date" :value="end" @input="changeEndDate($event)" />
      </div>
      <div class="ml-4">
        <label for="location" class="pr-2 font-bold">Location: </label>
        <select
          :value="location != null ? location.id : ''"
          @input="
            changeLocation(
              userLocations.find((v) => v.id == $event.target.value)
            )
          "
          class="w-60"
        >
          <option value="">All</option>
          <option
            v-for="option in userLocations"
            :key="option.id"
            :value="option.id"
          >
            {{ option.name }}
          </option>
        </select>
      </div>
    </div>
    <div v-for="location in locations" :key="location.name" class="w-max mb-8">
      <h2>{{ location.name }}</h2>
      <h3>
        Trucks Received: <b>{{ location.total }}</b>
      </h3>
      <div
        v-for="summary in location.summary"
        :key="summary.product"
        class="p-2 border border-primary border-2 m-2 hover:bg-primary hover:text-white hover:cursor-pointer"
        @click="toggleDetails(summary)"
      >
        <p>
          {{ summary.product ?? "Unknown" }} -
          <b>{{ (summary.amount / 1000).toFixed(2) }} Tonnes</b>
        </p>
      </div>
      <h3 v-if="location.summary.length">Total: <b>{{(location.summary.reduce((a,b) => a + b['amount'], 0) / 1000).toFixed(2) }} Tonnes</b></h3>
    </div>
      <!-- TODO: refactor: get totals from api -->
    <h3 class="mb-8">Total: <b>{{(locations.reduce((a,b) => a + b.summary.reduce((a,b) => a + b['amount'], 0), 0) / 1000).toFixed(2) }} Tonnes</b></h3>
    <modal :showing="showDetails" @close="toggleDetails" :showClose="true" :fw="true">
      <div class="p-4">
        <h3 class="font-bold text-lg">{{details.product}} Shortages</h3>
        <data-table :rows="details.dispatches" :headings="headings" />
      </div>
    </modal>
  </div>
</template>
<script>
import DataTable from '../components/DataTable.vue';
import Modal from "../components/Modal.vue";

export default
  {
  inject: ["authHeaders", "user"],
  components: { Modal, DataTable },
  async mounted() {
    this.userLocations = this.user.value.locations;
    this.fetchReport();
  },
  data() {
    const date = new Date();
    date.setDate(date.getDate() - 1);
    return {
      startDate: date,
      endDate: new Date(),
      locations: [],
      userLocations: [],
      location: {},
      showDetails: false,
      details: {},
      headings: [
        {
          title: "Driver",
          value: "driver"
        },
        {
          title: "Number Plate",
          value: "numberPlate"
        },
        {
          title: "Dispatched",
          value: "dispatched"
        },
        {
          title: "Received",
          value: "received"
        },
        {
          title: "Diff",
          value: "diff"
        },
        {
          title: "Dispatched By",
          value: "dispatchedBy"
        },
        {
          title: "Received By",
          value: "receivedBy"
        },
        {
          title: "Dispatched On",
          isDate: true,
          value: "dispatchedOn"
        },
        {
          title: "Received On",
          isDate: true,
          value: "receivedOn"
        },
      ]
    };
  },
  computed: {
    start() {
      return this.startDate.toISOString().substr(0, 10);
    },
    end() {
      return this.endDate.toISOString().substr(0, 10);
    },
  },
  methods: {
    toggleDetails(details) {
      this.details = details
      this.showDetails = !this.showDetails
    },
    formatDate(date) {
      return date
        .toLocaleDateString("en-GB", {
          day: "numeric",
          month: "long",
          year: "numeric",
        })
        .split(" ")
        .join(" ");
    },
    async fetchReport() {
      const locationQuery =
        this.location.id != null ? "&officeId=" + this.location.id : "";
      this.locations = await fetch(
        import.meta.env.VITE_API +
          "/api/reports/shortages?date=" +
          this.startDate.toISOString().substr(0, 10) +
          "&toDate=" + this.endDate.toISOString().substr(0, 10) +
          locationQuery,
        await this.authHeaders()
      ).then((res) => res.json());
    },
    changeLocation(location) {
      this.location = location;
      this.fetchReport();
    },
    changeStartDate(event) {
      this.startDate = new Date(event.target.value);
      this.fetchReport();
    },
    changeEndDate(event) {
      this.endDate = new Date(event.target.value);
      this.fetchReport();
    },
  },
};
</script>