<template>
  <div>
    <index-header
      title="Transport Instructions"
      to="transport-instructions/new"
    />
    <data-table
      :headings="headings"
      :rows="items"
      links="transport-instructions"
    >
      <template v-slot:closed="{ item }">
        {{ item.closed ? "❌" : "🔲" }}
      </template>
    </data-table>
  </div>
</template>
<script>
import DataTable from "../components/DataTable.vue";
import IndexHeader from "../components/IndexHeader.vue";
import { TpisApi } from "../plugins/api-classes";
export default {
  inject: ["authHeaders", "getOfficeId"],
  async mounted() {
    this.api = new TpisApi(await this.authHeaders());
    this.items = await this.api.getAll();
  },
  components: {
    DataTable,
    IndexHeader,
  },
  data() {
    return {
      api: null,
      items: [],
      headings: [
        {
          title: "Contract",
          value: "contractNumber",
          sub: "contract",
        },
        {
          title: "Kinetic Ref",
          value: "kineticRef",
        },
        {
          title: "Product",
          value: "name",
          sub: "product",
        },
        {
          title: "From",
          value: "name",
          sub: "fromLocation",
        },
        {
          title: "To",
          value: "name",
          sub: "toLocation",
        },
        {
          title: "Closed",
          value: "closed",
        },
      ],
    };
  },
};
</script>