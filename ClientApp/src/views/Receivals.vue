<template>
  <div>
    <div class="flex justify-between">
      <div>
        <h2>Manual Receivals</h2>
        <div class="flex flex-row">
          <input
            type="text"
            v-model="searchTerm"
            class="
              px-4
              py-1
              text-gray-600
              border-2
              rounded
              focus:outline-none
              focus:shadow-none
              lg:block
            "
            placeholder="Search ..."
          />
          <button
            class="hidden lg:block ml-4 px-6 py-2 bg-accent text-white rounded hover:bg-primary"
          >
            Search Kinetic Trucks
          </button>
        </div>
      </div>
      <crm-link />
    </div>
    <filters />
    <div
      class="
        w-full
        flex
        justify-center
        overflow-y-auto
        scrollbar-w-2
        scrollbar-track-gray-lighter
        scrollbar-thumb-rounded
        scrollbar-thumb-gray
        scrolling-touch
      "
    >
      <data-table :headings="headings" :rows="filtered()" :color="isDispatched">
        <template v-slot:heading-filter>
          <button @click="switchFilter()" class="text-center w-full">
            {{ filter }}
          </button>
        </template>
        <template v-slot:rnett="{ item }">
          {{
            item.receivalWeight.gross != 0 && item.receivalWeight.tare != 0
              ? item.receivalWeight.gross - item.receivalWeight.tare
              : 0
          }}
        </template>
        <template v-slot:product="{ item }">
          <div style="max-width: 150px; overflow: clip" class="mr-2">
            {{ item.product.name }}
          </div>
        </template>
        <template v-slot:action="{ item }">
          <button
            v-if="
              item.vehicleType == 'Truck' &&
              ((isBooking(item) && weight(item).tare == 0) ||
                (!isBooking(item) &&
                  (weight(item).gross == 0 || weight(item).tare == 0)))
            "
            @click="toggleWeigh(item)"
            :class="
              weight(item).gross != 0 && weight(item).tare != 0
                ? 'bg-gray-200 text-gray-600 cursor-not-allowed'
                : 'bg-accent hover:bg-primary text-white'
            "
            class="py-1 px-2 rounded shadow"
          >
            Weigh
          </button>
          <button
            v-else-if="
              item.vehicleType == 'Wagon' && item.initialWeight.tare == 0
            "
            @click="toggleAssign(item)"
            class="
              py-1
              px-2
              rounded
              shadow
              bg-accent
              hover:bg-primary
              text-white
            "
          >
            Assign
          </button>
          <button
            v-else-if="item.vehicleType != 'Wagon' && isServer"
            @click="print(item)"
            class="
              py-1
              px-2
              rounded
              shadow
              bg-accent
              hover:bg-primary
              text-white
            "
          >
            Print
          </button>
          <button
            v-else-if="item.vehicleType != 'Wagon' && !isServer"
            v-print="'#ticket'"
            @click="print(item)"
            class="
              py-1
              px-2
              rounded
              shadow
              bg-accent
              hover:bg-primary
              text-white
            "
          >
            Print
          </button>
        </template>
      </data-table>
      <Ticket :dispatch="printedDispatch" :toggle="closePrint" />
      <Modal :showing="showWeigh" @close="toggleWeigh" :showClose="true">
        <Weight :dispatch="selected" :updateDispatch="updateDispatch" />
      </Modal>
      <Modal
        :showing="showAssign"
        @close="toggleAssign"
        :showClose="true"
        :xl="true"
      >
        <Assign :dispatch="selected" :updateDispatch="updateDispatch" />
      </Modal>
    </div>
  </div>
</template>

<script>
import Weight from "../components/Weight.vue";
import Assign from "../components/Assign.vue";
import Modal from "../components/Modal.vue";
import Ticket from "../components/Ticket.vue";
import DataTable from "../components/DataTable.vue";
import Filters from "../components/Filters.vue";
import CrmLink from "../components/CrmLink.vue";
import { mapState, mapActions } from "vuex";
import print from "vue3-print-nb";

export default {
  directives: {
    print,
  },
  components: {
    Modal,
    Assign,
    Weight,
    Ticket,
    DataTable,
    Filters,
    CrmLink,
  },
  inject: ["authHeaders", "getOfficeId"],
  mounted() {
    this.$store.commit("dispatches/setCurrentQuery", "getTemp");
    this.$store.dispatch("dispatches/getTemp");
  },
  data() {
    return {
      showWeigh: false,
      showAssign: false,
      searchTerm: "",
      bookings: [],
      selected: null,
      printedDispatch: null,
      filter: "↕",
      headings: [
        { title: "↕", value: "filter", slotname: "filter" },
        {
          title: "Num. Plate",
          value: "numberPlate",
          sub: "booking",
        },
        {
          title: "Driver",
          headClass: "pr-2",
          value: "driverName",
          sub: "booking",
        },
        {
          title: "From",
          headClass: "pr-2",
          value: "name",
          sub: "fromOffice",
        },
        {
          title: "To",
          headClass: "pr-2",
          value: "name",
          sub: "toOffice",
        },
        {
          title: "Product",
          headClass: "pr-2",
          value: "name",
          sub: "product",
          class: "font-bold",
          slotname: "product",
        },
        {
          title: "Rec. Gross",
          headClass: "pl-2",
          sub: "receivalWeight",
          value: "gross",
          class: "text-right text-purple-700",
        },
        {
          title: "Rec. Tare",
          headClass: "pl-2",
          sub: "receivalWeight",
          value: "tare",
          slotname: "rtare",
          class: "text-right text-purple-700",
        },
        {
          title: "Rec. Nett",
          sub: "receivalWeight",
          value: "rnett",
          slotname: "rnett",
          class: "font-bold text-right text-purple-700",
        },
        { title: "", value: "action", slotname: "action" },
      ],
    };
  },
  computed: mapState({
    isServer: () => import.meta.env.VITE_SERVER == "true",
    dispatches: (state) => state.dispatches.temp,
  }),
  methods: {
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
    search(d) {
      return this.searchTerm == ""
        ? true
        : d.booking.numberPlate
            ?.replace(" ", "")
            .toLowerCase()
            .includes(this.searchTerm.toLowerCase().replace(" ", "")) ||
            d.product.name
              ?.replace(" ", "")
              .toLowerCase()
              .includes(this.searchTerm.toLowerCase().replace(" ", ""));
    },
    isBooking(item) {
      return item != null ? item.status == "Booking" : false;
    },
    isPending(item) {
      return item != null ? item.status == "Held" : false;
    },
    isDispatched(dispatch) {
      return dispatch != null
        ? dispatch.fromOffice.id == this.getOfficeId()
        : false;
    },
    weight(item) {
      return this.isDispatched(item) || this.isBooking(item)
        ? item.initialWeight
        : item.receivalWeight;
    },
    toggleWeigh(item) {
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
      this.$store.commit("dispatches/updateTempDispatch", weightedDispatch);
      this.toggleWeigh(null);
    },
  },
};
</script>

<style scoped>
.td {
  @apply z-20 sticky font-semibold text-gray-700;
}
</style>