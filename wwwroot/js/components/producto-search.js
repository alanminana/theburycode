// wwwroot/js/components/producto-search.js
const ProductoSearch = {
    name: 'ProductoSearch',
    template: `
        <div class="producto-search">
            <div class="input-group">
                <input 
                    v-model="searchTerm" 
                    @input="buscar"
                    class="form-control" 
                    placeholder="Buscar por código o nombre..."
                    ref="searchInput"
                >
                <button class="btn btn-primary" @click="abrirModal">
                    <i class="fas fa-search"></i>
                </button>
            </div>
            
            <!-- Resultados dropdown -->
            <div v-if="mostrarResultados && productos.length > 0" class="search-results">
                <div 
                    v-for="producto in productos" 
                    :key="producto.id"
                    @click="seleccionar(producto)"
                    class="search-result-item"
                >
                    <div class="d-flex justify-content-between">
                        <div>
                            <strong>{{ producto.codigo }}</strong> - {{ producto.nombre }}
                            <br>
                            <small class="text-muted">{{ producto.categoria }} | {{ producto.marca }}</small>
                        </div>
                        <div class="text-end">
                            <strong>\${{ formatPrice(producto.precioVenta) }}</strong>
                            <br>
                            <small :class="producto.stock > 0 ? 'text-success' : 'text-danger'">
                                Stock: {{ producto.stock }}
                            </small>
                        </div>
                    </div>
                </div>
            </div>
        </div>
    `,
    props: ['modelValue'],
    emits: ['update:modelValue', 'productoSeleccionado'],
    data() {
        return {
            searchTerm: '',
            productos: [],
            mostrarResultados: false,
            debounceTimer: null
        };
    },
    methods: {
        async buscar() {
            clearTimeout(this.debounceTimer);
            this.debounceTimer = setTimeout(async () => {
                if (this.searchTerm.length < 2) {
                    this.productos = [];
                    return;
                }

                try {
                    const response = await axios.get('/api/search/productos', {
                        params: { termino: this.searchTerm }
                    });
                    this.productos = response.data;
                    this.mostrarResultados = true;
                } catch (error) {
                    console.error('Error buscando productos:', error);
                }
            }, 300);
        },
        seleccionar(producto) {
            this.$emit('productoSeleccionado', producto);
            this.searchTerm = producto.codigo;
            this.mostrarResultados = false;
        },
        abrirModal() {
            console.log('Modal de búsqueda avanzada no implementado');
        },
        formatPrice(price) {
            // Usar función global de site.js
            return window.formatPrice(price);
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
window.ProductoSearch = ProductoSearch;