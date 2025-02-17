import axios from 'axios';

const api = axios.create({
    baseURL: 'http://localhost:8001/api/', // Убедитесь, что это соответствует вашему бэкенду
    headers: {
        'Content-Type': 'application/json',
    },
});

export default api;
