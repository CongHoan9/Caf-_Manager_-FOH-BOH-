// Lấy thông tin user và role hiện tại
function getCurrentUser() {
    const userJson = localStorage.getItem('user');
    if (!userJson) return null;
    try { return JSON.parse(userJson); } catch { return null; }
}

function getCurrentRole() {
    const user = getCurrentUser();
    if (!user) return '';
    return user.role || user.Role || 'Staff';
}

function getHomePageByRole(role) {
    switch (role) {
        case 'Admin': return 'dashboard.html';
        case 'ThuNgan': return 'tables.html';
        case 'PhucVu': return 'tables.html';
        case 'Bep': return 'kitchen.html';
        case 'Staff': return 'tables.html';
        default: return 'dashboard.html';
    }
}

// Kiểm tra quyền truy cập trang theo role
function isPageAllowedForRole(page, role) {
    if (!page || page === '' || page === '/') return true;
    if (page.includes('index.html')) return true;

    const rules = {
        'Admin': ['dashboard.html', 'tables.html', 'kitchen.html', 'hoadon.html', 'menu.html', 'kho.html', 'lichlam.html', 'users.html'],
        'ThuNgan': ['dashboard.html', 'tables.html', 'hoadon.html', 'menu.html', 'lichlam.html'],
        'PhucVu': ['tables.html', 'kitchen.html', 'menu.html', 'lichlam.html'],
        'Bep': ['kitchen.html', 'menu.html', 'kho.html', 'lichlam.html'],
        'Staff': ['dashboard.html', 'tables.html', 'kitchen.html', 'hoadon.html', 'menu.html', 'kho.html', 'lichlam.html']
    };

    const allowed = rules[role] || rules['Staff'];
    return allowed.some(p => page.includes(p));
}

// Kiểm tra nếu chưa đăng nhập mà vào trang quản trị
function checkAuth() {
    const token = localStorage.getItem('token');
    const currentPath = window.location.pathname;
    const role = getCurrentRole();

    if (!token && !currentPath.includes('index.html')) {
        window.location.href = 'index.html';
        return;
    }
    if (token && (currentPath.includes('index.html') || currentPath === '/')) {
        window.location.href = getHomePageByRole(role);
        return;
    }
    if (token && !isPageAllowedForRole(currentPath, role)) {
        window.location.href = getHomePageByRole(role);
    }
}

// Render Sidebar bằng cách tạo trực tiếp các phần tử DOM theo đúng Role
function renderSidebar(role) {
    const sidebar = document.getElementById('accordionSidebar');
    if (!sidebar) return;

    sidebar.innerHTML = '';
    const path = window.location.pathname;
    const homeHref = getHomePageByRole(role);

    const brand = document.createElement('a');
    brand.className = 'sidebar-brand d-flex align-items-center justify-content-center p-3 text-white text-decoration-none';
    brand.href = homeHref;
    brand.innerHTML = '<div class="sidebar-brand-text mx-3 fs-4 fw-bold">BanCoffee</div>';
    sidebar.appendChild(brand);

    const items = [
        { href: 'dashboard.html', label: 'Tổng quan', roles: ['Admin', 'ThuNgan', 'Staff'] },
        { href: 'tables.html', label: 'Sơ đồ Bàn (POS)', roles: ['Admin', 'ThuNgan', 'PhucVu', 'Staff'] },
        { href: 'kitchen.html', label: 'Bếp / Pha chế', roles: ['Admin', 'Bep', 'PhucVu', 'Staff'] },
        { href: 'hoadon.html', label: 'Hóa Đơn', roles: ['Admin', 'ThuNgan', 'Staff'] },
        { href: 'menu.html', label: 'Sản phẩm (Menu)', roles: ['Admin', 'ThuNgan', 'PhucVu', 'Bep', 'Staff'] },
        { href: 'kho.html', label: 'Kho (Nguyên Liệu)', roles: ['Admin', 'Bep', 'Staff'] },
        { href: 'lichlam.html', label: 'Ca làm & Lịch làm', roles: ['Admin', 'ThuNgan', 'PhucVu', 'Bep', 'Staff'] },
        { href: 'users.html', label: 'Tài khoản & Nhân viên', roles: ['Admin'] }
    ];

    items.forEach(item => {
        if (item.roles.includes(role)) {
            const li = document.createElement('li');
            li.className = 'nav-item' + (path.includes(item.href) ? ' active' : '');
            const a = document.createElement('a');
            a.className = 'nav-link';
            a.href = item.href;
            a.textContent = item.label;
            li.appendChild(a);
            sidebar.appendChild(li);
        }
    });
}

function getRoleDisplayName(role) {
    const map = {
        'Admin': 'Quản trị viên (Admin)',
        'ThuNgan': 'Thu ngân',
        'PhucVu': 'Phục vụ',
        'Bep': 'Bếp / Pha chế',
        'Staff': 'Nhân viên (Staff)'
    };
    return map[role] || role;
}

// Thực sự xóa bỏ khỏi DOM các phần tử không thuộc quyền của Role hiện tại (không dùng class ẩn)
function applyRoleDOM() {
    const role = getCurrentRole();
    document.querySelectorAll('[data-roles]').forEach(el => {
        const allowedRoles = el.getAttribute('data-roles').split(',').map(s => s.trim());
        if (!allowedRoles.includes(role)) {
            el.remove();
        }
    });
}

// Làm rỗng toàn bộ các vùng nhập liệu khi mở trang
function clearAllInputs(root = document) {
    root.querySelectorAll('input:not([type="hidden"]):not([type="checkbox"]):not([type="radio"]):not([type="button"]):not([type="submit"]), textarea').forEach(el => {
        el.value = '';
    });
}

// Xử lý hiển thị tên người dùng và phân quyền DOM
function loadUserInfo() {
    const user = getCurrentUser();
    if (!user) return;

    const role = getCurrentRole();
    renderSidebar(role);

    const userNameEl = document.getElementById('userDropdownName');
    if (userNameEl) {
        const displayName = user.hoten || user.Hoten || user.taikhoan || 'User';
        userNameEl.innerHTML = `<strong>${displayName}</strong> <span class="badge bg-primary ms-1">${getRoleDisplayName(role)}</span>`;
    }

    applyRoleDOM();
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
    clearAllInputs();
    loadUserInfo();
    const btnLogout = document.getElementById('btnLogout');
    if (btnLogout) {
        btnLogout.addEventListener('click', (e) => {
            e.preventDefault();
            logout();
        });
    }
});
window.addEventListener("pageshow", () => {
    clearAllInputs();
});
