<template>
  <div class="content-wrapper">
    <router-view />
  </div>
</template>

<script setup>
import '@/assets/styles/base.css'
import '@/assets/styles/colors.css'

</script>

<style scoped>
.content-wrapper {
  height: 100vh;
  width: 100vw;

  background: url('@/assets/img/mainBackground.jpg') no-repeat center center fixed;
  background-size: cover;
}
</style>