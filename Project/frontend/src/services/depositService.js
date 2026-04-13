const API_URL_DEPOSITS = `${import.meta.env.VITE_API_URL}/Deposits`;

export const depositService = {
  async openDeposit(depositData) {
    const token = localStorage.getItem('token')?.replace(/"/g, '');

    const response = await fetch(`${API_URL_DEPOSITS}/open`, {
      method: 'POST',
      headers: { 
        'Content-Type': 'application/json',
        'Authorization': `Bearer ${token}`
      },
      
      body: JSON.stringify(depositData)
    });

    if (!response.ok) {
      const error = await response.json();
      throw new Error(error.message || 'Ошибка при открытии вклада');
    }
    return response.json();
  },


  async getUserDeposits(userId) {
    const token = localStorage.getItem('token')?.replace(/"/g, '');

    const response = await fetch(`${API_URL_DEPOSITS}/user/${userId}`, {
      method: 'GET',
      headers: {
        'Authorization': `Bearer ${token}`
      }
    });

    if (!response.ok) throw new Error('Не удалось загрузить вклады');

    return response.json();
  }
};