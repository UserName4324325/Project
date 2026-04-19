const API_URL_REMITTANCE = `${import.meta.env.VITE_API_URL}/remittance`;

const getAuthHeaders = () => {
    const token = localStorage.getItem('token');
    return {
        'Content-Type': 'application/json',
        'Authorization': `Bearer ${token}`
    };
};

export const remittanceService = {
    async getHistory(userId) {
        const response = await fetch(`${API_URL_REMITTANCE}/history/${userId}`, {
            headers: getAuthHeaders()
        });
        if (!response.ok) throw new Error("Ошибка загрузки истории");
        return response.json();
    },

    async remittance(data) {
        const response = await fetch(`${API_URL_REMITTANCE}/remittance`, {
            method: 'POST',
            headers: getAuthHeaders(),
            body: JSON.stringify(data)
        });
        if (!response.ok) {
            const error = await response.text();
            throw new Error(error || "Ошибка перевода");
        }
        return true;
    }
};