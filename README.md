# ✅ TodoApi

> REST API с JWT авторизацией. Третий проект на пути к backend разработке.

---

## О проекте

TodoApi — это полноценный REST API на ASP.NET Core с регистрацией, авторизацией через JWT токены и CRUD операциями для задач. Каждый пользователь видит только свои задачи.

---

## Возможности

- 📝 Регистрация и вход
- 🔐 JWT авторизация
- ✅ Создание, чтение, обновление и удаление задач
- 👤 Каждый пользователь видит только свои задачи
- 🔒 Пароли хранятся в зашифрованном виде (BCrypt)

---

## Эндпоинты

| Метод | Путь | Авторизация | Описание |
|-------|------|-------------|----------|
| POST | `/register` | Нет | Регистрация |
| POST | `/login` | Нет | Вход, возвращает JWT токен |
| GET | `/todos` | Да | Получить все свои задачи |
| POST | `/todos` | Да | Создать задачу |
| PUT | `/todos/{id}` | Да | Обновить задачу |
| DELETE | `/todos/{id}` | Да | Удалить задачу |

---

## Как запустить

### Требования
- .NET 10.0 или выше

### Запуск

```bash
git clone https://github.com/strikxr93939/TodoApi.git
cd TodoApi
dotnet ef database update
dotnet run
```

После запуска API доступен на `http://localhost:5005`

---

## Примеры запросов

### Регистрация
```json
POST /register
{
  "username": "user",
  "password": "123456"
}
```

### Вход
```json
POST /login
{
  "username": "user",
  "password": "123456"
}
```

Ответ:
```json
{
  "token": "eyJhbGci..."
}
```

### Создать задачу
```json
POST /todos
Authorization: Bearer <токен>

{
  "title": "Написать код",
  "isDone": false
}
```

### Обновить задачу
```json
PUT /todos/1
Authorization: Bearer <токен>

{
  "title": "Написать код",
  "isDone": true
}
```

---

## Технологии

- **C#** / **.NET 10**
- **ASP.NET Core** — Minimal API
- **Entity Framework Core** — работа с базой данных
- **SQLite** — хранение данных
- **JWT** — авторизация
- **BCrypt** — хэширование паролей

---

## Структура проекта

```
TodoApi/
├── Program.cs        # Все эндпоинты и логика
├── User.cs           # Модель пользователя
├── TodoItem.cs       # Модель задачи
├── AppDbContext.cs   # Контекст базы данных
├── Migrations/       # Миграции Entity Framework
└── TodoApi.csproj    # Конфигурация проекта
```

---

## Автор

**strikxr93939** — строю backend по 30 минут каждое утро ☕
