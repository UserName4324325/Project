import { apiClient } from './apiClient';


export const userService = {
  async getUser() {
    return await apiClient.get('/user');
  },

  async getAllUsers() {
    return await apiClient.get('/user/all');
  }
};