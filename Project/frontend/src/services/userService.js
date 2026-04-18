
const API_URL_USER = `${import.meta.env.VITE_API_URL}/User`;

const getHeaders = () => {
  const token = localStorage.getItem('token');
  return {
    'Authorization': `Bearer ${token}`,
    'Content-Type': 'application/json'
  };
};

export const userService = {
  getUser: async () => {
    const response = await fetch(`${API_URL_USER}`, {
      method: 'GET',
      headers: getHeaders()
    });
    
    if (!response.ok) {
      if (response.status === 401) localStorage.removeItem('token');
      throw new Error('Ошибка загрузки профиля');
    }
    return response.json();
  },

  getBalance: async () => {
    const response = await fetch(`${API_URL_USER}/balance`, {
      method: 'GET',
      headers: getHeaders()
    });
    
    if (!response.ok) throw new Error('Ошибка загрузки баланса');
    return response.json();
  }
};