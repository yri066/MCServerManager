﻿/**Url текущей страницы */
let pathPage = window.location.href;

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
                if (xmlhttprequest.response == null) { //Перенаправление
                    location.assign(xmlhttprequest.responseURL.split('%2F').slice(0, -1).join('%2F'));
                }

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
        .catch(error => {
            if (status == error) {
                return;
            }

            status = error;
            showErrorToast(error);
        });
}

function showErrorToast(errorText) {
    $(".toast-container-extra").html(`
			<div class="toast align-items-center text-white bg-danger w-100 p-3 show" role="alert" aria-live="assertive" aria-atomic="true">
				<div class="d-flex">
					<div class="toast-body text-break">
						${errorText}
					</div>
				<button type="button" class="btn-close btn-close-white me-2 m-auto" data-bs-dismiss="toast" aria-label="Close"></button>
				</div>
			</div>`);
}