let hoaDonModal;
let menuItems = [];
let details = []; // Array of ChiTietHoaDonModel

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
        if (tables) {
            tables.forEach(b => {
                cboBan.innerHTML += `<option value="${b.ban_id}">${b.ten_ban} (${b.khu_vuc || 'Khu chung'})</option>`;
            });
        }
    } catch (e) { console.error("Lỗi tải bàn:", e); }
    
    try {
        const kms = await fetchAPI('/KhuyenMai/search', 'POST', { page: 1, pageSize: 100 });
        const cboKm = document.getElementById('khuyen_mai_id');
        if (kms && kms.data) {
            kms.data.forEach(km => {
                cboKm.innerHTML += `<option value="${km.khuyen_mai_id}">${km.ten_khuyen_mai}</option>`;
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
                response.data.forEach(hd => {
                    const shortId = hd.ma_hoa_don ? hd.ma_hoa_don.substring(0, 8) + '...' : '';
                    const date = new Date(hd.ngay_tao).toLocaleString('vi-VN');
                    
                    const tr = document.createElement('tr');
                    tr.innerHTML = `
                        <td>${shortId}</td>
                        <td>${hd.ho_ten || 'Khách lẻ'}</td>
                        <td>${date}</td>
                        <td><span class="badge bg-secondary">${hd.hinh_thuc_thanh_toan}</span></td>
                        <td class="text-primary fw-bold">${Number(hd.thanh_tien).toLocaleString('vi-VN')}đ</td>
                        <td>
                            <button class="btn btn-sm btn-warning" onclick="editHoaDon('${hd.ma_hoa_don}')">Chi tiết</button>
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
    
    document.getElementById('lblTongTien').innerText = '0đ';
    document.getElementById('lblGiamGia').innerText = '0đ';
    document.getElementById('lblThanhTien').innerText = '0đ';
    
    details = [];
    renderDetailTable();
    document.getElementById('modalTitle').innerText = 'Lập Hóa Đơn Mới';
    hoaDonModal.show();
}

async function editHoaDon(id) {
    try {
        const hd = await fetchAPI(`/HoaDon/get-by-id/${id}`, 'GET');
        if (hd) {
            document.getElementById('ma_hoa_don').value = hd.ma_hoa_don;
            document.getElementById('ho_ten').value = hd.ho_ten;
            document.getElementById('dia_chi').value = hd.dia_chi;
            document.getElementById('hinh_thuc_thanh_toan').value = hd.hinh_thuc_thanh_toan;
            document.getElementById('ghi_chu').value = hd.ghi_chu;
            document.getElementById('ban_id').value = hd.ban_id || '';
            document.getElementById('khuyen_mai_id').value = hd.khuyen_mai_id || '';
            
            document.getElementById('lblTongTien').innerText = Number(hd.tong_tien).toLocaleString('vi-VN') + 'đ';
            document.getElementById('lblGiamGia').innerText = Number(hd.giam_gia).toLocaleString('vi-VN') + 'đ';
            document.getElementById('lblThanhTien').innerText = Number(hd.thanh_tien).toLocaleString('vi-VN') + 'đ';

            // Load chi tiết
            details = hd.listjson_chitiet || [];
            details.forEach(d => d.status = 0); // 0 = unchanged
            
            renderDetailTable();
            document.getElementById('modalTitle').innerText = 'Chi tiết Hóa Đơn';
            hoaDonModal.show();
        }
    } catch (err) {
        alert("Lỗi lấy hóa đơn: " + err.message);
    }
}

function addDetailRow() {
    details.push({
        ma_chi_tiet: '',
        item_id: '',
        so_luong: 1,
        don_gia: 0,
        status: 1 // 1 = add
    });
    renderDetailTable();
}

function removeDetailRow(index) {
    if (details[index].ma_chi_tiet) {
        details[index].status = 3; // 3 = delete
    } else {
        details.splice(index, 1); // Not saved yet, remove completely
    }
    renderDetailTable();
}

// Global exposure cho inline onclick
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
    if (details[index].status === 0) details[index].status = 2; // 2 = edit
    renderDetailTable();
};

function renderDetailTable() {
    const tbody = document.querySelector('#detailTable tbody');
    if (!tbody) return;
    tbody.innerHTML = '';
    details.forEach((d, i) => {
        if (d.status === 3) return; // Hide deleted
        
        let itemOptions = '<option value="">-- Chọn --</option>';
        menuItems.forEach(mi => {
            itemOptions += `<option value="${mi.item_id}" ${d.item_id == mi.item_id ? 'selected' : ''}>${mi.item_name}</option>`;
        });

        const tr = document.createElement('tr');
        tr.innerHTML = `
            <td><select class="form-select form-select-sm" onchange="updateDetail(${i}, 'item_id', this.value)">${itemOptions}</select></td>
            <td><input type="number" class="form-control form-control-sm" min="1" value="${d.so_luong}" onchange="updateDetail(${i}, 'so_luong', parseInt(this.value))"></td>
            <td><input type="number" class="form-control form-control-sm" value="${d.don_gia}" onchange="updateDetail(${i}, 'don_gia', parseFloat(this.value))"></td>
            <td><button type="button" class="btn btn-sm btn-danger" onclick="removeDetailRow(${i})">X</button></td>
        `;
        tbody.appendChild(tr);
    });
}

window.saveHoaDon = async function() {
    const id = document.getElementById('ma_hoa_don').value;
    const payload = {
        ho_ten: document.getElementById('ho_ten').value,
        dia_chi: document.getElementById('dia_chi').value,
        hinh_thuc_thanh_toan: document.getElementById('hinh_thuc_thanh_toan').value,
        ban_id: document.getElementById('ban_id').value,
        khuyen_mai_id: document.getElementById('khuyen_mai_id').value,
        ghi_chu: document.getElementById('ghi_chu').value,
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