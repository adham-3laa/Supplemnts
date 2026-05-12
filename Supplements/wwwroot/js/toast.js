function showToast(message, type) {
    const container = document.getElementById('toastContainer');
    if (!container) return;

    const colors = {
        success: 'bg-green-500',
        error: 'bg-red-500',
        warning: 'bg-yellow-500',
        info: 'bg-blue-500'
    };
    const icons = {
        success: 'check_circle',
        error: 'error',
        warning: 'warning',
        info: 'info'
    };

    const bgColor = colors[type] || colors.info;
    const icon = icons[type] || icons.info;

    const toast = document.createElement('div');
    toast.className = `flex items-center gap-3 px-4 py-3 rounded-lg text-white shadow-lg transform translate-x-full opacity-0 transition-all duration-300 ${bgColor}`;
    toast.innerHTML = `
        <span class="material-icons text-xl">${icon}</span>
        <span class="text-sm font-medium">${message}</span>
        <button onclick="this.parentElement.remove()" class="ml-auto hover:opacity-80">
            <span class="material-icons text-lg">close</span>
        </button>
    `;

    container.appendChild(toast);

    requestAnimationFrame(() => {
        toast.classList.remove('translate-x-full', 'opacity-0');
    });

    setTimeout(() => {
        toast.classList.add('translate-x-full', 'opacity-0');
        setTimeout(() => toast.remove(), 300);
    }, 4000);
}
