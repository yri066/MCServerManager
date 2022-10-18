/**Текущее состояние сервера. */
let status = "";

/**Список пользователей на сервере */
let usersInfo = {
	version: "",
	userList: [],
	count: 0
};

/**Таймер */
let timer = setInterval(() => takeAction(`${pathPage}/GetStatus`), 1500);

/**
 * Загрузка Json по ссылке.
 * @param {string} url Ссылка на Json.
 * @returns Promise с загруженными данными
 */
function loadJson(url) {
	if (typeof url !== "string") {
		throw `Перменная должена быть типом String (${typeof str})`;
	}

	if (url == "" || url == null) {
		throw `Строка не должна быть пустой`;
	}

	return new Promise(function (resolve, reject) {
		let xmlhttprequest = new XMLHttpRequest();
		xmlhttprequest.open('GET', url, true);
		xmlhttprequest.responseType = 'json';

		xmlhttprequest.onload = function () {
			let status = xmlhttprequest.status;

			if (status == 200) {
				resolve(xmlhttprequest.response);
			} else {
				reject(xmlhttprequest.response.errorText);
			}
		};

		xmlhttprequest.onerror = () => reject(`Не удалось загрузить данные.`);
		xmlhttprequest.send();
	});
};

/**
 * Выполняет запрос к Api.
 * @param {string} url Ссылка на Api.
 */
function takeAction(url) {
	queryHandling(url, data => {
		changeState(data);
	});
}

/**
 * Выполняет обработку запроса
 * @param {any} url Ссылка на Api.
 * @param {any} resolve Ответ от Api.
 */
function queryHandling(url, resolve) {
	loadJson(url)
		.then(data => {
			resolve(data);
		})
		.catch(error => $("#StatusServer").html(`Состояние сервера: ${error}`));
}

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
			list += `<div class="card">
			<div class="card-body">
				${usersInfo.userList[x]}
			</div>
		</div>`;
		}

		$("#user-list").html(list);
	});
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
			$("#StartServer").show();
			$("#RebootServer").hide();
			$("#StopServer").hide();
			$("#CloseServer").hide();
			break;
		case "Launch":
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
		case "Shutdown":
			$("#StartServer").hide();
			$("#RebootServer").hide();
			$("#StopServer").hide();
			$("#CloseServer").show();
			break;
		case "Reboot":
			$("#StartServer").hide();
			$("#RebootServer").hide();
			$("#StopServer").hide();
			$("#CloseServer").show();
			break;
		case "Error":
			$("#StartServer").show();
			$("#RebootServer").hide();
			$("#StopServer").hide();
			$("#CloseServer").hide();
			break;
		default:
			$("#StartServer").hide();
			$("#RebootServer").hide();
			$("#StopServer").hide();
			$("#CloseServer").hide();
	}
}