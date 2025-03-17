(function() {
    const SwaggerThemeSwitcher = {
        elements: {
            createLabel: () => {
                const label = document.createElement('label');
                label.textContent = 'Theme \u00A0\u00A0\u00A0';
                label.className = 'theme-label swagger-ui label';
                label.style.setProperty('color', '#333333', 'important');
                return label;
            },
            createThemeSelect: () => {
                const select = document.createElement('select');
                select.className = 'theme-select';
                return select;
            },
            createContainer: () => {
                const container = document.createElement('div');
                container.className = 'theme-container';
                return container;
            },
            createFavicon: () => {
                const existingFavicons = document.querySelectorAll("link[rel='icon']");
                existingFavicons.forEach(favicon => {
                    favicon.href = '/images/logo-idecision2.png';
                });
            },
            createOpenInNewTabButton: () => {
                const button = document.createElement('button');
                button.textContent = 'Open in New Tab';
                button.className = 'swagger-ui btn new-tab-button';
                button.style.cssText = 'margin-left:10px;background:#4990e2;color:white;border:none;';
                button.onclick = () => {
                    const select = document.querySelector('select.servers select, select.servers-select');
                    if (select) {
                        const url = select.value;
                        if (url) {
                            const swaggerPath = window.location.pathname;
                            const targetUrl = new URL(swaggerPath, url).href;
                            window.open(targetUrl, '_blank');
                        }
                    }
                };
                return button;
            },
            handleCorsError: () => {
                const checkAndAddButton = () => {
                    const errorElements = document.querySelectorAll('.errors .message.thrown');
                    const hideButton = document.querySelector('.errors__clear-btn');
                    
                    const hasCorsError = Array.from(errorElements).some(element => 
                        element.textContent.includes('Failed to fetch')
                    );
                    
                    if (hasCorsError && hideButton) {
                        const existingButton = document.querySelector('.new-tab-button');
                        if (!existingButton) {
                            const button = document.createElement('button');
                            button.textContent = 'Open in New Tab';
                            button.className = 'btn new-tab-button';
                            button.style.cssText = 'margin-right:10px;background:#4990e2;color:white;border:none;';
                            
                            const select = document.querySelector('#select');
                            button.onclick = () => {
                                if (select?.value) {
                                    const baseUrl = new URL(select.value).origin;
                                    window.open(`${baseUrl}/swagger/index.html`, '_blank');
                                }
                            };
                            
                            hideButton.parentNode.insertBefore(button, hideButton);
                        }
                    }
                };

                // Add event listener for XHR/Fetch completion
                const originalFetch = window.fetch;
                window.fetch = function() {
                    return originalFetch.apply(this, arguments)
                        .finally(() => {
                            setTimeout(checkAndAddButton, 500);
                        });
                };

                const originalXHR = window.XMLHttpRequest;
                window.XMLHttpRequest = function() {
                    const xhr = new originalXHR();
                    const originalOnReadyStateChange = xhr.onreadystatechange;
                    xhr.onreadystatechange = function() {
                        if (xhr.readyState === 4) {
                            setTimeout(checkAndAddButton, 1500);
                        }
                        if (originalOnReadyStateChange) {
                            originalOnReadyStateChange.apply(this, arguments);
                        }
                    };
                    return xhr;
                };

                // Initial check
                setTimeout(checkAndAddButton, 1500);
            }
        },
        themes: {
            getOptions: () => [
                { name: 'Monokai (Auto: Dark/Light)', value: '/themes/theme-monokai-dark.css' },
                { name: 'Material', value: '/themes/theme-material.css' },
                { name: 'Monokai (Light & Dark Combination)', value: '/themes/theme-monokai.css' },
                { name: 'Feeling Blue', value: '/themes/theme-feeling-blue.css' },
                { name: 'Flattop', value: '/themes/theme-flattop.css' },
                { name: 'Muted', value: '/themes/theme-muted.css' },
                { name: 'Newspaper', value: '/themes/theme-newspaper.css' },
                { name: 'Outline', value: '/themes/theme-outline.css' }
            ].sort((a, b) => a.name.localeCompare(b.name)),
            populateOptions: (select) => {
                const defaultTheme = '/themes/theme-monokai-dark.css';
                SwaggerThemeSwitcher.themes.getOptions().forEach(theme => {
                    const option = document.createElement('option');
                    option.value = theme.value;
                    option.text = theme.name;
                    option.selected = theme.value === defaultTheme;
                    select.appendChild(option);
                });
            },
            update: (themePath) => {
                let link = document.querySelector('link[data-theme="swagger-theme"]');
                if (!link) {
                    link = document.createElement('link');
                    link.setAttribute('data-theme', 'swagger-theme');
                    link.setAttribute('rel', 'stylesheet');
                    document.head.appendChild(link);
                }
                link.setAttribute('href', themePath);
                localStorage.setItem('swagger_theme', themePath);
            },
            handleSaved: (select) => {
                const defaultTheme = '/themes/theme-monokai-dark.css';
                const savedTheme = localStorage.getItem('swagger_theme') ?? defaultTheme;
                SwaggerThemeSwitcher.themes.update(savedTheme);
                select.value = savedTheme;
            }
        },
        init: () => {
            const container = SwaggerThemeSwitcher.elements.createContainer();
            const label = SwaggerThemeSwitcher.elements.createLabel();
            const select = SwaggerThemeSwitcher.elements.createThemeSelect();

            SwaggerThemeSwitcher.themes.populateOptions(select);
            SwaggerThemeSwitcher.elements.createFavicon();

            select.onchange = function() {
                SwaggerThemeSwitcher.themes.update(this.value);
            };

            SwaggerThemeSwitcher.themes.handleSaved(select);

            container.appendChild(label);
            container.appendChild(select);
            document.body.appendChild(container);

            // Add CORS error handler
            SwaggerThemeSwitcher.elements.handleCorsError();
        }
    };

    if (document.body) {
        SwaggerThemeSwitcher.init();
    } else {
        document.addEventListener('DOMContentLoaded', SwaggerThemeSwitcher.init);
    }
})();
