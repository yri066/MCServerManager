/**
 * Действие на нажатие кнопки Старт.
 */
$("#StartServer").click(function () {
	takeAction(`${pathPage}/Start`);
});

/**
 * Действие на нажатие кнопки Перезагрузка.
 */
$("#RebootServer").click(function () {
	takeAction(`${pathPage}/Restart`);
});

/**
 * Действие на нажатие кнопки Остановить.
 */
$("#StopServer").click(function () {
	takeAction(`${pathPage}/Stop`);
});

/**
 * Действие на нажатие кнопки Выключить.
 */
$("#CloseServer").click(function () {
	takeAction(`${pathPage}/Close`);
});

/**
 * Выводит список игроков на страницу.
 */
function checkUserList(element) {
	if (usersInfo.version == element.userListVersion) {
		return;
	}

	queryHandling(`${pathPage}/GetUserList`, function (element) {
		usersInfo = element;

		$("#user-count").html(`Количество игроков: ${usersInfo.count}`);

		let list = "";
		for (let x = 0; x < usersInfo.count; x++) {
			list += `
			<div class="card">
				<div class="row align-items-center">
					<div class="col">
						<div class="card-body">
							${usersInfo.userList[x]}
						</div>
					</div>
					<div class="col-auto text-end">
						<div class="card-body">
							<button type="button" class="btn btn-danger" onclick="kickUser('${usersInfo.userList[x]}')">Исключить</button>
						</div>
					</div>
				</div>
			</div>`;
		}

		$("#user-list").html(list);
	});
}

/**Отправляет сообщение о исключении пользователя с сервера. */
function kickUser(user) {
	let pathPage = new URL(window.location.origin + window.location.pathname + "/Console");
	$.post(pathPage, { message: `kick ${user}` });
}

/**
 * Изменяет видимость кнопок действий.
 * @param {any} element Информация о сервере.
 */
function changeState(element) {
	checkUserList(element);

	if (status == element.status) {
		return;
	}

	status = element.status;
	$("#StatusServer").html(`Состояние сервера: ${status}`);

	switch (status) {
		case "Off":
		case "Error":
			$("#StartServer").show();
			$("#RebootServer").hide();
			$("#StopServer").hide();
			$("#CloseServer").hide();
			break;
		case "Launch":
		case "Shutdown":
		case "Reboot":
			$("#StartServer").hide();
			$("#RebootServer").hide();
			$("#StopServer").hide();
			$("#CloseServer").show();
			break;
		case "Run":
			$("#StartServer").hide();
			$("#RebootServer").show();
			$("#StopServer").show();
			$("#CloseServer").show();
			break;
		default:
			$("#StartServer").hide();
			$("#RebootServer").hide();
			$("#StopServer").hide();
			$("#CloseServer").hide();
	}
}