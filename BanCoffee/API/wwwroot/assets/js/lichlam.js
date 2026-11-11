let lichModal, caModal;
let userList = [];
let caList = [];

document.addEventListener("DOMContentLoaded", async () => {
    const role = typeof getCurrentRole === 'function' ? getCurrentRole() : '';
    const isAdmin = (role === 'Admin');

    // Thực sự Add DOM các nút và cột Hành động nếu là Admin
    const lichActions = document.getElementById('lichHeaderActions');
    if (lichActions) {
        lichActions.innerHTML = '';
        if (isAdmin) {
            const btn = document.createElement('button');
            btn.className = 'btn btn-primary btn-sm';
            btn.textContent = '+ Phân công';
            btn.onclick = showAddLichModal;
            lichActions.appendChild(btn);
        }
    }

    const caActions = document.getElementById('caHeaderActions');
    if (caActions) {
        caActions.innerHTML = '';
        if (isAdmin) {
            const btn = document.createElement('button');
            btn.className = 'btn btn-primary btn-sm';
            btn.textContent = '+ Thêm Ca';
            btn.onclick = showAddCaModal;
            caActions.appendChild(btn);
        }
    }

    const lichHeadRow = document.getElementById('lichHeadRow');
    if (lichHeadRow && isAdmin) {
        const th = document.createElement('th');
        th.style.width = '140px';
        th.textContent = 'Hành động';
        lichHeadRow.appendChild(th);
    }

    const caHeadRow = document.getElementById('caHeadRow');
    if (caHeadRow && isAdmin) {
        const th = document.createElement('th');
        th.style.width = '140px';
        th.textContent = 'Hành động';
        caHeadRow.appendChild(th);
    }

    const lModalEl = document.getElementById('lichModal');
    if (lModalEl && isAdmin) {
        lichModal = new bootstrap.Modal(lModalEl);
    } else if (lModalEl) {
        lModalEl.remove();
    }
    
    const cModalEl = document.getElementById('caModal');
    if (cModalEl && isAdmin) {
        caModal = new bootstrap.Modal(cModalEl);
    } else if (cModalEl) {
        cModalEl.remove();
    }

    await loadCaLam();
    await loadUsers();
    await loadLichLam();
});

function formatTimeSpan(val) {
    if (!val) return '';
    return String(val).substring(0, 5);
}

// ================= CA LÀM (SHIFT) =================
async function loadCaLam() {
    const tbody = document.querySelector('#caTable tbody');
    if (!tbody) return;
    const role = typeof getCurrentRole === 'function' ? getCurrentRole() : '';
    const isAdmin = (role === 'Admin');
    const colSpan = isAdmin ? 4 : 3;

    try {
        const response = await fetchAPI('/CaLam/get-all', 'GET');
        if (response) {
            caList = response;
            
            const cboCa = document.getElementById('lich_ca_lam_id');
            if (cboCa) {
                cboCa.innerHTML = '<option value="">-- Chọn ca làm --</option>';
                caList.forEach(c => {
                    cboCa.innerHTML += `<option value="${c.ca_lam_id}">${c.ten_ca} (${formatTimeSpan(c.gio_bat_dau)} - ${formatTimeSpan(c.gio_ket_thuc)})</option>`;
                });
            }

            tbody.innerHTML = '';
            if (response.length === 0) {
                tbody.innerHTML = `<tr><td colspan="${colSpan}" class="text-center">Chưa có ca làm nào</td></tr>`;
            } else {
                response.forEach(c => {
                    const tr = document.createElement('tr');
                    let actionCell = '';
                    if (isAdmin) {
                        actionCell = `
                            <td class="text-nowrap">
                                <button class="btn btn-sm btn-warning me-1" onclick="editCa('${c.ca_lam_id}')">Sửa</button>
                                <button class="btn btn-sm btn-danger" onclick="deleteCa('${c.ca_lam_id}')">Xóa</button>
                            </td>
                        `;
                    }
                    tr.innerHTML = `
                        <td class="fw-bold">${c.ten_ca}</td>
                        <td>${formatTimeSpan(c.gio_bat_dau)}</td>
                        <td>${formatTimeSpan(c.gio_ket_thuc)}</td>
                        ${actionCell}
                    `;
                    tbody.appendChild(tr);
                });
            }
        }
    } catch (err) {
        tbody.innerHTML = `<tr><td colspan="${colSpan}" class="text-center text-danger">Lỗi: ${err.message}</td></tr>`;
    }
}

window.showAddCaModal = function() {
    if (!caModal) return;
    document.getElementById('ca_lam_id').value = '';
    document.getElementById('ten_ca').value = '';
    document.getElementById('gio_bat_dau').value = '';
    document.getElementById('gio_ket_thuc').value = '';
    document.getElementById('caModalLabel').innerText = 'Thêm Ca Làm Mới';
    caModal.show();
};

window.editCa = async function(id) {
    if (!caModal) return;
    try {
        const c = await fetchAPI(`/CaLam/get-by-id/${id}`, 'GET');
        if (c) {
            document.getElementById('ca_lam_id').value = c.ca_lam_id || '';
            document.getElementById('ten_ca').value = c.ten_ca || '';
            document.getElementById('gio_bat_dau').value = formatTimeSpan(c.gio_bat_dau);
            document.getElementById('gio_ket_thuc').value = formatTimeSpan(c.gio_ket_thuc);
            document.getElementById('caModalLabel').innerText = 'Sửa Ca Làm';
            caModal.show();
        }
    } catch (err) {
        alert("Lỗi: " + err.message);
    }
};

