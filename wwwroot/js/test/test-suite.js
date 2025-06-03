'use strict';
class TestSuite {
    constructor() {
        this.tests = [];
        this.results = [];
    }
    addTest(name, testFn, category = 'general') {
        this.tests.push({ name, testFn, category });
    }
    async runCategory(category) {
        const results = [];
        for (const t of this.tests.filter(t => t.category === category)) {
            try {
                await t.testFn();
                results.push({ name: t.name, status: 'PASS', category });
            } catch (e) {
                results.push({ name: t.name, status: 'FAIL', error: e.message, category });
            }
        }
        this.results.push(...results);
        return results;
    }
    async runAll() {
        this.results = [];
        for (const category of [...new Set(this.tests.map(t => t.category))]) {
            await this.runCategory(category);
        }
        this.showResults();
        return this.results;
    }
    showResults() {
        const resultsDiv = document.getElementById('testResults');
        if (!resultsDiv) return;
        const passed = this.results.filter(r => r.status === 'PASS').length;
        const failed = this.results.filter(r => r.status === 'FAIL').length;
        let html = `
            <div class="mb-3">
                <h5>Resumen de Tests</h5>
                <div class="row">
                    <div class="col-md-4">
                        <div class="alert alert-info"><strong>Total:</strong> ${this.results.length} tests</div>
                    </div>
                    <div class="col-md-4">
                        <div class="alert alert-success"><strong>Pasados:</strong> ${passed} tests</div>
                    </div>
                    <div class="col-md-4">
                        <div class="alert ${failed > 0 ? 'alert-danger' : 'alert-success'}"><strong>Fallidos:</strong> ${failed} tests</div>
                    </div>
                </div>
            </div>
            <table class="table table-striped table-hover">
                <thead>
                    <tr><th>Test</th><th>Categoría</th><th>Estado</th><th>Detalles</th></tr>
                </thead>
                <tbody>
        `;
        for (const r of this.results) {
            const sc = r.status === 'PASS' ? 'success' : 'danger';
            const si = r.status === 'PASS' ? '✅' : '❌';
            html += `<tr>
                        <td>${r.name}</td>
                        <td><span class="badge bg-secondary">${r.category}</span></td>
                        <td><span class="badge bg-${sc}">${si} ${r.status}</span></td>
                        <td>${r.error || '-'}</td>
                     </tr>`;
        }
        html += '</tbody></table>';
        resultsDiv.innerHTML = html;
    }
}

const testSuite = new TestSuite();

// Tests de librerías
testSuite.addTest('Vue.js 3 cargado', () => {
    if (typeof Vue === 'undefined') throw new Error('Vue.js no está cargado');
    if (!Vue.version.startsWith('3')) throw new Error('Se requiere Vue 3');
}, 'librerias');
testSuite.addTest('Axios cargado', () => {
    if (typeof axios === 'undefined') throw new Error('Axios no está cargado');
}, 'librerias');
testSuite.addTest('jQuery cargado', () => {
    if (typeof $ === 'undefined' && typeof jQuery === 'undefined') throw new Error('jQuery no está cargado');
}, 'librerias');
testSuite.addTest('Bootstrap JS cargado', () => {
    if (typeof bootstrap === 'undefined') throw new Error('Bootstrap JS no está cargado');
}, 'librerias');
testSuite.addTest('SweetAlert2 cargado', () => {
    if (typeof Swal === 'undefined') throw new Error('SweetAlert2 no está cargado');
}, 'librerias');
testSuite.addTest('Font Awesome CSS cargado', () => {
    if (!document.querySelector('link[href*="font-awesome"]')) throw new Error('Font Awesome CSS no está cargado');
}, 'librerias');

testSuite.addTest('formatPrice definido', () => {
    if (typeof formatPrice !== 'function') throw new Error('formatPrice no está definida');
    if (formatPrice(1234.56) !== '1.234,56') throw new Error(`formatPrice retornó ${formatPrice(1234.56)}`);
}, 'funciones');

