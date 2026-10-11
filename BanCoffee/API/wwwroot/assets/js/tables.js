let currentTables = [];
let menuItems = [];

document.addEventListener("DOMContentLoaded", () => {
    loadTables();
    loadMenuItems();

    const btnAdd = document.getElementById('btnAddTable');
    if (btnAdd) {
        btnAdd.addEventListener('click', () => {
            openAddTableModal(); // Danh cho admin/staff cu
        });
    }
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

        const userJson = localStorage.getItem('user');
        const user = userJson ? JSON.parse(userJson) : null;
        const role = user ? (user.Role || user.role) : '';

        currentTables.forEach(table => {
            let bgClass = 'bg-trong';
            if (table.trang_thai === 'Đang phục vụ' || table.trang_thai === 'Đang sử dụng') bgClass = 'bg-dang-phuc-vu';
            else if (table.trang_thai === 'Đã đặt') bgClass = 'bg-da-dat';

            const khuVuc = table.khu_vuc || 'Khu chung';
            const soCho = table.so_cho || 4;

            const div = document.createElement('div');
            div.className = 'col-xl-3 col-md-4 col-sm-6 mb-4';
            
            let btnConfig = '';
            if (role === 'Admin') {
                btnConfig = `<button class="btn btn-sm btn-light position-absolute top-0 end-0 m-1" onclick="event.stopPropagation(); openEditTableModal('${table.ban_id}')">⚙️</button>`;
            }

            div.innerHTML = `
                <div class="card shadow table-card ${bgClass} h-100 py-2 position-relative" onclick="openOrderModal('${table.ban_id}')" style="cursor:pointer">
                    ${btnConfig}
                    <div class="card-body text-center">
                        <h5 class="font-weight-bold text-dark mb-1">${table.ten_ban}</h5>
                        <div class="text-xs mb-2">${khuVuc} - ${soCho} chỗ</div>
                        <span class="badge bg-secondary text-white">${table.trang_thai}</span>
                    </div>
                </div>
            `;
            container.appendChild(div);
        });
        
        if (typeof loadUserInfo === 'function') {
            loadUserInfo(); 
        }
    } catch (err) {
        console.error(err);
    }
}

async function loadMenuItems() {
    try {
        menuItems = await fetchAPI('/Item/get-all', 'GET');
    } catch(e) { console.error("Khong the tai Menu", e); }
}

window.openOrderModal = async function(id) {
    const table = currentTables.find(t => t.ban_id === id);
    if (!table) return;

    document.getElementById('posTableName').innerText = table.ten_ban;
    document.getElementById('posTableId').value = table.ban_id;
    
    const userJson = localStorage.getItem('user');
    const user = userJson ? JSON.parse(userJson) : null;
    const role = user ? (user.Role || user.role) : '';
    
    if (role === 'ThuNgan' || role === 'Admin') {
        document.getElementById('btnCheckout').style.display = 'inline-block';
    } else {
        document.getElementById('btnCheckout').style.display = 'none';
    }

    await reloadOrderView(id);
    new bootstrap.Modal(document.getElementById('orderModal')).show();
};

async function reloadOrderView(banId) {
    try {
        const order = await fetchAPI(`/Order/get-by-table/${banId}`, 'GET');
        const tbody = document.getElementById('posOrderList');
        const spanTotal = document.getElementById('posTotal');
        
        if (!order || !order.items || order.items.length === 0) {
            tbody.innerHTML = `<tr><td colspan="5" class="text-center">Chưa có order</td></tr>`;
            spanTotal.innerText = '0';
            return;
        }

        tbody.innerHTML = '';
        let total = 0;
        const states = ["Chờ làm", "Đang làm", "Xong", "Đã giao"];
        
        order.items.forEach(it => {
            const tr = document.createElement('tr');
            const tt = (it.price || 0) * (it.quantity || 1);
            total += tt;
            
            let badgeClass = 'bg-secondary';
            if (it.state === 1) badgeClass = 'bg-warning text-dark';
            if (it.state === 2) badgeClass = 'bg-success';
            
            tr.innerHTML = `
                <td>${it.item_name}</td>
                <td>${it.quantity}</td>
                <td>${(it.price||0).toLocaleString()}</td>
                <td>${it.note || ''}</td>
                <td><span class="badge ${badgeClass}">${states[it.state] || ''}</span></td>
            `;
            tbody.appendChild(tr);
        });
        
        spanTotal.innerText = total.toLocaleString();
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
        opt.innerText = `${mi.item_name} - ${mi.item_price.toLocaleString()}đ`;
        sel.appendChild(opt);
    });
    
    document.getElementById('addQty').value = '1';
    document.getElementById('addNote').value = '';
    
    new bootstrap.Modal(document.getElementById('addItemModal')).show();
};

