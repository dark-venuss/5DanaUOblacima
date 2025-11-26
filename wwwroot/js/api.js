const API_BASE_URL = 'http://localhost:8080/api';

function showAlert(message, type = 'success') {
    const alert = document.getElementById('alert');
    alert.textContent = message;
    alert.className = `alert show alert-${type}`;
    
    setTimeout(() => {
        alert.classList.remove('show');
    }, 3000);
}

async function apiRequest(endpoint, method = 'GET', data = null) {
    try {
        const options = {
            method,
            headers: {
                'Content-Type': 'application/json',
            }
        };

        if (data) {
            options.body = JSON.stringify(data);
        }

        const response = await fetch(`${API_BASE_URL}${endpoint}`, options);
        
        if (!response.ok) {
            const error = await response.json();
            throw new Error(error.message || `HTTP error! status: ${response.status}`);
        }

        if (response.status === 204) {
            return null;
        }

        return await response.json();
    } catch (error) {
        console.error('API Error:', error);
        showAlert(error.message, 'error');
        throw error;
    }
}

const StudentsAPI = {
    getAll: () => apiRequest('/students'),
    getById: (id) => apiRequest(`/students/${id}`),
    create: (data) => apiRequest('/students', 'POST', data),
    update: (id, data) => apiRequest(`/students/${id}`, 'PUT', data),
    delete: (id) => apiRequest(`/students/${id}`, 'DELETE')
};

const CanteensAPI = {
    getAll: () => apiRequest('/canteens'),
    getById: (id) => apiRequest(`/canteens/${id}`),
    create: (data) => apiRequest('/canteens', 'POST', data),
    update: (id, data) => apiRequest(`/canteens/${id}`, 'PUT', data),
    delete: (id) => apiRequest(`/canteens/${id}`, 'DELETE')
};

const ReservationsAPI = {
    getAll: () => apiRequest('/reservations'),
    getById: (id) => apiRequest(`/reservations/${id}`),
    getByStudentId: (studentId) => apiRequest(`/reservations/student/${studentId}`),
    getByCanteenId: (canteenId) => apiRequest(`/reservations/canteen/${canteenId}`),
    create: (data) => apiRequest('/reservations', 'POST', data),
    update: (id, data) => apiRequest(`/reservations/${id}`, 'PUT', data),
    cancel: (id) => apiRequest(`/reservations/${id}`, 'DELETE')
};
