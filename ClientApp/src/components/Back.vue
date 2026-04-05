<template>
  <div
    @click="$router.back()"
    class="
      flex
      p-2
      rounded
      transition
      hover:bg-accent
      hover:text-white
      mr-4
      cursor-default
    "
  >
    <svg
      xmlns="http://www.w3.org/2000/svg"
      class="h-6 w-6"
      fill="none"
      viewBox="0 0 24 24"
      stroke="currentColor"
    >
      <path
        stroke-linecap="round"
        stroke-linejoin="round"
        stroke-width="2"
        d="M15 19l-7-7 7-7"
      />
    </svg>
    Back
  </div>
</template>