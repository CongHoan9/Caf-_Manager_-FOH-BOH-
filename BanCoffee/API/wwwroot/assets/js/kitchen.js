let tableMap = {};

document.addEventListener("DOMContentLoaded", async () => {
    if (typeof loadUserInfo === 'function') loadUserInfo();
    await loadTableMap();
    await loadQueue();
    
    // Auto refresh every 5 seconds
    setInterval(loadQueue, 5000);
});

async function loadTableMap() {
    try {
        const tables = await fetchAPI('/Ban/get-all', 'GET');
        if (tables) {
            tables.forEach(t => {
                tableMap[t.ban_id] = t.ten_ban;
            });
        }
    } catch (e) {
        console.error(e);
    }
}

async function loadQueue() {
    try {
        const queue = await fetchAPI('/Kitchen/queue', 'GET');
        renderKanban(queue || []);
    } catch (err) {
        console.error(err);
    }
}

function renderKanban(queue) {
    const col0 = document.getElementById('col-state-0');
    const col1 = document.getElementById('col-state-1');
    const col2 = document.getElementById('col-state-2');
    if (!col0 || !col1 || !col2) return;
    
    col0.innerHTML = ''; col1.innerHTML = ''; col2.innerHTML = '';
    let c0 = 0, c1 = 0, c2 = 0;

    const role = typeof getCurrentRole === 'function' ? getCurrentRole() : 'Admin';
    const canCook = ['Admin', 'Bep', 'Staff'].includes(role);
    const canServe = ['Admin', 'PhucVu', 'Bep', 'Staff'].includes(role);

    queue.forEach(q => {
        const item = q.item;
        const timeOrdered = new Date(item.time_ordered).toLocaleTimeString('vi-VN', {hour: '2-digit', minute:'2-digit'});
        const tableName = tableMap[q.ban_id] || `Bàn ${(q.ban_id || '').substring(0, 6)}`;
        
        let actions = '';
        if (item.state === 0) {
            if (canCook) {
                actions = `<button class="btn btn-warning btn-sm w-100 fw-bold" onclick="updateState('${q.order_id}', '${item.id}', 1)">Bắt đầu pha chế &rarr;</button>`;
            } else {
                actions = `<div class="text-muted small text-center">Đang chờ Bếp nhận...</div>`;
            }
            c0++;
        } else if (item.state === 1) {
            if (canCook) {
                actions = `<button class="btn btn-primary btn-sm w-100 fw-bold" onclick="updateState('${q.order_id}', '${item.id}', 2)">Hoàn tất pha chế &rarr;</button>`;
            } else {
                actions = `<div class="text-primary small text-center">Bếp đang làm...</div>`;
            }
            c1++;
        } else if (item.state === 2) {
            if (canServe) {
                actions = `<button class="btn btn-success btn-sm w-100 fw-bold" onclick="updateState('${q.order_id}', '${item.id}', 3)">Đã giao cho khách &check;</button>`;
            } else {
                actions = `<div class="text-success text-center fw-bold mt-2">Chờ phục vụ bê đi</div>`;
            }
            c2++;
        }

        const div = document.createElement('div');
        div.className = `ticket-card state-${item.state}`;
        div.innerHTML = `
            <div class="d-flex justify-content-between align-items-center mb-2">
                <span class="fw-bold fs-6 text-dark">${item.item_name} <span class="badge bg-danger ms-1">x${item.quantity}</span></span>
                <span class="badge bg-secondary">${tableName}</span>
            </div>
            ${item.note ? `<div class="text-danger small mb-2">Ghi chú: ${item.note}</div>` : ''}
            <div class="item-time mb-3">Giờ gọi: ${timeOrdered}</div>
            ${actions}
        `;
        
        if (item.state === 0) col0.appendChild(div);
        else if (item.state === 1) col1.appendChild(div);
        else if (item.state === 2) col2.appendChild(div);
    });

    if (c0 === 0) col0.innerHTML = '<div class="text-muted text-center py-4 small">Không có món chờ</div>';
    if (c1 === 0) col1.innerHTML = '<div class="text-muted text-center py-4 small">Không có món đang làm</div>';
    if (c2 === 0) col2.innerHTML = '<div class="text-muted text-center py-4 small">Không có món chờ giao</div>';

    document.getElementById('count-0').innerText = c0;
    document.getElementById('count-1').innerText = c1;
    document.getElementById('count-2').innerText = c2;
}

window.updateState = async function(orderId, itemId, newState) {
    try {
        await fetchAPI('/Kitchen/update-state', 'POST', {
            order_id: orderId,
            item_id: itemId,
            state: newState
        });
        loadQueue();
    } catch(e) {
        alert("Lỗi: " + e.message);
    }
};
