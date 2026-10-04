let lichModal, caModal;
let userList = [];
let caList = [];

document.addEventListener("DOMContentLoaded", async () => {
    const lModalEl = document.getElementById('lichModal');
    if (lModalEl) lichModal = new bootstrap.Modal(lModalEl);
    
    const cModalEl = document.getElementById('caModal');
    if (cModalEl) caModal = new bootstrap.Modal(cModalEl);

    await loadCaLam();
    await loadUsers();
    loadLichLam();
});

// ================= CA LÀM (SHIFT) =================
async function loadCaLam() {
    const tbody = document.querySelector('#caTable tbody');
    if (!tbody) return;
    try {
        const response = await fetchAPI('/CaLam/get-all', 'GET');
        if (response) {
            caList = response; // Store for dropdown
            
            // Fill dropdown
            const cboCa = document.getElementById('lich_ca_lam_id');
            if (cboCa) {
                cboCa.innerHTML = '<option value="">-- Chọn ca làm --</option>';
                caList.forEach(c => {
                    cboCa.innerHTML += `<option value="${c.ca_lam_id}">${c.ten_ca} (${c.gio_bat_dau.substring(0,5)} - ${c.gio_ket_thuc.substring(0,5)})</option>`;
                });
            }

            // Fill table
            tbody.innerHTML = '';
            if (response.length === 0) {
                tbody.innerHTML = '<tr><td colspan="4" class="text-center">Chưa có ca làm nào</td></tr>';
            } else {
                response.forEach(c => {
                    const tr = document.createElement('tr');
                    tr.innerHTML = `
                        <td>${c.ten_ca}</td>
                        <td>${c.gio_bat_dau}</td>
                        <td>${c.gio_ket_thuc}</td>
                        <td class="admin-only">
                            <button class="btn btn-sm btn-warning me-1" onclick="editCa('${c.ca_lam_id}')">Sửa</button>
                            <button class="btn btn-sm btn-danger" onclick="deleteCa('${c.ca_lam_id}')">Xóa</button>
                        </td>
                    `;
                    tbody.appendChild(tr);
                });
                if (typeof loadUserInfo === 'function') loadUserInfo(); 
            }
        }
    } catch (err) {
        tbody.innerHTML = `<tr><td colspan="4" class="text-center text-danger">Lỗi: ${err.message}</td></tr>`;
    }
}

window.showAddCaModal = function() {
    document.getElementById('ca_lam_id').value = '';
    document.getElementById('ten_ca').value = '';
    document.getElementById('gio_bat_dau').value = '';
    document.getElementById('gio_ket_thuc').value = '';
    document.getElementById('caModalLabel').innerText = 'Thêm Ca Làm Mới';
    caModal.show();
};

window.editCa = async function(id) {
    try {
        const c = await fetchAPI(`/CaLam/get-by-id/${id}`, 'GET');
        if (c) {
            document.getElementById('ca_lam_id').value = c.ca_lam_id || '';
            document.getElementById('ten_ca').value = c.ten_ca || '';
            document.getElementById('gio_bat_dau').value = c.gio_bat_dau ? c.gio_bat_dau.substring(0,5) : '';
            document.getElementById('gio_ket_thuc').value = c.gio_ket_thuc ? c.gio_ket_thuc.substring(0,5) : '';
            document.getElementById('caModalLabel').innerText = 'Sửa Ca Làm';
            caModal.show();
        }
    } catch (err) {
        alert("Lỗi: " + err.message);
    }
};

window.saveCa = async function() {
    const id = document.getElementById('ca_lam_id').value;
    const payload = {
        ten_ca: document.getElementById('ten_ca').value,
        gio_bat_dau: document.getElementById('gio_bat_dau').value + ":00",
        gio_ket_thuc: document.getElementById('gio_ket_thuc').value + ":00"
    };
    try {
        if (!id) {
            await fetchAPI('/CaLam/create', 'POST', payload);
            alert('Đã thêm ca làm!');
        } else {
            payload.ca_lam_id = id;
            await fetchAPI('/CaLam/update', 'POST', payload);
            alert('Đã cập nhật ca làm!');
        }
        caModal.hide();
        loadCaLam();
    } catch (err) {
        alert('Lỗi: ' + err.message);
    }
};

window.deleteCa = async function(id) {
    if (!confirm('Bạn có chắc muốn xóa ca làm này?')) return;
    try {
        await fetchAPI('/CaLam/delete', 'POST', { ca_lam_id: id });
        loadCaLam();
    } catch (err) {
        alert('Lỗi: ' + err.message);
    }
};

