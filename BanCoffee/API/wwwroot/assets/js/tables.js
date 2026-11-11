let currentTables = [];
let menuItems = [];
let tableModalInstance = null;
let orderModalInstance = null;
let addItemModalInstance = null;

document.addEventListener("DOMContentLoaded", () => {
    const role = typeof getCurrentRole === 'function' ? getCurrentRole() : '';

    // Thực sự Add DOM nút Thêm Bàn nếu là Admin, không dùng class ẩn
    const headerActions = document.getElementById('tableHeaderActions');
    if (headerActions) {
        headerActions.innerHTML = '';
        if (role === 'Admin') {
            const btnAdd = document.createElement('button');
            btnAdd.className = 'btn btn-sm btn-primary';
            btnAdd.id = 'btnAddTable';
            btnAdd.textContent = '+ Thêm Bàn Mới';
            btnAdd.addEventListener('click', openAddTableModal);
            headerActions.appendChild(btnAdd);
        }
    }

    const tmEl = document.getElementById('tableModal');
    if (tmEl && role === 'Admin') {
        tableModalInstance = new bootstrap.Modal(tmEl);
    } else if (tmEl) {
        tmEl.remove();
    }

    const omEl = document.getElementById('orderModal');
    if (omEl) orderModalInstance = new bootstrap.Modal(omEl);

    const aimEl = document.getElementById('addItemModal');
    if (aimEl) addItemModalInstance = new bootstrap.Modal(aimEl);

    loadTables();
    loadMenuItems();
});

async function loadTables() {
    const container = document.getElementById('tableContainer');
    if (!container) return;
    try {
        const tables = await fetchAPI('/Ban/get-all', 'GET');
        currentTables = tables || [];
        container.innerHTML = '';
        if (currentTables.length === 0) {
            container.innerHTML = '<p class="text-muted">Chưa có dữ liệu bàn.</p>';
            return;
        }

        const role = typeof getCurrentRole === 'function' ? getCurrentRole() : '';

        currentTables.forEach(table => {
            let bgClass = 'bg-trong';
            if (table.trang_thai === 'Đang phục vụ' || table.trang_thai === 'Đang sử dụng') bgClass = 'bg-dang-phuc-vu';
            else if (table.trang_thai === 'Đã đặt') bgClass = 'bg-da-dat';

            const khuVuc = table.khu_vuc || 'Tầng 1';
            const soCho = table.so_cho || 4;

            const div = document.createElement('div');
            div.className = 'col-xl-3 col-md-4 col-sm-6 mb-4';
            
            let btnConfig = '';
            if (role === 'Admin') {
                btnConfig = `<button class="btn btn-sm btn-light position-absolute top-0 end-0 m-2" title="Sửa / Xóa bàn" onclick="event.stopPropagation(); openEditTableModal('${table.ban_id}')">⚙️</button>`;
            }

            let badgeColor = 'bg-success';
            if (table.trang_thai === 'Đang phục vụ' || table.trang_thai === 'Đang sử dụng') badgeColor = 'bg-warning text-dark';
            else if (table.trang_thai === 'Đã đặt') badgeColor = 'bg-danger';

            div.innerHTML = `
                <div class="card table-card ${bgClass} h-100 py-3 position-relative" onclick="openOrderModal('${table.ban_id}')">
                    ${btnConfig}
                    <div class="card-body text-center">
                        <h5 class="fw-bold text-dark mb-1">${table.ten_ban}</h5>
                        <div class="small text-muted mb-2">${khuVuc} &middot; ${soCho} chỗ</div>
                        <span class="badge ${badgeColor}">${table.trang_thai}</span>
                    </div>
                </div>
            `;
            container.appendChild(div);
        });
    } catch (err) {
        console.error(err);
    }
}

async function loadMenuItems() {
    try {
        menuItems = await fetchAPI('/Item/get-all', 'GET') || [];
    } catch(e) { console.error("Không thể tải Menu", e); }
}

