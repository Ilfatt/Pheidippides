import axios from 'axios';

const API = axios.create({
    baseURL: 'http://localhost:5282/api',
});

export async function getSchedule() {
    try {
        const response = await API.get('/schedule/get', {
            headers: {
                'Authorization': `Bearer ${localStorage.getItem('token')}`
            }
        });

        return response.data;
    } catch (error) {
        console.error('Ошибка при получении расписания:', error);
        throw error;
    }
}

export async function updateLeadRotationRule(rule) {
    try {
        const response = await API.put(`/team/set_lead_rotation_rule?leadRotationRule=${rule}`, null, {
            headers: {
                'Authorization': `Bearer ${localStorage.getItem('token')}`
            }
        });

        return response.data;
    } catch (error) {
        console.error('Ошибка при получении расписания:', error);
        throw error;
    }
}
