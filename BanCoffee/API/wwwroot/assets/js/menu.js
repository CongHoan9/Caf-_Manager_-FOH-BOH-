let menuModal;
let itemGroups = [];

document.addEventListener("DOMContentLoaded", async () => {
    const role = typeof getCurrentRole === 'function' ? getCurrentRole() : '';
    const isAdmin = (role === 'Admin');

    const headerActions = document.getElementById('menuHeaderActions');
    if (headerActions) {
        headerActions.innerHTML = '';
        if (isAdmin) {
            const btn = document.createElement('button');
            btn.className = 'btn btn-primary btn-sm';
            btn.textContent = '+ Thêm Món';
            btn.onclick = showAddModal;
            headerActions.appendChild(btn);
        }
    }

    const headRow = document.getElementById('menuHeadRow');
    if (headRow && isAdmin) {
        const th = document.createElement('th');
        th.style.width = '140px';
        th.textContent = 'Hành động';
        headRow.appendChild(th);
    }

    const modalEl = document.getElementById('menuModal');
    if (modalEl && isAdmin) {
        menuModal = new bootstrap.Modal(modalEl);
    } else if (modalEl) {
        modalEl.remove();
    }

    await loadItemGroups();
    await loadMenuItems();
});

async function loadItemGroups() {
    try {
        const res = await fetchAPI('/ItemGroup/get-menu', 'GET');
        itemGroups = [];
        const flatten = (list) => {
            (list || []).forEach(g => {
                itemGroups.push({ id: g.item_group_id, name: g.item_group_name });
                if (g.children && g.children.length > 0) flatten(g.children);
            });
        };
        flatten(res);

        const sel = document.getElementById('item_group_id');
        if (sel) {
            sel.innerHTML = '';
            itemGroups.forEach(g => {
                sel.innerHTML += `<option value="${g.id}">${g.name}</option>`;
            });
        }
    } catch (e) {
        console.error("Lỗi tải nhóm món:", e);
    }
}

function getGroupName(groupId) {
    const g = itemGroups.find(x => x.id === groupId);
    return g ? g.name : 'Mặc định';
}

async function loadMenuItems() {
    const tbody = document.querySelector('#menuTable tbody');
    if (!tbody) return;
    const role = typeof getCurrentRole === 'function' ? getCurrentRole() : '';
    const isAdmin = (role === 'Admin');
    const colSpan = isAdmin ? 4 : 3;

    try {
        const response = await fetchAPI('/Item/search', 'POST', { page: 1, pageSize: 100 });
        if (response && response.data) {
            tbody.innerHTML = '';
            if (response.data.length === 0) {
                tbody.innerHTML = `<tr><td colspan="${colSpan}" class="text-center">Không có dữ liệu</td></tr>`;
            } else {
                response.data.forEach(item => {
                    const tr = document.createElement('tr');
                    let actionCell = '';
                    if (isAdmin) {
                        actionCell = `
                            <td class="text-nowrap">
                                <button class="btn btn-sm btn-warning me-1" onclick="editItem('${item.item_id}')">Sửa</button>
                                <button class="btn btn-sm btn-danger" onclick="deleteItem('${item.item_id}')">Xóa</button>
                            </td>
                        `;
                    }
                    tr.innerHTML = `
                        <td class="fw-bold">${item.item_name}</td>
                        <td><span class="badge bg-info text-dark">${getGroupName(item.item_group_id)}</span></td>
                        <td class="fw-bold text-success text-end">${Number(item.item_price).toLocaleString('vi-VN')} đ</td>
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

window.showAddModal = function() {
    if (!menuModal) return;
    document.getElementById('item_id').value = '';
    document.getElementById('item_name').value = '';
    document.getElementById('item_price').value = '';
    if (itemGroups.length > 0) {
        document.getElementById('item_group_id').value = itemGroups[0].id;
    }
    document.getElementById('menuModalLabel').innerText = 'Thêm Sản phẩm';
    menuModal.show();
};

window.editItem = async function(id) {
    if (!menuModal) return;
    try {
        const item = await fetchAPI(`/Item/get-by-id/${id}`, 'GET');
        if (item) {
            document.getElementById('item_id').value = item.item_id || '';
            document.getElementById('item_name').value = item.item_name || '';
            document.getElementById('item_price').value = item.item_price || '';
            if (item.item_group_id) {
                document.getElementById('item_group_id').value = item.item_group_id;
            }
            document.getElementById('menuModalLabel').innerText = 'Sửa Sản phẩm';
            menuModal.show();
        }
    } catch (err) {
        alert("Lỗi: " + err.message);
    }
};

window.saveItem = async function() {
    const id = document.getElementById('item_id').value;
    const itemName = document.getElementById('item_name').value.trim();
    const itemPrice = parseFloat(document.getElementById('item_price').value);
    const groupId = document.getElementById('item_group_id').value || (itemGroups.length > 0 ? itemGroups[0].id : null);

    if (!itemName) {
        alert("Vui lòng nhập tên món!");
        return;
    }
    if (isNaN(itemPrice) || itemPrice <= 0) {
        alert("Giá bán phải lớn hơn 0!");
        return;
    }

    const payload = {
        item_name: itemName,
        item_price: itemPrice,
        item_group_id: groupId
    };
    try {
        if (!id) {
            await fetchAPI('/Item/create-item', 'POST', payload);
            alert('Đã thêm sản phẩm thành công!');
        } else {
            payload.item_id = id;
            await fetchAPI('/Item/update-item', 'POST', payload);
            alert('Đã cập nhật sản phẩm thành công!');
        }
        menuModal.hide();
        loadMenuItems();
    } catch (err) {
        alert('Lỗi: ' + err.message);
    }
};

window.deleteItem = async function(id) {
    if (!confirm('Bạn có chắc muốn xóa món này?')) return;
    try {
        await fetchAPI('/Item/delete', 'POST', { item_id: id });
        alert('Xóa món thành công!');
        loadMenuItems();
    } catch (err) {
        alert('Lỗi: ' + err.message);
    }
};
