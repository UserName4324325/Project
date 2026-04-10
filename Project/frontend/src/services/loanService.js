const API_URL = "http://localhost:5037/api/Loans";

export const loanService = {
  async takeLoan(loanData) {
    const token = localStorage.getItem('token')?.replace(/"/g, '');
    const response = await fetch(`${API_URL}/take`, {
      method: 'POST',
      headers: {
        'Content-Type': 'application/json',
        'Authorization': `Bearer ${token}`
      },
      body: JSON.stringify(loanData)
    });

    if (!response.ok) {
      const error = await response.json();
      throw new Error(error.message || 'Ошибка при получении кредита');
    }
    return response.json();
  },

  async getUserLoans(userId) {
    const token = localStorage.getItem('token')?.replace(/"/g, '');
    const response = await fetch(`${API_URL}/user/${userId}`, {
      method: 'GET',
      headers: {
        'Authorization': `Bearer ${token}`
      }
    });
    if (!response.ok) throw new Error('Не удалось загрузить кредиты');
    return response.json();
  }
};