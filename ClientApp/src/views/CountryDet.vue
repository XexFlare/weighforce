<template>
  <details-view
    :state="state"
    :create="create"
    :update="update"
    :toggle="toggle"
    :start="start"
    :finish="finish"
    :doDelete="doDelete"
  >
    <editable title="Name" v-model="details.name" :state="state" />
  </details-view>
</template>
<script>
import { CountriesApi } from "../plugins/api-classes";
import Editable from "../components/Editable.vue";
import DetailsView from "../components/DetailsView.vue";
export default {
  inject: ["authHeaders"],
  async mounted() {
    this.api = new CountriesApi(await this.authHeaders());
    if (this.creating) {
      this.state.edit = true;
    } else {
      this.details = await this.api.get(this.$route.params.id);
      this.state.loaded = this.details?.id != 0;
    }
    this.state.initialized = true;
  },
  components: {
    Editable,
    DetailsView,
  },
  computed: {
    creating() {
      return this.$route.params.id == null;
    },
  },
  data() {
    return {
      state: {
        edit: false,
        loading: false,
        loaded: false,
        initialized: false,
        errors: [],
      },
      details: {
        id: 0,
      },
      api: null,
    };
  },
  methods: {
    start() {
      this.state.loading = true;
    },
    finish() {
      this.state.loading = false;
    },
    validate() {
      this.state.errors = [];
      if (this.details.name) return true;
      this.state.errors.push({ field: "Name", message: "is required" });
      return false;
    },
    async create() {
      return this.validate() && (await this.api.create(this.details));
    },
    async update() {
      return (
        this.validate() &&
        (await this.api.update(this.$route.params.id, this.details))
      );
    },
    async doDelete() {
      await this.api.delete(this.$route.params.id);
      this.$router.back();
    },
    toggle() {
      if (this.creating) this.$router.back();
      this.state.edit = !this.state.edit;
    },
  },
};
</script>