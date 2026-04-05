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
    <selectable
      title="Unit"
      v-model="details.unit"
      :state="state"
      :options="units"
      displayKey="name"
    />
  </details-view>
</template>
<script>
import { ProductsApi, UnitsApi } from "../plugins/api-classes";
import Editable from "../components/Editable.vue";
import DetailsView from "../components/DetailsView.vue";
import Selectable from "../components/Selectable.vue";
export default {
  inject: ["authHeaders"],
  async mounted() {
    this.api = new ProductsApi(await this.authHeaders());
    this.unitsApi = new UnitsApi(await this.authHeaders());
    if (this.creating) {
      this.state.edit = true;
      await this.getRelated();
    } else {
      this.details = await this.api.get(this.$route.params.id);
      this.state.loaded = this.details?.id != 0;
    }
    this.state.initialized = true;
  },
  components: {
    Editable,
    DetailsView,
    Selectable,
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
      units: [],
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
      if (!this.validate()) return false;
      var res = await this.api.create(this.details);
      if (res?.status == 400) {
        this.state.errors = [];
        this.state.errors.push({
          field: "Name",
          message: "" + res.detail,
          server: true,
        });
        return false;
      }
      return true;
    },
    async update() {
      if (!this.validate()) return false;
      var res = await this.api.update(this.$route.params.id, this.details);
      if (res?.status == 400) {
        this.state.errors = [];
        this.state.errors.push({
          field: "Name",
          message: "" + res.detail,
          server: true,
        });
        return false;
      }
      return true;
    },
    async doDelete() {
      await this.api.delete(this.$route.params.id);
      this.$router.back();
    },
    async toggle() {
      if (this.creating) this.$router.back();
      else {
        if (!this.state.related) {
          await this.getRelated();
        }
        this.state.edit = !this.state.edit;
      }
    },
    async getRelated() {
      this.state.loading = true;
      this.units = await this.unitsApi.getAll();
      this.state.loading = false;
      this.state.related = true;
    },
  },
};
</script>