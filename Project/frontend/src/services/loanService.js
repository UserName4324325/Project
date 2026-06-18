import { apiClient } from './apiClient';

export const loanService = {
  async takeLoan(loanData) {
    try {
      return await apiClient.post('/loan/add', loanData);
    } catch (error) {
      throw new Error(error.message || 'Ошибка при получении кредита');
    }
  },

  async getUserLoans() {
    try {
      return await apiClient.get(`/loan/user`);
    } catch (error) {
      throw new Error('Не удалось загрузить кредиты');
    }
  }
};