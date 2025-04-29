<template>
  <HeaderLayout />
  <div class="team-container-wrapper">
    <div class="team-container">
      <div class="team-container__header">
        <p class="header__team-name">{{ teamData.name }}</p>

        <div class="header__codes-copy">
          <div class="codes-copy__code">
            <label>Код приглашения:</label>
            <div class="code-container">
              <span>∗∗∗∗∗∗</span>
              <div class="tooltip-container">
                <img
                  class="tooltip__copy-icon"
                  src="@/assets/img/copyIcon.png" alt="Копировать"
                  @click="copyToClipboard(teamData.inviteToken, 'invite')"
                >
                <span class="tooltip-text" v-if="showTooltip.invite">{{ tooltipText.invite }}</span>
              </div>
            </div>
          </div>

          <div class="codes-copy__code">
            <label>Код для создания инцидента:</label>
            <div class="code-container">
              <span>∗∗∗∗∗∗</span>
              <div class="tooltip-container">
                <img
                  class="tooltip__copy-icon"
                  src="@/assets/img/copyIcon.png" alt="Копировать"
                  @click="copyToClipboard(teamData.incidentCreateToken, 'incident')"
                >
                <span class="tooltip-text" v-if="showTooltip.incident">{{ tooltipText.incident }}</span>
              </div>
            </div>
          </div>
        </div>
      </div>

      <div class="team-container__members">
        <div class="members__lead">
          <p class="members-label">Лидер</p>
          <UserInfoItem
            :userData="teamStore.teamData?.lead"
          />
        </div>

        <p class="members-label">Команда</p>
        <div class="members__command">
          <UserInfoItem
            v-for="stuff in personalData"
            :key="stuff.id"
            class="members-item"
            :userData="stuff"
          />
        </div>
      </div>
    </div>
  </div>
</template>

<script setup>
import HeaderLayout from '@/components/layout/HeaderLayout.vue';
import { onMounted, ref } from 'vue';
import { useTeamStore } from "@/stores/teamStore.js";
import UserInfoItem from "@/components/UserInfoItem.vue";

const teamStore = useTeamStore();

const teamData = ref({});
const personalData = ref([]);

const showTooltip = ref({
  invite: false,
  incident: false,
})

const tooltipText = ref({
  invite: '',
  incident: '',
})

const getUsersData = async () => {
  try {
    await teamStore.fetchTeamData();
    teamData.value = teamStore.teamData;
    personalData.value = teamStore.teamData?.workers;
  } catch (error) {
    console.error(error);
  }
};

const copyToClipboard = async (text, type) => {
  try {
    await navigator.clipboard.writeText(text);
    tooltipText.value[type] = 'Скопировано!';
    showTooltip.value[type] = true;

    setTimeout(() => {
      showTooltip.value[type] = false;
    }, 2000);
  } catch (err) {
    console.error('Ошибка копирования:', err);
  }
};

onMounted(async () => {
  await teamStore.fetchTeamData();
  await getUsersData();
});
</script>

<style scoped>
.team-container-wrapper {
  display: flex;
  justify-content: center;
  align-items: center;
}

.team-container {
  display: flex;
  flex-direction: column;
  align-items: start;
  gap: 10px;
  padding: 40px 50px;
  margin: 80px 0;
  width: 80vw;

  background: var(--primary-background-color);
  border-radius: 20px;
  box-shadow: 0 10px 15px rgba(0, 0, 0, 0.1);
}

.team-container__header {
  display : flex;
  flex-direction: column;
  gap: 20px;
  padding-bottom: 30px;

  border-bottom: 1px solid var(--secondary-background-color);
}

.header__team-name {
  font-size: 32px;
  font-weight: bold;
}

.header__codes-copy {
  display: flex;
  gap: 40px;
}

.codes-copy__code {
  display: flex;
  gap: 10px;
  align-items: baseline;
}

.tooltip__copy-icon {
  width: 20px;

  cursor: pointer;
}

.code-container {
  display: flex;
  align-items: center;
  justify-content: center;
  gap: 5px;
}

.code-container span{
  display: inline-block;
  vertical-align: sub;
}

.tooltip-container {
  position: relative;
  display: inline-block;
}

.tooltip-text {
  visibility: visible;
  background-color: black;
  color: #fff;
  text-align: center;
  padding: 5px 8px;
  border-radius: 6px;
  position: absolute;
  top: -35px;
  left: 50%;
  transform: translateX(-50%);
  white-space: nowrap;
  font-size: 12px;
  opacity: 0.9;
}

.members__lead {
  padding-bottom: 30px;
}

.members-label {
  font-size: 20px;
  font-weight: bold;
  padding-bottom: 10px;
}

.members__command {
  display: flex;
  flex-direction: row;
  gap: 20px;
  flex-wrap: wrap;
}
</style>
