// API базовий URL
const apiUrl = '/api';

// Елементи DOM
const booksTableBody = document.getElementById('booksTableBody');
const refreshBooksBtn = document.getElementById('refreshBooks');
const saveBookBtn = document.getElementById('saveBookBtn');
const updateBookBtn = document.getElementById('updateBookBtn');
const addToStorageBtn = document.getElementById('addToStorageBtn');
const searchByAuthorBtn = document.getElementById('searchByAuthor');
const searchByPublisherBtn = document.getElementById('searchByPublisher');
const searchByISBNBtn = document.getElementById('searchByISBN');
const storageSelect = document.getElementById('storageSelect');

// Завантаження при старті сторінки
document.addEventListener('DOMContentLoaded', () => {
    loadBooks();

    // Налаштування обробників подій
    refreshBooksBtn.addEventListener('click', loadBooks);
    saveBookBtn.addEventListener('click', createBook);
    updateBookBtn.addEventListener('click', updateBook);
    addToStorageBtn.addEventListener('click', addBookToStorage);
    searchByAuthorBtn.addEventListener('click', searchBooksByAuthor);
    searchByPublisherBtn.addEventListener('click', searchBooksByPublisher);
    searchByISBNBtn.addEventListener('click', searchBookByISBN);

    // Очищення форм при показі модальних вікон
    document.getElementById('addBookModal').addEventListener('show.bs.modal', clearAddBookForm);
    document.getElementById('addToStorageModal').addEventListener('show.bs.modal', loadStoragesForSelect);
});

// Функція для завантаження списку книг
async function loadBooks() {
    try {
        const response = await fetch(`${apiUrl}/Book`);
        if (!response.ok) {
            throw new Error(`HTTP помилка! Статус: ${response.status}`);
        }

        const books = await response.json();
        displayBooks(books);
    } catch (error) {
        console.error('Помилка при завантаженні книг:', error);
        showAlert('Помилка при завантаженні книг: ' + error.message, 'danger');
    }
}

// Функція для відображення книг у таблиці
function displayBooks(books) {
    booksTableBody.innerHTML = '';

    if (books.length === 0) {
        booksTableBody.innerHTML = `<tr><td colspan="10" class="text-center">Книги не знайдено</td></tr>`;
        return;
    }

    books.forEach(book => {
        const row = document.createElement('tr');
        row.innerHTML = `
            <td>${book.id}</td>
            <td>${escapeHtml(book.title)}</td>
            <td>${escapeHtml(book.author)}</td>
            <td>${escapeHtml(book.publisher || '')}</td>
            <td>${escapeHtml(book.isbn || '')}</td>
            <td>${book.format}</td>
            <td>${book.size}</td>
            <td>${book.pageCount}</td>
            <td>${new Date(book.createdDate).toLocaleDateString()}</td>
            <td>
                <button class="btn btn-sm btn-primary btn-action" onclick="openEditBookModal(${book.id})">
                    <i class="bi bi-pencil"></i>
                </button>
                <button class="btn btn-sm btn-success btn-action" onclick="openAddToStorageModal(${book.id})">
                    <i class="bi bi-hdd"></i>
                </button>
                <button class="btn btn-sm btn-info btn-action" onclick="openViewStoragesModal(${book.id})">
                    <i class="bi bi-eye"></i>
                </button>
                <button class="btn btn-sm btn-danger btn-action" onclick="deleteBook(${book.id})">
                    <i class="bi bi-trash"></i>
                </button>
            </td>
        `;
        booksTableBody.appendChild(row);
    });
}

