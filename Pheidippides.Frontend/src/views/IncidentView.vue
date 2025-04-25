<template>
  <HeaderLayout />
  <div class="incident-container-wrapper">
    <IncidentItem
      v-for="incident in incidentsData"
      :key="incident.id"
      :incident-data="incident"
      @update-incident-acknowledge="getIncidentData()"
    />
  </div>
</template>

<script setup>
import HeaderLayout from '@/components/layout/HeaderLayout.vue';
import IncidentItem from '@/components/IncidentItem.vue';

import { onMounted, ref } from 'vue';

import {getIncidents} from '@/api/incidentApi.js';

const incidentsData = ref(null);

const getIncidentData = async () => {
  try {
    incidentsData.value = await getIncidents();
  } catch (e) {
    alert('Ошибка входа: ' + e.message);
  }
};

onMounted(async () => {
  await getIncidentData();
});
</script>

<style scoped>
.incident-container-wrapper{
  display: flex;
  flex-direction: column;
  align-items: center;
  justify-content: center;
}

</style>