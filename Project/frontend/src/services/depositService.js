import { apiClient } from './apiClient';

export const depositService = {
  async openDeposit(depositData) {
    try {
      return await apiClient.post('/deposit/add', depositData);
    } catch (error) {
      throw new Error(error.message || 'Ошибка при открытии вклада');
    }
  },

  async getUserDeposits() {
    try {
      return await apiClient.get(`/deposit/user`);
    } catch (error) {
      throw new Error('Не удалось загрузить вклады');
    }
  }
};