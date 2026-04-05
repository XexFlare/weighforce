<template>
  <div style="width: 1000px; margin: 0 auto 0 auto" class="">
    <div v-if="chart">
      <column-charts :chart="chart" />
    </div>
    <h2 class="mt-6">Reports</h2>
    <div class="mt-2 mb-4 flex">
      <router-link class="nav-link mr-4" to="/reports/daily-dispatches"
        >Overweights</router-link
      >
      <router-link class="nav-link mr-4" to="/reports/weekly-dispatches"
        >Underweights</router-link
      >
      <router-link class="nav-link mr-4" to="/reports/Shortages"
        >Shortage Report</router-link
      >
    </div>
    <report-download />
    <h2 class="mt-6">Dispatch Summary</h2>
    <div class="mt-2 mb-4 flex">
      <router-link class="nav-link mr-4" to="/reports/daily-dispatches"
        >Daily</router-link
      >
      <router-link class="nav-link mr-4" to="/reports/weekly-dispatches"
        >Weekly</router-link
      >
      <router-link class="nav-link" to="/reports/annual-dispatches"
        >Annual</router-link
      >
    </div>
    <hr />
    <h2 class="mt-6">Receival Summary</h2>
    <div class="mt-2 mb-4 flex">
      <router-link class="nav-link mr-4" to="/reports/daily-receivals"
        >Daily</router-link
      >
      <router-link class="nav-link mr-4" to="/reports/weekly-receivals"
        >Weekly</router-link
      >
      <router-link class="nav-link" to="/reports/annual-receivals"
        >Annual</router-link
      >
    </div>
    <hr />
    <div class="mt-6">
      <router-link class="nav-link" to="/reports/osr"
        >Liwonde Rail to OSR Report</router-link
      >
    </div>
    <div class="mt-6">
      <router-link class="nav-link" to="/reports/limits"
        >Train Receival Report</router-link
      >
    </div>
  </div>
</template>

<script>
import ColumnCharts from "../components/ColumnCharts.vue";
import ReportDownload from '../components/ReportDownload.vue';
export default {
  components: { ColumnCharts, ReportDownload },
  inject: ["authHeaders"],
  async mounted() {
    this.chart = await fetch(
      `${import.meta.env.VITE_API}/api/dispatches/report`,
      await this.authHeaders()
    )
      .then((data) => data.json())
      .catch((error) => console.error("Unable to get chart.", error));
    console.log(this.chart);
  },
  data() {
    return {
      chart: null,
    };
  },
};
</script>

<style scoped>
.nav-link {
  @apply w-40 text-center px-3 py-2 bg-primary font-medium text-gray-200 rounded-md transition duration-500 ease-in-out;
}
.router-link-active,
.nav-link:hover {
  @apply bg-accent text-white;
}
.nav-link:focus {
  @apply outline-none bg-accent;
}
nav:after {
  content: "";
  position: absolute;
  top: 60px;
  left: 0;
  border-style: solid;
  border-width: 45px 0px 0px 100vw;
  border-color: #4d7d77 transparent transparent transparent;
}
</style>