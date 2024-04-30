const editButton = document.querySelector(".edit-button");
const saveButton = document.querySelector(".save-button");
const cancelButton = document.querySelector(".cancel-button");

const addService = document.querySelector(".add-service");
const editServiceGroup = document.querySelector(".edit-service-group");
let blockContentItems = document.querySelectorAll(".service-item-content-block");
let serviceList;
let serviceItems;
let originalListContent;
let changedServiceRatings = new Map();

editButton.addEventListener("click", initiateServiceEdit);
saveButton.addEventListener("click", updateServiceRating);
cancelButton.addEventListener("click", cancelServiceEdit);


function initiateServiceEdit() {
    addService.style.display = 'none';
    editServiceGroup.style.display = 'inline';

    originalListContent = document.getElementsByClassName("service-list")[0].innerHTML;
    serviceList = document.querySelector(".service-list");
    serviceItems = document.querySelectorAll(".service-item");

    document.querySelectorAll(".service-item-content-block").forEach(item => { item.style.display = 'none'; });

    if ('ontouchstart' in document.documentElement) {
        enableMobileEdit();
    }
    else {
        enableDesktopEdit();
    }
}

function updateServiceRating() {
    serviceList.removeEventListener("click", handleMoveItem);
    addService.style.display = 'inline';
    editServiceGroup.style.display = 'none';

    document.querySelectorAll(".service-item-content-block").forEach(item => { item.style.display = 'inline'; });
    document.querySelectorAll(".move-item").forEach(item => { item.remove(); });

    console.log(changedServiceRatings);

    let updatedRatingsData = {};
    changedServiceRatings.forEach((value, key) => {
        updatedRatingsData[key] = value;
    });

    let jsonData = JSON.stringify(updatedRatingsData);

    $.ajax({
        url: `${window.location.href}/UpdateRateServices`,
        type: 'POST',
        cache: false,
        data: { content: jsonData },
        beforeSend: function () {
        },
        success: function (data) {
        },
        error: function (xhr, textStatus, errorThrown) {
            cancelServiceEdit();
            showErrorToast(textStatus);
        }
    });

}

function cancelServiceEdit() {
    serviceList.removeEventListener("click", handleMoveItem);

    addService.style.display = 'inline';
    editServiceGroup.style.display = 'none';

    document.getElementsByClassName("service-list")[0].innerHTML = originalListContent;
}

function enableDesktopEdit() {
    document.querySelectorAll(".service-item-content").forEach(item => {
        item.innerHTML += `
        <div class="col-auto text-end move-item">
            <label class="form-label">
                <svg xmlns="http://www.w3.org/2000/svg" width="16" height="16" fill="currentColor" class="bi bi-arrows-vertical" viewBox="0 0 16 16">
                    <path d="M8.354 14.854a.5.5 0 0 1-.708 0l-2-2a.5.5 0 0 1 .708-.708L7.5 13.293V2.707L6.354 3.854a.5.5 0 1 1-.708-.708l2-2a.5.5 0 0 1 .708 0l2 2a.5.5 0 0 1-.708.708L8.5 2.707v10.586l1.146-1.147a.5.5 0 0 1 .708.708z" />
                </svg>
            </label>
        </div>`;
    });

    enableDragAndDrop();
}

function enableMobileEdit() {
    document.querySelectorAll(".service-item-content").forEach(item => {
        item.innerHTML += `
        <div class="col-auto text-end move-item">
            <button type="button" class="btn btn-outline-dark up-btn">&#9650;</button>
            <button type="button" class="btn btn-outline-dark down-btn">&#9660;</button>
        </div>`;
    });

    serviceList.addEventListener("click", handleMoveItem);
}

function handleMoveItem(e) {
    const target = e.target;

    if (target.classList.contains("up-btn")) {
        moveItem(target.closest(".service-item"), -1); // Поднять вверх
    } else if (target.classList.contains("down-btn")) {
        moveItem(target.closest(".service-item"), 1); // Переместитесь вниз
    }
}

function moveItem(item, direction) {
    const index = Array.from(serviceList.children).indexOf(item);
    const newIndex = index + direction;

    if (newIndex >= 0 && newIndex < serviceList.children.length) {
        // Поменяйте элементы местами
        const referenceNode = serviceList.children[newIndex];
        serviceList.insertBefore(item, direction === 1 ? referenceNode.nextSibling : referenceNode);
    }

    updateServiceRatings();
}

function updateServiceRatings() {
    let elements = document.querySelectorAll(".service-item");

    for (let i = 0; i < elements.length; i++) {
        elements[i].value = i;
    }

    changedServiceRatings = new Map();

    for (let i = 0; i < serviceItems.length; i++) {
        changedServiceRatings.set(serviceItems[i].id, serviceItems[i].value);
    }
}

function enableDragAndDrop() {

    for (let i = 0; i < serviceItems.length; i++) {
        serviceItems[i].draggable = "true";
        serviceItems[i].value = i;
    }

    serviceItems.forEach(item => {
        item.addEventListener("dragstart", () => {
            // Добавление dragging класса к элементу после задержки
            setTimeout(() => item.classList.add("dragging"), 0);
        });

        // Удаление dragging класса из элемента по событию dragend
        item.addEventListener("dragend", () => {
            item.classList.remove("dragging");
            
            updateServiceRatings();
        });
    });

    const initSortableList = (e) => {
        e.preventDefault();
        const draggingItem = document.querySelector(".dragging");
        // Получение всех элементов, кроме перетаскиваемых в данный момент, и создание из них массива
        let siblings = [...serviceList.querySelectorAll(".service-item:not(.dragging)")];

        // Нахождение дочернего элемента, после которого должен быть помещен перетаскиваемый элемент
        let nextSibling = siblings.find(sibling => {
            return e.clientY <= sibling.offsetTop + sibling.offsetHeight / 4;
        });

        // Вставка перетаскиваемого элемента перед найденным дочерним элементом
        serviceList.insertBefore(draggingItem, nextSibling);
    }


    serviceList.addEventListener("dragover", initSortableList);
    serviceList.addEventListener("dragenter", e => e.preventDefault());
}