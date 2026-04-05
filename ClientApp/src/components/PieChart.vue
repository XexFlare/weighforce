<template>
  <div>
    <div id="chart">
      <apexchart
        type="pie"
        width="380"
        :options="options"
        :series="chart.series"
      ></apexchart>
    </div>
  </div>
</template>
<script>
export default {
  inject: ["authHeaders"],
  props: ["type", "chart"],
  computed: {
    options() {
      return {
        ...this.chartOptions,
        labels: this.chart.labels,
        title: {
          text: `Trucks ${this.type} in all locations`,
          align: "center",
          style: { color: "#4d7d77" },
        },
      };
    },
  },
  data: function () {
    return {
      chartOptions: {
        chart: {
          width: 380,
          type: "pie",
        },
        theme: {
          monochrome: {
            enabled: true,
            color: "#4d7d77",
          },
        },
        legend: {
          formatter: function (seriesName, opts) {
            return seriesName + ":  " + opts.w.globals.series[opts.seriesIndex];
          },
        },
        dataLabels: {
          enabled: true,
          formatter: function (seriesName, opts) {
            return opts.w.globals.series[opts.seriesIndex];
          },
        },
        responsive: [
          {
            breakpoint: 480,
            options: {
              chart: {
                width: 200,
              },
              legend: {
                position: "bottom",
              },
            },
          },
        ],
      },
    };
  },
};
</script>