$("#updateContrasenia").click(function () {
	if ($("#formUpdateContrasenia").valid()) {
		if ($("#ContraseniaActual").val() === $("#ContraseniaNueva").val()) {
			toastr.error("La contraseña nueva no puede ser igual a la contraseña actual", "Error");
		} else if ($("#ContraseniaNueva").val() !== $("#ContraseniaConfirmar").val()) {
			toastr.error("La verificación de la nueva contraseña no coincide", "Error");
		} else {
			$.ajax({
				cache: false,
				type: 'POST',
				url: siteLocation + "Autenticacion/CambiarContrasenia",
				data: $("#formUpdateContrasenia").serialize(),
				success: function (result) {
					if (result.isSuccessFully) {
						location.replace(location.origin + "/Home/Index?updateContrasenia=true");
					} else {
						toastr.error("Servicio no disponible", "Error");
					}
				},
				error: function (res) {
					toastr.error("Servicio no disponible", "Error");
				}
			});
		}
	}
});

window.addEventListener('DOMContentLoaded', (event) => {
	if (document.body.contains(document.getElementById("updateContraseniaCheckBox"))) {
		if (document.getElementById("updateContraseniaCheckBox").checked) {
			toastr.success("La contraseña se actualizó correctamente", "");
		}
	}
});

