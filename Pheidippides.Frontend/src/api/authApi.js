import axios from 'axios';

const API = axios.create({
    baseURL: 'http://localhost:5282/api',
});

export async function login(phoneNumber, password) {
    try {
        const res = await API.get('/auth/login', {
            params: { phoneNumber, password }
        })
        return res.data.accessToken
    } catch (error) {
        console.error('Ошибка авторизации:', error);
        throw error.response.data.detail || error.response.data.title || 'Ошибка';
    }
}

export async function sendCode(phoneNumber) {
    try {
        await API.post('/auth/send_activation_code', { phoneNumber })
    } catch (error) {
        console.error('Ошибка отправки кода:', error);
        throw error.response.data.detail || error.response.data.title || 'Ошибка';
    }
}

export async function registerUser(data) {
    try {
        const res = await API.post('/user/register', data)
        return res.data.accessToken
    } catch (error) {
        console.error('Ошибка регистрации:', error);
        throw error.response.data.detail || error.response.data.title || 'Ошибка';
    }
}