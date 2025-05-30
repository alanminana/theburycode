// Configuración global para Vue.js
Vue.config.productionTip = false;

// Configuración global para axios
if (typeof axios !== 'undefined') {
    axios.defaults.headers.common['X-Requested-With'] = 'XMLHttpRequest';

    // Agregar token CSRF si existe
    const token = document.querySelector('meta[name="__RequestVerificationToken"]');
    if (token) {
        axios.defaults.headers.common['RequestVerificationToken'] = token.getAttribute('content');
    }
}

// Función global para formatear precios
window.formatPrice = function (value) {
    return new Intl.NumberFormat('es-AR', {
        minimumFractionDigits: 2,
        maximumFractionDigits: 2
    }).format(value || 0);
};