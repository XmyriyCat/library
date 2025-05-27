# Library Web-application
#### Использованные технологии
1. .Net 9.0;
2. Entity Framework Core;
3. MS SQL;
4. Mapster;
5. FluentValidation;
6. Authentication via jwt access & refresh token (ex.: IdentityServer4);
7. EF Fluent API;
8. React;
9. xUnit;
10. Docker
11. Redis
12. Nginx

## Функциональность Web API
#### Работа с книгами
1. Получение списка всех книг; ✅
2. Получение определенной книги по её Id; ✅
3. Получение книги по её ISBN; ✅
4. Добавление новой книги; ✅
5. Изменение информации о существующей книге; ✅
6. Удаление книги; ✅
7. Выдача книг на руки пользователю; ✅
8. Возможность добавления изображения к книге и его хранение; ✅
9. *Отправка уведомления об истечении срока выдачи книги (будет плюсом). ✅

#### Работа с авторами
1. Получение списка всех авторов; ✅
2. Получение автора по его Id; ✅
3. Добавление нового автора; ✅
4. Изменение информации о существующем авторе; ✅
5. Удаление автора; ✅
6. Получение всех книг по автору. ✅

#### Разработка клиентской части приложения
1. Должна быть реализована страница регистрации/аутентификации; ✅
2. Реализовать страницу отображения списка книг. Если все книги с данным автором и названием были взяты из библиотеки, отображать, что книги нет в наличии; ✅
3. Реализовать страницу, отображающую информацию о конкретной книге. Если книга в наличии, пользователь может ее взять; ✅
4. Для админа реализовать страницу добавления/редактирования книги; ✅
5. Для админа на странице с информацией о книге должны быть кнопки "Редактировать", "Удалить". При нажатии на кнопку редактирования происходит редирект на страницу редактирования/добавления книги. При нажатии на кнопку удаления открывается модальное окно с подтверждением удаления книги; ✅
6. У каждого пользователя должна быть страница, на которой он сможет просматривать книги, которые взял из библиотеки;
7. Реализовать пагинацию, поиск по названию книги, фильтрацию по жанру/автору. ✅

#### Требования к Web API
1. Реализация policy-based авторизации с использованием refresh и jwt access токенов; ✅
2. Внедрение паттерна репозиторий; ✅
3. Разработка middleware для глобальной обработки исключений; ✅
4. Реализация пагинации; ✅
5. Обеспечение покрытия unit-тестами всех use cases/сервисов одной модели и двух методов ее репозитория (один на получение и один на сохранение данных). Модель выбирается на усмотрение кандидата (для всех моделей должны быть написаны use cases/сервисы). Разрешено использовать InMemoryDatabase; 🔁 
6. *Внедрение кеширования изображений (будет плюсом). ✅ (Реализовано  при помощи Redis)

#### Второстепенные требования
1. Использование AsNoTracking в get запросах; ✅
2. Отсутствие закомментированного или неиспользуемого кода; ✅
3. Логгирование ключевых событий и ошибок; ✅
4. Использование FirstOrDefault() вместо First() + проверку на null, если элемент может отсутствовать; ✅
5. Сохранение данных при их создании в паттерне Репозитрий вне зависимости от использования паттерна UnitOfWork; ✅

