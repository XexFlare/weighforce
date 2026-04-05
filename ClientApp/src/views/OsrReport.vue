<template>
  <div>
    <index-header />
    <div class="overflow-x-scroll">
      <pagination />
      <data-table :headings="headings" :rows="osr" class="w-max" />
    </div>
  </div>
</template>

<script>
import DataTable from "../components/DataTable.vue";
import IndexHeader from "../components/IndexHeader.vue";
import { mapState } from "vuex";
import WButton from "../components/WButton.vue";
import Pagination from "../components/Pagination.vue";

export default {
  components: {
    IndexHeader,
    DataTable,
    WButton,
    Pagination,
  },
  created() {
    this.$store.commit("dispatches/setCurrentQuery", "getOsr");
    this.$store.dispatch("dispatches/getOsr");
  },
  methods: {
    nextPage() {
      this.$store.dispatch("dispatches/nextPage");
    },
  },
  computed: mapState({
    osr: (state) => state.dispatches.osr,
  }),
  data() {
    return {
      headings: [
        {
          title: "Date",
          value: "date",
          isDate: true,
        },
        {
          title: "Product",
          value: "product",
        },
        {
          title: "Warehouse",
          value: "warehouse",
        },
        {
          title: "Team Stats",
          value: "teamStats",
        },
        {
          title: "Vessel",
          value: "vessel",
        },
        {
          title: "BL",
          value: "bl",
        },
        {
          title: "GDN # (TC)",
          value: "gdnId",
        },
        {
          title: "GDN Vallis",
          value: "gdnValis",
        },
        {
          title: "CRM Release Nr",
          value: "crmReleaseNo",
        },
        {
          title: "Wagon Type",
          value: "wagonType",
        },
        {
          title: "Seal Number",
          value: "sealNumber",
        },
        {
          title: "# of Bags",
          value: "bags",
        },
        {
          title: "Unit/Bag",
          value: "unit",
        },
        {
          title: "Tare WT",
          value: "tare",
        },
        {
          title: "Gross WT",
          value: "gross",
        },
        {
          title: "Product WT",
          value: "productWeight",
        },
        {
          title: "Railed At&Train",
          value: "railedAtTrain",
        },
        {
          title: "Date of Arrival",
          value: "arrivalDate",
        },
        {
          title: "Bags Received",
          value: "bagsReceived",
        },
        {
          title: "Wet",
          value: "wet",
        },
        {
          title: "Quantity Received",
          value: "qtyReceived",
        },
        {
          title: "Excess Shortage",
          value: "diff",
        },
      ],
    };
  },
};
</script>