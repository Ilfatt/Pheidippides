<template>
  <div class="user-item">
    <div class="user-item__user-info">
      <div class="user-info__name-wrapper">
        <p class="user-info__name">{{ formattedFullName }}</p>
        <p class="user-info__id">#{{ userData?.id }}</p>
      </div>
    </div>

    <div class="user-item__connection-item-wrapper">
      <div class="user-item__connection-item">
        <span class="connection-item__label">Телефон: </span>
        <div class="connection-item__data-wrapper">
          <span
            class="connection-item__data"
            @click="handleClick('phone', userData?.phoneNumber)"
          >
            {{ showData.phone ? userData?.phoneNumber : '••••••••' }}
          </span>
          <span
            v-if="copiedData.phone"
            class="copied-tooltip"
          >
            Скопировано!
          </span>
        </div>
      </div>

      <div v-if="userData?.email" class="user-item__connection-item">
        <span class="connection-item__label">Email: </span>
        <div class="connection-item__data-wrapper">
          <span
            class="connection-item__data"
            @click="handleClick('email', userData?.email)"
          >
            {{ showData.email ? userData?.email : '••••••••' }}
          </span>
          <span
            v-if="copiedData.email"
            class="copied-tooltip"
          >
            Скопировано!
          </span>
        </div>
      </div>

      <div v-if="userData?.yandexOAuthToken" class="user-item__connection-item">
        <span class="connection-item__label">YandexID: </span>
        <div class="connection-item__data-wrapper">
          <span
            class="connection-item__data"
            @click="handleClick('yandex', 'connected')"
          >
            {{ showData.yandex ? 'connected' : '••••••' }}
          </span>
          <span
            v-if="copiedData.yandex"
            class="copied-tooltip"
          >
            Скопировано!
          </span>
        </div>
      </div>

      <div v-if="userData?.yandexScenarioName" class="user-item__connection-item">
        <span class="connection-item__label">Сценарий: </span>
        <div class="connection-item__data-wrapper">
          <span
            class="connection-item__data"
            @click="handleClick('scenario', userData?.yandexScenarioName)"
          >
            {{ showData.scenario ? userData.yandexScenarioName : '••••••••' }}
          </span>
          <span
            v-if="copiedData.scenario"
            class="copied-tooltip"
          >
            Скопировано!
          </span>
        </div>
      </div>
    </div>
  </div>
</template>

<script setup>
import { ref, computed } from 'vue';

const props = defineProps({
  userData: {},
});

const showData = ref({
  phone: false,
  email: false,
  yandex: false,
  scenario: false,
});

const copiedData = ref({
  phone: false,
  email: false,
  yandex: false,
  scenario: false,
});

const toggleVisibility = (key) => {
  showData.value[key] = !showData.value[key];
};

const copyToClipboard = async (text) => {
  try {
    await navigator.clipboard.writeText(text);
    console.log('Скопировано:', text);
  } catch (err) {
    console.error('Ошибка копирования:', err);
  }
};

const handleClick = async (key, text) => {
  if (!showData.value[key]) {
    toggleVisibility(key);
  } else {
    await copyToClipboard(text);
    copiedData.value[key] = true;
    setTimeout(() => {
      copiedData.value[key] = false;
    }, 2000); // Показывать "Скопировано!" на 2 секунды
  }
};

const formattedFullName = computed(() => {
  if (!props.userData || !props.userData.surname) return '';

  const firstInitial = props.userData.firstName ? props.userData.firstName[0] + '.' : '';
  const secondInitial = props.userData.secondName ? props.userData.secondName[0] + '.' : '';

  return `${props.userData.surname} ${firstInitial} ${secondInitial}`;
});
</script>

<style scoped>
.user-item {
  display: flex;
  flex-direction: column;
  gap: 5px;
  width: 250px;
  height: 150px;
  padding: 10px;

  border: 1px solid var(--input-border);
  border-radius: 10px;
  background: var(--highlight-background-color);
  box-shadow: 0 10px 15px rgba(0, 0, 0, 0.1);
}

.user-info__name-wrapper {
  display: flex;
  gap: 5px;
}

.user-info__name,
.user-info__id {
  display: flex;
  align-items: center;
  font-size: 20px;
}

.user-info__name {
  font-weight: bold;
}

.user-info__id {
  opacity: 0.5;
}

.connection-item__data-wrapper {
  position: relative;
  display: inline-block;
}

.connection-item__data {
  cursor: pointer;
  user-select: none;
  transition: opacity 0.3s;
}

.connection-item__data:hover {
  opacity: 0.7;
}

.copied-tooltip {
  position: absolute;
  top: -22px;
  left: 50%;
  transform: translateX(-50%);
  background-color: #333;
  color: #fff;
  padding: 3px 6px;
  border-radius: 5px;
  font-size: 10px;
  white-space: nowrap;
  opacity: 0.9;
}
</style>
