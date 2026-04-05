<template>
  <div>
    <apexchart
      v-if="chart"
      width="500"
      type="bar"
      :options="options"
      :series="chart.series"
    ></apexchart>
  </div>
</template>
<script>
export default {
  inject: ["authHeaders"],
  props: ["chart", "seriesName", "title"],
  computed: {
    series() {
      return this.chart.series;
    },
    options() {
      return {
        title: {
          text: this.title,
          align: "center",
          style: { color: "#4d7d77" },
          align: "left",
          offsetX: 110,
        },
        xaxis: {
          categories: this.chart.labels,
        },
        colors: ["#4d7d77", "#749994"],
        plotOptions: {
          bar: {
            borderRadius: 5,
            columnWidth: "95%",
            dataLabels: {
              position: "top",
              hideOverflowingLabels: true,
            },
          },
        },
        dataLabels: {
          enabled: true,
          enabledOnSeries: [0,1],
        },
        yaxis: [
          {
            labels: {
              style: {
                colors: "#4d7d77",
              },
            },
            title: {
              style: {
                color: "#4d7d77",
              },
            },
          },
          {
            seriesName: "Tonnage",
            floating: false,
            opposite: true,
            labels: {
              style: {
                colors: "#749994",
              },
            },
            title: {
              style: {
                color: "#749994",
              },
            },
          },
        ],
        tooltip: {
          fixed: {
            enabled: true,
            position: "topLeft", // topRight, topLeft, bottomRight, bottomLeft
            offsetY: 30,
            offsetX: 60,
          },
        },
        legend: {
          horizontalAlign: "left",
          offsetX: 40,
        },
      };
    },
  },
};
</script>