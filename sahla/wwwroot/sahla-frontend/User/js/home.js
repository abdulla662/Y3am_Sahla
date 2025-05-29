const currentUrl = new URL(window.location.href);
const tokenParam = currentUrl.searchParams.get('token');

if (tokenParam && tokenParam.split('.').length === 3) {
    localStorage.setItem('jwtToken', tokenParam);
    alert('Google login successful!');
    window.history.replaceState({}, document.title, '/sahla-frontend/User/html/home.html');
}