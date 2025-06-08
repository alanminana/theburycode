console.log('categoria-index.js cargado');

function onTipoChange() {
    const tipo = $('#categoriaTipo').val();
    console.log('Tipo cambiado a:', tipo);
    $('#divParent').toggle(tipo === 'S');
}

function guardarCategoria() {
    const tipo = $('#categoriaTipo').val();
    const nombre = $('#categoriaNombre').val();
    const parentId = tipo === 'S' ? $('#categoriaParent').val() : null;

    console.log('Guardando categoría:', { tipo, nombre, parentId });

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
            console.log('Respuesta crear categoría:', response);
            if (response.success) {
                $('#modalCategoria').modal('hide');
                Swal.fire('Éxito', 'Categoría creada', 'success')
                    .then(() => location.reload());
            } else {
                Swal.fire('Error', response.error, 'error');
            }
        },
        error: function (xhr, status, error) {
            console.error('Error AJAX:', error);
            Swal.fire('Error', 'Error al crear categoría', 'error');
        }
    });
}

function eliminarCategoria(id) {
    console.log('Eliminar categoría ID:', id);

    Swal.fire({
        title: '¿Eliminar categoría?',
        text: 'Esta acción no se puede deshacer',
        icon: 'warning',
        showCancelButton: true,
        confirmButtonText: 'Sí, eliminar'
    }).then((result) => {
        if (result.isConfirmed) {
            $.post('/Categoria/Delete', { id: id })
                .done(response => {
                    console.log('Respuesta eliminar:', response);
                    if (response.success) {
                        Swal.fire('Eliminado', 'Categoría eliminada', 'success')
                            .then(() => location.reload());
                    } else {
                        Swal.fire('Error', response.error, 'error');
                    }
                })
                .fail(function (xhr, status, error) {
                    console.error('Error al eliminar:', error);
                    Swal.fire('Error', 'Error al eliminar', 'error');
                });
        }
    });
}

// Funciones para marcas
function onTipoMarcaChange() {
    const tipo = $('#marcaTipo').val();
    console.log('Tipo marca cambiado a:', tipo);
    $('#divMarcaParent').toggle(tipo === 'S');
}

function guardarMarca() {
    const tipo = $('#marcaTipo').val();
    const nombre = $('#marcaNombre').val();
    const parentId = tipo === 'S' ? $('#marcaParent').val() : null;

    console.log('Guardando marca:', { tipo, nombre, parentId });

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
            console.log('Respuesta crear marca:', response);
            if (response.success) {
                $('#modalMarca').modal('hide');
                Swal.fire('Éxito', 'Marca creada', 'success')
                    .then(() => location.reload());
            } else {
                Swal.fire('Error', response.error, 'error');
            }
        },
        error: function (xhr, status, error) {
            console.error('Error AJAX marca:', error);
            Swal.fire('Error', 'Error al crear marca', 'error');
        }
    });
}

function eliminarMarca(id) {
    console.log('Eliminar marca ID:', id);

    Swal.fire({
        title: '¿Eliminar marca?',
        text: 'Esta acción no se puede deshacer',
        icon: 'warning',
        showCancelButton: true,
        confirmButtonText: 'Sí, eliminar'
    }).then((result) => {
        if (result.isConfirmed) {
            $.post('/Categoria/DeleteMarca', { id: id })
                .done(response => {
                    console.log('Respuesta eliminar marca:', response);
                    if (response.success) {
                        Swal.fire('Eliminado', 'Marca eliminada', 'success')
                            .then(() => location.reload());
                    } else {
                        Swal.fire('Error', response.error, 'error');
                    }
                })
                .fail(function (xhr, status, error) {
                    console.error('Error al eliminar marca:', error);
                    Swal.fire('Error', 'Error al eliminar', 'error');
                });
        }
    });
}

// Inicialización
$(document).ready(function () {
    console.log('Documento listo - categoria-index.js');
});