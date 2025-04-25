<template>
  <HeaderLayout />
  <div class="profile-container-wrapper">
    <div class="profile-container">
      <div class="profile__content">
        <div class="content__user-info">
          <div class="user-info__name-wrapper">
            <p class="user-info__name">{{ userData.firstName }}</p>
            <p class="user-info__id">#{{ userData.id }}</p>
          </div>
          <div
            v-if="userData.role === 1"
            class="user-info__role"
          >
            <img src="@/assets/img/leadIcon.svg" alt="">
            <span>lead</span>
          </div>
          <div
            v-else
            class="user-info__role"
          >
            <img src="@/assets/img/stuffIcon.svg" alt="">
            <span>stuff</span>
          </div>
        </div>
        <div class="content__connection-item-wrapper">
          <div class="content__connection-item">
            <span class="connection-item__label">Телефон: </span>
            <span class="connection-item__data">{{ userData.phoneNumber }}</span>
          </div>
          <div
            v-if="userData.email"
            class="content__connection-item"
          >
            <span class="connection-item__label">Email: </span>
            <span class="connection-item__data">{{ userData.email }}</span>
          </div>
          <div
            v-if="userData.yandexOAuthToken"
            class="content__connection-item"
          >
            <span class="connection-item__label">YandexID: </span>
            <span class="connection-item__data">connected</span>
          </div>
        </div>
      </div>

      <div
        class="profile__edit-button"
        @click="showEditModal = true"
      >
        <img src="@/assets/img/editIcon.svg" alt="Edit" />
        <p>
          Редактировать
        </p>
      </div>
    </div>
    <EditProfileModal
        v-if="showEditModal"
        :data="userData"
        @close="showEditModal = false"
        @save="saveEdit"
    />
  </div>
</template>

<script setup>
import HeaderLayout from '@/components/layout/HeaderLayout.vue';
import EditProfileModal from '@/components/modals/EditProfileModal.vue';

import { onMounted, ref } from 'vue';

import { getUserInfo } from '@/api/profileApi.js';
import {useTeamStore} from "@/stores/teamStore.js";

const showEditModal = ref(false);
const userData = ref({});

const connectionData = ref({
  phone: { label: 'Телефон', data: '89872525226' },
  email: { label: 'Email', data: null },
  yandexID: { label: 'YandexID', data: null },
});

const saveEdit = (updatedData) => {
  connectionData.value = updatedData
  showEditModal.value = false
};

const getUserData = async () => {
  try {
    userData.value = await getUserInfo();
  } catch (error) {
    console.error(error);
  }
};

const teamStore = useTeamStore();

onMounted(async () => {
  await getUserData();

  teamStore.fetchTeamData();
  console.log(userData.value);
});
</script>

<style scoped>
.profile-container-wrapper{
  display: flex;
  justify-content: center;
  align-items: center;
}

.profile-container {
  display: flex;
  align-items: start;
  justify-content: space-between;
  gap: 40px;
  padding: 40px 50px;
  margin-top: 80px;
  text-align: center;
  width: 70vw;

  background: var(--primary-background-color);
  border-radius: 20px;
  box-shadow: 0 10px 15px rgba(0, 0, 0, 0.1);
}

.profile__edit-button {
  display: flex;
  justify-content: center;
  align-items: center;
  gap: 8px;
  padding: 4px 8px;

  font-size: 20px;

  color: var(--secondary-background-color);
  border: 1px solid var(--secondary-background-color);
  border-radius: 5px;
}

.profile__edit-button:hover {
  cursor: pointer;
}

.content__user-info {
  display: flex;
  flex-direction: column;
  gap: 5px;
  padding-bottom: 30px;

  border-bottom: 1px solid var(--secondary-background-color);
}

.user-info__name-wrapper,
.user-info__role {
  display: flex;
  gap: 5px;
}

.user-info__name,
.user-info__id {
  display: flex;
  align-items: center;
}

.user-info__name {
  font-size: 24px;
  font-weight: bold;
}

.user-info__id {
  font-size: 24px;
  opacity: 0.5;
}

.user-info__role {
  color: var(--link-color);
}

.content__connection-item-wrapper {
  padding-top: 30px;
}
</style>