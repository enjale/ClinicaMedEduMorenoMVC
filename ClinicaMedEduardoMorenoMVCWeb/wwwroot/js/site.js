// Please see documentation at https://learn.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// Write your JavaScript code.

// ==========================================================================
// Confirmación con el modal del proyecto (#modalConfirmar en _LayoutDoctor)
// Uso: agregar data-confirmar="¿Mensaje?" a un <form> o a su botón de envío.
// Opcionales: data-confirmar-titulo="Título" y data-confirmar-boton="Texto del botón".
// ==========================================================================
(function () {
    document.addEventListener('submit', function (e) {
        const form = e.target;
        const origen = [e.submitter, form].find(function (el) { return el && el.dataset && el.dataset.confirmar; });

        if (!origen || form.dataset.confirmado === 'true') return;

        // Se detiene el envío hasta que el usuario confirme
        e.preventDefault();
        e.stopImmediatePropagation();

        const modal = document.getElementById('modalConfirmar');
        if (!modal || !window.bootstrap) {
            // Sin el modal disponible se usa la confirmación del navegador
            if (window.confirm(origen.dataset.confirmar)) enviar(form, e.submitter);
            return;
        }

        document.getElementById('modalConfirmarTitulo').textContent = origen.dataset.confirmarTitulo || 'Confirmar eliminación';
        document.getElementById('modalConfirmarMensaje').textContent = origen.dataset.confirmar;
        const aceptar = document.getElementById('modalConfirmarAceptar');
        aceptar.textContent = origen.dataset.confirmarBoton || 'Eliminar';

        const instancia = bootstrap.Modal.getOrCreateInstance(modal);
        aceptar.onclick = function () {
            instancia.hide();
            enviar(form, e.submitter);
        };
        instancia.show();
    }, true);

    function enviar(form, boton) {
        form.dataset.confirmado = 'true';
        if (form.requestSubmit) {
            form.requestSubmit(boton || undefined);
        } else {
            form.submit();
        }
    }
})();
