<template>
  <div>
    <div class="flex justify-between">
      <div>
        <h2>Truck Database</h2>
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
            Search Trucks
          </button>
          <router-link
            to="clienttrucks"
            class="hidden lg:block ml-2 px-6 py-2 bg-accent text-white rounded hover:bg-primary"
          >
            Client Trucks
          </router-link>
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
      <data-table :headings="headings" :rows="filtered()" :bgColor="diffColor" :color="isDispatched">
        <template v-slot:heading-filter>
          <button @click="switchFilter()" class="text-center w-full">
            {{ filter }}
          </button>
        </template>
        <template v-slot:filter="{ item }">
          {{ isDispatched(item) ? "⬆Disp" : "⬇Rec" }}
        </template>
        <template v-slot:view="{ item }">
          <button @click="viewDetails(item)">
            <svg
              xmlns="http://www.w3.org/2000/svg"
              class="h-3 w-3"
              fill="none"
              viewBox="0 0 24 24"
              stroke="currentColor"
            >
              <path
                stroke-linecap="round"
                stroke-linejoin="round"
                stroke-width="2"
                d="M15 12a3 3 0 11-6 0 3 3 0 016 0z"
              />
              <path
                stroke-linecap="round"
                stroke-linejoin="round"
                stroke-width="2"
                d="M2.458 12C3.732 7.943 7.523 5 12 5c4.478 0 8.268 2.943 9.542 7-1.274 4.057-5.064 7-9.542 7-4.477 0-8.268-2.943-9.542-7z"
              />
            </svg>
          </button>
        </template>
        <template v-slot:product="{ item }">
          <div style="max-width: 150px; overflow: clip" class="mr-2">
            {{ item.product?.name }}
          </div>
        </template>
        <template v-slot:dnett="{ item }">
          {{
            item.initialWeight.gross != 0 && item.initialWeight.tare != 0
              ? item.initialWeight.gross - item.initialWeight.tare
              : 0
          }}
        </template>
        <template v-slot:rnett="{ item }">
          {{
            item.receivalWeight.gross != 0 && item.receivalWeight.tare != 0
              ? item.receivalWeight.gross - item.receivalWeight.tare
              : 0
          }}
        </template>
        <template v-slot:nett="{ item }">
          {{
            weight(item).gross != 0 ? weight(item).gross - weight(item).tare : 0
          }}
        </template>
        <template v-slot:1stdiff="{ item }">
          {{ item.firstWeight != null && item.receivalWeight.gross != 0 ? item.firstWeight.gross - item.receivalWeight.gross : "-" }}
        </template>
        <template v-slot:createdAt="{ item }">
          {{
            item.initialWeight?.gross == 0
              ? new Date(item.booking.createdAt)
                  .toLocaleDateString("en-GB", {
                    day: "numeric",
                    month: "short",
                    year: "numeric",
                  })
                  .split(" ")
                  .join("-")
              : item.initialWeight?.gross != 0
              ? new Date(item.initialWeight.grossAt)
                  .toLocaleDateString("en-GB", {
                    day: "numeric",
                    month: "short",
                    year: "numeric",
                  })
                  .split(" ")
                  .join("-")
              : ""
          }}
        </template>
        <template v-slot:action="{ item }">
          <button
            v-if="isServer"
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
            v-else
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
    </div>
    <Modal :showing="detailed" @close="detailed = null">
      <dispatch-details :dispatch="detailed" />
    </Modal>
    <Ticket :dispatch="printedDispatch" :toggle="closePrint" />
  </div>
</template>

<script>
import Ticket from "../components/Ticket.vue";
import DataTable from "../components/DataTable.vue";
import Filters from "../components/Filters.vue";
import { mapState } from "vuex";
import DispatchDetails from "../components/DispatchDetails.vue";
import Modal from "../components/Modal.vue";
import CrmLink from "../components/CrmLink.vue";
import print from "vue3-print-nb";
import { BaseService } from '../plugins/base-service';

