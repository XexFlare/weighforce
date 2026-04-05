<template>
  <div>
    <div class="flex justify-between">
      <div>
        <h2>Wagons Speed Alerts</h2>
        <div class="flex flex-row">
          <input type="text" v-model="searchTerm" class="
              px-4
              py-1
              text-gray-600
              border-2
              rounded
              focus:outline-none
              focus:shadow-none
              lg:block
            " placeholder="Search ..." />
        </div>
      </div>
    </div>
    <filters />

    <div class="
        w-full
        lg:flex
        lg:justify-center
        overflow-y-auto
        scrollbar-w-2
        scrollbar-track-gray-lighter
        scrollbar-thumb-rounded
        scrollbar-thumb-gray
        scrolling-touch
      ">
      <link-table :headings="headings" :rows="filtered()" :bgColor="diffColor" :color="isDispatched" :onSelect="onSelect">
        <template v-slot:view="{ item }">
          <button @click="viewDetails(item)">
            <svg xmlns="http://www.w3.org/2000/svg" class="h-3 w-3" fill="none" viewBox="0 0 24 24" stroke="currentColor">
              <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2"
                d="M15 12a3 3 0 11-6 0 3 3 0 016 0z" />
              <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2"
                d="M2.458 12C3.732 7.943 7.523 5 12 5c4.478 0 8.268 2.943 9.542 7-1.274 4.057-5.064 7-9.542 7-4.477 0-8.268-2.943-9.542-7z" />
            </svg>
          </button>
        </template>
        <template v-slot:createdAt="{ item }">
          {{ formatDate(item.booking.createdAt) }}
        </template>
      </link-table>
    </div>
  </div>
</template>

<script>
import Ticket from "../components/Ticket.vue";
import LinkTable from "../components/LinkTable.vue";
import DispatchDetails from "../components/DispatchDetails.vue";
import { mapState } from "vuex";
import { BaseService } from '../plugins/base-service';
import Filters from "../components/Filters.vue";