// ================= NHÂN VIÊN DROPDOWN =================
async function loadUsers() {
    try {
        const response = await fetchAPI('/Users/search', 'POST', { page: 1, pageSize: 500, hoten: "", taikhoan: "" });
        if (response && response.data) {
            userList = response.data;
            const cbo = document.getElementById('lich_user_id');
            if (cbo) {
                cbo.innerHTML = '<option value="">-- Chọn nhân viên --</option>';
                userList.forEach(u => {
                    cbo.innerHTML += `<option value="${u.user_id}">${u.hoten} (${u.taikhoan})</option>`;
                });
            }
        }
    } catch (e) {
        console.error("Lỗi tải danh sách user:", e);
    }
}

// ================= LỊCH LÀM VIỆC (SCHEDULE) =================
async function loadLichLam() {
    const tbody = document.querySelector('#lichTable tbody');
    if (!tbody) return;
    try {
        const response = await fetchAPI('/LichLamViec/search', 'POST', { page: 1, pageSize: 100 });
        if (response && response.data) {
            tbody.innerHTML = '';
            if (response.data.length === 0) {
                tbody.innerHTML = '<tr><td colspan="5" class="text-center">Chưa có lịch phân công</td></tr>';
            } else {
                response.data.forEach(l => {
                    const ngay = l.ngay_lam ? l.ngay_lam.substring(0,10) : '';
                    const tr = document.createElement('tr');
                    tr.innerHTML = `
                        <td>${ngay}</td>
                        <td class="fw-bold">${l.hoten || 'Không xác định'}</td>
                        <td>${l.ten_ca || 'Không xác định'}</td>
                        <td>${l.ghi_chu || ''}</td>
                        <td class="admin-only">
                            <button class="btn btn-sm btn-warning me-1" onclick="editLich('${l.lich_id}')">Sửa</button>
                            <button class="btn btn-sm btn-danger" onclick="deleteLich('${l.lich_id}')">Xóa</button>
                        </td>
                    `;
                    tbody.appendChild(tr);
                });
                if (typeof loadUserInfo === 'function') loadUserInfo(); 
            }
        }
    } catch (err) {
        tbody.innerHTML = `<tr><td colspan="5" class="text-center text-danger">Lỗi: ${err.message}</td></tr>`;
    }
}

window.showAddLichModal = function() {
    document.getElementById('lich_id').value = '';
    document.getElementById('lich_user_id').value = '';
    document.getElementById('lich_ca_lam_id').value = '';
    document.getElementById('lich_ngay_lam').value = new Date().toISOString().substring(0,10);
    document.getElementById('lich_ghi_chu').value = '';
    document.getElementById('lichModalLabel').innerText = 'Phân công Lịch Làm';
    lichModal.show();
};

window.editLich = async function(id) {
    try {
        const l = await fetchAPI(`/LichLamViec/get-by-id/${id}`, 'GET');
        if (l) {
            document.getElementById('lich_id').value = l.lich_id || '';
            document.getElementById('lich_user_id').value = l.user_id || '';
            document.getElementById('lich_ca_lam_id').value = l.ca_lam_id || '';
            document.getElementById('lich_ngay_lam').value = l.ngay_lam ? l.ngay_lam.substring(0,10) : '';
            document.getElementById('lich_ghi_chu').value = l.ghi_chu || '';
            document.getElementById('lichModalLabel').innerText = 'Sửa phân công Lịch Làm';
            lichModal.show();
        }
    } catch (err) {
        alert("Lỗi: " + err.message);
    }
};

window.saveLich = async function() {
    const id = document.getElementById('lich_id').value;
    const payload = {
        user_id: document.getElementById('lich_user_id').value,
        ca_lam_id: document.getElementById('lich_ca_lam_id').value,
        ngay_lam: document.getElementById('lich_ngay_lam').value,
        ghi_chu: document.getElementById('lich_ghi_chu').value
    };
    try {
        if (!id) {
            await fetchAPI('/LichLamViec/create', 'POST', payload);
            alert('Đã phân công lịch!');
        } else {
            payload.lich_id = id;
            await fetchAPI('/LichLamViec/update', 'POST', payload);
            alert('Đã cập nhật lịch phân công!');
        }
        lichModal.hide();
        loadLichLam();
    } catch (err) {
        alert('Lỗi: ' + err.message);
    }
};

window.deleteLich = async function(id) {
    if (!confirm('Bạn có chắc muốn xóa lịch phân công này?')) return;
    try {
        await fetchAPI('/LichLamViec/delete', 'POST', { lich_id: id });
        loadLichLam();
    } catch (err) {
        alert('Lỗi: ' + err.message);
    }
};
