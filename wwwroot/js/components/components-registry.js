// wwwroot/js/components/components-registry.js
// Registro centralizado de componentes Vue

// Asegurar que los componentes estén cargados
if (typeof ClienteSearch === 'undefined') {
    console.error('ClienteSearch no está cargado');
}
if (typeof ProductoSearch === 'undefined') {
    console.error('ProductoSearch no está cargado');
}
if (typeof DetalleGrid === 'undefined') {
    console.error('DetalleGrid no está cargado');
}
if (typeof FormaPagoSelector === 'undefined') {
    console.error('FormaPagoSelector no está cargado');
}

// Registro global de componentes para fácil acceso
window.Components = {
    ClienteSearch,
    ProductoSearch,
    DetalleGrid,
    FormaPagoSelector
};

// Función helper para registrar componentes en una app Vue
window.registerComponents = function (app) {
    app.component('cliente-search', ClienteSearch);
    app.component('producto-search', ProductoSearch);
    app.component('detalle-grid', DetalleGrid);
    app.component('forma-pago-selector', FormaPagoSelector);

    console.log('Componentes registrados:', Object.keys(window.Components));
};

// Para uso con Vue 3 createApp
window.createVueApp = function (options = {}) {
    const app = Vue.createApp(options);
    registerComponents(app);
    return app;
};