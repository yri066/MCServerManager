/**Url текущей страницы. */
let pathPage = new URL(window.location.origin + window.location.pathname);
/**Url получения состояния приложения. */
let getStatusUrl = new URL('GetStatus', pathPage);
/**Версия вывода консоли. */
let consoleVersion = "00000000-0000-0000-0000-000000000000";

/**Получение состояния. */
let timer = setInterval(() => {
    queryHandling(getStatusUrl, data => {
        checkConsole(data);
    });
}, 1500);

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
        .catch(error => { });
}

/**
 * Загрузка Json по ссылке.
 * @param {*} url Ссылка на Json.
 * @returns Promise с загруженными данными
 */
function loadJson(url) {

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
 * Выводит сообщения в консоль.
 * @param {*} element Объект с информацией о состоянии приложения.
 */
function checkConsole(element) {

    if (consoleVersion == element.consoleVersion) {
        return;
    }

    if (consoleVersion == "00000000-0000-0000-0000-000000000000") {
        $("#consoleView").empty();
    }

    //Отправляет Get запрос и выводит полученные сообщения в консоль.
    queryHandling(`${pathPage}/${consoleVersion}`, function (data) {

        consoleVersion = data.version;
        let list = "";
        for (let x = data.console.length - 1; x >= 0; x--) {
            if (data.console[x] == null) {
                data.console[x] = "&nbsp;";
            }
            list += `<div class="console">${data.console[x]}</div>`;
        }

        $("#consoleView").prepend(list);

        while ($("#consoleView p").length > 1000) {
            $("#consoleView").find("p:last").remove();
        }
    });
}

/**
 * Действия, выполняемые после загрузки страницы.
 */
window.onload = function () {

    var input = document.getElementById("inputConsole");

    input.addEventListener("keypress", function (event) {
        if (event.key === "Enter") {
            postCommand(input.value);
            input.value = "";
        }
    });

    $("#button-addon2").click(function () {
        postCommand(input.value);
        input.value = "";
    });
}

/**
 * Отправляет команды в Api приложения.
 * @param {*} command Текст команды.
 */
function postCommand(command) {
    $.ajax({
        url: pathPage,
        type: 'POST',
        cache: false,
        data: { message: command },
        beforeSend: function () {
        },
        success: function (data) {
        }
    });
}