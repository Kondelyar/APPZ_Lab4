// API базовий URL
const apiUrl = '/api';

// Елементи DOM
const storagesTableBody = document.getElementById('storagesTableBody');
const refreshStoragesBtn = document.getElementById('refreshStorages');
const saveStorageBtn = document.getElementById('saveStorageBtn');
const updateStorageBtn = document.getElementById('updateStorageBtn');
const searchByTypeBtn = document.getElementById('searchByType');

// Завантаження при старті сторінки
document.addEventListener('DOMContentLoaded', () => {
    loadStorages();

    // Налаштування обробників подій
    refreshStoragesBtn.addEventListener('click', loadStorages);
    saveStorageBtn.addEventListener('click', createStorage);
    updateStorageBtn.addEventListener('click', updateStorage);
    searchByTypeBtn.addEventListener('click', searchStoragesByType);

    // Очищення форм при показі модальних вікон
    document.getElementById('addStorageModal').addEventListener('show.bs.modal', clearAddStorageForm);
});

// Функція для завантаження списку сховищ
async function loadStorages() {
    try {
        const response = await fetch(`${apiUrl}/Storage`);
        if (!response.ok) {
            throw new Error(`HTTP помилка! Статус: ${response.status}`);
        }

        const storages = await response.json();
        displayStorages(storages);
    } catch (error) {
        console.error('Помилка при завантаженні сховищ:', error);
        showAlert('Помилка при завантаженні сховищ: ' + error.message, 'danger');
    }
}

// Функція для відображення сховищ у таблиці
function displayStorages(storages) {
    storagesTableBody.innerHTML = '';

    if (storages.length === 0) {
        storagesTableBody.innerHTML = `<tr><td colspan="8" class="text-center">Сховища не знайдено</td></tr>`;
        return;
    }

    storages.forEach(storage => {
        const row = document.createElement('tr');
        row.innerHTML = `
            <td>${storage.id}</td>
            <td>${escapeHtml(storage.name)}</td>
            <td>${storage.type}</td>
            <td>${escapeHtml(storage.location)}</td>
            <td>${storage.capacity}</td>
            <td>${storage.usedSpace}</td>
            <td>${storage.availableSpace}</td>
            <td>
                <button class="btn btn-sm btn-primary btn-action" onclick="openEditStorageModal(${storage.id})">
                    <i class="bi bi-pencil"></i>
                </button>
                <button class="btn btn-sm btn-danger btn-action" onclick="deleteStorage(${storage.id})">
                    <i class="bi bi-trash"></i>
                </button>
            </td>
        `;
        storagesTableBody.appendChild(row);
    });
}

// Функція для створення нового сховища
async function createStorage() {
    const storageData = {
        name: document.getElementById('storageName').value,
        type: document.getElementById('storageType').value,
        location: document.getElementById('storageLocation').value,
        capacity: parseFloat(document.getElementById('storageCapacity').value)
    };

    try {
        const response = await fetch(`${apiUrl}/Storage`, {
            method: 'POST',
            headers: {
                'Content-Type': 'application/json'
            },
            body: JSON.stringify(storageData)
        });

        if (!response.ok) {
            const errorText = await response.text();
            throw new Error(`HTTP помилка! Статус: ${response.status}, ${errorText}`);
        }

        // Закриття модального вікна і оновлення списку сховищ
        bootstrap.Modal.getInstance(document.getElementById('addStorageModal')).hide();
        showAlert('Сховище успішно додано', 'success');
        loadStorages();
    } catch (error) {
        console.error('Помилка при створенні сховища:', error);
        showAlert('Помилка при створенні сховища: ' + error.message, 'danger');
    }
}

// Функція для відкриття модального вікна редагування сховища
async function openEditStorageModal(storageId) {
    try {
        const response = await fetch(`${apiUrl}/Storage/${storageId}`);
        if (!response.ok) {
            throw new Error(`HTTP помилка! Статус: ${response.status}`);
        }

        const storage = await response.json();

        // Заповнення форми даними сховища
        document.getElementById('editStorageId').value = storage.id;
        document.getElementById('editStorageName').value = storage.name;
        document.getElementById('editStorageType').value = storage.type;
        document.getElementById('editStorageLocation').value = storage.location;
        document.getElementById('editStorageCapacity').value = storage.capacity;

        // Відкриття модального вікна
        const editStorageModal = new bootstrap.Modal(document.getElementById('editStorageModal'));
        editStorageModal.show();
    } catch (error) {
        console.error('Помилка при завантаженні даних сховища:', error);
        showAlert('Помилка при завантаженні даних сховища: ' + error.message, 'danger');
    }
}

