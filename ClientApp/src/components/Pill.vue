<template>
  <div
    class="group inline px-1 py-1 bg-blue-200 rounded mr-1 hover:bg-blue-300 transition duration-200 ease-in-out"
  >
    <button
      @click="remove(email, name)"
      class="hidden rounded-lg m-0 px-1 group-hover:inline hover:bg-gray-300 transition duration-200 ease-in-out"
    >
      -
    </button>
    {{ name }}
  </div>
</template>
<script>
export default {
  props: ['email','name','remove']
}
</script>