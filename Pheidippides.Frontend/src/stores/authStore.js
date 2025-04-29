import { defineStore } from 'pinia';
import { login, registerUser, sendCode } from '@/api/authApi.js';

export const useAuthStore = defineStore('auth', {
    state: () => ({
        token: localStorage.getItem('token') || ''
    }),
    actions: {
        async loginUser(phoneNumber, password) {
            const token = await login(phoneNumber, password)
            this.token = token
            localStorage.setItem('token', token)
        },

        async requestCode(phoneNumber) {
            await sendCode(phoneNumber)
        },

        async registerUser(userData) {
            const token = await registerUser(userData)
            this.token = token
            localStorage.setItem('token', token)
        },

        logout() {
            this.token = ''
            localStorage.removeItem('token')
        },
    }
})
