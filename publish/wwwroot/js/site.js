document.addEventListener('DOMContentLoaded', function() {
    const navItems = document.querySelectorAll('.nav-item');
    const currentPath = window.location.pathname.toLowerCase();

    navItems.forEach(item => {
        item.classList.remove('active');
        const href = item.getAttribute('onclick');
        if (href) {
            const match = href.match(/\/\w+/g);
            if (match && currentPath.includes(match[0].toLowerCase())) {
                item.classList.add('active');
            }
        }
    });

    const pageTitle = document.getElementById('pageTitle');
    const activeNav = document.querySelector('.nav-item.active');
    if (pageTitle && activeNav) {
        const text = activeNav.textContent.trim();
        if (text && text.length < 30) {
            pageTitle.textContent = text;
        }
    }
});
