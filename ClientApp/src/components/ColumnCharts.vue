<template>
  <div id="chart">
    <apexchart
      type="bar"
      height="350"
      :options="options"
      :series="series"
    ></apexchart>
  </div>
</template>
<script>
export default {
  props: ["type", "chart"],

  computed: {
    series() {
      return [
        {
          name: "Dispatched",
          data: this.chart.dispatched,
        },
        {
          name: "Received",
          data: this.chart.received,
        },
      ];
    },
    options() {
      // const max = Math.max(this.chart.series);
      return {
        chart: {
          type: "bar",
          height: 350,
          stacked: true,
          toolbar: {
            show: true,
          },
        },
        theme: {
          monochrome: {
            enabled: true,
            color: "#4d7d77",
          },
        },
        plotOptions: {
          bar: {
            horizontal: false,
          },
        },
        dataLabels: {
          enabled: true,
        },
        stroke: {
          show: true,
          width: 2,
          colors: ["transparent"],
        },
        xaxis: {
          categories: this.chart.labels,
        },
        yaxis: {
          title: {
            text: "$ (thousands)",
          },
        },
        fill: {
          opacity: 1,
        },
        legend: {
          position: "right",
          offsetY: 40,
        },
      };
    },
  },
};
</script>