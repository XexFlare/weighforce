<template>
  <li class="px-3 py-1 ml-2 text-sm my-2 lg:pl-2 lg:pr-1 lg:self-center hover:bg-accent rounded-md transition duration-500 ease-in-out ">
    <router-link class="font-medium text-gray-400 antialiased hover:text-white"
      aria-current="page" :to="link"><slot /></router-link>
  </li>
</template>

<script>
export default {
  props: ["title", "link"],
};
</script>
<style>

.nav-link {
  @apply px-3 py-1 ml-2 self-center text-sm font-medium text-gray-400 antialiased rounded-md transition duration-500 ease-in-out;
}
</style>