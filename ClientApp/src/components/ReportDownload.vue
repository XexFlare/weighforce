<template>
  <div>
    <w-button @click="toggleModal" class="nav-link mr-4">Dispatch Reports</w-button>
    <modal :showing="showModal" @close="toggleModal">
      <div class="p-8">
        <h2>Download</h2>
        <div class="flex justify-between my-2">
          <div>
            <label for="fromDate" class="pr-4">From</label>
            <input type="date" :value="from" @input="changeFrom($event)" />
          </div>
          <div>
            <label for="toDate" class="px-4">To</label>
            <input type="date" :value="to" @input="changeTo($event)" />
          </div>
        </div>
        <!-- Location Picker -->
        <div class="flex justify-between">
          <w-button @click="getDispatchReport">Dispatch Report</w-button>
          <w-button @click="getReceivedReport">Received Report</w-button>
          <w-button @click="getPendingReport">Pending Report</w-button>
        </div>
      </div>
    </modal>
  </div>
</template>
<script>
import Modal from "./Modal.vue";
import WButton from "./WButton.vue";
import axios from "axios";
export default {
  inject: ["authHeaders", "getOfficeId"],
  data() {
    const date = new Date();
    date.setDate(date.getDate() - 7);
    return {
      from: date.toISOString().substr(0, 10),
      to: new Date().toISOString().substr(0, 10),
      showModal: false,
    };
  },
  components: { Modal, WButton },
  methods: {
    toggleModal() {
      this.showModal = !this.showModal;
    },
    changeFrom(event) {
      this.from = event.target.value
    },
    changeTo(event) {
      this.to = event.target.value
    },
    async getDispatchReport(){
      this.getReport('dispatched')
    },
    async getReceivedReport(){
      this.getReport('received')
    },
    async getPendingReport(){
      this.getReport('pending')
    },
    async getReport(type){
      const officeId = this.getOfficeId();
      await fetch(
      `${import.meta.env.VITE_API}/api/reports/${type}?OfficeId=${officeId}&Date=${this.from}&ToDate=${this.to}`,
        await this.authHeaders()
    )
      .then((data) => data.json())
      .then(this.download)
      .catch((error) => console.error("Unable to get chart.", error));
    },
    download({url}) {
      axios({
        url: `${import.meta.env.VITE_APP}/${url}`,
        method: "GET",
        responseType: "blob",
      }).then((response) => {
        var fileURL = window.URL.createObjectURL(new Blob([response.data]));
        var fileLink = document.createElement("a");

        fileLink.href = fileURL;
        fileLink.setAttribute("download", url.split("/")[1]);
        document.body.appendChild(fileLink);

        fileLink.click();
      });
    },
  },
};
</script>
<style scoped>
.nav-link {
  @apply w-40 text-center px-3 py-2 bg-primary font-medium text-gray-200 rounded-md transition duration-500 ease-in-out;
}
.router-link-active,
.nav-link:hover {
  @apply bg-accent text-white;
}
.nav-link:focus {
  @apply outline-none bg-accent;
}
nav:after {
  content: "";
  position: absolute;
  top: 60px;
  left: 0;
  border-style: solid;
  border-width: 45px 0px 0px 100vw;
  border-color: #4d7d77 transparent transparent transparent;
}
</style>