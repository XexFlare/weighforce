<template>
  <div>
    <div v-if="isAuthenticated.value">
      <div class="lg:flex justify-between mt-4 items-start">
        <div class="items-center flex flex-col justify-center">
          <h2 class="flex flex-col text-2xl font-bold text-primary">Main Menu</h2>
          <div v-if="inRoles(['operator'])" class="flex">
            <router-link class="nav-link" to="/transit"
              >Trucks in Transit</router-link
            >
          </div>
          <div class="mt-4 flex">
            <router-link class="nav-link" to="/trucks"
              >Vehicle Database</router-link
            >
          </div>
          <div v-if="inRoles(['dispatch', 'manager'])" class="mt-4 flex">
            <router-link class="nav-link" to="/booking">Book Truck</router-link>
          </div>
          <div v-if="inRoles(['dispatch', 'manager'])" class="mt-4 flex">
            <router-link class="nav-link" to="/receival"
              >Receive Truck</router-link
            >
          </div>
          <div v-if="inRoles(['link'])" class="mt-4 flex">
            <router-link class="nav-link" to="/link"
              >Link Wagons</router-link
            >
          </div>
          <div v-if="inRoles(['link','manager'])" class="mt-4 flex">
            <router-link class="nav-link" to="/speed"
              >Wagon Speed Alert</router-link
            >
          </div>
        </div>
        <div v-if="dash" class="hidden lg:!block">
          <div class="lg:flex justify-between">
            <chart-2 title="Dispatched per Day" seriesName="Tons Dispatched" :chart="dash.dispatches2" />
            <chart-2 title="Received per Day" seriesName="Tons Received" :chart="dash.receivals2" />
          </div>
          <div class="lg:flex my-2 justify-center">
            <div>
              <label for="fromDate" class="pr-4">From</label>
              <input type="date" :value="fromDate" @input="changeFrom($event)" />
            </div>
            <div>
              <label for="toDate" class="px-4">To</label>
              <input type="date" :value="toDate" @input="changeTo($event)" />
            </div>
          </div>
          <div class="lg:flex justify-between">
            <pie-chart type="Dispatched" :chart="dash.topDispatches" />
            <radial-bar type="Received" :chart="dash.topReceivals" />
          </div>
        </div>
      </div>
      <hr class="bg-accent mt-2" />
      <div class="flex mt-4">
        <img :src="logo.value" alt="" style="height: 4rem" />
        <img
          src="../assets/meridian.jpg"
          alt=""
          style="height: 4rem; margin-left: 2rem"
        />
      </div>
    </div>
    <div v-else>
      <h2 class="flex flex-col text-2xl font-bold text-primary">WeightForce</h2>
      <div class="mt-4 flex">
        <router-link class="nav-link w" to="/authentication/login"
          >Login</router-link
        >
      </div>
    </div>
  </div>
</template>
<style scoped>
.nav-link {
  @apply w-32 text-center px-2 py-2 bg-primary text-gray-200 rounded-md transition duration-500 ease-in-out;
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
.bt-menu li {
  @apply px-1;
}
.bt-menu li:after {
  padding: 1em;
  content: "|";
}
.bt-menu li:last-child:after {
  content: "";
}
</style>
<script>
import Chart from "../components/Chart.vue";
import Tab from "../components/Tab.vue";
import PieChart from "../components/PieChart.vue";
import RadialBar from "../components/RadialBar.vue";
import ColumnCharts from "../components/ColumnCharts.vue";
import Chart2 from '../components/Chart2.vue';

export default {
  inject: ["isAuthenticated", "user", "authHeaders","logo"],
  components: { Chart, PieChart, Tab, RadialBar, ColumnCharts, Chart2 },
  async mounted() {
    this.dash = await fetch(
      `${import.meta.env.VITE_API}/api/dispatches/dash`,
      await this.authHeaders()
    )
      .then((data) => data.json())
      .then((dash) => {
        return {
          topDispatches: {
            labels: dash.topDispatches.map((d) => d.name),
            series: dash.topDispatches.map((d) => d.value),
          },
          topReceivals: {
            labels: dash.topReceivals.map((d) => d.name),
            series: dash.topReceivals.map((d) => d.value),
          },
          dispatches2: {
            labels: dash.dailyDispatches.map((d) => d.name),
            series: [
              {
                name: "Trucks",
                type: "column",
                data: dash.dailyDispatches.map((d) => d.value)
              },
              {
                name: "Tons",
                type: "column",
                data: dash.dailyDispatches.map((d) => d.sub)
              }
            ]
          },
          receivals2: {
            labels: dash.dailyReceivals.map((d) => d.name),
            series: [
              {
                name: "Trucks",
                type: "column",
                data: dash.dailyReceivals.map((d) => d.value)
              },
              {
                name: "Tons",
                type: "column",
                data: dash.dailyReceivals.map((d) => d.sub)
              }
            ]
          },
          dispatches: {
            labels: dash.dailyDispatches.map((d) => d.name),
            series: dash.dailyDispatches.map((d) => d.value),
          },
          receivals: {
            labels: dash.dailyReceivals.map((d) => d.name),
            series: dash.dailyReceivals.map((d) => d.value),
          },
          dispatchedTons: {
            labels: dash.dailyDispatches.map((d) => d.name),
            series: dash.dailyDispatches.map((d) => d.sub),
          },
          receivedTons: {
            labels: dash.dailyReceivals.map((d) => d.name),
            series: dash.dailyReceivals.map((d) => d.sub),
          },
        };
      })
      .catch((error) => console.error("Unable to get chart.", error));
  },
  data() {
    const date = new Date();
    return {
      selected: "Received",
      dash: null,
      from: date,
      to: date
    };
  },
  computed: {
    fromDate() { return this.from.toISOString().substr(0, 10) },
    toDate () { return this.to.toISOString().substr(0, 10) }
  },
  methods: {
    changeFrom(event) {
      this.from = new Date(event.target.value)
      this.fetchDashes()
    },
    changeTo(event) {
      this.to = new Date(event.target.value)
      this.fetchDashes()
    },
    async fetchDashes() {
      this.dash = await fetch(`${import.meta.env.VITE_API}/api/dispatches/dash?from=${this.fromDate}&to=${this.toDate}`,
      await this.authHeaders()
    )
      .then((data) => data.json())
      .then((dash) => {
        return {
          ...this.dash,
          topDispatches: {
            labels: dash.topDispatches.map((d) => d.name + " " + d.value),
            series: dash.topDispatches.map((d) => d.value),
          },
          topReceivals: {
            labels: dash.topReceivals.map((d) => d.name),
            series: dash.topReceivals.map((d) => d.value),
          }
        };
      })
      .catch((error) => console.error("Unable to get chart.", error));
      // console.log(this.dash);
    },
    inRoles(roleList) {
      if (roleList == null) return true;
      const roles = roleList.map((role) => role.toLowerCase());
      return this.user.value.roles.some((role) =>
        roles.includes(role.toLowerCase())
      );
    },
    setSelected(tab) {
      console.log("Setting tab to ", tab);
      this.selected = tab;
    },
  },
};
</script>