// Функція для оновлення сховища
async function updateStorage() {
    const storageId = document.getElementById('editStorageId').value;
    const storageData = {
        name: document.getElementById('editStorageName').value,
        type: document.getElementById('editStorageType').value,
        location: document.getElementById('editStorageLocation').value,
        capacity: parseFloat(document.getElementById('editStorageCapacity').value)
    };

    try {
        const response = await fetch(`${apiUrl}/Storage/${storageId}`, {
            method: 'PUT',
            headers: {
                'Content-Type': 'application/json'
            },
            body: JSON.stringify(storageData)
        });

        if (!response.ok) {
            const errorText = await response.text();
            throw new Error(`HTTP помилка! Статус: ${response.status}, ${errorText}`);
        }

        // Закриття модального вікна і оновлення списку сховищ
        bootstrap.Modal.getInstance(document.getElementById('editStorageModal')).hide();
        showAlert('Сховище успішно оновлено', 'success');
        loadStorages();
    } catch (error) {
        console.error('Помилка при оновленні сховища:', error);
        showAlert('Помилка при оновленні сховища: ' + error.message, 'danger');
    }
}

// Функція для видалення сховища
async function deleteStorage(storageId) {
    if (!confirm('Ви впевнені, що хочете видалити це сховище?')) {
        return;
    }

    try {
        const response = await fetch(`${apiUrl}/Storage/${storageId}`, {
            method: 'DELETE'
        });

        if (!response.ok) {
            throw new Error(`HTTP помилка! Статус: ${response.status}`);
        }

        showAlert('Сховище успішно видалено', 'success');
        loadStorages();
    } catch (error) {
        console.error('Помилка при видаленні сховища:', error);
        showAlert('Помилка при видаленні сховища: ' + error.message, 'danger');
    }
}

// Функція для пошуку сховищ за типом
async function searchStoragesByType() {
    const type = document.getElementById('storageTypeFilter').value;

    if (type === '') {
        // Якщо тип не вибрано, повернути всі сховища
        loadStorages();
        return;
    }

    try {
        const response = await fetch(`${apiUrl}/Storage/ByType/${encodeURIComponent(type)}`);
        if (!response.ok) {
            throw new Error(`HTTP помилка! Статус: ${response.status}`);
        }

        const storages = await response.json();
        displayStorages(storages);
    } catch (error) {
        console.error('Помилка при пошуку сховищ за типом:', error);
        showAlert('Помилка при пошуку сховищ за типом: ' + error.message, 'danger');
    }
}

// Функція для очищення форми додавання сховища
function clearAddStorageForm() {
    document.getElementById('addStorageForm').reset();
}

// Функція для відображення повідомлень користувачу
function showAlert(message, type) {
    // Перевірка, чи існує контейнер для повідомлень
    let alertContainer = document.querySelector('.alert-container');
    if (!alertContainer) {
        alertContainer = document.createElement('div');
        alertContainer.className = 'alert-container position-fixed top-0 end-0 p-3';
        document.body.appendChild(alertContainer);
    }

    // Створення повідомлення
    const alertEl = document.createElement('div');
    alertEl.className = `alert alert-${type} alert-dismissible fade show`;
    alertEl.innerHTML = `
        ${message}
        <button type="button" class="btn-close" data-bs-dismiss="alert" aria-label="Close"></button>
    `;

    // Додавання повідомлення в контейнер
    alertContainer.appendChild(alertEl);

    // Автоматичне закриття повідомлення через 5 секунд
    setTimeout(() => {
        const bsAlert = new bootstrap.Alert(alertEl);
        bsAlert.close();
    }, 5000);
}

// Функція для екранування HTML
function escapeHtml(unsafe) {
    if (unsafe === null || unsafe === undefined) return '';
    return unsafe
        .toString()
        .replace(/&/g, "&amp;")
        .replace(/</g, "&lt;")
        .replace(/>/g, "&gt;")
        .replace(/"/g, "&quot;")
        .replace(/'/g, "&#039;");
}