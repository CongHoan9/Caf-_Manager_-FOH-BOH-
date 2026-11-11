let userModal;

document.addEventListener("DOMContentLoaded", () => {
    const modalEl = document.getElementById('userModal');
    if (modalEl) {
        userModal = new bootstrap.Modal(modalEl);
    }
    loadUsers();
});

function roleBadge(r) {
    switch (r) {
        case 'Admin': return '<span class="badge bg-danger">Admin</span>';
        case 'ThuNgan': return '<span class="badge bg-primary">Thu ngân</span>';
        case 'PhucVu': return '<span class="badge bg-success">Phục vụ</span>';
        case 'Bep': return '<span class="badge bg-warning text-dark">Bếp / Pha chế</span>';
        default: return `<span class="badge bg-info text-dark">${r || 'Staff'}</span>`;
    }
}

async function loadUsers() {
    const tbody = document.querySelector('#userTable tbody');
    if (!tbody) return;
    try {
        const response = await fetchAPI('/Users/search', 'POST', {
            page: 1,
            pageSize: 100,
            hoten: "",
            taikhoan: ""
        });
        
        if (response && response.data) {
            tbody.innerHTML = '';
            if (response.data.length === 0) {
                tbody.innerHTML = '<tr><td colspan="6" class="text-center">Không có dữ liệu</td></tr>';
            } else {
                response.data.forEach(user => {
                    const tr = document.createElement('tr');
                    tr.innerHTML = `
                        <td class="fw-bold">${user.hoten || ''}</td>
                        <td>${user.taikhoan || ''}</td>
                        <td>${user.email || ''}</td>
                        <td>${user.diachi || ''}</td>
                        <td>${roleBadge(user.role)}</td>
                        <td class="text-nowrap">
                            <button class="btn btn-sm btn-warning me-1" onclick="editUser('${user.user_id}')">Sửa</button>
                            <button class="btn btn-sm btn-danger" onclick="deleteUser('${user.user_id}')">Xóa</button>
                        </td>
                    `;
                    tbody.appendChild(tr);
                });
            }
        }
    } catch (err) {
        tbody.innerHTML = `<tr><td colspan="6" class="text-center text-danger">Lỗi tải dữ liệu: ${err.message}</td></tr>`;
    }
}

window.showAddModal = function() {
    if (!userModal) return;
    document.getElementById('userForm').reset();
    document.getElementById('userId').value = '';
    document.getElementById('taikhoan').value = '';
    document.getElementById('matkhau').value = '';
    document.getElementById('hoten').value = '';
    document.getElementById('email').value = '';
    document.getElementById('diachi').value = '';
    document.getElementById('taikhoan').readOnly = false;
    document.getElementById('role').value = 'PhucVu';
    document.getElementById('userModalLabel').innerText = 'Thêm Tài khoản mới';
    userModal.show();
};

window.editUser = async function(id) {
    if (!userModal) return;
    try {
        const user = await fetchAPI(`/Users/get-by-id/${id}`, 'GET');
        if (user) {
            document.getElementById('userId').value = user.user_id;
            document.getElementById('taikhoan').value = user.taikhoan;
            document.getElementById('taikhoan').readOnly = true;
            document.getElementById('hoten').value = user.hoten || '';
            document.getElementById('role').value = user.role || 'PhucVu';
            document.getElementById('email').value = user.email || '';
            document.getElementById('diachi').value = user.diachi || '';
            document.getElementById('matkhau').value = '';
            document.getElementById('userModalLabel').innerText = 'Sửa Tài khoản';
            userModal.show();
        }
    } catch (err) {
        alert('Không thể lấy thông tin nhân viên: ' + err.message);
    }
};

window.saveUser = async function() {
    const id = document.getElementById('userId').value;
    const payload = {
        taikhoan: document.getElementById('taikhoan').value.trim(),
        hoten: document.getElementById('hoten').value.trim(),
        role: document.getElementById('role').value,
        email: document.getElementById('email').value.trim(),
        diachi: document.getElementById('diachi').value.trim()
    };
    
    const password = document.getElementById('matkhau').value;

    if (!payload.taikhoan || !payload.hoten) {
        alert("Vui lòng nhập Tài khoản và Họ tên!");
        return;
    }

    try {
        if (!id) {
            if (!password) {
                alert("Mật khẩu là bắt buộc khi tạo mới!");
                return;
            }
            payload.matkhau = password;
            await fetchAPI('/Users/create-user1', 'POST', payload);
            alert("Tạo tài khoản thành công!");
        } else {
            payload.user_id = id;
            if (password) {
                payload.matkhau = password;
            }
            await fetchAPI('/Users/update-user', 'POST', payload);
            alert("Cập nhật tài khoản thành công!");
        }
        
        userModal.hide();
        loadUsers();
    } catch (err) {
        alert("Lỗi khi lưu tài khoản: " + err.message);
    }
};

window.deleteUser = async function(id) {
    if (!confirm('Bạn có chắc chắn muốn xóa nhân viên này?')) return;
    try {
        await fetchAPI('/Users/delete-user', 'POST', { user_id: id });
        alert('Xóa thành công!');
        loadUsers();
    } catch (err) {
        alert('Lỗi khi xóa: ' + err.message);
    }
};
