import axios from 'axios';

import { defineStore } from 'pinia';
import { ref } from 'vue';

export const useTeamStore = defineStore('team', () => {
    const teamData = ref(null);

    const fetchTeamData = async () => {
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
        }
    }

    return {
        teamData,
        fetchTeamData,
    }
})
