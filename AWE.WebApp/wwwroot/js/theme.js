/* Theme Management JavaScript */

class ThemeManager {
    constructor() {
        this.html = document.documentElement;
        this.storageKey = 'awe-theme-preference';
        this.prefersDark = window.matchMedia('(prefers-color-scheme: dark)');
        this.init();
    }

    init() {
        // Load saved theme or use system preference
        const saved = localStorage.getItem(this.storageKey);
        const theme = saved || this.getSystemTheme();
        this.setTheme(theme);

        // Listen for system theme changes
        this.prefersDark.addEventListener('change', (e) => {
            if (!localStorage.getItem(this.storageKey)) {
                this.setTheme(e.matches ? 'dark' : 'light');
            }
        });

        // Expose toggle method to window
        window.toggleTheme = () => this.toggle();
    }

    getSystemTheme() {
        return this.prefersDark.matches ? 'dark' : 'light';
    }

    setTheme(theme) {
        const html = document.documentElement;
        html.classList.add('no-transition');
        
        html.setAttribute('data-theme', theme);
        localStorage.setItem(this.storageKey, theme);
        
        // Update theme-color meta tag for mobile browsers
        const metaThemeColor = document.querySelector('meta[name="theme-color"]');
        if (metaThemeColor) {
            metaThemeColor.setAttribute('content', theme === 'dark' ? '#1f2937' : '#667eea');
        }

        // Trigger custom event for other listeners
        window.dispatchEvent(new CustomEvent('theme-changed', { detail: { theme } }));

        // Remove no-transition class after transition
        setTimeout(() => {
            html.classList.remove('no-transition');
        }, 50);
    }

    toggle() {
        const current = this.html.getAttribute('data-theme') || this.getSystemTheme();
        const newTheme = current === 'light' ? 'dark' : 'light';
        this.setTheme(newTheme);
    }

    getTheme() {
        return this.html.getAttribute('data-theme') || this.getSystemTheme();
    }
}

// Initialize theme manager when DOM is ready
if (document.readyState === 'loading') {
    document.addEventListener('DOMContentLoaded', () => {
        new ThemeManager();
    });
} else {
    new ThemeManager();
}
