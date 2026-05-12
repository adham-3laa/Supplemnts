document.addEventListener('DOMContentLoaded', function () {
    const toggle = document.getElementById('darkModeToggle');
    const html = document.documentElement;

    const saved = localStorage.getItem('darkMode');
    if (saved === 'true') {
        html.classList.add('dark');
    }

    if (toggle) {
        toggle.addEventListener('click', function () {
            html.classList.toggle('dark');
            localStorage.setItem('darkMode', html.classList.contains('dark'));
        });
    }

    const mobileToggle = document.getElementById('mobileDarkModeToggle');
    if (mobileToggle) {
        mobileToggle.addEventListener('click', function () {
            html.classList.toggle('dark');
            localStorage.setItem('darkMode', html.classList.contains('dark'));
            const icon = this.querySelector('i');
            if (icon) {
                icon.textContent = html.classList.contains('dark') ? 'light_mode' : 'dark_mode';
            }
        });
    }
});
