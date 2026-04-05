<template>
  <div>
    <index-header title="Contracts" to="contracts/new" />
    <data-table :headings="headings" :rows="items" links="contracts" />
  </div>
</template>
<script>
import DataTable from "../components/DataTable.vue";
import IndexHeader from "../components/IndexHeader.vue";
import { ContractsApi } from "../plugins/api-classes";
export default {
  inject: ["authHeaders"],
  async mounted() {
    let api = new ContractsApi(await this.authHeaders());
    this.items = await api.getAll();
  },
  components: {
    DataTable,
    IndexHeader,
  },
  data() {
    return {
      items: [],
      headings: [
        {
          title: "ERP Company",
          value: "erpCompany",
        },
        {
          title: "Contract Number",
          value: "contractNumber",
        },
        {
          title: "Item",
          value: "item",
        },
        {
          title: "Vessel",
          value: "vessel",
        },
        {
          title: "Supplier",
          value: "supplier",
        },
      ],
    };
  },
};
</script>