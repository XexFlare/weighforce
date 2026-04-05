<template>
  <div class="w-full flex items-stretch" style="min-height: 400px">
    <div
      id="weighImage"
      class="w-3/6"
      style="
        background-position: center center;
        background-repeat: no-repeat;
        background-size: cover;
        display: block;
      "
    >
      <div class="h-full block"></div>
    </div>
    <div class="ml-6 flex flex-col items-center justify-center w-3/6 px-2 py-8">
      <h3 class="text-4xl text-gray-900 text-center">WEIGH TRUCK</h3>
      <h3 class="text-xl text-gray-900 text-center text-primary">
        {{ dispatch.booking.numberPlate }} - {{ dispatch.product.name }}
      </h3>
      <div class="flex w-full justify-center items-center pl-6 mt-2">
        <h3
          class="text-2xl text-gray-900 text-center border p-2 w-1/2"
        >
          {{ weight }}
        </h3>
        <div class="flex">
          <button
            v-show="!showUpdate"
            @click="showExtra = !showExtra"
            class="rounded-full my-2 p-1 ml-2 hover:bg-accent hover:text-white"
          >
            <svg
              xmlns="http://www.w3.org/2000/svg"
              class="h-6 w-6"
              fill="none"
              viewBox="0 0 24 24"
              stroke="currentColor"
              v-if="showExtra"
            >
              <path
                stroke-linecap="round"
                stroke-linejoin="round"
                stroke-width="2"
                d="M5 15l7-7 7 7"
              />
            </svg>
            <svg
              xmlns="http://www.w3.org/2000/svg"
              class="h-6 w-6"
              fill="none"
              viewBox="0 0 24 24"
              stroke="currentColor"
              v-else
            >
              <path
                stroke-linecap="round"
                stroke-linejoin="round"
                stroke-width="2"
                d="M19 9l-7 7-7-7"
              />
            </svg>
          </button>
          <button
            v-if="inRoles(['admin', 'manager'])"
            v-show="!showExtra && dispatch.status == 'Dispatching'"
            @click="toggleUpdate"
            class="rounded-full my-2 p-1 ml-2 hover:bg-accent hover:text-white"
          >
            <svg
              xmlns="http://www.w3.org/2000/svg"
              class="h-5 w-5"
              viewBox="0 0 20 20"
              fill="currentColor"
            >
              <path
                fill-rule="evenodd"
                d="M4 2a1 1 0 011 1v2.101a7.002 7.002 0 0111.601 2.566 1 1 0 11-1.885.666A5.002 5.002 0 005.999 7H9a1 1 0 010 2H4a1 1 0 01-1-1V3a1 1 0 011-1zm.008 9.057a1 1 0 011.276.61A5.002 5.002 0 0014.001 13H11a1 1 0 110-2h5a1 1 0 011 1v5a1 1 0 11-2 0v-2.101a7.002 7.002 0 01-11.601-2.566 1 1 0 01.61-1.276z"
                clip-rule="evenodd"
              />
            </svg>
          </button>
        </div>
      </div>
      <div v-if="showExtra">
        <div class="flex justify-start my-4">
          <label class="w-12 p-2" for="Bags">Bags</label>
          <input
            class="
              w-24
              bg-gray-100
              p-1
              bordered
              border-accent border-2
              rounded
              shadow
            "
            type="number"
            name="Bags"
            v-model.number="bags"
          />
        </div>
        <div class="flex justify-start">
          <label class="w-12 p-2" for="Size">Size</label>
          <input
            class="
              w-24
              bg-gray-100
              p-1
              bordered
              border-accent border-2
              rounded
              shadow
            "
            type="number"
            name="Size"
            v-model.number="size"
          />
          <span class="p-2">KG</span>
        </div>
        <div v-if="isGSL">
          <div class="text-center mt-2">Delivery Note Number</div>
          <div class="flex justify-start">
            <input
              class="
                bg-gray-100
                p-1
                bordered
                border-accent border-2
                rounded
                shadow
              "
              v-model="deliveryNoteNumber"
            />
          </div>
        </div>
        <div class="text-center text-lg text-gray-500">
          Expected: {{ size * bags }} KG <br />
          Actual: {{ weight }} KG <br />
          <span
            :class="
              Math.abs(weight - size * bags) > size * (2 / 3)
                ? 'text-red-400'
                : 'text-green-600'
            "
            >Diff: {{ weight - size * bags }} KG</span
          >
        </div>
      </div>
      <div v-if="showUpdate">
        <ul>
          <selectable
            title="Contract"
            v-model="tpi"
            :options="tpis"
            :state="state"
            :displayFormat="contractDisplay"
          />
          <div class="flex justify-center">
            <div
              @click="changeTpi"
              class="
                p-1
                w-min
                hover:bg-accent
                rounded
                hover:text-white
                transition
                border-2 border-accent
                cursor-default
              "
            >
              Update
            </div>
          </div>
        </ul>
      </div>
      <div
        v-if="nettable"
        class="flex w-full justify-center items-center pl-6 mt-2"
      >
        <h3 class="text-xl text-gray-900 text-center p-2 underline">
          Nett: {{ weight - dispatch.initialWeight.tare }}
        </h3>
      </div>
      <button
        @click="weigh"
        :disabled="loading || showUpdate"
        :class="
          loading || showUpdate
            ? 'bg-gray-400 cursor-not-allowed hover:bg-gray-400'
            : 'bg-accent'
        "
        class="
          rounded-md
          px-2
          py-2
          text-2xl text-white
          leading-normal
          mt-2
          border
          w-1/2
        "
      >
        Weigh
      </button>
      <img class="h-16 w-48 mt-2" src="../assets/logo.png" />
    </div>
  </div>
