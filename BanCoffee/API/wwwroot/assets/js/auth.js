// Kiểm tra nếu chưa đăng nhập mà vào trang quản trị
function checkAuth() {
    const token = localStorage.getItem('token');
    const currentPath = window.location.pathname;

    if (!token && !currentPath.includes('index.html')) {
        window.location.href = 'index.html';
    } else if (token && currentPath.includes('index.html')) {
        window.location.href = 'dashboard.html';
    }
}

// Xử lý hiển thị tên người dùng và phân quyền giao diện
function loadUserInfo() {
    const userJson = localStorage.getItem('user');
    if (userJson) {
        const user = JSON.parse(userJson);
        const userNameEl = document.getElementById('userDropdownName');
        if (userNameEl) {
            userNameEl.innerText = user.Hoten || user.taikhoan;
        }

        // --- PHÂN QUYỀN GIAO DIỆN (DYNAMIC UI) ---
        const role = user.Role || user.role;
        
        // Cấp quyền Admin: Ẩn các nút chỉ dành riêng cho Staff (nếu có)
        // Cấp quyền Staff: Ẩn các menu/nút chỉ dành riêng cho Admin
        const adminElements = document.querySelectorAll('.admin-only');
        const staffElements = document.querySelectorAll('.staff-only');

        if (role === 'Staff') {
            adminElements.forEach(el => el.style.display = 'none');
            // Cấm Staff vào thẳng trang users.html hoặc kho.html qua URL
            if (window.location.pathname.includes('users.html') || window.location.pathname.includes('kho.html')) {
                alert('Bạn không có quyền truy cập trang này!');
                window.location.href = 'dashboard.html';
            }
        } else if (role === 'Admin') {
            staffElements.forEach(el => el.style.display = 'none');
        }
    }
}

// Hàm đăng xuất
function logout() {
    localStorage.removeItem('token');
    localStorage.removeItem('user');
    window.location.href = 'index.html';
}

// Chạy kiểm tra ngay khi load script
checkAuth();
document.addEventListener("DOMContentLoaded", () => {
    loadUserInfo();
    const btnLogout = document.getElementById('btnLogout');
    if (btnLogout) {
        btnLogout.addEventListener('click', (e) => {
            e.preventDefault();
            logout();
        });
    }
});
