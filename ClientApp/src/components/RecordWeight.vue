<template>
  <div class="w-full flex items-center">
    <div class="w-3/6">
      <img
        class="h-full w-full"
        src="../assets/weight.jpg"
        alt="ChitChat Logo"
      />
    </div>
    <div class="ml-6 pt-1 flex flex-col items-center justify-center w-3/6 p-3">
      <h3 class="text-2xl text-gray-900 text-center">RECORD WEIGH</h3>
      <h3
        class="text-xl text-gray-900 text-center text-primary"
        v-if="dispatch"
      >
        {{ dispatch.booking.numberPlate }} - {{ dispatch.product.name }}
      </h3>
      <h3
        class="text-xl text-gray-900 text-center text-primary"
        v-else-if="booking"
      >
        {{ booking.numberPlate }} - {{ booking.driverName }}
      </h3>
      <label for="gross">Gross Weight</label><input type="number" name="gross" id="gross" v-model="gross" 
        class="px-4 py-1 text-sm text-gray-600 border-2 rounded focus:outline-none focus:shadow-none lg:block"
      >
      <label for="tare">Tare Weight</label><input type="number" name="tare" id="tare" v-model="tare" 
        class="px-4 py-1 text-sm text-gray-600 border-2 rounded focus:outline-none focus:shadow-none lg:block"
      >
      <button
        @click="weigh"
        class="rounded-md px-2 py-1 text-xl text-white leading-normal mt-2 border bg-accent w-1/2"
      >
        Record Weigh
      </button>
      <img class="h-16 w-48 mt-2" src="../assets/logo.png" />
    </div>
  </div>
</template>

<script>
  export default {
    inject: ["authStringHeader"],
    props: {
      dispatch: Object,
      booking: Object,
      close: Function,
      updateDispatch: Function,
    },
    methods: {
      async weigh() {
          fetch(
            `${import.meta.env.VITE_API}/api/dispatches/clients/${this.dispatch.id}`,
            {
              method: "POST",
              headers: {
                Accept: "application/json",
                "Content-Type": "application/json",
                ...(await this.authStringHeader()),
              },
              body: JSON.stringify({ gross: parseInt(this.gross), tare: parseInt(this.tare) }),
            }
          )
            .then((res) => res.json())
            .then((res) => this.updateDispatch(res))
            .catch((error) => console.error("Unable to update item.", error));
      }
    },
    data() {
      return {
        tare: 0,
        gross: 0
      };
    },
  };
</script>