window.openOrderModal = async function(id) {
    const table = currentTables.find(t => t.ban_id === id);
    if (!table) return;

    document.getElementById('posTableName').innerText = `${table.ten_ban} (${table.khu_vuc || 'Tầng 1'})`;
    document.getElementById('posTableId').value = table.ban_id;
    document.getElementById('posTableQuickStatus').value = table.trang_thai || 'Trống';
    
    const role = typeof getCurrentRole === 'function' ? getCurrentRole() : '';
    const canCheckout = (role === 'ThuNgan' || role === 'Admin' || role === 'Staff');

    const paymentSlot = document.getElementById('posPaymentMethodSlot');
    const checkoutSlot = document.getElementById('posCheckoutSlot');
    if (paymentSlot) {
        paymentSlot.innerHTML = canCheckout ? `
            <label class="form-label small fw-bold mb-1">Hình thức thanh toán (Thu ngân):</label>
            <select class="form-select form-select-sm" id="posPaymentMethod">
                <option value="Tiền mặt">Tiền mặt</option>
                <option value="Chuyển khoản">Chuyển khoản</option>
                <option value="Ví điện tử">Ví điện tử</option>
            </select>
        ` : '';
    }
    if (checkoutSlot) {
        checkoutSlot.innerHTML = canCheckout ? `
            <button type="button" class="btn btn-primary fw-bold" id="btnCheckout" onclick="checkoutOrder()">THANH TOÁN &amp; LẬP HÓA ĐƠN</button>
        ` : '';
    }

    await reloadOrderView(id);
    orderModalInstance.show();
};

window.quickChangeTableStatus = async function(newStatus) {
    const banId = document.getElementById('posTableId').value;
    if (!banId) return;
    try {
        await fetchAPI('/Ban/update-trang-thai', 'POST', { ban_id: banId, trang_thai: newStatus });
        await loadTables();
    } catch (e) {
        alert("Lỗi đổi trạng thái bàn: " + e.message);
    }
};

async function reloadOrderView(banId) {
    try {
        const order = await fetchAPI(`/Order/get-by-table/${banId}`, 'GET');
        const tbody = document.getElementById('posOrderList');
        const spanTotal = document.getElementById('posTotal');
        
        if (!order || !order.items || order.items.length === 0) {
            tbody.innerHTML = `<tr><td colspan="5" class="text-center text-muted">Bàn chưa gọi món nào</td></tr>`;
            spanTotal.innerText = '0';
            return;
        }

        tbody.innerHTML = '';
        let total = 0;
        const states = ["Chờ làm", "Đang pha chế", "Đã xong (Chờ giao)", "Đã giao khách"];
        
        order.items.forEach(it => {
            const tr = document.createElement('tr');
            const tt = (it.price || 0) * (it.quantity || 1);
            total += tt;
            
            let badgeClass = 'bg-secondary';
            if (it.state === 1) badgeClass = 'bg-primary';
            if (it.state === 2) badgeClass = 'bg-success';
            if (it.state === 3) badgeClass = 'bg-dark';
            
            tr.innerHTML = `
                <td class="fw-bold">${it.item_name}</td>
                <td class="text-center">${it.quantity}</td>
                <td class="text-end">${(it.price || 0).toLocaleString('vi-VN')} đ</td>
                <td>${it.note || ''}</td>
                <td class="text-center"><span class="badge ${badgeClass}">${states[it.state] || 'Chờ làm'}</span></td>
            `;
            tbody.appendChild(tr);
        });
        
        spanTotal.innerText = total.toLocaleString('vi-VN');
    } catch(e) {
        console.error(e);
    }
}

window.openAddItemModal = function() {
    const sel = document.getElementById('addSelectMenu');
    sel.innerHTML = '';
    menuItems.forEach(mi => {
        const opt = document.createElement('option');
        opt.value = mi.item_id;
        opt.dataset.name = mi.item_name;
        opt.dataset.price = mi.item_price;
        opt.innerText = `${mi.item_name} - ${Number(mi.item_price).toLocaleString('vi-VN')} đ`;
        sel.appendChild(opt);
    });
    
    document.getElementById('addQty').value = '';
    document.getElementById('addNote').value = '';
    
    addItemModalInstance.show();
};

window.confirmAddItem = async function() {
    const banId = document.getElementById('posTableId').value;
    const sel = document.getElementById('addSelectMenu');
    const opt = sel.options[sel.selectedIndex];
    
    if (!opt) return alert("Vui lòng chọn món!");
    
    const qty = parseInt(document.getElementById('addQty').value) || 1;
    const note = document.getElementById('addNote').value;
    
    const payload = {
        ban_id: banId,
        item_id: opt.value,
        item_name: opt.dataset.name,
        quantity: qty,
        price: parseFloat(opt.dataset.price) || 0,
        note: note
    };
    
    try {
        await fetchAPI('/Order/add-item', 'POST', payload);
        addItemModalInstance.hide();
        await reloadOrderView(banId);
        
        const table = currentTables.find(t => t.ban_id === banId);
        if (table && table.trang_thai !== 'Đang phục vụ') {
            await fetchAPI('/Ban/update-trang-thai', 'POST', {
                ban_id: banId,
                trang_thai: 'Đang phục vụ'
            });
            document.getElementById('posTableQuickStatus').value = 'Đang phục vụ';
            await loadTables();
        }
    } catch(e) {
        alert(e.message || "Không thể gọi món (Có thể hết nguyên liệu)");
    }
};

