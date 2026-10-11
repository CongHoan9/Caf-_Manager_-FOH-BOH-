document.addEventListener("DOMContentLoaded", () => {
    loadQueue();
    if (typeof loadUserInfo === 'function') loadUserInfo();
    
    // Auto refresh every 5 seconds for Real-time feel
    setInterval(loadQueue, 5000);
});

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
    
    col0.innerHTML = ''; col1.innerHTML = ''; col2.innerHTML = '';
    let c0 = 0, c1 = 0, c2 = 0;

    queue.forEach(q => {
        const item = q.item;
        const timeOrdered = new Date(item.time_ordered).toLocaleTimeString('vi-VN', {hour: '2-digit', minute:'2-digit'});
        
        let actions = '';
        if (item.state === 0) {
            actions = `<button class="btn btn-warning btn-sm w-100 fw-bold" onclick="updateState('${q.order_id}', '${item.id}', 1)">Bắt đầu làm</button>`;
            c0++;
        } else if (item.state === 1) {
            actions = `<button class="btn btn-success btn-sm w-100 fw-bold" onclick="updateState('${q.order_id}', '${item.id}', 2)">Hoàn tất</button>`;
            c1++;
        } else if (item.state === 2) {
            actions = `<div class="text-success text-center fw-bold mt-2">Chờ phục vụ bê đi</div>`;
            c2++;
        }

        const div = document.createElement('div');
        div.className = `ticket-card state-${item.state}`;
        div.innerHTML = `
            <div class="d-flex justify-content-between align-items-center mb-2">
                <span class="fw-bold fs-5 text-dark">${item.item_name} x${item.quantity}</span>
                <span class="badge bg-secondary">Bàn ${q.ban_id.split('-')[0].substring(0,6)}...</span>
            </div>
            ${item.note ? `<div class="text-danger small mb-2">Ghi chú: ${item.note}</div>` : ''}
            <div class="item-time mb-3">Giờ gọi: ${timeOrdered}</div>
            ${actions}
        `;
        
        if (item.state === 0) col0.appendChild(div);
        else if (item.state === 1) col1.appendChild(div);
        else if (item.state === 2) col2.appendChild(div);
    });

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
