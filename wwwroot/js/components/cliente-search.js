// wwwroot/js/components/cliente-search.js
const ClienteSearch = {
    name: 'ClienteSearch',
    template: `
        <div class="cliente-search">
            <div class="input-group">
                <input 
                    v-model="searchTerm" 
                    @input="buscar"
                    class="form-control" 
                    placeholder="Buscar por DNI, nombre o apellido..."
                    ref="searchInput"
                >
                <button class="btn btn-primary" @click="abrirModal">
                    <i class="fas fa-search"></i>
                </button>
            </div>
            
            <!-- Resultados dropdown -->
            <div v-if="mostrarResultados && clientes.length > 0" class="search-results">
                <div 
                    v-for="cliente in clientes" 
                    :key="cliente.id"
                    @click="seleccionar(cliente)"
                    class="search-result-item"
                >
                    <div class="d-flex justify-content-between">
                        <div>
                            <strong>{{ cliente.nombreCompleto }}</strong>
                            <br>
                            <small class="text-muted">DNI: {{ cliente.dni }}</small>
                        </div>
                        <div class="text-end">
                            <span :class="getScoringClass(cliente.scoring)">
                                Scoring: {{ cliente.scoring || 'N/A' }}
                            </span>
                        </div>
                    </div>
                </div>
            </div>
        </div>
    `,
    props: ['modelValue'],
    emits: ['update:modelValue', 'clienteSeleccionado'],
    data() {
        return {
            searchTerm: '',
            clientes: [],
            mostrarResultados: false,
            debounceTimer: null
        }
    },
    methods: {
        async buscar() {
            clearTimeout(this.debounceTimer);
            this.debounceTimer = setTimeout(async () => {
                if (this.searchTerm.length < 2) {
                    this.clientes = [];
                    return;
                }

                try {
                    const response = await axios.get('/api/search/clientes', {
                        params: { termino: this.searchTerm }
                    });
                    this.clientes = response.data;
                    this.mostrarResultados = true;
                } catch (error) {
                    console.error('Error buscando clientes:', error);
                }
            }, 300);
        },
        seleccionar(cliente) {
            this.$emit('clienteSeleccionado', cliente);
            this.$emit('update:modelValue', cliente.id);
            this.searchTerm = cliente.nombreCompleto;
            this.mostrarResultados = false;
        },
        getScoringClass(scoring) {
            if (!scoring) return 'text-muted';
            return scoring >= 8 ? 'text-success' : scoring >= 5 ? 'text-warning' : 'text-danger';
        },
        abrirModal() {
            // Implementar modal de búsqueda avanzada
            console.log('Modal de búsqueda avanzada no implementado');
        }
    },
    mounted() {
        // Cerrar resultados al hacer clic fuera
        document.addEventListener('click', (e) => {
            if (!this.$el.contains(e.target)) {
                this.mostrarResultados = false;
            }
        });
    },
    beforeUnmount() {
        clearTimeout(this.debounceTimer);
    }
};

// Exportar para uso global
window.ClienteSearch = ClienteSearch;