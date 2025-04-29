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
  min-height: 100vh;
  width: 100%;

  background: url('@/assets/img/mainBackground.jpg') no-repeat center center fixed;
  background-size: 100% 100%;
}
</style>