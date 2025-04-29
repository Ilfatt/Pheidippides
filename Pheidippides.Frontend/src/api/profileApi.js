import axios from 'axios';

import { useTeamStore } from '@/stores/teamStore'

const API = axios.create({
    baseURL: 'http://localhost:5282/api',
})


export async function getUserId() {
    try {
        const response = await API.get('/user/get_current_user', {
            headers: {
                'Authorization': `Bearer ${localStorage.getItem('token')}`
            }
        });

        return response.data;
    } catch (error) {
        console.error('Ошибка при получении инцидентов:', error);
        throw error;
    }
}

export const getUserInfo = async () => {
    const teamStore = await useTeamStore();
    const userData = await getUserId();

    const allMembers = [
        teamStore.teamData?.lead,
        ...(teamStore.teamData?.workers || [])
    ];

    const currentUser = allMembers.find(member => member?.id === userData.currentUserId);

    return currentUser || null;
}

export async function updateProfileInfo(profileData) {
    try {
        const response = await API.put(`/user/update_profile_info`, profileData, {
            headers: {
                'accept': 'text/plain',
                'Authorization': `Bearer ${localStorage.getItem('token')}`
            }
        });

        return response.data;
    } catch (error) {
        console.error('Ошибка дачи ответа:', error);
        throw error;
    }
}
