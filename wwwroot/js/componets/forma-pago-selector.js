// wwwroot/js/components/forma-pago-selector.js
const FormaPagoSelector = {
    template: `
        <div class="forma-pago-selector">
            <div class="row">
                <div class="col-md-4">
                    <label class="form-label">Forma de Pago</label>
                    <select v-model="formaPagoId" @change="onFormaPagoChange" class="form-select">
                        <option value="">Seleccione...</option>
                        <option v-for="fp in formasPago" :key="fp.id" :value="fp.id">
                            {{ fp.descripcion }}
                        </option>
                    </select>
                </div>
                
                <div class="col-md-4" v-if="requiereBanco">
                    <label class="form-label">Banco</label>
                    <select v-model="bancoId" class="form-select">
                        <option value="">Seleccione...</option>
                        <option v-for="banco in bancos" :key="banco.id" :value="banco.id">
                            {{ banco.descripcion }}
                        </option>
                    </select>
                </div>
                
                <div class="col-md-4" v-if="requiereTarjeta">
                    <label class="form-label">Tipo Tarjeta</label>
                    <select v-model="tipoTarjetaId" class="form-select">
                        <option value="">Seleccione...</option>
                        <option v-for="tarjeta in tiposTarjeta" :key="tarjeta.id" :value="tarjeta.id">
                            {{ tarjeta.descripcion }}
                        </option>
                    </select>
                </div>
                
                <div class="col-md-4" v-if="permiteCuotas">
                    <label class="form-label">Cuotas</label>
                    <input 
                        type="number" 
                        v-model.number="cuotas" 
                        :max="maxCuotas"
                        min="1"
                        class="form-control"
                    >
                </div>
            </div>
        </div>
    `,
    props: ['modelValue'],
    emits: ['update:modelValue'],
    data() {
        return {
            formaPagoId: null,
            bancoId: null,
            tipoTarjetaId: null,
            cuotas: 1,
            formasPago: [],
            bancos: [],
            tiposTarjeta: [],
            requiereBanco: false,
            requiereTarjeta: false,
            permiteCuotas: false,
            maxCuotas: 1
        }
    },
    async mounted() {
        await this.cargarDatos();
    },
    methods: {
        async cargarDatos() {
            try {
                const [fp, bancos, tarjetas] = await Promise.all([
                    axios.get('/api/formaspago'),
                    axios.get('/api/bancos'),
                    axios.get('/api/tipostarjeta')
                ]);

                this.formasPago = fp.data;
                this.bancos = bancos.data;
                this.tiposTarjeta = tarjetas.data;
            } catch (error) {
                console.error('Error cargando datos:', error);
            }
        },
        async onFormaPagoChange() {
            if (!this.formaPagoId) return;

            try {
                const response = await axios.get(`/api/formaspago/${this.formaPagoId}/datos`);
                const datos = response.data;

                this.requiereBanco = datos.requiereBanco;
                this.requiereTarjeta = datos.requiereTarjeta;
                this.permiteCuotas = datos.permiteCuotas;
                this.maxCuotas = datos.maximoCuotas || 1;

                // Limpiar campos si no son requeridos
                if (!this.requiereBanco) this.bancoId = null;
                if (!this.requiereTarjeta) this.tipoTarjetaId = null;
                if (!this.permiteCuotas) this.cuotas = 1;

                this.emitirCambios();
            } catch (error) {
                console.error('Error obteniendo datos forma pago:', error);
            }
        },
        emitirCambios() {
            this.$emit('update:modelValue', {
                formaPagoId: this.formaPagoId,
                bancoId: this.bancoId,
                tipoTarjetaId: this.tipoTarjetaId,
                cuotas: this.cuotas
            });
        }
    },
    watch: {
        bancoId() { this.emitirCambios(); },
        tipoTarjetaId() { this.emitirCambios(); },
        cuotas() { this.emitirCambios(); }
    }
};