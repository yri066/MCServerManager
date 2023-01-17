/**Url текущей страницы */
let pathPage = new URL(window.location.origin + window.location.pathname);

/**Текущее состояние сервера. */
let status = "";

/**Список пользователей на сервере */
let usersInfo = {
	version: "",
	userList: [],
	count: 0
};

/**Получение состояния сервиса */
let timer = setInterval(() => takeAction(`${pathPage}/GetStatus`), 1500);

/**
 * Загрузка Json по ссылке.
 * @param {string} url Ссылка на Json.
 * @returns Promise с загруженными данными
 */
function loadJson(url) {
	if (typeof url !== "string") {
		throw `Переменная должна быть типом String (${typeof str})`;
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
		.catch(error => $("#StatusService").html(`Состояние сервиса: ${error}`));
}

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
			$("#StartServer").hide();
			$("#CloseServer").show();
			break;
		default:
			$("#StartServer").hide();
			$("#CloseServer").hide();
	}
}