window.checkoutOrder = async function() {
    if (!confirm("Xác nhận thanh toán và xuất hóa đơn cho bàn này?")) return;
    const banId = document.getElementById('posTableId').value;
    const paymentEl = document.getElementById('posPaymentMethod');
    const paymentMethod = paymentEl ? paymentEl.value : 'Tiền mặt';
    try {
        const res = await fetchAPI('/Order/checkout', 'POST', {
            ban_id: banId,
            payment_method: paymentMethod,
            giam_gia: 0,
            ghi_chu: ''
        });
        alert(res.message || "Thanh toán thành công!");
        
        await fetchAPI('/Ban/update-trang-thai', 'POST', {
            ban_id: banId,
            trang_thai: 'Trống'
        });
        
        orderModalInstance.hide();
        await loadTables();
    } catch(e) {
        alert("Lỗi thanh toán: " + e.message);
    }
};

window.clearOrder = async function() {
    if (!confirm("Hủy đơn hàng của bàn này và đưa bàn về trạng thái Trống?")) return;
    const banId = document.getElementById('posTableId').value;
    try {
        await fetchAPI(`/Order/clear/${banId}`, 'POST');
        await fetchAPI('/Ban/update-trang-thai', 'POST', {
            ban_id: banId,
            trang_thai: 'Trống'
        });
        
        orderModalInstance.hide();
        await loadTables();
    } catch(e) {
        alert("Lỗi: " + e.message);
    }
};

// ADMIN: Thêm / Sửa / Xóa Bàn
window.openAddTableModal = function() {
    if (!tableModalInstance) return;
    document.getElementById('tableId').value = '';
    document.getElementById('tableModalTitle').innerText = 'Thêm bàn mới';
    document.getElementById('tableName').value = '';
    document.getElementById('tableZone').value = '';
    document.getElementById('tableCapacity').value = '';
    document.getElementById('tableStatus').value = 'Trống';
    const delSlot = document.getElementById('deleteTableSlot');
    if (delSlot) delSlot.innerHTML = '';
    
    tableModalInstance.show();
};

window.openEditTableModal = function(id) {
    if (!tableModalInstance) return;
    const table = currentTables.find(t => t.ban_id === id);
    if (!table) return;

    document.getElementById('tableId').value = table.ban_id;
    document.getElementById('tableModalTitle').innerText = 'Sửa thông tin bàn';
    document.getElementById('tableName').value = table.ten_ban || '';
    document.getElementById('tableZone').value = table.khu_vuc || 'Tầng 1';
    document.getElementById('tableCapacity').value = table.so_cho || 4;
    document.getElementById('tableStatus').value = table.trang_thai || 'Trống';
    const delSlot = document.getElementById('deleteTableSlot');
    if (delSlot) {
        delSlot.innerHTML = '<button type="button" class="btn btn-danger" id="btnDeleteTable" onclick="deleteTable()">Xóa bàn</button>';
    }
    
    tableModalInstance.show();
};

window.saveTable = async function() {
    const id = document.getElementById('tableId').value;
    const model = {
        ban_id: id || undefined,
        ten_ban: document.getElementById('tableName').value.trim(),
        khu_vuc: document.getElementById('tableZone').value.trim() || 'Tầng 1',
        so_cho: parseInt(document.getElementById('tableCapacity').value) || 4,
        trang_thai: document.getElementById('tableStatus').value
    };

    if (!model.ten_ban) {
        alert("Vui lòng nhập tên bàn!");
        return;
    }

    try {
        if (!id) {
            await fetchAPI('/Ban/create-ban', 'POST', model);
            alert("Thêm bàn mới thành công!");
        } else {
            await fetchAPI('/Ban/update-ban', 'POST', model);
            alert("Cập nhật bàn thành công!");
        }
        tableModalInstance.hide();
        await loadTables();
    } catch(e) {
        alert("Lỗi: " + e.message);
    }
};

window.deleteTable = async function() {
    const id = document.getElementById('tableId').value;
    if (!id) return;
    if (!confirm("Bạn có chắc chắn muốn xóa bàn này?")) return;
    try {
        await fetchAPI('/Ban/delete-ban', 'POST', { ban_id: id });
        alert("Xóa bàn thành công!");
        tableModalInstance.hide();
        await loadTables();
    } catch(e) {
        alert("Không thể xóa bàn: " + e.message);
    }
};
