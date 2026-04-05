<template>
  <div>
    Status:
    <div
      id="status"
      class="bordered border-2 p-2 max-h-96 h-96 overflow-scroll"
    >
      <ul>
        <li
          v-for="(stat, i) in status"
          :key="i"
          :class="
            stat.type == 'error'
              ? 'text-red-400'
              : stat.type == 'success'
              ? 'text-green-400'
              : 'text-blue-400'
          "
        >
          {{ stat.message }}
        </li>
      </ul>
    </div>
    <button
      @click="connect()"
      class="p-2 bg-accent hover:bg-primary text-white rounded mt-4"
    >
      Sync
    </button>
  </div>
</template>

<script>
export default {
  inject: ["authHeaders"],
  async created() {
    if (this.$socket.socket == false) {
      await this.$socket.start({
        log: true, // Logging is optional but very helpful during development
      });
    }
  },
  data() {
    return {
      status: [],
    };
  },
  methods: {
    async connect() {
      const that = this;
      this.$socket.socket.stream("StreamStatus").subscribe({
        next: (item) => {
          if (that.status.findIndex((i) => i.message == item.message) == -1)
            that.status.push(item);
        },
        complete: () => {
          that.status = "Stream Closed";
        },
        error: (err) => {
          that.status = "Error: " + err;
        },
      });
      this.status = [
        {
          type: "Error",
          message: "Starting Sync",
        },
      ];
      that.sync = await fetch(`${import.meta.env.VITE_API}/api/sync`, {
        headers: { ...(await that.authHeaders()) },
        method: "POST",
      })
        .then((data) => {
          return {
            res: true,
            message: "",
          };
        })
        .catch((e) => {
          console.log(e);
          this.status.push({
            type: "Error",
            message: "Error On Starting Sync",
          });
          return {
            res: false,
            message: e.message,
          };
        });
    },
  },
  updated() {
    var elem = this.$el.querySelector("#status");
    elem.scrollTop = elem.clientHeight;
  },
};
</script>

<style scoped>
</style>