// Функція для створення нової книги
async function createBook() {
    const bookData = {
        title: document.getElementById('title').value,
        description: document.getElementById('description').value,
        size: parseFloat(document.getElementById('size').value),
        format: document.getElementById('format').value,
        author: document.getElementById('author').value,
        publisher: document.getElementById('publisher').value,
        isbn: document.getElementById('isbn').value,
        pageCount: parseInt(document.getElementById('pageCount').value)
    };

    try {
        const response = await fetch(`${apiUrl}/Book`, {
            method: 'POST',
            headers: {
                'Content-Type': 'application/json'
            },
            body: JSON.stringify(bookData)
        });

        if (!response.ok) {
            const errorText = await response.text();
            throw new Error(`HTTP помилка! Статус: ${response.status}, ${errorText}`);
        }

        // Закриття модального вікна і оновлення списку книг
        bootstrap.Modal.getInstance(document.getElementById('addBookModal')).hide();
        showAlert('Книгу успішно додано', 'success');
        loadBooks();
    } catch (error) {
        console.error('Помилка при створенні книги:', error);
        showAlert('Помилка при створенні книги: ' + error.message, 'danger');
    }
}

// Функція для відкриття модального вікна редагування книги
async function openEditBookModal(bookId) {
    try {
        const response = await fetch(`${apiUrl}/Book/${bookId}`);
        if (!response.ok) {
            throw new Error(`HTTP помилка! Статус: ${response.status}`);
        }

        const book = await response.json();

        // Заповнення форми даними книги
        document.getElementById('editBookId').value = book.id;
        document.getElementById('editTitle').value = book.title;
        document.getElementById('editDescription').value = book.description || '';
        document.getElementById('editSize').value = book.size;
        document.getElementById('editFormat').value = book.format;
        document.getElementById('editAuthor').value = book.author;
        document.getElementById('editPublisher').value = book.publisher || '';
        document.getElementById('editIsbn').value = book.isbn || '';
        document.getElementById('editPageCount').value = book.pageCount;

        // Відкриття модального вікна
        const editBookModal = new bootstrap.Modal(document.getElementById('editBookModal'));
        editBookModal.show();
    } catch (error) {
        console.error('Помилка при завантаженні даних книги:', error);
        showAlert('Помилка при завантаженні даних книги: ' + error.message, 'danger');
    }
}

// Функція для оновлення книги
async function updateBook() {
    const bookId = document.getElementById('editBookId').value;
    const bookData = {
        title: document.getElementById('editTitle').value,
        description: document.getElementById('editDescription').value,
        size: parseFloat(document.getElementById('editSize').value),
        format: document.getElementById('editFormat').value,
        author: document.getElementById('editAuthor').value,
        publisher: document.getElementById('editPublisher').value,
        isbn: document.getElementById('editIsbn').value,
        pageCount: parseInt(document.getElementById('editPageCount').value)
    };

    try {
        const response = await fetch(`${apiUrl}/Book/${bookId}`, {
            method: 'PUT',
            headers: {
                'Content-Type': 'application/json'
            },
            body: JSON.stringify(bookData)
        });

        if (!response.ok) {
            const errorText = await response.text();
            throw new Error(`HTTP помилка! Статус: ${response.status}, ${errorText}`);
        }

        // Закриття модального вікна і оновлення списку книг
        bootstrap.Modal.getInstance(document.getElementById('editBookModal')).hide();
        showAlert('Книгу успішно оновлено', 'success');
        loadBooks();
    } catch (error) {
        console.error('Помилка при оновленні книги:', error);
        showAlert('Помилка при оновленні книги: ' + error.message, 'danger');
    }
}

// Функція для видалення книги
async function deleteBook(bookId) {
    if (!confirm('Ви впевнені, що хочете видалити цю книгу?')) {
        return;
    }

    try {
        const response = await fetch(`${apiUrl}/Book/${bookId}`, {
            method: 'DELETE'
        });

        if (!response.ok) {
            throw new Error(`HTTP помилка! Статус: ${response.status}`);
        }

        showAlert('Книгу успішно видалено', 'success');
        loadBooks();
    } catch (error) {
        console.error('Помилка при видаленні книги:', error);
        showAlert('Помилка при видаленні книги: ' + error.message, 'danger');
    }
}

