<template>
  <table class="text-left table-collapse w-full">
    <thead class="">
      <tr class="bg-gray-100 border-b border-t border-border-gray-900">
        <th v-for="(item, index) in headings" v-bind:key="item.title" class="td" :class="item.headClass">
          <slot :name="'heading-' + item.value">
            <div @click="sortField(item.title)" class="mx-2">
              {{ item.title }}
            </div>
          </slot>
        </th>
      </tr>
    </thead>
    <tbody class="align-baseline">
      <tr v-for="(row, index) in items" v-bind:key="row" @click="selectRow(row.data)" :key="row.data.id"
        :class="{ 'bg-green-100': (row.data.id == selected), 'bg-blue-100': (row.data.id != selected)}">
        <td v-for="(item, index) in row.values" v-bind:key="item + ';' + row.data.id" class="
            px-1
            py-2
            font-mono
            text-sm text-blue-700
            whitespace-nowrap
            border-t border-gray-200
          " :class="headings[index].class">
          <slot :item="row.data" :name="headings[index].slotname ?? headings[index].value">
            <span v-if="links != null">
              <router-link :to="`${links}/${row.data.id}`">
                {{ item }}</router-link>
            </span>
            <span v-else>
              {{ item }}
            </span>
          </slot>
        </td>
      </tr>
    </tbody>
  </table>
</template>
<script>
export default {
  props: ["headings", "rows", "color", "bgColor", "links", "defaultSort",'onSelect'],
  data() {
    return {
      sortBy: this.defaultSort ?? 0,
      asc: false,
      selected: null,
      sortType: "int",
    };
  },
  computed: {
    items() {
      return this.rows
        ?.map((row) => {
          return {
            values: this.headings.map((h) => {
              const value =
                h.sub != null
                  ? row[h.sub] != null
                    ? row[h.sub][h.value] ?? ""
                    : "" ?? ""
                  : row[h.value] ?? "";
              return h.isDate ?? false & (value != "")
                ? new Date(value)
                  .toLocaleDateString("en-GB", {
                    day: "numeric",
                    month: "short",
                    year: "numeric",
                  })
                  .split(" ")
                  .join("-")
                : value;
            }),
            data: row,
          };
        })
        .sort(this.sort);
    },
  },
  methods: {
    selectRow(row) {
      this.selected = row.id;
      this.onSelect(row)
    },
    colorize(param, index) {
      return this.color != null ? this.color(param) : index % 2;
    },
    sortField(title) {
      const index = this.headings.findIndex(function (post, index) {
        if (post.title == title) return true;
      });
      this.sortType =
        this.headings[index].isDate ?? false
          ? "date"
          : this.items.length != 0
            ? typeof this.items[0].values[index]
            : "number";
      if (this.sortBy == index) this.asc = !this.asc;
      else {
        this.sortBy = index;
        this.asc = true;
      }
    },
    sort(a, b) {
      return this.sortType == "date"
        ? this.asc
          ? new Date(a.values[this.sortBy]) > new Date(b.values[this.sortBy])
            ? 1
            : -1
          : new Date(a.values[this.sortBy]) < new Date(b.values[this.sortBy])
            ? 1
            : -1
        : this.sortType == "string"
          ? this.asc
            ? ("" + a.values[this.sortBy]).localeCompare(b.values[this.sortBy])
            : ("" + b.values[this.sortBy]).localeCompare(a.values[this.sortBy])
          : this.asc
            ? a.values[this.sortBy] - b.values[this.sortBy]
            : b.values[this.sortBy] - a.values[this.sortBy];
    },
  },
};
</script>
<style scoped>
.highlight {
  background-color: red;
}

tr:hover {
  cursor: pointer;
}
</style>