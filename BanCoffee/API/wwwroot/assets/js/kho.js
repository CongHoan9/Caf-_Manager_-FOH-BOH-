// ===== QUẢN LÝ KHO NGUYÊN LIỆU =====
const NGUONG_SAP_HET = 10;

let nlModal, nhapModal;
let dsNguyenLieu = [];

document.addEventListener('DOMContentLoaded', () => {
    const role = typeof getCurrentRole === 'function' ? getCurrentRole() : '';
    const isAdmin = (role === 'Admin');

    const addSlot = document.getElementById('khoAddBtnSlot');
    if (addSlot) {
        addSlot.innerHTML = '';
        if (isAdmin) {
            const btnAdd = document.createElement('button');
            btnAdd.className = 'btn btn-primary btn-sm text-nowrap';
            btnAdd.id = 'btnAdd';
            btnAdd.textContent = '+ Thêm nguyên liệu';
            btnAdd.addEventListener('click', showAddModal);
            addSlot.appendChild(btnAdd);
        }
    }

    const nlModalEl = document.getElementById('nlModal');
    if (nlModalEl && isAdmin) {
        nlModal = new bootstrap.Modal(nlModalEl);
        const btnSaveNL = document.getElementById('btnSaveNL');
        if (btnSaveNL) btnSaveNL.addEventListener('click', saveNguyenLieu);
    } else if (nlModalEl) {
        nlModalEl.remove();
    }

    nhapModal = new bootstrap.Modal(document.getElementById('nhapModal'));
    document.getElementById('lblNguong').innerText = NGUONG_SAP_HET;

    document.getElementById('btnSaveNhap').addEventListener('click', saveNhapKho);
    document.getElementById('cboLoc').addEventListener('change', renderTable);
    document.getElementById('nhap_so_luong').addEventListener('input', capNhatTomTatNhap);
    document.getElementById('nhap_gia').addEventListener('input', capNhatTomTatNhap);

    const searchInput = document.getElementById('txtSearch');
    if (searchInput) searchInput.value = '';

    let timer;
    searchInput.addEventListener('input', () => {
        clearTimeout(timer);
        timer = setTimeout(loadKho, 300);
    });

    document.querySelector('#khoTable tbody').addEventListener('click', (e) => {
        const btn = e.target.closest('button[data-action]');
        if (!btn) return;
        const id = btn.dataset.id;
        if (btn.dataset.action === 'nhap') showNhapModal(id);
        else if (btn.dataset.action === 'edit') editNguyenLieu(id);
        else if (btn.dataset.action === 'delete') deleteNguyenLieu(id);
    });

    loadKho();
});