// Функція для відкриття модального вікна додавання книги до сховища
function openAddToStorageModal(bookId) {
    document.getElementById('bookToStorageId').value = bookId;
    const addToStorageModal = new bootstrap.Modal(document.getElementById('addToStorageModal'));
    addToStorageModal.show();
}

// Функція для завантаження списку сховищ для вибору
async function loadStoragesForSelect() {
    try {
        const response = await fetch(`${apiUrl}/Storage`);
        if (!response.ok) {
            throw new Error(`HTTP помилка! Статус: ${response.status}`);
        }

        const storages = await response.json();
        storageSelect.innerHTML = '';

        if (storages.length === 0) {
            storageSelect.innerHTML = '<option value="">Сховища не знайдено</option>';
            return;
        }

        storages.forEach(storage => {
            const option = document.createElement('option');
            option.value = storage.id;
            option.textContent = `${storage.name} (${storage.type}) - Доступно: ${storage.availableSpace} ГБ`;
            storageSelect.appendChild(option);
        });
    } catch (error) {
        console.error('Помилка при завантаженні сховищ:', error);
        showAlert('Помилка при завантаженні сховищ: ' + error.message, 'danger');
    }
}

// Функція для додавання книги до сховища
async function addBookToStorage() {
    const bookId = document.getElementById('bookToStorageId').value;
    const storageId = document.getElementById('storageSelect').value;
    const path = document.getElementById('storagePath').value;

    if (!storageId) {
        showAlert('Будь ласка, виберіть сховище', 'warning');
        return;
    }

    if (!path) {
        showAlert('Будь ласка, вкажіть шлях до файлу', 'warning');
        return;
    }

    try {
        const response = await fetch(`${apiUrl}/Book/${bookId}/Storage/${storageId}`, {
            method: 'POST',
            headers: {
                'Content-Type': 'application/json'
            },
            body: JSON.stringify(path)
        });

        if (!response.ok) {
            const errorText = await response.text();
            throw new Error(`HTTP помилка! Статус: ${response.status}, ${errorText}`);
        }

        // Закриття модального вікна
        bootstrap.Modal.getInstance(document.getElementById('addToStorageModal')).hide();
        showAlert('Книгу успішно додано до сховища', 'success');
    } catch (error) {
        console.error('Помилка при додаванні книги до сховища:', error);
        showAlert('Помилка при додаванні книги до сховища: ' + error.message, 'danger');
    }
}

// Функція для відкриття модального вікна перегляду сховищ книги
async function openViewStoragesModal(bookId) {
    try {
        const response = await fetch(`${apiUrl}/Book/${bookId}/Storage`);
        if (!response.ok) {
            throw new Error(`HTTP помилка! Статус: ${response.status}`);
        }

        const storages = await response.json();
        const bookResponse = await fetch(`${apiUrl}/Book/${bookId}`);
        if (!bookResponse.ok) {
            throw new Error(`HTTP помилка! Статус: ${bookResponse.status}`);
        }

        const book = await bookResponse.json();

        // Оновлення заголовка модального вікна
        document.getElementById('viewStoragesModalLabel').textContent = `Сховища для книги: ${book.title}`;

        // Відображення сховищ у таблиці
        const bookStoragesTableBody = document.getElementById('bookStoragesTableBody');
        bookStoragesTableBody.innerHTML = '';

        if (storages.length === 0) {
            bookStoragesTableBody.innerHTML = `<tr><td colspan="6" class="text-center">Книга не додана до жодного сховища</td></tr>`;
        } else {
            storages.forEach(storage => {
                // Пошук відповідного ContentStorage для отримання шляху
                const contentStorage = book.contentStorages?.find(cs => cs.storageId === storage.id);
                const path = contentStorage ? contentStorage.path : 'N/A';

                const row = document.createElement('tr');
                row.innerHTML = `
                    <td>${storage.id}</td>
                    <td>${escapeHtml(storage.name)}</td>
                    <td>${storage.type}</td>
                    <td>${escapeHtml(storage.location)}</td>
                    <td>${escapeHtml(path)}</td>
                    <td>
                        <button class="btn btn-sm btn-danger" onclick="removeBookFromStorage(${bookId}, ${storage.id})">
                            <i class="bi bi-trash"></i> Видалити
                        </button>
                    </td>
                `;
                bookStoragesTableBody.appendChild(row);
            });
        }

        // Відкриття модального вікна
        const viewStoragesModal = new bootstrap.Modal(document.getElementById('viewStoragesModal'));
        viewStoragesModal.show();
    } catch (error) {
        console.error('Помилка при завантаженні сховищ книги:', error);
        showAlert('Помилка при завантаженні сховищ книги: ' + error.message, 'danger');
    }
}

