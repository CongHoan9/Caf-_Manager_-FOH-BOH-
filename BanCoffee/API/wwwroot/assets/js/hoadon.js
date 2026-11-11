let hoaDonModal;
let menuItems = [];
let kmList = [];
let details = [];

document.addEventListener("DOMContentLoaded", async () => {
    const modalEl = document.getElementById('hoaDonModal');
    if (modalEl) {
        hoaDonModal = new bootstrap.Modal(modalEl);
    }
    await loadDropdowns();
    loadHoaDons();
});

async function loadDropdowns() {
    try {
        const tables = await fetchAPI('/Ban/get-all', 'GET');
        const cboBan = document.getElementById('ban_id');
        if (tables && cboBan) {
            cboBan.innerHTML = '<option value="">-- Mua mang đi / Không chọn bàn --</option>';
            tables.forEach(b => {
                cboBan.innerHTML += `<option value="${b.ban_id}">${b.ten_ban} (${b.khu_vuc || 'Tầng 1'})</option>`;
            });
        }
    } catch (e) { console.error("Lỗi tải bàn:", e); }
    
    try {
        const kms = await fetchAPI('/KhuyenMai/search', 'POST', { page: 1, pageSize: 100 });
        const cboKm = document.getElementById('khuyen_mai_id');
        if (kms && kms.data && cboKm) {
            kmList = kms.data;
            cboKm.innerHTML = '<option value="">-- Không dùng --</option>';
            kms.data.forEach(km => {
                cboKm.innerHTML += `<option value="${km.khuyen_mai_id}">${km.ten_khuyen_mai} (${km.ma_khuyen_mai})</option>`;
            });
        }
    } catch (e) { console.error("Lỗi tải khuyến mãi:", e); }

    try {
        const items = await fetchAPI('/Item/search', 'POST', { page: 1, pageSize: 500 });
        if (items && items.data) {
            menuItems = items.data;
        }
    } catch (e) { console.error("Lỗi tải menu:", e); }
}

async function loadHoaDons() {
    const tbody = document.querySelector('#hoaDonTable tbody');
    if (!tbody) return;
    try {
        const response = await fetchAPI('/HoaDon/search', 'POST', {
            page: 1, pageSize: 50
        });
        
        if (response && response.data) {
            tbody.innerHTML = '';
            if (response.data.length === 0) {
                tbody.innerHTML = '<tr><td colspan="6" class="text-center">Chưa có hóa đơn nào</td></tr>';
            } else {
                const role = typeof getCurrentRole === 'function' ? getCurrentRole() : '';
                response.data.forEach(hd => {
                    const shortId = hd.ma_hoa_don ? hd.ma_hoa_don.substring(0, 8) + '...' : '';
                    const date = new Date(hd.ngay_tao).toLocaleString('vi-VN');
                    const deleteBtn = role === 'Admin'
                        ? `<button class="btn btn-sm btn-danger ms-1" onclick="deleteHoaDon('${hd.ma_hoa_don}')">Xóa</button>`
                        : '';
                    
                    const tr = document.createElement('tr');
                    tr.innerHTML = `
                        <td title="${hd.ma_hoa_don}">${shortId}</td>
                        <td class="fw-bold">${hd.ho_ten || 'Khách lẻ'}</td>
                        <td>${date}</td>
                        <td><span class="badge bg-secondary">${hd.hinh_thuc_thanh_toan}</span></td>
                        <td class="text-primary fw-bold text-end">${Number(hd.thanh_tien).toLocaleString('vi-VN')} đ</td>
                        <td class="text-nowrap">
                            <button class="btn btn-sm btn-warning" onclick="editHoaDon('${hd.ma_hoa_don}')">Sửa / Chi tiết</button>
                            ${deleteBtn}
                        </td>
                    `;
                    tbody.appendChild(tr);
                });
            }
        }
    } catch (err) {
        tbody.innerHTML = `<tr><td colspan="6" class="text-center text-danger">Lỗi: ${err.message}</td></tr>`;
    }
}

function showAddModal() {
    document.getElementById('ma_hoa_don').value = '';
    document.getElementById('ho_ten').value = '';
    document.getElementById('dia_chi').value = '';
    document.getElementById('ghi_chu').value = '';
    document.getElementById('ban_id').value = '';
    document.getElementById('khuyen_mai_id').value = '';
    document.getElementById('hinh_thuc_thanh_toan').value = 'Tiền mặt';
    
    details = [];
    if (menuItems.length > 0) {
        details.push({
            ma_chi_tiet: '',
            item_id: menuItems[0].item_id,
            so_luong: 1,
            don_gia: menuItems[0].item_price,
            status: 1
        });
    }
    renderDetailTable();
    document.getElementById('modalTitle').innerText = 'Lập Hóa Đơn Mới';
    hoaDonModal.show();
}

async function editHoaDon(id) {
    try {
        const hd = await fetchAPI(`/HoaDon/get-by-id/${id}`, 'GET');
        if (hd) {
            document.getElementById('ma_hoa_don').value = hd.ma_hoa_don;
            document.getElementById('ho_ten').value = hd.ho_ten || '';
            document.getElementById('dia_chi').value = hd.dia_chi || '';
            document.getElementById('hinh_thuc_thanh_toan').value = hd.hinh_thuc_thanh_toan || 'Tiền mặt';
            document.getElementById('ghi_chu').value = hd.ghi_chu || '';
            document.getElementById('ban_id').value = hd.ban_id || '';
            document.getElementById('khuyen_mai_id').value = hd.khuyen_mai_id || '';
            
            details = hd.listjson_chitiet || [];
            details.forEach(d => d.status = 0);
            
            renderDetailTable();
            document.getElementById('modalTitle').innerText = 'Chi tiết / Cập nhật Hóa Đơn';
            hoaDonModal.show();
        }
    } catch (err) {
        alert("Lỗi lấy hóa đơn: " + err.message);
    }
}