</template>

<script>
import { Booking, TpisApi } from "../plugins/api-classes";
import Selectable from "./Selectable.vue";
export default {
  inject: ["authStringHeader", "authHeaders", "user"],
  components: { Selectable },
  async created() {
    if (this.$socket.socket == false) {
      await this.$socket.start({
        log: true, // Logging is optional but very helpful during development
      });
    }
    // this.randomize();
    this.connect();
  },
  unmounted() {
    this.$socket.invoke("CloseStream");
  },
  props: {
    dispatch: Object,
    booking: Object,
    close: Function,
    updateDispatch: Function,
  },
  computed: {
    nettable() {
      return this.dispatch.status == "Dispatching";
    },
    isGSL: () => import.meta.env.VITE_GSL_SITE == "true",
  },
  methods: {
    toggleUpdate() {
      this.showUpdate = !this.showUpdate;
      this.getTpis();
    },
    async getTpis() {
      if (!this.tpis) {
        const headers = await this.authHeaders();
        const tpisApi = new TpisApi(headers);
        this.tpi = this.dispatch.booking.transportInstruction;
        this.tpis = await tpisApi.get("local");
        this.state = {
          edit: true,
          loading: false,
          loaded: true,
          initialized: true,
          related: true,
          errors: [],
        };
      }
    },
    async changeTpi() {
      const headers = await this.authHeaders();
      if (this.tpi.id != this.dispatch.booking.transportInstruction.id) {
        const api = new Booking(headers);
        const res = await api.update(`${this.dispatch.booking.id}/tpi`, {
          id: this.tpi.id,
        });
        if (res != null)
          this.$store.commit("dispatches/updateBooking", {
            updatedDispatch: this.dispatch,
            booking: res,
          });
      }
      this.showUpdate = false;
    },
    contractDisplay(contract) {
      return `${contract?.product?.name ?? "_"}: 
      ${contract?.contract?.contractNumber ?? "_"}
      (to: ${contract?.toLocation?.name ?? "_"})`;
    },
    inRoles(roleList) {
      if (roleList == null) return true;
      const roles = roleList.map((role) => role.toLowerCase());
      return this.user.value.roles.some((role) =>
        roles.includes(role.toLowerCase())
      );
    },
    async randomize() {
      function sleep(ms) {
        return new Promise((resolve) => setTimeout(resolve, ms));
      }
      for (let i = 0; i < 10; i++) {
        this.weight = Math.floor(Math.random() * 100000);
        await sleep(1000);
      }
    },
    async weigh() {
      if (this.dispatch != null) {
        this.loading = true;
        fetch(
          `${import.meta.env.VITE_API}/api/dispatches/${this.dispatch.id}`,
          {
            method: "POST",
            headers: {
              Accept: "application/json",
              "Content-Type": "application/json",
              ...(await this.authStringHeader()),
            },
            body: JSON.stringify({
              amount: this.weight,
              bags: this.bags,
              size: this.size,
              deliveryNoteNumber: this.deliveryNoteNumber,
            }),
          }
        )
          .then((res) => res.json())
          .then((res) => this.updateDispatch(res))
          .then(() => {
            this.loading = false;
          })
          .catch((error) => {
            this.loading = false;
            console.error("Unable to update item.", error);
          });
      }
    },
    connect() {
      const that = this;
      this.$socket.socket.stream("StreamWeight").subscribe({
        next: (item) => {
          that.weight = item.amount;
        },
        complete: () => {
          that.status = "Stream Closed";
        },
        error: (err) => {
          that.status = "Error: " + err;
        },
      });
    },
  },

  sockets: {
    ReceiveMessage(args) {
      this.status += "<li>Data received: " + args + "</li>";
    },
    Weight(args) {
      this.weight = args[0];
    },
  },
  data() {
    return {
      weight: 0,
      status: "Waiting",
      loading: false,
      bags: null,
      size: 50,
      deliveryNoteNumber: null,
      showExtra: false,
      showUpdate: false,
      tpis: null,
      tpi: null,
      state: {
        edit: true,
        loading: true,
        loaded: false,
        initialized: false,
        related: false,
        errors: [],
      },
    };
  },
};
</script>
<style scoped>
#weighImage {
  background-image: url("../assets/weight.jpg");
}
</style>