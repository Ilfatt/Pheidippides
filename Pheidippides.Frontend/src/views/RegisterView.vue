<template>
  <AuthLayout
    auth-title="Регистрация"
    auth-description="Вы хотите создать или вступить в команду?"
  >
    <div class="auth-switcher">
      <button
          :class="{ active: method === 'create' }"
          @click="method = 'create'"
      >
        Создать
      </button>
      <button
          :class="{ active: method === 'join' }"
          @click="method = 'join'"
      >
        Вступить
      </button>
    </div>

    <input
        v-if="method === 'create'"
        class="auth-input"
        v-model="userData.teamName"
        type="text"
        placeholder="название команды"
    />
    <input
        v-else
        class="auth-input"
        v-model="userData.teamInviteCode"
        type="text"
        placeholder="код команды"
    />

    <input
        class="auth-input"
        v-model="userData.userName"
        type="text"
        placeholder="имя пользователя"
    />
    <input
        class="auth-input"
        v-model="userData.phoneNumber"
        type="text"
        placeholder="номер телефона"
    />
    <input
      class="auth-input"
      v-model="userData.phoneActivationCode"
      type="text"
      placeholder="код подверждения"
    />
    <input
        class="auth-input"
        v-model="userData.password"
        type="password"
        placeholder="пароль"
    />
    <p class="auth-error">
      {{ errorMassage }}
    </p>
    <button
        class="auth-button"
        @click="registerUser"
    >
      Зарегестрироваться
    </button>
    <div>
      Уже есть аккаунт?
      <router-link
          class="auth-link"
          to="/login"
      >
        Авторизоваться
      </router-link>
    </div>
  </AuthLayout>
</template>

<script setup>
import AuthLayout from '@/components/layout/AuthLayout.vue';

import { ref } from 'vue';
import { useRouter } from 'vue-router'

import { useAuthStore } from '@/stores/authStore.js';

const userData = ref({
  teamName: null,
  teamInviteCode: null,
  userName: null,
  phoneNumber: null,
  phoneActivationCode: null,
  password: null,
});

const method = ref('create');
const router = useRouter();
const auth = useAuthStore();
const errorMassage = ref('');

const registerUser = async () => {
  const dataToSend = {
    FirstName: userData.value.userName,
    SecondName: userData.value.userName,
    Surname: userData.value.userName,
    PhoneNumber: userData.value.phoneNumber,
    PhoneActivationCode: userData.value.phoneActivationCode,
    Password: userData.value.password,
  };

  if (method.value === 'create') {
    dataToSend.TeamName = userData.value.teamName;
  } else if (method.value === 'join') {
    dataToSend.TeamInviteCode = userData.value.teamInviteCode;
  }

  try {
    await auth.registerUser(dataToSend);

    router.push('/schedule');
  } catch (e) {
    errorMassage.value = e;
  }
}
</script>

<style scoped>
.auth-switcher {
  display: flex;
  margin-bottom: 15px;
  justify-content: center;
}

.auth-switcher button {
  padding: 2px 30px;
  border: 1px solid var(--secondary-text-color);
  background: transparent;
  color: var(--primary-text-color);
  cursor: pointer;
  transition: 0.2s ease;
}

.auth-switcher button:first-child {
  border-radius: 10px 0 0 10px;
}

.auth-switcher button:last-child {
  border-radius: 0 10px 10px 0;
}

.auth-switcher button.active {
  background: var(--secondary-background-color);
  color: var(--primary-background-color);
}

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