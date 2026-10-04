let userModal;

        document.addEventListener("DOMContentLoaded", () => {
            userModal = new bootstrap.Modal(document.getElementById('userModal'));
            loadUsers();
        });

        async function loadUsers() {
            const tbody = document.querySelector('#userTable tbody');
            try {
                // Sử dụng API search với page = 1, pageSize = 50
                const response = await fetchAPI('/Users/search', 'POST', {
                    page: 1,
                    pageSize: 50,
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
                                <td>${user.hoten || ''}</td>
                                <td>${user.taikhoan || ''}</td>
                                <td>${user.email || ''}</td>
                                <td>${user.sdt || ''}</td>
                                <td><span class="badge bg-${user.role === 'Admin' ? 'danger' : 'info'}">${user.role || 'Staff'}</span></td>
                                <td>
                                    <button class="btn btn-sm btn-warning" onclick="editUser('${user.user_id}')">Sửa</button>
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

        function showAddModal() {
            document.getElementById('userForm').reset();
            document.getElementById('userId').value = '';
            document.getElementById('taikhoan').readOnly = false;
            document.getElementById('userModalLabel').innerText = 'Thêm Tài khoản mới';
            userModal.show();
        }

        async function editUser(id) {
            try {
                const user = await fetchAPI(`/Users/get-by-id/${id}`, 'GET');
                if (user) {
                    document.getElementById('userId').value = user.user_id;
                    document.getElementById('taikhoan').value = user.taikhoan;
                    document.getElementById('taikhoan').readOnly = true;
                    document.getElementById('hoten').value = user.hoten;
                    document.getElementById('role').value = user.role;
                    document.getElementById('email').value = user.email || '';
                    document.getElementById('sdt').value = user.sdt || '';
                    document.getElementById('matkhau').value = ''; // Luôn để trống
                    document.getElementById('userModalLabel').innerText = 'Sửa Tài khoản';
                    userModal.show();
                }
            } catch (err) {
                alert('Không thể lấy thông tin nhân viên: ' + err.message);
            }
        }

        async function saveUser() {
            const id = document.getElementById('userId').value;
            const payload = {
                taikhoan: document.getElementById('taikhoan').value,
                hoten: document.getElementById('hoten').value,
                role: document.getElementById('role').value,
                email: document.getElementById('email').value,
                sdt: document.getElementById('sdt').value
            };
            
            const password = document.getElementById('matkhau').value;

            try {
                if (!id) {
                    // Create (CreateUser1)
                    if (!password) {
                        alert("Mật khẩu là bắt buộc khi tạo mới!");
                        return;
                    }
                    payload.matkhau = password;
                    await fetchAPI('/Users/create-user1', 'POST', payload);
                    alert("Tạo tài khoản thành công!");
                } else {
                    // Update
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
        }

        async function deleteUser(id) {
            if (!confirm('Bạn có chắc chắn muốn xóa nhân viên này?')) return;
            try {
                await fetchAPI('/Users/delete-user', 'POST', { user_id: id });
                alert('Xóa thành công!');
                loadUsers();
            } catch (err) {
                alert('Lỗi khi xóa: ' + err.message);
            }
        }