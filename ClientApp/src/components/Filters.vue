<template>
  <div class="lg:flex my-2">
    <div>
      <label for="fromDate" class="pr-4">From</label>
      <input type="date" :value="from" @input="changeFrom($event)" />
    </div>
    <div>
      <label for="toDate" class="px-4">To</label>
      <input type="date" :value="to" @input="changeTo($event)" />
    </div>
    <div class="ml-8 flex">
      <label class="inline-flex items-center">
        <input
          v-model="type"
          @change="changeType"
          type="radio"
          class="form-radio"
          value=""
        />
        <span class="mx-2">All</span>
      </label>
      <label class="inline-flex items-center">
        <input
          v-model="type"
          @change="changeType"
          type="radio"
          class="form-radio"
          value="Truck"
        />
        <span class="mx-2">Trucks</span>
      </label>
      <label class="inline-flex items-center">
        <input
          v-model="type"
          @change="changeType"
          type="radio"
          class="form-radio"
          value="Wagon"
        />
        <span class="mx-2">Trains</span>
      </label>
    </div>
    <pagination />
  </div>
</template>
<script>
import { mapGetters, mapState } from "vuex";
import Pagination from "./Pagination.vue";
export default {
  components: { Pagination },
  mounted() {
    this.$store.dispatch("dispatches/resetPage");
  },
  data() {
    const date = new Date();
    date.setDate(date.getDate() - 15);
    return {};
  },
  computed: {
    ...mapGetters("dispatches", {
      from: "fromDate",
      to: "toDate",
    }),
    ...mapState("dispatches", ["type"]),
  },
  methods: {
    changeFrom(event) {
      this.$store.dispatch(
        "dispatches/setFromDate",
        new Date(event.target.value)
      );
    },
    changeTo(event) {
      this.$store.dispatch(
        "dispatches/setToDate",
        new Date(event.target.value)
      );
    },
    changeType(event) {
      this.$store.dispatch("dispatches/setType", event.target.value);
    },
  },
};
</script>