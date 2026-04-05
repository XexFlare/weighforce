import vue from '@vitejs/plugin-vue'

export default {
    plugins: [vue()],
    optimizeDeps: {
        include: ['apexcharts', 'vue3-apexcharts']
    }
}