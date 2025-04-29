<template>
  <HeaderLayout />

  <div class="schedule-view-wrapper">
    <div class="schedule-view">
      <div class="schedule-view__header">
        <h1>Ротация дежурных</h1>

        <div class="rotation-settings">
          <label for="rotation-select">Ротация лида:</label>
          <select
            id="rotation-select"
            v-model="selectedRule"
            @change="handleRuleChange"
            :disabled="!isLead"
          >
            <option value="0">Лид не дежурит</option>
            <option value="1">Лид всегда дежурный</option>
            <option value="2">Лид в ротации</option>
          </select>
        </div>
      </div>

      <table class="schedule-table">
        <thead>
        <tr>
          <th></th>
          <th v-for="date in scheduleDates" :key="date">{{ formatDate(date) }}</th>
        </tr>
        </thead>
        <tbody>
        <tr>
          <td class="team-name">Команда</td>
          <template v-for="(group, groupIndex) in userScheduleGrouped">
            <td
              v-if="group.rowspan > 0"
              :key="groupIndex"
              :colspan="group.rowspan"
            >
              <div
                class="duty-bar"
                :style="{ backgroundColor: getColorForIndex(group.colorIndex) }"
              >
                {{ userIdToNameMap[group.userId] || '-' }}
              </div>
            </td>
          </template>
        </tr>

        <tr v-if="selectedRule == 1 && teamStore.teamData?.lead">
          <td class="team-name">Лид</td>
          <td :colspan="scheduleDates.length">
            <div
              class="duty-bar"
              :style="{ backgroundColor: getColorForIndex(99) }"
            >
              {{ userIdToNameMap[teamStore.teamData.lead.id] || 'Лид' }}
            </div>
          </td>
        </tr>
        </tbody>
      </table>
    </div>
  </div>
</template>

<script setup>
import HeaderLayout from '@/components/layout/HeaderLayout.vue';
import {onMounted, ref, computed, watch} from 'vue';
import {getSchedule, updateLeadRotationRule} from '@/api/scheduleApi.js';
import { useTeamStore } from '@/stores/teamStore';
import dayjs from 'dayjs';
import {getUserId} from "@/api/profileApi.js";

const teamStore = useTeamStore();
const scheduleItems = ref([]);
const userSchedule = ref({});
const scheduleDates = ref([]);
const teamName = ref('');
const selectedRule = ref(0);

const colorCycle = [
  'var(--additional-green)',
  'var(--additional-red)',
  'var(--additional-yellow)',
  'var(--additional-orange)',
];

const getColorForIndex = (index) => colorCycle[index % colorCycle.length];

const userIdToNameMap = computed(() => {
  const map = {};
  if (teamStore.teamData) {
    const { lead, workers } = teamStore.teamData;

    const formatFullName = (user) => {
      if (!user) return '-';
      const firstInitial = user.firstName ? user.firstName[0] + '.' : '';
      const lastInitial = user.secondName ? user.secondName[0] + '.' : '';
      return `${user.surname || ''} ${firstInitial}${lastInitial}`.trim();
    };

    if (lead) map[lead.id] = formatFullName(lead);
    if (workers && Array.isArray(workers)) {
      workers.forEach(worker => {
        map[worker.id] = formatFullName(worker);
      });
    }
  }
  return map;
});

const formatDate = (date) => dayjs(date).format('DD.MM');
const formatDateKey = (date) => dayjs(date).format('YYYY-MM-DD');

const userScheduleGrouped = computed(() => {
  const result = [];
  let lastUserId = null;
  let currentSpan = 0;
  let colorIndex = -1;

  for (let i = 0; i < scheduleDates.value.length; i++) {
    const date = scheduleDates.value[i];
    const dateKey = formatDateKey(date);
    const userId = userSchedule.value[dateKey];

    if (userId === lastUserId) {
      currentSpan++;
    } else {
      if (currentSpan > 0) {
        result.push({ userId: lastUserId, rowspan: currentSpan, colorIndex });
      }
      colorIndex++;
      currentSpan = 1;
      lastUserId = userId;
    }
  }

  if (currentSpan > 0) {
    result.push({ userId: lastUserId, rowspan: currentSpan, colorIndex });
  }

  return result;
});

const loadSchedule = async () => {
  const response = await getSchedule();
  scheduleItems.value = response.scheduleItems;

  const startDate = dayjs().startOf('day');
  const endDate = startDate.add(13, 'day');
  const dates = [];

  for (let d = startDate; d.isBefore(endDate) || d.isSame(endDate); d = d.add(1, 'day')) {
    dates.push(d.toDate());
  }

  scheduleDates.value = dates;

  const map = {};
  for (const item of scheduleItems.value) {
    const from = dayjs(item.from);
    const to = dayjs(item.to);

    for (const date of dates) {
      const d = dayjs(date);
      if (d.isSame(from) || (d.isAfter(from) && d.isBefore(to))) {
        map[d.format('YYYY-MM-DD')] = item.userId;
      }
    }
  }

  userSchedule.value = map;
};

const handleRuleChange = async () => {
  try {
    await updateLeadRotationRule(selectedRule.value);
    await teamStore.fetchTeamData();
    await loadSchedule();
  } catch (e) {
    console.error('Ошибка при обновлении правила ротации лида:', e);
  }
};

const isLead = ref(false);

const checkCurrentUser = async () => {
  const currentUserId = await getUserId();

  return teamStore.teamData?.lead?.id === currentUserId.currentUserId;
};

onMounted(async () => {
  await teamStore.fetchTeamData();
  teamName.value = teamStore.teamData.name
  await loadSchedule();
  selectedRule.value = teamStore.teamData?.leadRotationRule ?? 0;
  isLead.value = await checkCurrentUser();
});
</script>

<style scoped>
.schedule-view-wrapper {
  display: flex;
  justify-content: center;
  align-items: center;
}

.schedule-view {
  display: flex;
  flex-direction: column;
  gap: 40px;
  padding: 40px 50px;
  margin-top: 80px;
  width: 90vw;

  background: var(--primary-background-color);
  border-radius: 20px;
  box-shadow: 0 10px 15px rgba(0, 0, 0, 0.1);
}

.schedule-view__header {
  display: flex;
  align-items: center;
  justify-content: space-between;
}

.schedule-table {
  width: 100%;
  border-collapse: collapse;
  table-layout: fixed;
  text-align: center;
}

.schedule-table th,
.schedule-table td {
  padding: 12px 1px;
  word-break: break-word;
}

.schedule-table th {
  background-color: #f3f3f3;
}

.team-name {
  font-weight: bold;
  background-color: #f9f9f9;
}

.duty-bar {
  margin: 6px 0;
  padding: 8px;
  border-radius: 6px;
  color: white;
  font-weight: bold;
}

.schedule-table tr:nth-child(2) .duty-bar {
  margin: 6px 0;
  padding: 8px;
  border-radius: 6px;
  color: white;
  font-weight: bold;
}
</style>
