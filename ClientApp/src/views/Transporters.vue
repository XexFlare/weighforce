<template>
  <div>
    <index-header title="Transporters" to="transporters/new" />
    <data-table :headings="headings" :rows="items" links="transporters" />
  </div>
</template>
<script>
import DataTable from "../components/DataTable.vue";
import IndexHeader from "../components/IndexHeader.vue";
import { Transporter } from "../plugins/api-classes";
export default {
  inject: ["authHeaders", "getOfficeId"],
  async mounted() {
    let transporter = new Transporter(await this.authHeaders());
    this.items = await transporter.getAll();
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
          title: "Email",
          value: "email",
        },
        {
          title: "Phone",
          value: "phoneNumber",
        },
      ],
    };
  },
};
</script>