const API_BASE_URL = '/api'; // Hoặc https://localhost:5001/api tuỳ cổng chạy

// Hàm tiện ích gọi API
async function fetchAPI(endpoint, method = 'GET', body = null) {
    const token = localStorage.getItem('token');
    const headers = {
        'Content-Type': 'application/json'
    };

    if (token) {
        headers['Authorization'] = `Bearer ${token}`;
    }

    const config = {
        method: method,
        headers: headers
    };

    if (body) {
        config.body = JSON.stringify(body);
    }

    try {
        const response = await fetch(`${API_BASE_URL}${endpoint}`, config);
        
        // Bắt lỗi Unauthorized (hết hạn token)
        if (response.status === 401) {
            localStorage.removeItem('token');
            localStorage.removeItem('user');
            window.location.href = 'index.html';
            return null;
        }

        const data = await response.json();
        
        if (!response.ok) {
            throw new Error(data.message || 'Có lỗi xảy ra từ máy chủ');
        }

        return data;
    } catch (error) {
        console.error("API Error:", error);
        throw error;
    }
}

