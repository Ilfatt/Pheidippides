<template>
  <div class="auth-container-wrapper">
    <div class="auth-container">
      <h1 class="auth-container__title">
        {{ authTitle }}
      </h1>
      <p class="auth-container__subtitle">
        {{ authDescription }}
      </p>

      <div class="auth-container_content">
        <slot />
      </div>
    </div>
  </div>
</template>

<script setup>
defineProps({
  authTitle: String,
  authDescription: String,
});

</script>

<style scoped>
.auth-container-wrapper {
  display: flex;
  justify-content: center;
  align-items: center;
  height: 100vh;
  width: 100vw;

  background: url('@/assets/img/authBackground.jpg') no-repeat center center fixed;
  background-size: cover;
}

.auth-container {
  display: flex;
  flex-direction: column;
  align-items: center;
  padding: 40px 50px;
  text-align: center;
  width: 480px;

  background: var(--primary-background-color);
  border-radius: 20px;
  box-shadow: 0 10px 15px rgba(0, 0, 0, 0.1);
}

.auth-container__title {
  font-size: 48px;
  color: var(--primary-text-color);
  font-weight: bold;
}

.auth-container__subtitle {
  width: 70%;

  color: var(--secondary-text-color);
  margin-bottom: 30px;
}

.auth-container_content {
  display: flex;
  flex-direction: column;
  align-items: center;
  gap: 10px;
  width: 100%;
}

button {
  background: #c58d5f;
  color: white;
  border: none;
  padding: 12px 25px;
  font-size: 16px;
  border-radius: 8px;
  cursor: pointer;
  box-shadow: 0 4px #9e6f3f;
  transition: all 0.2s ease;
}

button:hover {
  background: #b17c4f;
}
</style>
