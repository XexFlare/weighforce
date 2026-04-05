<template>
  <div>
    <div id="chart">
      <apexchart
        type="radialBar"
        width=380
        :options="options"
        :series="series"
      ></apexchart>
    </div>
  </div>
</template>
<script>
export default {
  inject: ["authHeaders"],
  props: ["type", "chart"],
  computed: {
    series() {
      const max = Math.max(...this.chart.series);
      const offset = Math.ceil((max / 100) * 10);
      const valueToPercent = (val) => Math.floor((val * 100) / (max + offset));
      return this.chart.series.map((val) => valueToPercent(val));
    },
    options() {
      const max = Math.max(...this.chart.series);
      const offset = Math.ceil((max / 100) * 10);
      return {
        ...this.chartOptions,
        labels: this.chart.labels,
        title: {
          text: `Trucks ${this.type} in all locations`,
          align: "center",
          style: { color: "#4d7d77" },
        },
        // chartOptions: {
        legend: {
          ...this.chartOptions.legend,
          formatter: function (seriesName, opts) {
            const val = Math.ceil(
              (opts.w.globals.series[opts.seriesIndex] * (max + offset)) / 100
            );
            return seriesName + ":  " + val;
          },
        },
        plotOptions: {
          ...this.chartOptions.plotOptions,
          radialBar: {
            ...this.chartOptions.plotOptions.radialBar,
            dataLabels: {
              name: {
                show: true,
              },
              value: {
                show: true,
                formatter: (val) => Math.ceil((val * (max + offset)) / 100),
              },
            },
          },
        },
      };
    },
  },
  data: function () {
    return {
      chartOptions: {
        chart: {
          height: 350,
          width: 380,
          type: "radialBar",
        },
        plotOptions: {
          radialBar: {
            offsetY: -20,
            startAngle: 0,
            endAngle: 270,
            hollow: {
              margin: 5,
              size: "30%",
              background: "transparent",
              image: undefined,
            },
            dataLabels: {},
          },
        },
        theme: {
          monochrome: {
            enabled: true,
            color: "#4d7d77",
          },
        },
        legend: {
          show: true,
          floating: true,
          position: "left",
          offsetX: 0,
          labels: {
            useSeriesColors: true,
          },
        },
      },
    };
  },
};
</script>