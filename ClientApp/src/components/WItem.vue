<template>
  <div v-show="value" class="item">
    <h3 class="font-bold text-primary">{{ title }}</h3>
    <p class="h-4">
      {{
        isDate
          ? value != null
            ? new Date(value).toLocaleDateString("en-GB", {
                day: "numeric",
                month: "short",
                year: "numeric",
                hour: "numeric",
                minute: "numeric",
              })
            : ""
          : value
      }}
    </p>
  </div>
</template>
<style scoped>
.item {
  padding-bottom: 10px;
}
</style>
<script>
export default {
  props: ["title", "value", "isDate"],
};
</script>