import axios from 'axios';

import { defineStore } from 'pinia';
import { ref } from 'vue';

export const useTeamStore = defineStore('team', () => {
    const teamData = ref(null);
    const loading = ref(false);
    const error = ref(null);

    const fetchTeamData = async () => {
        loading.value = true;
        error.value = null;
        try {
            const response = await axios.get('http://localhost:5282/api/team/get', {
                headers: {
                    'accept': 'text/plain',
                    'Authorization': `Bearer ${localStorage.getItem('token')}`
                }
            });
            teamData.value = response.data;
        } catch (err) {
            console.error('Ошибка при получении команды:', err);
            error.value = err;
        } finally {
            loading.value = false;
        }
    }

    return {
        teamData,
        loading,
        error,
        fetchTeamData,
    }
})
