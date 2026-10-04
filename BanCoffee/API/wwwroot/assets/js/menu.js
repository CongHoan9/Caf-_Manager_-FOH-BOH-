let menuModal;
document.addEventListener("DOMContentLoaded", () => {
    const modalEl = document.getElementById('menuModal');
    if (modalEl) {
        menuModal = new bootstrap.Modal(modalEl);
    }
    loadMenuItems();
});

async function loadMenuItems() {
    const tbody = document.querySelector('#menuTable tbody');
    if (!tbody) return;
    try {
        const response = await fetchAPI('/Item/search', 'POST', { page: 1, pageSize: 100 });
        if (response && response.data) {
            tbody.innerHTML = '';
            if (response.data.length === 0) {
                tbody.innerHTML = '<tr><td colspan="3" class="text-center">Không có dữ liệu</td></tr>';
            } else {
                response.data.forEach(item => {
                    const tr = document.createElement('tr');
                    tr.innerHTML = `
                        <td>${item.item_name}</td>
                        <td class="fw-bold text-success">${Number(item.item_price).toLocaleString('vi-VN')} đ</td>
                        <td class="admin-only">
                            <button class="btn btn-sm btn-warning me-1" onclick="editItem('${item.item_id}')">Sửa</button>
                            <button class="btn btn-sm btn-danger" onclick="deleteItem('${item.item_id}')">Xóa</button>
                        </td>
                    `;
                    tbody.appendChild(tr);
                });
                
                if (typeof loadUserInfo === 'function') {
                    loadUserInfo(); 
                }
            }
        }
    } catch (err) {
        tbody.innerHTML = `<tr><td colspan="3" class="text-center text-danger">Lỗi: ${err.message}</td></tr>`;
    }
}

window.showAddModal = function() {
    document.getElementById('item_id').value = '';
    document.getElementById('item_name').value = '';
    document.getElementById('item_price').value = '0';
    document.getElementById('item_group_id').value = '';
    document.getElementById('menuModalLabel').innerText = 'Thêm Sản phẩm';
    menuModal.show();
};

window.editItem = async function(id) {
    try {
        const item = await fetchAPI(`/Item/get-by-id/${id}`, 'GET');
        if (item) {
            document.getElementById('item_id').value = item.item_id || '';
            document.getElementById('item_name').value = item.item_name || '';
            document.getElementById('item_price').value = item.item_price || 0;
            document.getElementById('item_group_id').value = item.item_group_id || '';
            document.getElementById('menuModalLabel').innerText = 'Sửa Sản phẩm';
            menuModal.show();
        }
    } catch (err) {
        alert("Lỗi: " + err.message);
    }
};

window.saveItem = async function() {
    const id = document.getElementById('item_id').value;
    const payload = {
        item_name: document.getElementById('item_name').value,
        item_price: parseFloat(document.getElementById('item_price').value),
        item_group_id: document.getElementById('item_group_id').value
    };
    try {
        if (!id) {
            await fetchAPI('/Item/create-item', 'POST', payload);
            alert('Đã thêm sản phẩm!');
        } else {
            payload.item_id = id;
            await fetchAPI('/Item/update-item', 'POST', payload);
            alert('Đã cập nhật sản phẩm!');
        }
        menuModal.hide();
        loadMenuItems();
    } catch (err) {
        alert('Lỗi: ' + err.message);
    }
};

window.deleteItem = async function(id) {
    if (!confirm('Xóa món này?')) return;
    try {
        await fetchAPI('/Item/delete', 'POST', { item_id: id });
        loadMenuItems();
    } catch (err) {
        alert('Lỗi: ' + err.message);
    }
};