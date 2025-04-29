<template>
  <div class="incident-item">
    <div class="incident-item__incident-info">
      <div class="incident-item__incident-info-left">
        <p class="incident-info-left__title">{{ incidentData.title }}</p>
        <p class="incident-info-left__description">{{ incidentData.description }}</p>
      </div>

      <div class="incident-item__incident-info-right">
        <div class="incident-info-right__time">
          <p>открыт: {{ convertDate(incidentData.createdAt) }}</p>
        </div>

        <div
          class="incident-info-right__level"
          :class="`incident-info-right__level--${getLevelType()}`"
        >
          {{ getLevelType() }}
        </div>
      </div>
    </div>
    <button
      class="incident-item__incident-info-button"
      v-if="incidentData.needAcknowledgeCurrentUser"
      @click="getIncidentAcknowledge()"
    >
      Акнуть
    </button>
  </div>
</template>

<script setup>
import { format } from 'date-fns';

import { incidentAcknowledge } from '@/api/incidentApi.js';

const props = defineProps({
  incidentData: Object,
});

const emit = defineEmits(["updateIncidentAcknowledge"]);

const getLevelType = () => {
  return props.incidentData.level === 1 ? 'high' : 'low';
};

const convertDate = (dateStr) => {
  const date = new Date(dateStr);

  return format(date, 'dd.MM.yyyy HH:mm');
};

const getIncidentAcknowledge = async () => {
  await incidentAcknowledge(props.incidentData.id);

  emit('updateIncidentAcknowledge');
};
</script>

<style scoped>
.incident-item {
  display: flex;
  flex-direction: column;
  align-items: end;
  margin-top: 50px;
  padding: 20px 50px;

  background: var(--primary-background-color);
  border-radius: 20px;
  box-shadow: 0 10px 15px rgba(0, 0, 0, 0.1);
}

.incident-item__incident-info {
  display: flex;
  justify-content: space-between;
  align-items: center;
  gap: 40px;
  width: 80vw;
}

.incident-item__incident-info-left {
  width: 70%;
}

.incident-info-left__title {
  font-size: 32px;
  font-weight: bold;
}

.incident-info-left__description {
  display: -webkit-box;
  -webkit-line-clamp: 2;
  -webkit-box-orient: vertical;
  overflow: hidden;
  text-overflow: ellipsis;

  opacity: 0.5;
}

.incident-item__incident-info-right {
  display: flex;
  flex-direction: row;
  align-items: center;
  gap: 10px
}

.incident-info-right__time {


  font-size: 14px;

  opacity: 0.5;
}

.incident-info-right__level {
  width: fit-content;
  padding: 3px 10px;

  border-radius: 10px;
}

.incident-info-right__level--high {
  color: var(--additional-red);
  border: 1px solid var(--additional-red);
}

.incident-info-right__level--low {
  color: var(--additional-yellow);
  border: 1px solid var(--additional-yellow);
}

.incident-item__incident-info-button {
  width: fit-content;
  padding: 5px 15px;

  border-radius: 10px;

  color: var(--primary-background-color);
  background: var(--additional-red);

  cursor: pointer;
}
</style>