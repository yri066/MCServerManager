var state = "";

/**Таймер */
let timer = setInterval(() => getState(`${document.URL}/GetStatus`), 1500);

function getState(url) {
    getElement(url, function (element) {
	    changeState(element);
    });
}

function getElement(url, c) {
    request(new XMLHttpRequest());

    function request(xhr) {
        xhr.open('GET', url, true);
        
        xhr.onreadystatechange = function () {
            if (xhr.readyState == 4) {
                c(xhr.responseText);
            }
        }
		
		xhr.send();
		
		xhr.onerror = function (e) {
			$("#StatusServer").html(`Состояние сервера: Ошибка соединения`);
		};
    }
}

$("#StartServer").click(function() {
	getState(`${document.URL}/Start`);
});

$("#RebootServer").click(function() {
	getState(`${document.URL}/Restart`);
});

$("#StopServer").click(function() {
	getState(`${document.URL}/Stop`);
});

$("#CloseServer").click(function() {
	getState(`${document.URL}/Close`);
});

function changeState(element) {
	if (state == element) {
		return;
	}
	
	state = element;
	$("#StatusServer").html(`Состояние сервера: ${state}`);
	
	switch (state) {
		case "Off":
			$( "#StartServer" ).show();
			$( "#RebootServer" ).hide();
			$( "#StopServer" ).hide();
			$( "#CloseServer" ).hide();
			break;
		case "Launch":
			$( "#StartServer" ).hide();
			$( "#RebootServer" ).hide();
			$( "#StopServer" ).hide();
			$( "#CloseServer" ).show();
			break;
		case "Run":
			$( "#StartServer" ).hide();
			$( "#RebootServer" ).show();
			$( "#StopServer" ).show();
			$( "#CloseServer" ).show();
			break;
		case "Shutdown":
			$( "#StartServer" ).hide();
			$( "#RebootServer" ).hide();
			$( "#StopServer" ).hide();
			$( "#CloseServer" ).show();
			break;
		case "Reboot":
			$( "#StartServer" ).hide();
			$( "#RebootServer" ).hide();
			$( "#StopServer" ).hide();
			$( "#CloseServer" ).show();
			break;
		case "Error":
			$( "#StartServer" ).show();
			$( "#RebootServer" ).hide();
			$( "#StopServer" ).hide();
			$( "#CloseServer" ).hide();
			break;
		default :
			$( "#StartServer" ).hide();
			$( "#RebootServer" ).hide();
			$( "#StopServer" ).hide();
			$( "#CloseServer" ).hide();
	}
}