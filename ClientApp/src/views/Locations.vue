<template>
  <div>
    <index-header to="locations/new" />
    <data-table :headings="headings" :rows="items" links="locations">
      <template v-slot:isClient="{ item }">
        {{ item.isClient ? "✅" : "🔲" }}
      </template>
    </data-table>
  </div>
</template>
<script>
import DataTable from "../components/DataTable.vue";
import IndexHeader from "../components/IndexHeader.vue";
import { LocationsApi } from "../plugins/api-classes";
export default {
  inject: ["authHeaders", "getOfficeId"],
  async mounted() {
    let api = new LocationsApi(await this.authHeaders());
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
        },
        {
          title: "Country",
          value: "name",
          sub: "country",
        },
        {
          title: "Is Customer",
          value: "isClient",
        },
      ],
      filterText: "",
    };
  },
};
</script>