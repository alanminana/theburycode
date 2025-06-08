// Funciones para Categorías
function onTipoChange() {
    const tipo = $('#categoriaTipo').val();
    $('#divParent').toggle(tipo === 'S');
    if (tipo === 'S') {
        Utils.cargarRubros('#categoriaParent');
    }
}

function guardarCategoria() {
    const tipo = $('#categoriaTipo').val();
    const nombre = $('#categoriaNombre').val();
    const parentId = tipo === 'S' ? $('#categoriaParent').val() : null;

    if (!nombre) {
        Swal.fire('Error', 'El nombre es requerido', 'error');
        return;
    }

    if (tipo === 'S' && !parentId) {
        Swal.fire('Error', 'Debe seleccionar un rubro padre', 'error');
        return;
    }

    $.ajax({
        url: '/Categoria/CreateAjax',
        type: 'POST',
        contentType: 'application/json',
        data: JSON.stringify({
            nombre: nombre,
            tipo: tipo,
            parentId: parentId
        }),
        success: function (response) {
            if (response.success) {
                $('#modalCategoria').modal('hide');
                Swal.fire('Éxito', 'Categoría creada', 'success');
                // Recargar las listas según el tipo
                if (tipo === 'R') {
                    Utils.cargarRubros('#rubroSelect');
                } else {
                    const rubroId = $('#rubroSelect').val();
                    if (rubroId) {
                        Utils.cargarSubrubros(rubroId, '#CategoriaId');
                    }
                }
            } else {
                Swal.fire('Error', response.error, 'error');
            }
        },
        error: function (xhr, status, error) {
            Swal.fire('Error', 'Error al crear categoría', 'error');
        }
    });
}

// Funciones para Marcas
function onTipoMarcaChange() {
    const tipo = $('#marcaTipo').val();
    $('#divMarcaParent').toggle(tipo === 'S');
    if (tipo === 'S') {
        Utils.cargarMarcas('#marcaParent');
    }
}

function guardarMarca() {
    const tipo = $('#marcaTipo').val();
    const nombre = $('#marcaNombre').val();
    const parentId = tipo === 'S' ? $('#marcaParent').val() : null;

    if (!nombre) {
        Swal.fire('Error', 'El nombre es requerido', 'error');
        return;
    }

    if (tipo === 'S' && !parentId) {
        Swal.fire('Error', 'Debe seleccionar una marca padre', 'error');
        return;
    }

    $.ajax({
        url: '/Categoria/CreateMarcaAjax',
        type: 'POST',
        contentType: 'application/json',
        data: JSON.stringify({
            nombre: nombre,
            tipo: tipo,
            parentId: parentId
        }),
        success: function (response) {
            if (response.success) {
                $('#modalMarca').modal('hide');
                Swal.fire('Éxito', 'Marca creada', 'success');
                // Recargar las listas según el tipo
                if (tipo === 'M') {
                    Utils.cargarMarcas('#MarcaId');
                } else {
                    const marcaId = $('#MarcaId').val();
                    if (marcaId) {
                        Utils.cargarSubmarcas(marcaId, '#SubmarcaId');
                    }
                }
            } else {
                Swal.fire('Error', response.error, 'error');
            }
        },
        error: function (xhr, status, error) {
            Swal.fire('Error', 'Error al crear marca', 'error');
        }
    });
}

// Inicialización
$(document).ready(function () {
    console.log('Documento listo - categoria-index.js');
});