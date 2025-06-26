# ConnectSphere
Описание
ConnectSphere — это WPF-приложение для просмотра и управления данными социальной сети, хранящимися в базе данных PostgreSQL. Приложение позволяет загружать таблицы, добавлять, удалять и редактировать строки, а также экспортировать и импортировать данные в формате JSON. Оно разработано с использованием современных паттернов проектирования и принципов SOLID для обеспечения гибкости и масштабируемости.

Особенности
Просмотр и редактирование таблиц социальной сети (например, users, notifications, publications и др.).
Добавление и удаление строк с автоматической генерацией первичных ключей.
Сохранение изменений в базе данных.
Экспорт и импорт данных в JSON-формате.
Поддержка динамического отображения данных через DataGrid.
Требования
Операционная система: Windows (рекомендуется Windows 10 или новее).
Среда разработки: Visual Studio 2019 или новее.
Зависимости:
Npgsql (для работы с PostgreSQL).
Newtonsoft.Json (для сериализации/десериализации JSON).
База данных: PostgreSQL (локальный сервер с базой ConnectSphere2, пользователь postgres, пароль 12345).
Установка
Клонируйте репозиторий:
bash

git clone https://github.com/ваш_пользователь/SocialAppViewer.git
cd SocialAppViewer
Установите зависимости:
Откройте проект в Visual Studio.
Установите пакеты NuGet:
Npgsql (последняя версия).
Newtonsoft.Json (последняя версия).
Или выполните через консоль менеджера пакетов:
bash

dotnet add package Npgsql
dotnet add package Newtonsoft.Json
Настройте базу данных:
Убедитесь, что PostgreSQL установлен и работает на localhost:5432.
Создайте базу данных ConnectSphere2 и таблицы, указанные в коде (например, users, notifications и др.).
Настройте строку подключения в MainViewModel.cs (если требуется изменить):

_factory = new PostgresTableDataFactory("Host=localhost;Port=5432;Username=postgres;Password=12345;Database=ConnectSphere2");
Соберите и запустите проект:
Выполните Build > Rebuild Solution в Visual Studio.
Нажмите F5 для запуска.
Использование
При запуске приложение отобразит список доступных таблиц в левой панели.
Выберите таблицу (например, notifications) для просмотра её содержимого в правой части.
Используйте кнопки:
Добавить: Добавляет новую строку с автоматической генерацией идентификатора.
Удалить: Удаляет выбранную строку.
Сохранить: Сохраняет изменения в базе данных.
Экспорт JSON: Сохраняет все таблицы в файл JSON.
Импорт JSON: Загружает данные из файла JSON.
Архитектура и паттерны
Используемые паттерны
MVVM (Model-View-ViewModel):
Model: ITableDataFactory и PostgresTableDataFactory предоставляют данные и бизнес-логику взаимодействия с базой данных.
View: MainWindow.xaml отображает данные и интерфейс.
ViewModel: MainViewModel управляет данными, командами (например, AddRow, SaveChanges) и синхронизацией с UI через INotifyPropertyChanged.
Factory Method:
Интерфейс ITableDataFactory определяет метод CreateTable для создания DataTable.
PostgresTableDataFactory реализует этот метод, создавая таблицы из PostgreSQL.
MainViewModel использует фабрику, что позволяет легко заменить источник данных.
Принципы SOLID
S (Single Responsibility Principle): Каждый класс (например, PostgresTableDataFactory, MainViewModel) имеет одну ответственность.
O (Open/Closed Principle): Интерфейс ITableDataFactory открыт для расширения (новые фабрики), но закрыт для модификации.
L (Liskov Substitution Principle): Любая реализация ITableDataFactory может заменить другую без нарушения работы.
I (Interface Segregation Principle): ITableDataFactory содержит только необходимые методы.
D (Dependency Inversion Principle): MainViewModel зависит от абстракции ITableDataFactory, а не от конкретной реализации.
Структура проекта
ITableDataFactory.cs: Определение интерфейса фабрики.
PostgresTableDataFactory.cs: Реализация фабрики для PostgreSQL.
MainViewModel.cs: Логика ViewModel с командами и управлением данными.
MainWindow.xaml.cs: Код-биhинд для инициализации.
MainWindow.xaml: Разметка пользовательского интерфейса.
