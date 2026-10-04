let currentTables = [];

document.addEventListener("DOMContentLoaded", () => {
    loadTables();

    // Hook add button explicitly just in case
    const btnAdd = document.getElementById('btnAddTable');
    if (btnAdd) {
        btnAdd.addEventListener('click', () => {
            openAddTableModal();
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

        currentTables.forEach(table => {
            let bgClass = 'bg-trong';
            if (table.trang_thai === 'Đang phục vụ' || table.trang_thai === 'Đang sử dụng') bgClass = 'bg-dang-phuc-vu';
            else if (table.trang_thai === 'Đã đặt') bgClass = 'bg-da-dat';

            const khuVuc = table.khu_vuc || 'Khu chung';
            const soCho = table.so_cho || 4;

            const div = document.createElement('div');
            div.className = 'col-xl-3 col-md-4 col-sm-6 mb-4';
            // We use standard onclick for simplicity since it's admin-only edit anyway
            div.innerHTML = `
                <div class="card shadow table-card ${bgClass} h-100 py-2" onclick="openEditTableModal('${table.ban_id}')" style="cursor:pointer">
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
        alert("Không thể tải danh sách bàn!");
    }
}

window.openAddTableModal = function() {
    document.getElementById('tableId').value = '';
    document.getElementById('tableModalTitle').innerText = 'Thêm bàn mới';
    document.getElementById('tableName').value = '';
    document.getElementById('tableZone').value = '';
    document.getElementById('tableCapacity').value = '4';
    document.getElementById('tableStatus').value = 'Trống';
    
    // Bỏ khóa các trường
    document.getElementById('tableName').disabled = false;
    document.getElementById('tableZone').disabled = false;
    document.getElementById('tableCapacity').disabled = false;
};

window.openEditTableModal = function(id) {
    const table = currentTables.find(t => t.ban_id === id);
    if (!table) return;

    document.getElementById('tableId').value = table.ban_id;
    document.getElementById('tableModalTitle').innerText = 'Sửa thông tin bàn';
    document.getElementById('tableName').value = table.ten_ban;
    document.getElementById('tableZone').value = table.khu_vuc;
    document.getElementById('tableCapacity').value = table.so_cho;
    
    let st = table.trang_thai;
    if (st === 'Đang sử dụng') st = 'Đang phục vụ'; // map về option có sẵn
    document.getElementById('tableStatus').value = st;
    
    const userJson = localStorage.getItem('user');
    const user = userJson ? JSON.parse(userJson) : null;
    const role = user ? (user.Role || user.role) : '';

    // Nếu không phải admin thì chỉ được đổi trạng thái
    if (role !== 'Admin') {
        document.getElementById('tableName').disabled = true;
        document.getElementById('tableZone').disabled = true;
        document.getElementById('tableCapacity').disabled = true;
    } else {
        document.getElementById('tableName').disabled = false;
        document.getElementById('tableZone').disabled = false;
        document.getElementById('tableCapacity').disabled = false;
    }

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
            // Thêm mới
            await fetchAPI('/Ban/create-ban', 'POST', model);
        } else {
            // Sửa
            await fetchAPI('/Ban/update-ban', 'POST', model);
        }
        bootstrap.Modal.getInstance(document.getElementById('tableModal')).hide();
        loadTables();
    } catch(e) {
        alert(e.message);
    }
};