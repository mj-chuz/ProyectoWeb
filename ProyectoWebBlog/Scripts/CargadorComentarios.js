function cargarComentarios(linkModalComentarios) {
    $("#modal-comentarios").empty();
    let idBuscador = '#' + linkModalComentarios;
    if ($(idBuscador).attr('value')) {
            $.ajax({

                type: 'GET',
                url: $("#comentarios-controller").data("request-url"),
                dataType: 'json',

                
                data: { identificadorPublicacion: $(idBuscador).attr('value') },

                success: function (comentarios) {
                    $.each(comentarios, function (i, comentario) {
                        $("#modal-comentarios").append(
                            '<div class="card cards-comentarios"> <div class="card-body"> <h5 class="card-text"></h5> <p class="card-text">' +
                            comentario.Texto + '</p > </div> <div class="card-footer"> <p class="card-text">Autor: ' +
                            comentario.Correo +' </div> </div>'
                        );
                    });

                },

                error: function (ex) {
                    alert('Fallo en la recuperación de comentarios' + ex);
                }

            });
            return false;
        } else {
            console.log("s");
        }

}