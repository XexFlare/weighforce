<template>
  <div style="width: 1000px; margin: 0 auto 0 auto">
    <h2>Send Email</h2>
    <br />
    <button @click="send()" class="bg-accent mt-4 px-2 py-1 text-white rounded hover:bg-primary">
      Send Email
    </button>
    <div class="mt-4">
      <output name="result">Status: {{ status }}</output>
    </div>
    <data-table :headings="headings" :rows="items"  />
  </div>
</template>

<script>
import DataTable from "../components/DataTable.vue";
export default {
  inject: ["authHeaders", "authStringHeader"],
  async mounted() {
    this.items = await fetch(
      `${import.meta.env.VITE_API}/api/dispatches/to-send`,
      await this.authHeaders()
    ).then((data) => data.json())
    .then((res) => res.data);
  },
  components: {
    DataTable,
  },
  data() {
    return {
      status: "",
      items: [],
      headings: [
        {
          title: "Train Number",
          value: "trailerNumber",
          sub: "booking",
        },
        {
          title: "Wagon Number",
          value: "numberPlate",
          sub: "booking",
        },
        {
          title: "Product",
          headClass: "pr-2",
          value: "name",
          slotname: "product",
          sub: "product",
          class: "font-bold",
        },
        {
          title: "Dispatched On",
          isDate: true,
          headClass: "pr-2",
          slotname: "createdAt",
          value: "grossAt",
          sub: "initialWeight",
        },
        {
          title: "Dis. Tare",
          headClass: "pl-2",
          sub: "initialWeight",
          value: "tare",
          slotname: "tare",
          class: "text-right text-purple-700",
        },
        {
          title: "Dis. Gross",
          headClass: "pl-2",
          sub: "initialWeight",
          value: "gross",
          slotname: "gross",
          class: "text-right text-purple-700",
        },
        {
          title: "Dis. Nett",
          value: "nett",
          slotname: "dnett",
          sub: "initialWeight",
          class: "font-bold text-right text-purple-700",
        },
        {
          title: "Rec. Gross",
          headClass: "pl-2",
          sub: "receivalWeight",
          value: "gross",
          slotname: "gross",
          class: "text-right text-purple-700",
        },
        {
          title: "Rec. Tare",
          headClass: "pl-2",
          sub: "receivalWeight",
          value: "tare",
          slotname: "tare",
          class: "text-right text-purple-700",
        },
        {
          title: "Rec. Nett",
          sub: "receivalWeight",
          value: "nett",
          slotname: "rnett",
          class: "font-bold text-right text-purple-700",
        },
        {
          title: "Diff",
          sub: "diff",
          slotname: "diff",
          value: "value",
          class: "text-right",
        },
        { title: "", value: "action", slotname: "action" },
      ],
    };
  },
  methods: {
    async send() {
      this.status = "Sending In Progress";
      fetch(`${import.meta.env.VITE_API}/api/reports/send`, {
        method: "POST",
        headers: await this.authStringHeader(),
        body: JSON.stringify(this.user),
      }).then((data) => {
        this.status = "Sending Complete"
      }).catch(e => {
        this.status = "Sending Failed"
      });
    }
  }
};
</script>