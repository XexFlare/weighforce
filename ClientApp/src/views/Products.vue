<template>
  <div>
    <index-header to="products/new" />
    <data-table :headings="headings" :rows="items" links="products" />
  </div>
</template>
<script>
import DataTable from "../components/DataTable.vue";
import IndexHeader from "../components/IndexHeader.vue";
import { ProductsApi } from "../plugins/api-classes";
export default {
  inject: ["authHeaders", "getOfficeId"],
  async mounted() {
    let api = new ProductsApi(await this.authHeaders());
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
          title: "Name",
          value: "name",
          slotname: "name",
        },
      ],
    };
  },
};
</script>