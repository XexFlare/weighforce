<template>
  <div>
    <apexchart
      v-if="chart"
      width="430"
      type="bar"
      :options="options"
      :series="series"
    ></apexchart>
  </div>
</template>
<script>
export default {
  inject: ["authHeaders"],
  props: ["chart","seriesName", "title"],
  computed: {
    series() {
      return [
        {
          name: this.seriesName,
          data: this.chart.series,
        },
      ];
    },
    options() {
      const max = Math.max(this.chart.series);
      return {
        xaxis: {
          categories: this.chart.labels,
        },
        yaxis: {
          floating: false,
          opposite: true,
        },
        offsetY: {
          max: max >= 6 ? max : 6,
          tickAmount: Math.round(max / 2),
        },
        title: {
          text: this.title,
          align: "center",
          style: { color: "#4d7d77" },
        },
        colors: ["#4d7d77", "#ff00ff"],
      };
    },
  },
};
</script>