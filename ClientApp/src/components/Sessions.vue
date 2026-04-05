<template>
  <div class="flex">
    <div>
    <h2>Receive Trucks</h2>
  <div
    class="w-full flex justify-center overflow-y-auto scrollbar-w-2 scrollbar-track-gray-lighter scrollbar-thumb-rounded scrollbar-thumb-gray scrolling-touch"
  >
    <table class="text-left table-collapse">
      <thead class="before:border">
        <tr class="bg-gray-100 border-b border-t border-border-gray-900">
          <th class="z-20 sticky p-2"></th>
          <th class="z-20 sticky font-semibold text-gray-700 p-2">
            Registration Number
          </th>
          <th class="z-20 sticky font-semibold text-gray-700 p-2 pr-6">
            Driver
          </th>
          <th class="z-20 sticky font-semibold text-gray-700 p-2 pr-6">
            Product
          </th>
          <th class="z-20 sticky font-semibold text-gray-700 p-2 pl-6">
            Tare (kg)
          </th>
          <th class="z-20 sticky font-semibold text-gray-700 p-2 pl-6">
            Gross (kg)
          </th>
          <th class="z-20 sticky font-semibold text-gray-700 p-2">
            Net Weight (kg)
          </th>
          <th class="z-20 sticky font-semibold text-gray-700 p-2"></th>
        </tr>
      </thead>
      <tbody class="align-baseline">
        <tr v-for="session in sessions" v-bind:key="session.id" :class="session.isDispatched ? 'bg-green-100' : 'bg-blue-100'">
          <td
            class="p-2 font-mono text-sm text-blue-700 whitespace-nowrap border-t border-gray-200"
          >
            {{ session.isDispatched ? "⬇" : "⬆" }}
          </td>
          <td
            class="p-2 font-mono text-sm text-blue-700 whitespace-nowrap border-t border-gray-200"
          >
            {{ session.regNumber }}
          </td>
          <td
            class="p-2 font-mono text-sm text-blue-700 whitespace-nowrap border-t border-gray-200"
          >
            {{ session.driver }}
          </td>
          <td
            class="p-2 font-bold font-mono text-sm text-blue-700 whitespace-nowrap border-t border-gray-200"
          >
            {{ session.product }}
          </td>
          <td
            class="p-2 font-mono text-sm text-right text-purple-700 whitespace-nowrap border-t border-gray-200"
          >
            {{ session.tare }}
          </td>
          <td
            class="p-2 font-mono text-sm text-right text-purple-700 whitespace-nowrap border-t border-gray-200"
          >
            {{ session.gross }}
          </td>
          <td
            class="p-2 font-bold font-mono text-sm text-right text-purple-700 whitespace-nowrap border-t border-gray-200"
          >
            {{ session.gross != 0 ? session.gross - session.tare : 0 }}
          </td>
          <td
            class="p-2"
          >
            <button
              :disabled="session.gross != 0"
              @click="toggleModal(session)"
              :class="
                session.gross != 0
                  ? 'bg-gray-200 text-gray-600 cursor-not-allowed'
                  : 'bg-blue-500 hover:bg-blue-700 text-white'
              "
              class="py-1 px-2 rounded shadow"
            >
              Weigh
            </button>
          </td>
        </tr>
      </tbody>
    </table>
  </div>

  <Modal :showing="show" @close="toggleModal" :showClose="true">
    <Weight :session="session" :updateSession="updateSession" />
  </Modal>
  </div>
  </div>
</template>

<script>
  import Weight from "./Weight.vue";
  import Modal from "./Modal.vue";

  export default {
    components: {
      Modal,
      Weight,
    },
    async mounted() {
      this.sessions = await fetch(
        `${import.meta.env.VITE_API}/api/sessions`
      ).then((data) => data.json());
    },
    data() {
      return {
        show: false,
        sessions: [],
        session: null,
      };
    },
    methods: {
      toggleModal(session) {
        this.session = session;
        this.show = !this.show;
      },
      updateSession(updatedSession) {
        this.sessions = this.sessions.map((session) => {
          if (session.id == updatedSession.id) {
            return updatedSession;
          }
          return session;
        });
        this.toggleModal(null);
      },
    },
  };
</script>