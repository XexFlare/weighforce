<template>
  <div>
    <index-header to="countries/new" />
    <data-table :headings="headings" :rows="items" links="countries" />
  </div>
</template>
<script>
import DataTable from "../components/DataTable.vue";
import IndexHeader from "../components/IndexHeader.vue";
import { CountriesApi } from "../plugins/api-classes";
export default {
  inject: ["authHeaders", "getOfficeId"],
  async mounted() {
    let api = new CountriesApi(await this.authHeaders());
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