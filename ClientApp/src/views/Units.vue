<template>
  <div>
    <index-header to="units/new" />
    <data-table :headings="headings" :rows="items" links="units" />
  </div>
</template>
<script>
import DataTable from "../components/DataTable.vue";
import IndexHeader from "../components/IndexHeader.vue";
import { UnitsApi } from "../plugins/api-classes";
export default {
  inject: ["authHeaders", "getOfficeId"],
  async mounted() {
    let api = new UnitsApi(await this.authHeaders());
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