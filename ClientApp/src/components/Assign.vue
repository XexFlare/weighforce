<template>
  <div class="">
    <div class="w-full flex items-center">
      <div v-if="init" class="ml-6 pt-1 flex flex-col items-center justify-center w-4/6 p-3">
        <div class="flex flex-col items-start">
          <h3 class="text-3xl text-gray-900 text-center">Set Initial Weight</h3>
          <h4 class="text-lg text-gray-900 text-center text-primary">
            Wagon Number: {{ dispatch.booking.numberPlate }}
          </h4>
          <h4 class="text-lg text-gray-900 text-center text-primary">
            Product: {{ dispatch.product.name }}
          </h4>
          <h4 class="text-md text-gray-900 text-center text-primary">
            Arrival Time: {{ new Date(dispatch.receivalWeight.grossAt)
              .toLocaleDateString("en-GB", {
                day: "numeric",
                month: "short",
                year: "numeric",
                hour: "numeric",
                minute: "2-digit"
              })
            }}
          </h4>
        </div>
        <div class="flex justify-start my-4">
          <label class="w-32 p-2" for="Tare">Tare</label>
          <input class="
              w-48
              bg-gray-100
              p-1
              bordered
              border-accent border-2
              rounded
              shadow
            " v-model.number="tare" />
        </div>
        <div class="flex justify-start my-4">
          <label class="w-32 p-2" for="Tare">Gross</label>
          <input class="
              w-48
              bg-gray-100
              p-1
              bordered
              border-accent border-2
              rounded
              shadow
            " v-model.number="gross" />
        </div>
        <button @click="setInit" class="
          rounded-md
          px-1
          py-1
          text-2xl text-white
          leading-normal
          mt-2
          border
          bg-accent
          w-1/2
        ">
          Set Weight
        </button>
        <button v-if="!init" @click="discard" class="
          rounded-md
          px-1
          py-1
          text-lg
          font-bold
          hover:bg-gray-100
          text-gray-400
          leading-normal
          mt-4
          w-1/2
        ">
          Discard
        </button>
      </div>
      <div v-else class="ml-6 pt-1 flex flex-col items-center justify-center w-4/6 p-3">
        <div class="flex flex-col items-start">
          <h3 class="text-3xl text-gray-900 text-center">Assign Wagon</h3>
          <h4 class="text-lg text-gray-900 text-center text-primary">
            Wagon Number: {{ dispatch.booking.numberPlate }}
          </h4>
          <h4 class="text-lg text-gray-900 text-center text-primary">
            Product: {{ dispatch.product.name }}
          </h4>
          <h4 class="text-md text-gray-900 text-center text-primary">
            Arrival Time: {{ new Date(dispatch.receivalWeight.grossAt)
              .toLocaleDateString("en-GB", {
                day: "numeric",
                month: "short",
                year: "numeric",
                hour: "numeric",
                minute: "2-digit"
              })
            }}
          </h4>
          <h3 class="text-lg text-gray-900 text-center text-primary">
            Gross: {{ dispatch.receivalWeight.gross }}
          </h3>
        </div>
        <div class="flex justify-start my-4">
          <label class="w-32 p-2" for="Wagon Number">Wagon Number</label>
          <input class="
              w-48
              bg-gray-100
              p-1
              bordered
              border-accent border-2
              rounded
              shadow
            " v-model.number="numberPlate" />
        </div>
        <div class="flex justify-start my-4">
          <label class="w-32 p-2" for="Product">Product</label>
          <div class="autocomplete">
            <input type="text" @input="onChange" v-model="search" @keydown.down="onArrowDown" @keydown.up="onArrowUp"
              @keydown.enter="onEnter" class="w-48 bg-gray-100 p-1 bordered border-accent border-2 rounded shadow" />
            <ul id="autocomplete-results" v-show="isOpen" class="autocomplete-results">
              <li class="loading" v-if="isLoading">
                Loading results...
              </li>
              <li v-else v-for="(result, i) in results" :key="i" @click="setResult(result)" class="autocomplete-result"
                :class="{ 'is-active': i === arrowCounter }">
                {{ result.name }}
              </li>
            </ul>
          </div>
        </div>
        <button @click="assign" class="
          rounded-md
          px-1
          py-1
          text-2xl text-white
          leading-normal
          mt-2
          border
          bg-accent
          w-1/2
        ">
          Assign
        </button>
        <button @click="discard" class="
          rounded-md
          px-1
          py-1
          text-lg
          font-bold
          hover:bg-gray-100
          text-gray-400
          leading-normal
          mt-4
          w-1/2
        ">
          Discard
        </button>
      </div>
      <button v-if="inRoles(['sys'])" @click="init = !init" class="rounded-full my-2 p-1 ml-2 hover:bg-accent hover:text-white">
        <svg xmlns="http://www.w3.org/2000/svg" class="h-6 w-6" fill="none" viewBox="0 0 24 24" stroke="currentColor"
          v-if="!init">
          <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M5 15l7-7 7 7" />
        </svg>
        <svg xmlns="http://www.w3.org/2000/svg" class="h-6 w-6" fill="none" viewBox="0 0 24 24" stroke="currentColor"
          v-else>
          <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M19 9l-7 7-7-7" />
        </svg>
      </button>
    </div>
  </div>
