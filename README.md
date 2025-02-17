# Web Library

## Описание проекта
Web Library - это веб-приложение для управления библиотекой, позволяющее пользователям просматривать книги, фильтровать их по жанрам и авторам, а администраторам управлять контентом.

## Запуск проекта с помощью Docker

### 1. Установите необходимые инструменты
Перед запуском убедитесь, что у вас установлены:
- [Docker](https://www.docker.com/)
- [Docker Compose](https://docs.docker.com/compose/install/)

### 2. Клонирование репозитория
```sh
git clone https://github.com/your-repository/Web_Library.git
cd Web_Library
```

### 3. Запуск проекта
Запустите контейнеры с помощью Docker Compose:
```sh
docker-compose up -d --build
```

### 4. Проверка работы
- **Backend (API):** http://localhost:8080/swagger
- **Frontend (UI):** http://localhost:5005

## Дополнительные команды

### Остановка контейнеров
```sh
docker-compose down
```

### Пересоздание базы данных (при необходимости)
```sh
docker-compose down -v
docker-compose up -d --build
```

## Отладка и решение проблем
- Если приложение не может подключиться к базе данных, убедитесь, что контейнер `db` работает:
  ```sh
  docker ps
  ```
- Проверить логи контейнеров:
  ```sh
  docker-compose logs -f
  ```
