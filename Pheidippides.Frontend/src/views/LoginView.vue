<template>
  <AuthLayout
      auth-title="Авторизация"
      auth-description="Для входа необходимо ввести номер телефона и пароль"
  >
    <input
        class="auth-input"
        v-model="phone"
        type="number"
        placeholder="номер телефона"
    />
    <input
        class="auth-input"
        v-model="password"
        type="password"
        placeholder="пароль"
    />
    <p class="auth-error">
      {{ errorMassage }}
    </p>
    <button
        class="auth-button"
        @click="login"
    >
      Войти
    </button>
    <div>
      Нет аккаунта?
      <router-link
          class="auth-link"
          to="/register"
      >
        Зарегистрироваться
      </router-link>
    </div>
  </AuthLayout>
</template>

<script setup>
import AuthLayout from '@/components/layout/AuthLayout.vue';

import { ref } from 'vue';
import { useRouter } from 'vue-router';

import { useAuthStore } from '@/stores/authStore.js';

const phone = ref('')
const password = ref('')
const router = useRouter()
const auth = useAuthStore()
const errorMassage = ref('');

const login = async () => {
  try {
    await auth.loginUser(phone.value, password.value)
    router.push('/schedule')
  } catch (e) {
    errorMassage.value = e;
  }
}
</script>

<style scoped>
.auth-link {
  color: var(--link-color);
}

.auth-input {
  width: 100%;
  margin-bottom: 15px;
  padding: 12px 15px;

  color: var(--primary-text-color);

  border-radius: 10px;
  box-shadow: inset 4px 4px 10px rgba(0, 0, 0, 0.1);
}

.auth-input::placeholder {
  color: var(--secondary-text-color);
  opacity: 0.5;
}

.auth-error {
  font-size: 14px;
  font-weight: bold;
  color: var(--additional-red);
}

.auth-button {
  width: fit-content;
  background: var(--secondary-background-color);
  color: var(--primary-background-color);
  border: none;
  padding: 4px 60px;
  font-size: 20px;
  border-radius: 10px;
  cursor: pointer;
  transition: all 0.2s ease;
}

.auth-button:hover {
  background: #b17c4f;
}
</style>