testSuite.addTest('ClienteSearch definido', () => {
    if (typeof ClienteSearch === 'undefined') throw new Error('ClienteSearch no está definido');
    if (!ClienteSearch.template || !ClienteSearch.methods || !ClienteSearch.emits) throw new Error('ClienteSearch estructura inválida');
}, 'componentes');
testSuite.addTest('ProductoSearch definido', () => {
    if (typeof ProductoSearch === 'undefined') throw new Error('ProductoSearch no está definido');
    if (!ProductoSearch.template || !ProductoSearch.methods || !ProductoSearch.methods.buscar) throw new Error('ProductoSearch estructura inválida');
}, 'componentes');
testSuite.addTest('DetalleGrid definido', () => {
    if (typeof DetalleGrid === 'undefined') throw new Error('DetalleGrid no está definido');
    if (!DetalleGrid.template || !DetalleGrid.props || !DetalleGrid.computed) throw new Error('DetalleGrid estructura inválida');
}, 'componentes');
testSuite.addTest('FormaPagoSelector definido', () => {
    if (typeof FormaPagoSelector === 'undefined') throw new Error('FormaPagoSelector no está definido');
    if (!FormaPagoSelector.template || !FormaPagoSelector.methods) throw new Error('FormaPagoSelector estructura inválida');
}, 'componentes');

// Tests de API
testSuite.addTest('API Clientes - Búsqueda', async () => {
    const res = await axios.get('/api/search/clientes?termino=test');
    if (!Array.isArray(res.data)) throw new Error('Respuesta inválida');
}, 'api');
testSuite.addTest('API Productos - Búsqueda', async () => {
    const res = await axios.get('/api/search/productos?termino=test');
    if (!Array.isArray(res.data)) throw new Error('Respuesta inválida');
}, 'api');
testSuite.addTest('API Clientes - GetByDNI', async () => {
    const res = await axios.get('/Cliente/Search?dni=12345671');
    if (res.data.success === undefined) throw new Error('Propiedad success ausente');
}, 'api');
testSuite.addTest('API Ciudades - Por Provincia', async () => {
    const res = await axios.get('/Cliente/GetCiudades?provinciaId=1');
    if (!Array.isArray(res.data)) throw new Error('Respuesta inválida');
}, 'api');

// Tests de base de datos
testSuite.addTest('DB Conexión', async () => {
    const res = await axios.get('/DatabaseTest/TestConnection');
    if (!res.data.success) throw new Error('No conecta BD');
}, 'database');

testSuite.addTest('DB Tablas', async () => {
    const res = await axios.get('/DatabaseTest/TestAllTables');
    const data = res.data;
    // Validar que al menos incluya lista de tablas existentes y missingTables
    if (!('allTables' in data)) {
        throw new Error('El endpoint no devuelve "allTables" con la lista de tablas');
    }
    if (!('missingTables' in data)) {
        throw new Error('El endpoint no devuelve "missingTables"');
    }
    if (!data.success) {
        throw new Error('El endpoint devolvió éxito = false');
    }
}, 'database');

async function runAllTests() {
    console.clear();
    await testSuite.runAll();
    const failed = testSuite.results.filter(r => r.status === 'FAIL').length;
    if (failed === 0) Swal.fire({ icon: 'success', title: '¡Tests Completados!', text: `Todos los ${testSuite.results.length} tests pasaron` });
    else Swal.fire({ icon: 'warning', title: 'Tests Completados', text: `${testSuite.results.length - failed} pasaron, ${failed} fallaron` });
}
async function runComponentTests() { await testSuite.runCategory('componentes'); testSuite.showResults(); }
async function runApiTests() { await testSuite.runCategory('api'); testSuite.showResults(); }
async function runDatabaseTests() { await testSuite.runCategory('database'); testSuite.showResults(); }

document.addEventListener('DOMContentLoaded', () => {
    console.log('🚀 Suite de tests cargada. Ejecuta runAllTests() para iniciar.');
});