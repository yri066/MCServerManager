/**
 * Действие на нажатие кнопки Старт.
 */
$("#StartServer").click(function () {
	takeAction(`${pathPage}/Start`);
});

/**
 * Действие на нажатие кнопки Выключить.
 */
$("#CloseServer").click(function () {
	takeAction(`${pathPage}/Close`);
});

/**
 * Изменяет видимость кнопок действий.
 * @param {any} element Информация о сервере.
 */
function changeState(element) {
	if (status == element.status) {
		return;
	}

	status = element.status;
	$("#StatusService").html(`Состояние сервиса: ${status}`);

	switch (status) {
		case "Off":
			$("#StartServer").show();
			$("#CloseServer").hide();
			break;
		case "Run":
		case "Launch":
			$("#StartServer").hide();
			$("#CloseServer").show();
			break;
		default:
			$("#StartServer").hide();
			$("#CloseServer").hide();
	}
}