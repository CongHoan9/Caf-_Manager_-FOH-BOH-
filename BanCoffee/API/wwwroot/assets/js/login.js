document.addEventListener('DOMContentLoaded', () => {
    const loginForm = document.getElementById('loginForm');
    if (!loginForm) return;

    const userInput = document.getElementById('txtUsername');
    const passInput = document.getElementById('txtPassword');
    if (userInput) userInput.value = '';
    if (passInput) passInput.value = '';

    loginForm.addEventListener('submit', async function(e) {
        e.preventDefault();
        const user = userInput.value.trim();
        const pass = passInput.value;
        const alertBox = document.getElementById('loginAlert');
        alertBox.classList.add('d-none');
        
        try {
            const data = await fetchAPI('/Users/login', 'POST', { Username: user, Password: pass });
            if (data && data.token) {
                localStorage.setItem('token', data.token);
                localStorage.setItem('user', JSON.stringify(data));
                const role = data.role || data.Role || 'Staff';
                if (typeof getHomePageByRole === 'function') {
                    window.location.href = getHomePageByRole(role);
                } else {
                    window.location.href = 'dashboard.html';
                }
            }
        } catch (err) {
            alertBox.innerText = err.message || "Đăng nhập thất bại. Vui lòng thử lại.";
            alertBox.classList.remove('d-none');
        }
    });
});