export default {
  directives: {
    print,
  },
  components: {
    Ticket,
    DataTable,
    Filters,
    CrmLink,
    DispatchDetails,
    Modal,
  },
  async mounted() {
    this.$store.commit("dispatches/setCurrentQuery", "getProcessed");
    this.$store.dispatch("dispatches/getProcessed");
    this.searchApi = new BaseService('dispatches/search', await this.authHeaders())
  },
  data() {
    return {
      show: false,
      searchTerm: "",
      dispatch: null,
      printedDispatch: null,
      lastSearchTerm: "",
      searchResult: [],
      detailed: null,
      filter: "↕",
      searchApi: null,
      headings: [
        { title: "↕", value: "filter", slotname: "filter" },
        {
          title: "Num. Plate",
          value: "numberPlate",
          slotname: "numberPlate",
          sub: "booking",
        },
        {
          title: "Driver",
          headClass: "pr-2",
          value: "driverName",
          slotname: "driverName",
          sub: "booking",
        },
        {
          title: "From",
          headClass: "pr-2",
          value: "name",
          slotname: "name",
          sub: "fromOffice",
        },
        {
          title: "To",
          headClass: "pr-2",
          value: "name",
          slotname: "name",
          sub: "toOffice",
        },
        {
          title: "Product",
          headClass: "pr-2",
          value: "name",
          slotname: "product",
          sub: "product",
          class: "font-bold",
        },
        {
          title: "Dispatched On",
          isDate: true,
          headClass: "pr-2",
          slotname: "createdAt",
          value: "grossAt",
          sub: "initialWeight",
        },
        {
          title: "Dis. Tare",
          headClass: "pl-2",
          sub: "initialWeight",
          value: "tare",
          slotname: "tare",
          class: "text-right text-purple-700",
        },
        {
          title: "Dis. Gross",
          headClass: "pl-2",
          sub: "initialWeight",
          value: "gross",
          slotname: "gross",
          class: "text-right text-purple-700",
        },
        {
          title: "Dis. Nett",
          value: "nett",
          slotname: "dnett",
          sub: "initialWeight",
          class: "font-bold text-right text-purple-700",
        },
        {
          title: "Rec. Gross",
          headClass: "pl-2",
          sub: "receivalWeight",
          value: "gross",
          slotname: "gross",
          class: "text-right text-purple-700",
        },
        {
          title: "Rec. Tare",
          headClass: "pl-2",
          sub: "receivalWeight",
          value: "tare",
          slotname: "tare",
          class: "text-right text-purple-700",
        },
        {
          title: "Rec. Nett",
          sub: "receivalWeight",
          value: "nett",
          slotname: "rnett",
          class: "font-bold text-right text-purple-700",
        },
        {
          title: "Diff",
          sub: "diff",
          slotname: "diff",
          value: "value",
          class: "text-right",
        },
        {
          title: "1st Gross Diff",
          sub: "firstWeight",
          slotname: "1stdiff",
          class: "font-bold text-right text-purple-700",
        },
        {
          title: "+/-",
          slotname: "percentage",
          sub: "diff",
          value: "percentage",
          class: "text-right",
        },
        { title: "↕", value: "view", slotname: "view" },
        { title: "", value: "action", slotname: "action" },
      ],
    };
  },
  computed: mapState({
    processed: (state) => state.dispatches.processed,
    isServer: () => import.meta.env.VITE_SERVER == "true",
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
      if(this.searchTerm.length > 2) {
        this.searchQuery();
        return this.searchResult
      }
      if (this.filter == "↕") {
        return this.processed.filter(this.search);
      }
      if (this.filter == "⬆") {
        return this.processed
          .filter(this.search)
          .filter((d) => this.isDispatched(d));
      }
      if (this.filter == "⬇") {
        return this.processed
          .filter(this.search)
          .filter((d) => !this.isDispatched(d));
      }
    },
    async searchQuery(){
      if(this.lastSearchTerm != this.searchTerm){
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
    isDispatched(dispatch) {
      return dispatch != null
        ? dispatch.fromOffice?.id == this.getOfficeId()
        : false;
    },
    diffColor(dispatch) {
      if(dispatch.booking.vehicleType == "Wagon" && dispatch.booking.trailerNumber != null){
        return dispatch.booking.trailerNumber % 2 ? 
          (this.isDispatched(dispatch) ? 'bg-green-100' : 'bg-blue-100')
          : (this.isDispatched(dispatch) ? 'bg-green-200' : 'bg-blue-200')
      }
      if(dispatch.diff != "-" && parseFloat(dispatch.diff.value) < -0.25) 
        return 'bg-red-100'
      return null
    },
    weight(dispatch) {
      return this.isDispatched(dispatch)
        ? dispatch.initialWeight
        : dispatch.receivalWeight;
    },
    viewDetails(dispatch) {
      this.detailed = dispatch;
    },
  },
  inject: ["getOfficeId","authHeaders"],
};
</script>

<style scoped>
.td {
  @apply z-20 sticky font-semibold text-gray-700 p-2;
}
</style>