// Функція для видалення книги зі сховища
async function removeBookFromStorage(bookId, storageId) {
    if (!confirm('Ви впевнені, що хочете видалити цю книгу зі сховища?')) {
        return;
    }

    try {
        const response = await fetch(`${apiUrl}/Book/${bookId}/Storage/${storageId}`, {
            method: 'DELETE'
        });

        if (!response.ok) {
            throw new Error(`HTTP помилка! Статус: ${response.status}`);
        }

        showAlert('Книгу успішно видалено зі сховища', 'success');

        // Оновлення списку сховищ
        openViewStoragesModal(bookId);
    } catch (error) {
        console.error('Помилка при видаленні книги зі сховища:', error);
        showAlert('Помилка при видаленні книги зі сховища: ' + error.message, 'danger');
    }
}

// Функція для пошуку книг за автором
async function searchBooksByAuthor() {
    const author = document.getElementById('authorFilter').value.trim();
    if (!author) {
        showAlert('Будь ласка, введіть автора для пошуку', 'warning');
        return;
    }

    try {
        const response = await fetch(`${apiUrl}/Book/ByAuthor/${encodeURIComponent(author)}`);
        if (!response.ok) {
            throw new Error(`HTTP помилка! Статус: ${response.status}`);
        }

        const books = await response.json();
        displayBooks(books);
    } catch (error) {
        console.error('Помилка при пошуку книг за автором:', error);
        showAlert('Помилка при пошуку книг за автором: ' + error.message, 'danger');
    }
}

// Функція для пошуку книг за видавцем
async function searchBooksByPublisher() {
    const publisher = document.getElementById('publisherFilter').value.trim();
    if (!publisher) {
        showAlert('Будь ласка, введіть видавця для пошуку', 'warning');
        return;
    }

    try {
        const response = await fetch(`${apiUrl}/Book/ByPublisher/${encodeURIComponent(publisher)}`);
        if (!response.ok) {
            throw new Error(`HTTP помилка! Статус: ${response.status}`);
        }

        const books = await response.json();
        displayBooks(books);
    } catch (error) {
        console.error('Помилка при пошуку книг за видавцем:', error);
        showAlert('Помилка при пошуку книг за видавцем: ' + error.message, 'danger');
    }
}

// Функція для пошуку книги за ISBN
async function searchBookByISBN() {
    const isbn = document.getElementById('isbnFilter').value.trim();
    if (!isbn) {
        showAlert('Будь ласка, введіть ISBN для пошуку', 'warning');
        return;
    }

    try {
        const response = await fetch(`${apiUrl}/Book/ByISBN/${encodeURIComponent(isbn)}`);
        if (!response.ok) {
            if (response.status === 404) {
                displayBooks([]);
                return;
            }
            throw new Error(`HTTP помилка! Статус: ${response.status}`);
        }

        const book = await response.json();
        displayBooks([book]);
    } catch (error) {
        console.error('Помилка при пошуку книги за ISBN:', error);
        showAlert('Помилка при пошуку книги за ISBN: ' + error.message, 'danger');
    }
}

// Функція для очищення форми додавання книги
function clearAddBookForm() {
    document.getElementById('addBookForm').reset();
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