window.confirmAddItem = async function() {
    const banId = document.getElementById('posTableId').value;
    const sel = document.getElementById('addSelectMenu');
    const opt = sel.options[sel.selectedIndex];
    
    if(!opt) return alert("Chọn món trước");
    
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
        bootstrap.Modal.getInstance(document.getElementById('addItemModal')).hide();
        await reloadOrderView(banId);
        
        // Auto update table status to Dang phuc vu
        const table = currentTables.find(t => t.ban_id === banId);
        if (table && table.trang_thai === 'Trống') {
            table.trang_thai = 'Đang phục vụ';
            await fetchAPI('/Ban/update-ban', 'POST', table);
            loadTables();
        }
    } catch(e) {
        alert(e.message || "Không thể gọi món (Có thể hết nguyên liệu)");
    }
};

window.checkoutOrder = async function() {
    if(!confirm("Bạn có chắc chắn thanh toán bàn này?")) return;
    const banId = document.getElementById('posTableId').value;
    try {
        const res = await fetchAPI('/Order/checkout', 'POST', {
            ban_id: banId,
            payment_method: 'Tiền mặt',
            giam_gia: 0,
            ghi_chu: ''
        });
        alert(res.message);
        
        // Update table back to Trống
        const table = currentTables.find(t => t.ban_id === banId);
        if (table) {
            table.trang_thai = 'Trống';
            await fetchAPI('/Ban/update-ban', 'POST', table);
        }
        
        bootstrap.Modal.getInstance(document.getElementById('orderModal')).hide();
        loadTables();
    } catch(e) {
        alert(e.message);
    }
};

window.clearOrder = async function() {
    if(!confirm("Hủy đơn hàng của bàn này (món đang làm cũng sẽ bị hủy)?")) return;
    const banId = document.getElementById('posTableId').value;
    try {
        await fetchAPI(`/Order/clear/${banId}`, 'POST');
        
        const table = currentTables.find(t => t.ban_id === banId);
        if (table) {
            table.trang_thai = 'Trống';
            await fetchAPI('/Ban/update-ban', 'POST', table);
        }
        
        bootstrap.Modal.getInstance(document.getElementById('orderModal')).hide();
        loadTables();
    } catch(e) {
        alert(e.message);
    }
};

// ADMIN: Edit Table mapping
window.openAddTableModal = function() {
    document.getElementById('tableId').value = '';
    document.getElementById('tableModalTitle').innerText = 'Thêm bàn mới';
    document.getElementById('tableName').value = '';
    document.getElementById('tableZone').value = '';
    document.getElementById('tableCapacity').value = '4';
    document.getElementById('tableStatus').value = 'Trống';
    
    document.getElementById('tableName').disabled = false;
    document.getElementById('tableZone').disabled = false;
    document.getElementById('tableCapacity').disabled = false;
    new bootstrap.Modal(document.getElementById('tableModal')).show();
};

window.openEditTableModal = function(id) {
    const table = currentTables.find(t => t.ban_id === id);
    if (!table) return;

    document.getElementById('tableId').value = table.ban_id;
    document.getElementById('tableModalTitle').innerText = 'Sửa thông tin bàn';
    document.getElementById('tableName').value = table.ten_ban;
    document.getElementById('tableZone').value = table.khu_vuc;
    document.getElementById('tableCapacity').value = table.so_cho;
    document.getElementById('tableStatus').value = table.trang_thai;
    
    document.getElementById('tableName').disabled = false;
    document.getElementById('tableZone').disabled = false;
    document.getElementById('tableCapacity').disabled = false;
    new bootstrap.Modal(document.getElementById('tableModal')).show();
};

window.saveTable = async function() {
    const id = document.getElementById('tableId').value;
    const model = {
        ban_id: id || '00000000-0000-0000-0000-000000000000',
        ten_ban: document.getElementById('tableName').value,
        khu_vuc: document.getElementById('tableZone').value,
        so_cho: parseInt(document.getElementById('tableCapacity').value) || 4,
        trang_thai: document.getElementById('tableStatus').value
    };

    if (!model.ten_ban) {
        alert("Vui lòng nhập tên bàn");
        return;
    }

    try {
        if (!id) {
            await fetchAPI('/Ban/create-ban', 'POST', model);
        } else {
            await fetchAPI('/Ban/update-ban', 'POST', model);
        }
        bootstrap.Modal.getInstance(document.getElementById('tableModal')).hide();
        loadTables();
    } catch(e) {
        alert(e.message);
    }
};