// ---------- Helpers ----------
const fmtTien = (n) => Number(n || 0).toLocaleString('vi-VN') + ' đ';
const fmtSo = (n) => Number(n || 0).toLocaleString('vi-VN', { maximumFractionDigits: 2 });
const fmtNgay = (s) => {
    if (!s) return '';
    const d = new Date(s);
    return (isNaN(d) || d.getFullYear() < 1900) ? '' : d.toLocaleDateString('vi-VN');
};
const escapeHtml = (s) => String(s ?? '').replace(/[&<>"']/g, c => ({ '&': '&amp;', '<': '&lt;', '>': '&gt;', '"': '&quot;', "'": '&#39;' }[c]));

function trangThai(ton) {
    if (ton <= 0) return { key: 'out', html: '<span class="badge bg-danger">Hết hàng</span>' };
    if (ton <= NGUONG_SAP_HET) return { key: 'low', html: '<span class="badge bg-warning text-dark">Sắp hết</span>' };
    return { key: 'ok', html: '<span class="badge bg-success">Còn hàng</span>' };
}

// ---------- Load & render ----------
async function loadKho() {
    const tbody = document.querySelector('#khoTable tbody');
    try {
        const filterName = document.getElementById('txtSearch').value.trim();
        const res = await fetchAPI('/NguyenLieu/search', 'POST', { page: 1, pageSize: 500, filterName });
        dsNguyenLieu = (res && res.data) ? res.data : [];
        capNhatThongKe();
        renderTable();
    } catch (err) {
        tbody.innerHTML = `<tr><td colspan="8" class="text-center text-danger">Lỗi: ${escapeHtml(err.message)}</td></tr>`;
    }
}

function capNhatThongKe() {
    const tongGiaTri = dsNguyenLieu.reduce((s, x) => s + (x.so_luong_ton || 0) * (x.gia_nhap || 0), 0);
    const sapHet = dsNguyenLieu.filter(x => (x.so_luong_ton || 0) <= NGUONG_SAP_HET).length;
    document.getElementById('statTong').innerText = dsNguyenLieu.length;
    document.getElementById('statGiaTri').innerText = fmtTien(tongGiaTri);
    document.getElementById('statSapHet').innerText = sapHet;
}

function renderTable() {
    const tbody = document.querySelector('#khoTable tbody');
    const loc = document.getElementById('cboLoc').value;
    const role = typeof getCurrentRole === 'function' ? getCurrentRole() : '';
    const isAdmin = (role === 'Admin');

    const rows = dsNguyenLieu.filter(x => {
        const k = trangThai(x.so_luong_ton || 0).key;
        if (loc === 'low') return k === 'low' || k === 'out';
        if (loc === 'out') return k === 'out';
        return true;
    });

    if (rows.length === 0) {
        tbody.innerHTML = '<tr><td colspan="8" class="text-center text-muted">Không có nguyên liệu nào</td></tr>';
        return;
    }

    tbody.innerHTML = rows.map(x => {
        const tt = trangThai(x.so_luong_ton || 0);
        const id = escapeHtml(x.nguyen_lieu_id);
        const adminBtns = isAdmin
            ? `<button class="btn btn-sm btn-warning me-1" data-action="edit" data-id="${id}">Sửa</button>
               <button class="btn btn-sm btn-danger" data-action="delete" data-id="${id}">Xóa</button>`
            : '';
        return `
            <tr class="${tt.key === 'out' ? 'table-danger' : (tt.key === 'low' ? 'table-warning' : '')}">
                <td class="fw-bold">${escapeHtml(x.ten_nguyen_lieu)}</td>
                <td>${escapeHtml(x.don_vi)}</td>
                <td class="text-end">${fmtSo(x.so_luong_ton)}</td>
                <td class="text-end">${fmtTien(x.gia_nhap)}</td>
                <td class="text-end">${fmtTien((x.so_luong_ton || 0) * (x.gia_nhap || 0))}</td>
                <td>${fmtNgay(x.ngay_nhap_cuoi)}</td>
                <td>${tt.html}</td>
                <td class="text-nowrap">
                    <button class="btn btn-sm btn-success me-1" data-action="nhap" data-id="${id}">Nhập kho</button>
                    ${adminBtns}
                </td>
            </tr>`;
    }).join('');
}

// ---------- Thêm / Sửa ----------
function showAddModal() {
    if (!nlModal) return;
    document.getElementById('nguyen_lieu_id').value = '';
    document.getElementById('ten_nguyen_lieu').value = '';
    document.getElementById('don_vi').value = '';
    document.getElementById('so_luong_ton').value = '';
    document.getElementById('gia_nhap').value = '';
    document.getElementById('so_luong_ton').disabled = false;
    document.getElementById('nlModalLabel').innerText = 'Thêm nguyên liệu';
    nlModal.show();
}

async function editNguyenLieu(id) {
    if (!nlModal) return;
    try {
        const x = await fetchAPI(`/NguyenLieu/get-by-id/${encodeURIComponent(id)}`, 'GET');
        if (!x) return;
        document.getElementById('nguyen_lieu_id').value = x.nguyen_lieu_id;
        document.getElementById('ten_nguyen_lieu').value = x.ten_nguyen_lieu || '';
        document.getElementById('don_vi').value = x.don_vi || '';
        document.getElementById('so_luong_ton').value = x.so_luong_ton || 0;
        document.getElementById('gia_nhap').value = x.gia_nhap || 0;
        document.getElementById('nlModalLabel').innerText = 'Sửa nguyên liệu';
        nlModal.show();
    } catch (err) {
        alert('Lỗi: ' + err.message);
    }
}

async function saveNguyenLieu() {
    const id = document.getElementById('nguyen_lieu_id').value;
    const payload = {
        ten_nguyen_lieu: document.getElementById('ten_nguyen_lieu').value.trim(),
        don_vi: document.getElementById('don_vi').value.trim(),
        so_luong_ton: parseFloat(document.getElementById('so_luong_ton').value) || 0,
        gia_nhap: parseFloat(document.getElementById('gia_nhap').value) || 0
    };
    if (!payload.ten_nguyen_lieu || !payload.don_vi) {
        alert('Vui lòng nhập tên nguyên liệu và đơn vị!');
        return;
    }
    try {
        if (!id) {
            await fetchAPI('/NguyenLieu/create', 'POST', payload);
            alert('Thêm nguyên liệu thành công!');
        } else {
            payload.nguyen_lieu_id = id;
            await fetchAPI('/NguyenLieu/update', 'POST', payload);
            alert('Cập nhật nguyên liệu thành công!');
        }
        nlModal.hide();
        loadKho();
    } catch (err) {
        alert('Lỗi: ' + err.message);
    }
}

async function deleteNguyenLieu(id) {
    const x = dsNguyenLieu.find(n => n.nguyen_lieu_id === id);
    if (!confirm(`Xóa nguyên liệu "${x ? x.ten_nguyen_lieu : ''}"?`)) return;
    try {
        await fetchAPI('/NguyenLieu/delete', 'POST', { nguyen_lieu_id: id });
        alert('Xóa nguyên liệu thành công!');
        loadKho();
    } catch (err) {
        alert('Lỗi: ' + err.message);
    }
}

// ---------- Nhập kho ----------
function showNhapModal(id) {
    const x = dsNguyenLieu.find(n => n.nguyen_lieu_id === id);
    if (!x) return;
    document.getElementById('nhap_id').value = id;
    document.getElementById('lblNhapTen').innerText = x.ten_nguyen_lieu;
    document.getElementById('lblNhapTon').innerText = `${fmtSo(x.so_luong_ton)} ${x.don_vi || ''}`;
    document.getElementById('lblNhapDonVi').innerText = x.don_vi || '';
    document.getElementById('nhap_so_luong').value = '';
    document.getElementById('nhap_gia').value = '';
    document.getElementById('nhap_gia').placeholder = `Giá hiện tại: ${fmtTien(x.gia_nhap)}`;
    capNhatTomTatNhap();
    nhapModal.show();
}

function capNhatTomTatNhap() {
    const x = dsNguyenLieu.find(n => n.nguyen_lieu_id === document.getElementById('nhap_id').value);
    if (!x) return;
    const sl = parseFloat(document.getElementById('nhap_so_luong').value) || 0;
    const gia = parseFloat(document.getElementById('nhap_gia').value) || x.gia_nhap || 0;
    document.getElementById('lblTonSauNhap').innerText = `${fmtSo((x.so_luong_ton || 0) + sl)} ${x.don_vi || ''}`;
    document.getElementById('lblThanhTienNhap').innerText = fmtTien(sl * gia);
}

async function saveNhapKho() {
    const id = document.getElementById('nhap_id').value;
    const soLuong = parseFloat(document.getElementById('nhap_so_luong').value);
    const giaNhap = parseFloat(document.getElementById('nhap_gia').value) || 0;
    if (!soLuong || soLuong <= 0) {
        alert('Số lượng nhập phải lớn hơn 0!');
        return;
    }
    try {
        await fetchAPI('/NguyenLieu/nhap-them', 'POST', { nguyen_lieu_id: id, so_luong: soLuong, gia_nhap: giaNhap });
        alert('Nhập kho thành công!');
        nhapModal.hide();
        loadKho();
    } catch (err) {
        alert('Lỗi: ' + err.message);
    }
}