window.deleteHoaDon = async function(id) {
    if (!confirm("Bạn có chắc chắn muốn xóa hóa đơn này?")) return;
    try {
        await fetchAPI('/HoaDon/delete', 'POST', { ma_hoa_don: id });
        alert("Đã xóa hóa đơn thành công!");
        loadHoaDons();
    } catch (err) {
        alert("Lỗi khi xóa hóa đơn: " + err.message);
    }
};

function addDetailRow() {
    const firstItem = menuItems.length > 0 ? menuItems[0] : null;
    details.push({
        ma_chi_tiet: '',
        item_id: firstItem ? firstItem.item_id : '',
        so_luong: 1,
        don_gia: firstItem ? firstItem.item_price : 0,
        status: 1
    });
    renderDetailTable();
}

function removeDetailRow(index) {
    if (details[index].ma_chi_tiet) {
        details[index].status = 3;
    } else {
        details.splice(index, 1);
    }
    renderDetailTable();
}

window.showAddModal = showAddModal;
window.editHoaDon = editHoaDon;
window.addDetailRow = addDetailRow;
window.removeDetailRow = removeDetailRow;
window.updateDetail = function(index, field, value) {
    details[index][field] = value;
    if (field === 'item_id') {
        const item = menuItems.find(i => i.item_id == value);
        if (item) details[index].don_gia = item.item_price;
    }
    if (details[index].status === 0) details[index].status = 2;
    renderDetailTable();
};

window.recalcSummary = function() {
    let tong = 0;
    details.forEach(d => {
        if (d.status !== 3) {
            tong += (Number(d.so_luong) || 0) * (Number(d.don_gia) || 0);
        }
    });
    let giam = 0;
    const kmId = document.getElementById('khuyen_mai_id').value;
    if (kmId) {
        const km = kmList.find(k => k.khuyen_mai_id === kmId);
        if (km) {
            if (km.loai_giam === 'Phần trăm') giam = tong * (km.gia_tri_giam / 100);
            else giam = km.gia_tri_giam;
        }
    }
    if (giam > tong) giam = tong;
    const thanhTien = tong - giam;

    document.getElementById('lblTongTien').innerText = tong.toLocaleString('vi-VN') + ' đ';
    document.getElementById('lblGiamGia').innerText = giam.toLocaleString('vi-VN') + ' đ';
    document.getElementById('lblThanhTien').innerText = thanhTien.toLocaleString('vi-VN') + ' đ';
};

function renderDetailTable() {
    const tbody = document.querySelector('#detailTable tbody');
    if (!tbody) return;
    tbody.innerHTML = '';
    details.forEach((d, i) => {
        if (d.status === 3) return;
        
        let itemOptions = '<option value="">-- Chọn món --</option>';
        menuItems.forEach(mi => {
            itemOptions += `<option value="${mi.item_id}" ${d.item_id == mi.item_id ? 'selected' : ''}>${mi.item_name}</option>`;
        });

        const tr = document.createElement('tr');
        tr.innerHTML = `
            <td><select class="form-select form-select-sm" onchange="updateDetail(${i}, 'item_id', this.value)">${itemOptions}</select></td>
            <td><input type="number" class="form-control form-control-sm" min="1" value="${d.so_luong}" onchange="updateDetail(${i}, 'so_luong', parseInt(this.value) || 1)"></td>
            <td><input type="number" class="form-control form-control-sm" value="${d.don_gia}" onchange="updateDetail(${i}, 'don_gia', parseFloat(this.value) || 0)"></td>
            <td class="text-center"><button type="button" class="btn btn-sm btn-danger" onclick="removeDetailRow(${i})">X</button></td>
        `;
        tbody.appendChild(tr);
    });
    recalcSummary();
}

window.saveHoaDon = async function() {
    const activeDetails = details.filter(d => d.status !== 3);
    if (activeDetails.length === 0) {
        alert("Hóa đơn phải có ít nhất 1 món!");
        return;
    }
    for (const d of activeDetails) {
        if (!d.item_id) {
            alert("Vui lòng chọn tên món cho tất cả các dòng!");
            return;
        }
    }

    const id = document.getElementById('ma_hoa_don').value;
    const payload = {
        ho_ten: document.getElementById('ho_ten').value.trim() || 'Khách lẻ',
        dia_chi: document.getElementById('dia_chi').value.trim(),
        hinh_thuc_thanh_toan: document.getElementById('hinh_thuc_thanh_toan').value,
        ban_id: document.getElementById('ban_id').value || null,
        khuyen_mai_id: document.getElementById('khuyen_mai_id').value || null,
        ghi_chu: document.getElementById('ghi_chu').value.trim(),
        listjson_chitiet: details
    };

    try {
        if (!id) {
            await fetchAPI('/HoaDon/create-hoa-don', 'POST', payload);
            alert("Tạo hóa đơn thành công!");
        } else {
            payload.ma_hoa_don = id;
            await fetchAPI('/HoaDon/update-hoa-don', 'POST', payload);
            alert("Cập nhật hóa đơn thành công!");
        }
        hoaDonModal.hide();
        loadHoaDons();
    } catch (e) {
        alert("Lỗi: " + e.message);
    }
};
