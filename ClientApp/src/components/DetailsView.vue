<template>
  <div>
    <div v-if="state.initialized">
      <form @submit="save" autocomplete="on">
        <details-header
          :title="title ?? $route.name"
          :state="state"
          :save="save"
          :toggle="toggle"
          :deletable="doDelete"
          :doDelete="toggleDelete"
        >
          <template v-slot:actions>
            <slot name="actions" />
          </template>
        </details-header>
        <ul
          v-if="state.loaded || state.edit"
          class="ml-8 lg:grid lg:grid-cols-2 gap-4"
        >
          <slot />
        </ul>
        <div v-else class="ml-8 lg:grid lg:grid-cols-2 gap-4">Error</div>
      </form>
      <modal :showing="showDelete" @close="toggleDelete" :showClose="true">
        <div class="p-8">
          <h2>Are you sure you want to delete this item?</h2>
          <div class="flex justify-right my-4">
            <w-button @click="toggleDelete()" title="Cancel" type="outline"
              >Cancel</w-button
            >
            <w-button
              @click="
                toggleDelete();
                doDelete();
              "
              class="ml-2"
              title="Yes"
              type="danger"
              >Yes</w-button
            >
          </div>
        </div>
      </modal>
    </div>
    <div v-else><loader /></div>
  </div>
</template>
<script>
import DetailsHeader from "./DetailsHeader.vue";
import Loader from "./Loader.vue";
import Modal from "./Modal.vue";
import WButton from "./WButton.vue";
export default {
  props: [
    "title",
    "state",
    "create",
    "update",
    "toggle",
    "start",
    "finish",
    "doDelete",
  ],
  data() {
    return {
      showDelete: false,
    };
  },
  components: {
    DetailsHeader,
    Loader,
    WButton,
    Modal,
  },
  computed: {
    creating() {
      return this.$route.params.id == null;
    },
  },
  methods: {
    toggleDelete() {
      this.showDelete = !this.showDelete;
    },
    async save(e) {
      e.preventDefault();
      this.start();
      const res = this.creating ? await this.create() : await this.update();
      if (res) this.toggle();
      this.finish();
    },
  },
};
</script>