window.saveCa = async function() {
    const id = document.getElementById('ca_lam_id').value;
    const tenCa = document.getElementById('ten_ca').value.trim();
    const gbd = document.getElementById('gio_bat_dau').value;
    const gkt = document.getElementById('gio_ket_thuc').value;

    if (!tenCa || !gbd || !gkt) {
        alert("Vui lòng nhập đầy đủ Tên ca và Giờ bắt đầu / kết thúc!");
        return;
    }

    const payload = {
        ten_ca: tenCa,
        gio_bat_dau: gbd.length === 5 ? gbd + ":00" : gbd,
        gio_ket_thuc: gkt.length === 5 ? gkt + ":00" : gkt
    };
    try {
        if (!id) {
            await fetchAPI('/CaLam/create', 'POST', payload);
            alert('Đã thêm ca làm thành công!');
        } else {
            payload.ca_lam_id = id;
            await fetchAPI('/CaLam/update', 'POST', payload);
            alert('Đã cập nhật ca làm thành công!');
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
        alert('Đã xóa ca làm!');
        loadCaLam();
    } catch (err) {
        alert('Lỗi: ' + err.message);
    }
};

// ================= NHÂN VIÊN DROPDOWN =================
async function loadUsers() {
    const role = typeof getCurrentRole === 'function' ? getCurrentRole() : '';
    if (role !== 'Admin') return;
    try {
        const response = await fetchAPI('/Users/search', 'POST', { page: 1, pageSize: 500, hoten: "", taikhoan: "" });
        if (response && response.data) {
            userList = response.data;
            const cbo = document.getElementById('lich_user_id');
            if (cbo) {
                cbo.innerHTML = '<option value="">-- Chọn nhân viên --</option>';
                userList.forEach(u => {
                    cbo.innerHTML += `<option value="${u.user_id}">${u.hoten} (${u.taikhoan} - ${u.role})</option>`;
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
    const role = typeof getCurrentRole === 'function' ? getCurrentRole() : '';
    const isAdmin = (role === 'Admin');
    const colSpan = isAdmin ? 5 : 4;

    try {
        const response = await fetchAPI('/LichLamViec/search', 'POST', { page: 1, pageSize: 100 });
        if (response && response.data) {
            tbody.innerHTML = '';
            if (response.data.length === 0) {
                tbody.innerHTML = `<tr><td colspan="${colSpan}" class="text-center">Chưa có lịch phân công</td></tr>`;
            } else {
                response.data.forEach(l => {
                    const ngay = l.ngay_lam ? l.ngay_lam.substring(0,10) : '';
                    const tr = document.createElement('tr');
                    let actionCell = '';
                    if (isAdmin) {
                        actionCell = `
                            <td class="text-nowrap">
                                <button class="btn btn-sm btn-warning me-1" onclick="editLich('${l.lich_id}')">Sửa</button>
                                <button class="btn btn-sm btn-danger" onclick="deleteLich('${l.lich_id}')">Xóa</button>
                            </td>
                        `;
                    }
                    tr.innerHTML = `
                        <td>${ngay}</td>
                        <td class="fw-bold">${l.hoten || 'Không xác định'}</td>
                        <td><span class="badge bg-info text-dark">${l.ten_ca || 'Không xác định'}</span></td>
                        <td>${l.ghi_chu || ''}</td>
                        ${actionCell}
                    `;
                    tbody.appendChild(tr);
                });
            }
        }
    } catch (err) {
        tbody.innerHTML = `<tr><td colspan="${colSpan}" class="text-center text-danger">Lỗi: ${err.message}</td></tr>`;
    }
}

window.showAddLichModal = function() {
    if (!lichModal) return;
    document.getElementById('lich_id').value = '';
    document.getElementById('lich_user_id').value = '';
    document.getElementById('lich_ca_lam_id').value = '';
    document.getElementById('lich_ngay_lam').value = '';
    document.getElementById('lich_ghi_chu').value = '';
    document.getElementById('lichModalLabel').innerText = 'Phân công Lịch Làm';
    lichModal.show();
};

window.editLich = async function(id) {
    if (!lichModal) return;
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
    const userId = document.getElementById('lich_user_id').value;
    const caId = document.getElementById('lich_ca_lam_id').value;
    const ngayLam = document.getElementById('lich_ngay_lam').value;

    if (!userId || !caId || !ngayLam) {
        alert("Vui lòng chọn Nhân viên, Ca làm và Ngày làm!");
        return;
    }

    const payload = {
        user_id: userId,
        ca_lam_id: caId,
        ngay_lam: ngayLam,
        ghi_chu: document.getElementById('lich_ghi_chu').value.trim()
    };
    try {
        if (!id) {
            await fetchAPI('/LichLamViec/create', 'POST', payload);
            alert('Đã phân công lịch thành công!');
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
        alert('Đã xóa lịch phân công!');
        loadLichLam();
    } catch (err) {
        alert('Lỗi: ' + err.message);
    }
};
