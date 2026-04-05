<template>
  <div style="width: 1000px; margin: 0 auto 0 auto">
    <div class="flex justify-between">
      <div>
        <h2>Over Weights</h2>
        <div class="flex flex-row">
          <input
            type="text"
            v-model="overSearchTerm"
            class="
              px-4
              py-1
              text-sm text-gray-600
              border-2
              rounded
              focus:outline-none
              focus:shadow-none
              lg:block
            "
            placeholder="Search ..."
          />
        </div>
      </div>
      <crm-link />
    </div>
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
      <table class="text-left table-collapse w-full">
        <thead class="before:border">
          <tr class="bg-gray-100 border-b border-t border-border-gray-900">
            <th class="td">Number Plate</th>
            <th class="td pr-6">Driver</th>
            <th class="td pr-6">From</th>
            <th class="td pr-6">To</th>
            <th class="td pr-6">Product</th>
            <th class="td pl-6">Initial Weight</th>
            <th class="td pl-6">Received Weight</th>
            <th class="td"></th>
          </tr>
        </thead>
        <tbody class="align-baseline">
          <tr
            v-for="dispatch in filteredOverweights()"
            v-bind:key="dispatch.id"
            class="bg-blue-100"
          >
            <td
              class="
                p-2
                font-mono
                text-sm text-blue-700
                whitespace-nowrap
                border-t border-gray-200
              "
            >
              {{ dispatch.booking.numberPlate }}
            </td>
            <td
              class="
                p-2
                font-mono
                text-sm text-blue-700
                whitespace-nowrap
                border-t border-gray-200
              "
            >
              {{ dispatch.booking.driverName }}
            </td>
            <td
              class="
                p-2
                font-mono
                text-sm text-blue-700
                whitespace-nowrap
                border-t border-gray-200
              "
            >
              {{ dispatch.fromOffice.name }}
            </td>
            <td
              class="
                p-2
                font-mono
                text-sm text-blue-700
                whitespace-nowrap
                border-t border-gray-200
              "
            >
              {{ dispatch.toOffice.name }}
            </td>
            <td
              class="
                p-2
                font-mono
                text-sm text-blue-700
                whitespace-nowrap
                border-t border-gray-200
                font-bold
              "
            >
              {{ dispatch.product.name }}
            </td>
            <td
              class="
                p-2
                font-mono
                text-sm text-right text-purple-700
                whitespace-nowrap
                border-t border-gray-200
              "
            >
              {{ dispatch.initialWeight.gross - dispatch.initialWeight.tare }}
            </td>
            <td
              class="
                p-2
                font-bold font-mono
                text-sm text-right text-purple-700
                whitespace-nowrap
                border-t border-gray-200
              "
            >
              {{ dispatch.receivalWeight.gross - dispatch.receivalWeight.tare }}
            </td>
            <td class="p-2">
              <button
                @click="print(dispatch)"
                class="
                  py-1
                  px-2
                  rounded
                  shadow
                  bg-blue-500
                  hover:bg-blue-700
                  text-white
                "
              >
                Print
              </button>
            </td>
          </tr>
        </tbody>
      </table>
    </div>
    <Ticket :dispatch="printedDispatch" ref="ticket" />
  </div>
</template>

<script>
import { ref } from "vue";
import Ticket from "../components/Ticket.vue";
import CrmLink from "../components/CrmLink.vue";

export default {
  components: {
    Ticket,
    CrmLink,
  },
  inject: ["authHeaders"],
  async mounted() {
    this.overweights = await fetch(
      `${import.meta.env.VITE_API}/api/dispatches/overweights`,
      await this.authHeaders()
    )
      .then((data) => data.json())
      .then((data) => data.data);
  },
  data() {
    return {
      show: false,
      underSearchTerm: "",
      overSearchTerm: "",
      overweights: [],
      underweights: [],
      dispatch: null,
      printedDispatch: null,
      filter: "↕",
    };
  },
  computed: {},
  methods: {
    print(dispatch) {
      this.printedDispatch = dispatch;
      this.$refs.ticket.print();
    },
    filteredUnderweights() {
      if (this.filter == "↕") {
        return this.underweights.filter((d) =>
          this.underSearchTerm == ""
            ? true
            : d.booking.numberPlate
                .replace(" ", "")
                .toLowerCase()
                .includes(this.underSearchTerm.toLowerCase().replace(" ", ""))
        );
      }
    },
    filteredOverweights() {
      if (this.filter == "↕") {
        return this.overweights.filter((d) =>
          this.overSearchTerm == ""
            ? true
            : d.booking.numberPlate
                .replace(" ", "")
                .toLowerCase()
                .includes(this.overSearchTerm.toLowerCase().replace(" ", ""))
        );
      }
    },
  },
};
</script>

<style scoped>
.td {
  @apply z-20 sticky font-semibold text-gray-700 p-2;
}
</style>