<template>
  <div style="width: 1000px; margin: 0 auto 0 auto">
    <h1>Annual Dispatch Summary</h1>
    <div class="flex p-4 my-2 bg-gray-100 w-max rounded">
      <div class="">
        <label for="date" class="pr-2 font-bold">Date: </label>
        <input type="date" :value="theDate" @input="changeDate($event)" />
      </div>
      <div class="ml-4">
        <label for="date" class="pr-2 font-bold">Location: </label>
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
    <h3>
      For <b>{{ formatDate(startDate) }}</b> to <b>{{ formatDate(endDate) }}</b>
    </h3>
    <div v-for="location in locations" :key="location.name" class="w-max mb-8">
      <h2>{{ location.name }}</h2>
      <h3>
        Trucks Dispatched: <b>{{ location.total }}</b>
      </h3>
      <div
        v-for="summary in location.summary"
        :key="summary.product"
        class="p-2 border border-primary border-2 m-2"
      >
        <p>
          {{ summary.product ?? "Unknown" }} -
          <b>{{ (summary.amount / 1000).toFixed(2) }} Tonnes</b>
        </p>
      </div>
      <div v-if="location.total > 0">
        Synced to Kinetic:
        <span
          :class="
            location.total > location.synced ? 'text-red-400' : 'text-green-400'
          "
          ><b>{{ location.synced }}</b></span
        >
      </div>
      <h3 v-if="location.summary.length">Total: <b>{{(location.summary.reduce((a,b) => a + b['amount'], 0) / 1000).toFixed(2) }} Tonnes</b></h3>
    </div>
      <!-- TODO: refactor: get totals from api -->
    <h3 class="mb-8">Total: <b>{{(locations.reduce((a,b) => a + b.summary.reduce((a,b) => a + b['amount'], 0), 0) / 1000).toFixed(2) }} Tonnes</b></h3>
  </div>
</template>
<script>
export default {
  inject: ["authHeaders", "user"],
  async mounted() {
    this.userLocations = this.user.value.locations;
    this.fetchReport();
  },
  data() {
    const date = new Date();
    date.setFullYear(date.getFullYear() - 1);
    return {
      startDate: date,
      endDate: new Date(),
      locations: [],
      userLocations: [],
      location: {},
    };
  },
  computed: {
    theDate() {
      return this.startDate.toISOString().substr(0, 10);
    },
  },
  methods: {
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
          "/api/reports/annual-dispatches?date=" +
          this.endDate.toISOString().substr(0, 10) +
          locationQuery,
        await this.authHeaders()
      ).then((res) => res.json());
    },
    changeLocation(location) {
      this.location = location;
      this.fetchReport();
    },
    changeDate(event) {
      let date = new Date(event.target.value);
      date.setDate(date.getFullYear() - 1);
      this.startDate = new Date(event.target.value);
      this.endDate = date;
      this.fetchReport();
    },
  },
};
</script>