import axios from 'axios';

const API = axios.create({
    baseURL: 'http://localhost:5282/api',
})


export async function getIncidents() {
    try {
        const response = await API.get('/incident/get_history', {
            headers: {
                'accept': 'text/plain',
                'Authorization': `Bearer ${localStorage.getItem('token')}`
            }
        });

        return response.data;
    } catch (error) {
        console.error('Ошибка при получении инцидентов:', error);
        throw error;
    }
}

export async function incidentAcknowledge(id) {
    try {
        const response = await API.put(`/incident/acknowledge?incidentId=${id}`, null, {
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
