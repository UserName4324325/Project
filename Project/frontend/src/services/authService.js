import { apiClient } from './apiClient';


export const authService = {
  async register(userData) {
    return await apiClient.post('/auth/register', userData);
  },

  async login(credentials) {
    const data = await apiClient.post('/auth/login', credentials);
    
    if (data.token) {
      localStorage.setItem('token', data.token);
      localStorage.setItem('user', JSON.stringify({
        id: data.id,
        fullName: data.fullName,
        balance: data.balance
      }));
    }
    console.log('Login ', data);
    return data;
  },

  async logout() {
    try {
      let result = await apiClient.post('/auth/logout');
    } catch (e) {
      console.warn('Сервер не смог корректно завершить сессию', e);
    } finally {
      localStorage.removeItem('token');
      localStorage.removeItem('user');
      window.location.reload(); 
    }
  },

  isAuthenticated() {
    return !!localStorage.getItem('token');
  }
};