export default {
  components: {
    Ticket,
    LinkTable,
    DispatchDetails,
    Filters
  },
  inject: ["authHeaders", "getOfficeId", "currentLocation"],
  async mounted() {
    this.$store.commit("dispatches/setCurrentQuery", "getWagons");
    this.$store.dispatch("dispatches/getWagons");
    this.searchApi = new BaseService('dispatches/search', await this.authHeaders())
  },
  data() {
    return {
      showWeigh: false,
      showAssign: false,
      searchTerm: "",
      showSelected: false,
      lastSearchTerm: "",
      searchResult: [],
      bookings: [],
      selected: null,
      clicked: null,
      printedDispatch: null,
      detailed: null,
      filter: "↕",
      headings: [
        {
          title: "Train",
          value: "trailerNumber",
          sub: "booking",
          slotname: "train",
        },
        {
          title: "Vehicle #",
          value: "wagonNo",
          sub: "booking",
        },
        {
          title: "Vehicle Type",
          sub: "booking",
          value: "vehicleType",
        },
        {
          title: "Speed",
          sub: "booking",
          value: "speed",
        },
        {
          title: "Date",
          isDate: true,
          headClass: "pr-2",
          value: "createdAt",
          sub: "booking",
          slotname: "createdAt",
        },
      ],
    };
  },
  computed: mapState({
    dispatches: (state) => state.dispatches.inTransit,
    isServer: () => import.meta.env.VITE_SERVER == "true",
  }),
  methods: {
    formatDate(date){
      var data = new Date(date)
        .toLocaleDateString("en-GB", {
          day: "numeric",
          month: "short",
          year: "numeric",
          hour: "numeric",
          minute: "numeric",
        })
        .split(" ");
      data[2] = data[2].split(",")[0]
      return `${data[0]}-${data[1]}-${data[2]} ${data[3]}`
    },
    onSelect(item) {
      this.selected = item
      this.showSelected = true
    },  
    closeSelected(){
      this.selected = null
      this.showSelected = false
    },
    canWeigh(item) {
      if (item.vehicleType == 'Truck' && this.currentLocation.value.id == item.booking.transportInstruction.fromLocation.id && this.isDispatching(item)) {
        return true
      }
      if (item.vehicleType == 'Truck' && this.currentLocation.value.id == item.booking.transportInstruction.toLocation.id && this.isReceiving(item)) {
        return true
      }
    },
    canPrint(item) {
      return item.vehicleType != 'Wagon' && this.isServer && !this.isDispatching(item)
        || item.vehicleType == 'Wagon'
    },
    isDispatching(item) {
      return item.initialWeight.gross == 0 && item.booking.tempTicketNumber == null
    },
    isReceiving(item) {
      return item.initialWeight.gross > 0 && item.booking.tempTicketNumber == null && item.receivalWeight.tare == 0
    },
    print(dispatch) {
      this.printedDispatch = dispatch;
      this.$store.dispatch("dispatches/print", dispatch);
    },
    closePrint() {
      this.printedDispatch = null;
    },
    switchFilter() {
      if (this.filter == "↕") {
        return (this.filter = "⬆");
      }
      if (this.filter == "⬆") {
        return (this.filter = "⬇");
      }
      if (this.filter == "⬇") {
        return (this.filter = "↕");
      }
    },
    filtered() {
      if (this.searchTerm.length > 2) {
        this.searchQuery();
        return this.searchResult
      }
      if (this.filter == "↕") {
        return this.dispatches?.filter(this.search);
      }
      if (this.filter == "⬆") {
        return this.dispatches
          ?.filter(this.search)
          .filter((d) => this.isDispatched(d));
      }
      if (this.filter == "⬇") {
        return this.dispatches
          ?.filter(this.search)
          .filter((d) => !this.isDispatched(d));
      }
    },
    async searchQuery() {
      if (this.lastSearchTerm != this.searchTerm) {
        this.searchResult = await this.searchApi.get(this.searchTerm);
        this.lastSearchTerm = this.searchTerm
      }
    },
    search(d) {
      return this.searchTerm == ""
        ? true
        : d.booking.numberPlate
          ?.replace(" ", "")
          .toLowerCase()
          .includes(this.searchTerm.toLowerCase().replace(" ", "")) ||
        d.initialWeight.ticketNumber
          ?.replace(" ", "")
          .toLowerCase()
          .includes(this.searchTerm.toLowerCase().replace(" ", "")) ||
        d.receivalWeight.ticketNumber
          ?.replace(" ", "")
          .toLowerCase()
          .includes(this.searchTerm.toLowerCase().replace(" ", "")) ||
        d.product.name
          ?.replace(" ", "")
          .toLowerCase()
          .includes(this.searchTerm.toLowerCase().replace(" ", ""));
    },
    isBooking(item) {
      return item != null
        ? item.status == "Booking" && item.initialWeight.tare == 0
        : false;
    },
    isPending(item) {
      return item != null ? item.status == "Held" : false;
    },
    isDispatched(dispatch) {
      return dispatch != null
        ? dispatch?.fromOffice?.id == this.getOfficeId()
        : false;
    },
    diffColor(dispatch) {
      if (dispatch.diff == "-") return null
      return parseFloat(dispatch.diff.value) < -0.25 ? 'bg-red-100' : null
    },
    weight(item) {
      return this.isDispatched(item) || this.isBooking(item)
        ? item?.initialWeight
        : item?.receivalWeight;
    },
    toggleWeigh(item) {
      console.log('123');
      if (item == null) {
        this.selected = null;
      } else this.selected = item;
      this.showWeigh = !this.showWeigh;
    },
    toggleAssign(item) {
      if (item == null) {
        this.selected = null;
      } else this.selected = item;
      this.showAssign = !this.showAssign;
    },
    updateDispatch(weightedDispatch) {
      if (weightedDispatch.status == "Discarded") {
        this.$store.commit("dispatches/removeDispatch", weightedDispatch);
        this.toggleAssign(null);
      } else if (weightedDispatch.vehicleType == "Wagon") {
        weightedDispatch.id = this.selected.id;
        this.$store.commit("dispatches/updateDispatch", weightedDispatch);
        this.toggleAssign(null);
      } else {
        this.$store.commit("dispatches/updateDispatch", weightedDispatch);
        this.toggleWeigh(null);
      }
      this.closeSelected()
    },
    viewDetails(dispatch) {
      this.detailed = dispatch;
    },
  },
};
</script>

<style scoped>
.td {
  @apply z-20 sticky font-semibold text-gray-700;
}
</style>