</template>

<script>
export default {

  inject: ["authStringHeader", "authHeaders", "user"],
  props: {
    dispatch: Object,
    close: Function,
    updateDispatch: Function,
    products: Array
  },
  async mounted() {
    document.addEventListener('click', this.handleClickOutside)
    this.items = this.products
    this.related = await fetch(
      `${import.meta.env.VITE_API}/api/dispatches/${this.dispatch.id
      }/related?size=40`,
      await this.authHeaders()
    )
      .then((data) => data.json())
      .then((data) => data.data)
      .catch((error) => console.error("Unable to get related.", error));
  },
  methods: {
    inRoles(roleList) {
      if (roleList == null) return true;
      const roles = roleList.map((role) => role.toLowerCase());
      return this.user.value.roles.some((role) =>
        roles.includes(role.toLowerCase())
      );
    },
    async assign() {
      if (this.value == null || this.dispatch == null) return
      fetch(
        `${import.meta.env.VITE_API}/api/dispatches/${this.dispatch.id
        }/assign`,
        {
          method: "POST",
          headers: {
            Accept: "application/json",
            "Content-Type": "application/json",
            ...(await this.authStringHeader()),
          },
          body: JSON.stringify({ product: this.value.id, numberPlate: "" + this.numberPlate }),
        }
      )
        .then((res) => res.json())
        .then((res) => this.updateDispatch(res))
        .catch((error) => console.error("Unable to update item.", error));
    },
    async setInit() {
      if (this.gross == "" || this.tare == "") {
        console.log("Thats not that", this.gross, this.tare);
        return
      }
      fetch(
        `${import.meta.env.VITE_API}/api/dispatches/${this.dispatch.id
        }/init`,
        {
          method: "POST",
          headers: {
            Accept: "application/json",
            "Content-Type": "application/json",
            ...(await this.authStringHeader()),
          },
          body: JSON.stringify({ tare: { amount: this.tare }, gross: { amount: this.gross } }),
        }
      )
        .then((res) => res.json())
        .then((res) => this.updateDispatch(res))
        .catch((error) => console.error("Unable to update item.", error));
    },
    async discard() {
      if (this.dispatch != null) {
        fetch(
          `${import.meta.env.VITE_API}/api/dispatches/${this.dispatch.id
          }/discard`,
          {
            method: "POST",
            headers: {
              Accept: "application/json",
              "Content-Type": "application/json",
              ...(await this.authStringHeader()),
            },
          }
        )
          .then((res) => res.json())
          .then((res) => this.updateDispatch(res))
          .catch((error) => console.error("Unable to update item.", error));
      }
    },
    setResult(result) {
      this.search = result.name;
      this.value = result;
      this.isOpen = false;
    },
    filterResults() {
      this.results = this.items?.filter((item) => {
        return item?.name?.toLowerCase().indexOf(this.search.toLowerCase()) > -1;
      }) ?? [];
    },
    onChange() {
      this.$emit('input', this.search);

      if (this.isAsync) {
        this.isLoading = true;
      } else {
        this.filterResults();
        this.isOpen = true;
      }
    },
    handleClickOutside(event) {
      if (!this.$el.contains(event.target)) {
        this.isOpen = false;
        this.arrowCounter = -1;
      }
    },
    onArrowDown() {
      if (this.arrowCounter < this.results.length) {
        this.arrowCounter = this.arrowCounter + 1;
      }
    },
    onArrowUp() {
      if (this.arrowCounter > 0) {
        this.arrowCounter = this.arrowCounter - 1;
      }
    },
    onEnter() {
      this.search = this.results[this.arrowCounter];
      this.isOpen = false;
      this.arrowCounter = -1;
    },
  },
  watch: {
    items: function (value, oldValue) {
      if (value?.length !== oldValue?.length) {
        this.results = value;
        this.isLoading = false;
      }
    },
  },
  destroyed() {
    document.removeEventListener('click', this.handleClickOutside)
  },
  data() {
    return {
      weight: 0,
      status: "Waiting",
      related: [],
      items: [],
      newId: "0",
      numberPlate: '',
      product: '',
      isOpen: false,
      results: [],
      value: null,
      search: '',
      isLoading: false,
      init: false,
      tare: "",
      gross: "",
      arrowCounter: -1,
    };
  },
};
</script>

<style>
.autocomplete {
  position: relative;
}

.autocomplete-results {
  padding: 0;
  margin: 0;
  border: 1px solid #eeeeee;
  height: 120px;
  overflow: auto;
}

.autocomplete-result {
  list-style: none;
  text-align: left;
  padding: 4px 2px;
  cursor: pointer;
}

.autocomplete-result.is-active,
.autocomplete-result:hover {
  background-color: #4AAE9B;
  color: white;
}
</style>