<template>
  <div class="ctn flex items-center justify-center">
    <div class="ani">
      <div class="stuff"></div>
    </div>
  </div>
</template>
<style scoped>
.ctn {
    border-radius: 100%;
    height: 100px;
}
  .stuff {
    border-radius: 100%;
    animation: grow 3s linear infinite;
  }
  .ani {
    border-radius: 100%;
    border: 4px solid #5d5;
    border-top: 6px solid #3f3;
    animation: spin 2s linear infinite;
  }
  @keyframes spin {
    0% {
      transform: rotate(0deg);
    }
    100% {
      transform: rotate(360deg);
    }
  }
  @keyframes grow {
    0% {
      width: 50px;
      height: 50px;
    }
    47% {
      width: 50px;
      height: 50px;
    }
    50% {
      width: 65px;
      height: 65px;
    }
    53% {
      width: 50px;
      height: 50px;
    }
    100% {
      width: 50px;
      height: 50px;
    }
  }
</style>