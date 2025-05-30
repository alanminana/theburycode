// Suite de testing para componentes Vue
class TestSuite {
    constructor() {
        this.tests = [];
        this.results = [];
    }

    addTest(name, testFn) {
        this.tests.push({ name, testFn });
    }

    async runAll() {
        console.log('🧪 Iniciando suite de testing...');

        for (let test of this.tests) {
            try {
                await test.testFn();
                this.results.push({ name: test.name, status: 'PASS' });
                console.log(`✅ ${test.name} - PASS`);
            } catch (error) {
                this.results.push({ name: test.name, status: 'FAIL', error });
                console.log(`❌ ${test.name} - FAIL:`, error);
            }
        }

        this.showResults();
    }

    showResults() {
        console.log('\n📊 Resultados del testing:');
        console.table(this.results);
    }
}

// Tests específicos
const testSuite = new TestSuite();

// Test 1: Verificar Vue.js está cargado
testSuite.addTest('Vue.js loaded', () => {
    if (typeof Vue === 'undefined') throw new Error('Vue.js no está cargado');
});

// Test 2: Verificar Axios está cargado
testSuite.addTest('Axios loaded', () => {
    if (typeof axios === 'undefined') throw new Error('Axios no está cargado');
});

// Test 3: Testear API de clientes
testSuite.addTest('API Clientes', async () => {
    const response = await axios.get('/api/search/clientes?termino=test');
    if (!Array.isArray(response.data)) throw new Error('API no retorna array');
});

// Test 4: Testear API de productos
testSuite.addTest('API Productos', async () => {
    const response = await axios.get('/api/search/productos?termino=test');
    if (!Array.isArray(response.data)) throw new Error('API no retorna array');
});

// Ejecutar tests automáticamente en páginas con Vue
if (typeof Vue !== 'undefined') {
    document.addEventListener('DOMContentLoaded', () => {
        // Esperar 2 segundos para que Vue se inicialice
        setTimeout(() => {
            testSuite.runAll();
        }, 2000);
    });
}