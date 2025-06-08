// wwwroot/js/components/detalle-grid.js
const DetalleGrid = {
    name: 'DetalleGrid',
    template: `
    <div class="detalle-grid">
      <table class="table table-striped">
        <thead>
          <tr>
            <th style="width: 40%">Producto</th>
            <th style="width: 15%">Cantidad</th>
            <th style="width: 15%">Precio Unit.</th>
            <th style="width: 15%">Subtotal</th>
            <th style="width: 15%">Acciones</th>
          </tr>
        </thead>
        <tbody>
          <tr v-for="(item, index) in items" :key="item.productoId">
            <td>
              <strong>{{ item.codigo }}</strong> - {{ item.nombre }}
              <br>
              <small class="text-muted">Stock: {{ item.stock }}</small>
            </td>
            <td>
              <input
                type="number"
                v-model.number="item.cantidad"
                @change="calcularSubtotal(index)"
                :max="validarStock ? item.stock : null"
                min="1"
                class="form-control form-control-sm"
              >
            </td>
            <td>
              <input
                type="number"
                v-model.number="item.precioUnit"
                @change="calcularSubtotal(index)"
                :readonly="precioFijo"
                step="0.01"
                class="form-control form-control-sm"
              >
            </td>
            <td>
              {{ '$' + formatPrice(item.subtotal) }}
            </td>
            <td>
              <button
                @click="eliminarItem(index)"
                class="btn btn-sm btn-danger"
              >
                <i class="fas fa-trash"></i>
              </button>
            </td>
          </tr>
          <tr v-if="items.length === 0">
            <td colspan="5" class="text-center text-muted">
              No hay productos agregados
            </td>
          </tr>
        </tbody>
        <tfoot>
          <tr>
            <td colspan="3" class="text-end"><strong>Total:</strong></td>
            <td><strong>{{ '$' + formatPrice(total) }}</strong></td>
            <td></td>
          </tr>
        </tfoot>
      </table>

      <div class="mt-3">
        <producto-search @producto-seleccionado="agregarProducto"></producto-search>
      </div>
    </div>
  `,

    props: {
        modelValue: {
            type: Array,
            default: () => []
        },
        validarStock: {
            type: Boolean,
            default: true
        },
        precioFijo: {
            type: Boolean,
            default: false
        }
    },

    emits: ['update:modelValue', 'totalChanged'],

    data() {
        return {
            items: []
        };
    },

    computed: {
        total() {
            return this.items.reduce((sum, item) => sum + (item.subtotal || 0), 0);
        }
    },

    watch: {
        modelValue: {
            immediate: true,
            handler(val) {
                this.items = [...val];
            }
        }
    },

    methods: {
        agregarProducto(producto) {
            const existente = this.items.find(i => i.productoId === producto.id);
            if (existente) {
                if (this.validarStock && existente.cantidad >= existente.stock) {
                    Swal.fire('Error', 'Stock insuficiente', 'warning');
                    return;
                }
                existente.cantidad++;
                this.calcularSubtotal(this.items.indexOf(existente));
                return;
            }

            this.items.push({
                productoId: producto.id,
                codigo: producto.codigo,
                nombre: producto.nombre,
                stock: producto.stock,
                cantidad: 1,
                precioUnit: producto.precioVenta,
                subtotal: producto.precioVenta
            });

            this.emitirCambios();
        },

        calcularSubtotal(index) {
            const item = this.items[index];
            if (item.cantidad < 1) item.cantidad = 1;
            if (this.validarStock && item.cantidad > item.stock) {
                item.cantidad = item.stock;
                Swal.fire('Atención', 'Cantidad ajustada al stock disponible', 'info');
            }
            item.subtotal = item.cantidad * item.precioUnit;
            this.emitirCambios();
        },

        eliminarItem(index) {
            this.items.splice(index, 1);
            this.emitirCambios();
        },

        emitirCambios() {
            this.$emit('update:modelValue', this.items);
            this.$emit('totalChanged', this.total);
        },

        formatPrice(value) {
            // Usar función global de site.js
            return window.formatPrice(value);
        }
    },

    components: {
        'producto-search': ProductoSearch
    }
};

// Exportar para uso global
window.DetalleGrid = DetalleGrid;