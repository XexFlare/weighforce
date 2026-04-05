<template>
  <div>
    <div class="flex justify-between">
      <div>
        <h2>Report</h2>
      </div>
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
      <data-table
        :headings="headings"
        :rows="filtered()"
        :color="isDispatched"
        defaultSort="5"
      >
        <template v-slot:heading-filter>
          <button @click="switchFilter()">{{ filter }}</button>
        </template>
        <template v-slot:dnett="{ item }">
          {{ item.initialWeight.gross != 0 ? item.initialWeight.gross : 0 }}
        </template>
        <template v-slot:rnett="{ item }">
          {{ item.receivalWeight.gross != 0 ? item.receivalWeight.gross : 0 }}
        </template>
        <template v-slot:diff="{ item }">
          {{ diff(item) }}
        </template>
        <template v-slot:percentage="{ item }">
          <span
            :class="percentage(item) > 0 ? 'text-green-800' : 'text-red-500'"
          >
            {{ percentage(item) }}%
          </span>
        </template>
      </data-table>
    </div>
  </div>
</template>

<script>
import DataTable from "../components/DataTable.vue";
import Filters from "../components/Filters.vue";
import { mapState } from "vuex";
export default {
  components: {
    DataTable,
    Filters,
  },
  created() {
    this.$store.commit("dispatches/setCurrentQuery", "getProcessed");
    this.$store.dispatch("dispatches/getProcessed");
  },
  data() {
    return {
      show: false,
      searchTerm: "",
      dispatch: null,
      headings: [
        {
          title: "Number Plate",
          value: "numberPlate",
          slotname: "numberPlate",
          sub: "booking",
        },
        {
          title: "Driver",
          headClass: "pr-6",
          value: "driverName",
          slotname: "driverName",
          sub: "booking",
        },
        {
          title: "From",
          headClass: "pr-6",
          value: "name",
          slotname: "name",
          sub: "fromOffice",
        },
        {
          title: "To",
          headClass: "pr-6",
          value: "name",
          slotname: "name",
          sub: "toOffice",
        },
        {
          title: "Product",
          headClass: "pr-6",
          value: "name",
          slotname: "name",
          sub: "product",
          class: "font-bold",
        },
        {
          title: "Dispatched On",
          isDate: true,
          headClass: "pr-6",
          value: "grossAt",
          sub: "initialWeight",
        },
        // {
        //   title: "Dis. Tare",
        //   headClass: "pl-6",
        //   sub: "initialWeight",
        //   value: "tare",
        //   slotname: "tare",
        //   class: "text-right text-purple-700",
        // },
        // {
        //   title: "Dis. Gross",
        //   headClass: "pl-6",
        //   sub: "initialWeight",
        //   value: "gross",
        //   slotname: "gross",
        //   class: "text-right text-purple-700",
        // },
        {
          title: "Dis. Gross",
          slotname: "dnett",
          sub: "initialWeight",
          class: "font-bold text-right text-purple-700",
        },
        // {
        //   title: "Rec. Gross",
        //   headClass: "pl-6",
        //   sub: "receivalWeight",
        //   value: "gross",
        //   slotname: "gross",
        //   class: "text-right text-purple-700",
        // },
        // {
        //   title: "Rec. Tare",
        //   headClass: "pl-6",
        //   sub: "receivalWeight",
        //   value: "tare",
        //   slotname: "tare",
        //   class: "text-right text-purple-700",
        // },
        {
          title: "Rec. Gross",
          sub: "receivalWeight",
          value: "nett",
          slotname: "rnett",
          class: "font-bold text-right text-purple-700",
        },
        {
          title: "Diff",
          sub: "receivalWeight",
          value: "diff",
          slotname: "diff",
          class: "font-bold text-right text-purple-700",
        },
        {
          title: "+/-",
          sub: "receivalWeight",
          value: "percentage",
          slotname: "percentage",
          class: "font-bold text-right",
        },
      ],
    };
  },
  computed: {
    ...mapState({
      processed: (state) => state.dispatches.processed,
    }),
  },
  methods: {
    diff(item) {
      const rnett = this.rnet(item);
      const dnett = this.dnet(item);
      return rnett != 0 && dnett != 0 ? rnett - dnett : 0;
    },
    dnet(item) {
      return item.initialWeight.gross ? item.initialWeight.gross : 0;
    },
    rnet(item) {
      return item.receivalWeight.gross ? item.receivalWeight.gross : 0;
    },
    percentage(item) {
      const dif = this.diff(item);
      return dif != 0 ? ((dif / this.dnet(item)) * 100).toFixed(2) : 0;
    },
    print(dispatch) {
      this.printedDispatch = dispatch;
      this.$refs.ticket.print();
    },
    filtered() {
      return this.processed.filter(this.search);
    },
    search(d) {
      return this.searchTerm == ""
        ? true
        : d.booking.numberPlate
            .replace(" ", "")
            .toLowerCase()
            .includes(this.searchTerm.toLowerCase().replace(" ", ""));
    },
    isDispatched(dispatch) {
      return dispatch.fromOffice.id == 1;
    },
    weight(dispatch) {
      return this.isDispatched(dispatch)
        ? dispatch.initialWeight
        : dispatch.receivalWeight;
    },
  },
};
</script>

<style scoped>
.td {
  @apply z-20 sticky font-semibold text-gray-700 p-2;
}
</style>