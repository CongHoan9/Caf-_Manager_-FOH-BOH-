document.addEventListener('DOMContentLoaded', () => {
    const loginForm = document.getElementById('loginForm');
    if (!loginForm) return;

    loginForm.addEventListener('submit', async function(e) {
        e.preventDefault();
        const user = document.getElementById('txtUsername').value;
        const pass = document.getElementById('txtPassword').value;
        const alertBox = document.getElementById('loginAlert');
        
        try {
            const data = await fetchAPI('/Users/login', 'POST', { Username: user, Password: pass });
            if (data && data.token) {
                localStorage.setItem('token', data.token);
                localStorage.setItem('user', JSON.stringify(data));
                window.location.href = 'dashboard.html';
            }
        } catch (err) {
            alertBox.innerText = err.message || "Đăng nhập thất bại. Vui lòng thử lại.";
            alertBox.classList.remove('d-none');
        }
    });
});
