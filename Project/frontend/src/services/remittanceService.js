import { apiClient } from './apiClient';

export const remittanceService = {
  async getHistory() {
    try {
      return await apiClient.get(`/remittance/history`);
    } catch (error) {
      throw new Error(error.message || "Ошибка загрузки истории");
    }
  },

  async remittance(data) {
    try {
      await apiClient.post('/remittance/add', data);

      return true;
    } catch (error) {
      throw new Error(error.message || "Ошибка перевода");
    }
  }
};