## Инструкция по запуску проекта
1. Клонируем репозиторий
```console
git clone https://github.com/XmyriyCat/library.git
```
2. Выполняем сборку файла compose.yaml в терминале
```console
docker compose build
```
3. Выполняем запуск файла compose.yaml в терминале
```console
docker compose up -d
```
4. Далее выполним просмотр запущенных docker контейнеров
```console
docker ps
```
после этого должны увидеть вот такой вывод:
```console
CONTAINER ID   IMAGE                                   COMMAND                  CREATED       STATUS       PORTS                                                   NAMES
a59561f32ab6   modsenlibrary-library.client            "/docker-entrypoint.…"   2 hours ago   Up 2 hours   0.0.0.0:3000->80/tcp, [::]:3000->80/tcp                 library.client
7ba2f6a47c6e   library.api                             "dotnet Library.Api.…"   2 hours ago   Up 2 hours   5100/tcp, 0.0.0.0:8080->8080/tcp, [::]:8080->8080/tcp   library-api
88a37e7971b8   redis:latest                            "docker-entrypoint.s…"   2 hours ago   Up 2 hours   0.0.0.0:6379->6379/tcp, [::]:6379->6379/tcp             redis-cache
3795da5a4132   mcr.microsoft.com/mssql/server:latest   "/bin/bash /entrypoi…"   2 hours ago   Up 2 hours   0.0.0.0:1433->1433/tcp, [::]:1433->1433/tcp             mssql-db
```
5. Далее открываем начальную страницу проекта
```
http://localhost:3000/dashboard
```
## Демонстрация работы приложения
1. После запуска веб-приложения пользователь попадает на главную страницу. Обратите внимаение, что пользователь не авторизован. Пользователю доступен поиск книг по заголовку & жанру & автору.
   ![Screenshot1](https://github.com/XmyriyCat/library/blob/feature/readme.images/Screenshot1.png)
2. Демонстрация поиска книги.
   ![Screenshot2](https://github.com/XmyriyCat/library/blob/feature/readme.images/Screenshot2.png)
3. Демонстрация просмотра книги. Обращаю внимание, что неавторизованный пользователь не может брать книги, он может их только просматривать.
   ![Screenshot3](https://github.com/XmyriyCat/library/blob/feature/readme.images/Screenshot3.png)
4. Демонстрация просмотра авторов.
   ![Screenshot4](https://github.com/XmyriyCat/library/blob/feature/readme.images/Screenshot4.png)
5. Демонстрация просмотра личных данных автора.
   ![Screenshot5](https://github.com/XmyriyCat/library/blob/feature/readme.images/Screenshot5.png)
6. Окно регистрации пользователя
   ![Screenshot6](https://github.com/XmyriyCat/library/blob/feature/readme.images/Screenshot6.png)
7. Окно логина пользователя
   ![Screenshot7](https://github.com/XmyriyCat/library/blob/feature/readme.images/Screenshot7.png)
8. Главная страница админа. Админ может добавлять новые книги и авторов, изменять их и удалять. Менеджер же может только добавлять и изменять без прав на удаление.<br />
   Данные пользователей для входа:<br />
   Email: paulito@gmail.com & Password: password [Админ]<br />
   Email: Ivan228@gmail.com & Password: password [Менеджер]<br />
   Email: tom@gmail.com & Password: password [Обычный юзер]<br />
   ![Screenshot8](https://github.com/XmyriyCat/library/blob/feature/readme.images/Screenshot8.png)
9. Форма создания книги. Присутствует валидация данных на стороне клиента для всех форм (логин, регистрация, добавления и изменение объекта).
   ![Screenshot9](https://github.com/XmyriyCat/library/blob/feature/readme.images/Screenshot9.png)
10. Демонстрация просмотра книги для АДМИНА! Стали доступны операции изменения и удаления книги.
   ![Screenshot10](https://github.com/XmyriyCat/library/blob/feature/readme.images/Screenshot10.png)
11. Форма изменения книги. 
   ![Screenshot11](https://github.com/XmyriyCat/library/blob/feature/readme.images/Screenshot11.png)
12. Результат изменения книги. 
   ![Screenshot12](https://github.com/XmyriyCat/library/blob/feature/readme.images/Screenshot12.png)
13. Модальное окно для подтверждения удаления книги. 
   ![Screenshot13](https://github.com/XmyriyCat/library/blob/feature/readme.images/Screenshot13.png)
14. После подтверждения удаления книги появляется уведомление об удалении книги. 
   ![Screenshot14](https://github.com/XmyriyCat/library/blob/feature/readme.images/Screenshot14.png)
15. Личный кабинет пользователя. На странице отображаются книги, которые пользователь взял в пользование. В случае если срок пользования книгой просрочен, то карточка книги становится красной и пользователю приходит уведомление, что нужно вернуть книгу.
   ![Screenshot15](https://github.com/XmyriyCat/library/blob/feature/readme.images/Screenshot15.png)
16. Демонстрация возврата книги 
   ![Screenshot16](https://github.com/XmyriyCat/library/blob/feature/readme.images/Screenshot16.png)
17. Кеш изображений, сохранённых в Redis. 
   ![Screenshot17](https://github.com/XmyriyCat/library/blob/feature/readme.images/Screenshot17.png)
