using MCServerManager.Library.Data.Models;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using System.Text.RegularExpressions;

namespace MCServerManager.Library.Actions
{
    /// <summary>
    /// Серверное приложение.
    /// </summary>
    public class GameServer : Application
    {
        /// <summary>
        /// Состояния сервера.
        /// </summary>
        public new enum Status
        {
            Run,
            Off,
            Launch,
            Shutdown,
            Reboot,
            Error
        }

        /// <summary>
        /// Информация о серверном приложении.
        /// </summary>
        [JsonIgnore]
        public new Server Data { get; private set; }

        /// <summary>
        /// Идентификатор сервера.
        /// </summary>
        public Guid ServerId { get { return Data.Id; } }

        /// <summary>
        /// Адрес сервера/ip.
        /// </summary>
        public string Address { get { return Data.Address; } }

        /// <summary>
        /// Используемый порт.
        /// </summary>
        public int? Port { get { return Data.Port; } }

        /// <summary>
        /// Список сервисов.
        /// </summary>
        private List<BackgroundService> _services = new();

        /// <summary>
        /// Список сервисов.
        /// </summary>
        public IEnumerable<BackgroundService> Services { get { return _services; } }

        /// <summary>
        /// Состояние сервера.
        /// </summary>
        [JsonIgnore]
        public new Status State { get; private set; }

        /// <summary>
        /// Список игроков на сервере.
        /// </summary>
        private UsersListServer<string> _userList = new();

        /// <summary>
        /// Список игроков на сервере.
        /// </summary>
        [JsonIgnore]
        public IUsersListServer<string> UserList { get { return _userList; } }

        /// <summary>
        /// Событие завершения работы серверного приложения.
        /// </summary>
        public new event ServerClosedEventHandler Closed;

        /// <summary>
        /// Событие начала работы серверного приложения.
        /// </summary>
        public new event ServerStartedEventHandler Started;

        /// <summary>
        /// Делегат события завершения работы серверного приложения при перезагрузке.
        /// </summary>
        delegate void ServerOffEventHandler();

        /// <summary>
        /// Событие завершения работы серверного приложения при перезагрузке.
        /// </summary>
        event ServerOffEventHandler ServerOff;

        /// <summary>
        /// Конструктор с параметром
        /// </summary>
        /// <param name="data">Информация о серверном приложении.</param>
        public GameServer(Server data, IConfiguration configuration) : base(data, configuration)
        {
            CheckServerData(data);

            Data = data;
            State = Status.Off;

            if (Data.Services != null)
            {
                foreach (var service in Data.Services)
                {
                    _services.Add(new BackgroundService(service, configuration));
                }
            }

            Started += (id) => AutoStartBackgroundService();
            ServerOff += () => CloseBackgroundService();
        }

        /// <summary>
        /// Обновляет настройки серверного приложения.
        /// </summary>
        /// <param name="data">Информация о серверном приложении.</param>
        public void UpdateData(Server data)
        {
            if (ServerId != data.Id)
            {
                throw new Exception("Идентификаторы не совпадают");
            }

            CheckServerData(data);

            base.UpdateData(data);
            Data.UpdateData(data);
        }

        /// <summary>
        /// Обновляет информацию о сервисе.
        /// </summary>
        /// <param name="serviceData">Информация о сервисе.</param>
        public void UpdateServiceData(Service serviceData)
        {
            var service = GetService(serviceData.Id);
            service.UpdateData(serviceData);

            var item = Data.Services.FirstOrDefault(x => x.Id == serviceData.Id);

            if (item != null)
            {
                item.UpdateData(serviceData);
            }
        }

        /// <summary>
        /// Добавить сервис.
        /// </summary>
        /// <param name="service">Сервис.</param>
        public void AddService(BackgroundService service)
        {
            if (ServerId != service.GameServerId)
            {
                throw new Exception("Сервис предназначен для использования с другим сервером.");
            }

            if (_services.FirstOrDefault(x => x.ServiceId == service.ServiceId) != null)
            {
                throw new Exception("Сервис с таким же id уже существует.");
            }

            _services.Add(service);
            Data.Services.Add(service.Data);
        }

        /// <summary>
        /// Получить сервис.
        /// </summary>
        /// <param name="serviceId">Идентификатор сервиса.</param>
        /// <returns>Сервис.</returns>
        /// <exception cref="Exception">Сервис не найден.</exception>
        public BackgroundService GetService(Guid serviceId)
        {
            var service = _services.FirstOrDefault(x => x.ServiceId == serviceId);

            if (service == null)
            {
                throw new Exception("Сервис не найден.");
            }

            return service;
        }

        /// <summary>
        /// Удалить сервис.
        /// </summary>
        /// <param name="serviceId">Идентификатор сервиса.</param>
        public void DeleteService(Guid serviceId)
        {
            var service = GetService(serviceId);

            if (service.State == Application.Status.Run)
            {
                service.Close();
            }

            _services.Remove(service);

            var item = Data.Services.FirstOrDefault(x => x.Id == serviceId);

            if (item != null)
            {
                Data.Services.Remove(item);
            }
        }

        /// <summary>
        /// Запускает серверное приложение.
        /// </summary>
        public new void Start()
        {
            if (State != Status.Off && State != Status.Error && State != Status.Reboot)
            {
                return;
            }

            if (State != Status.Reboot)
            {
                State = Status.Launch;
            }

            StartServer(new EventHandler((sender, e) =>
            {
                ProcessClosed();
                ServerOff?.Invoke();
            }));
        }

        /// <summary>
        /// Завершает работу серверного приложения.
        /// </summary>
        public void Stop()
        {
            if (State != Status.Run && State != Status.Reboot)
            {
                return;
            }

            var stopCommand = "stop";
            SendAppMessage(stopCommand);

            if (State != Status.Reboot)
            {
                State = Status.Shutdown;
            }
        }

        /// <summary>
        /// Очищает ресурсы процесса после завершения работы.
        /// </summary>
        private new void ProcessClosed()
        {
            _process.Dispose();

            if (State == Status.Launch)
            {
                State = Status.Error;
                return;
            }

            if (State != Status.Reboot)
            {
                State = Status.Off;
                // Вызывается событие отключения серверного приложения
                Closed?.Invoke(ServerId);
            }
        }

        /// <summary>
        /// Перезапустить серверное приложение.
        /// </summary>
        public void Restart()
        {
            if (State != Status.Run)
            {
                return;
            }

            ServerOff += RunOffServer;
            Stop();
            State = Status.Reboot;
        }

        /// <summary>
        /// Запускает серверное приложение после завершения работы при перезапуске.
        /// </summary>
        private void RunOffServer()
        {
            ServerOff -= RunOffServer;
            Start();
        }

        /// <summary>
        /// Отключает серверное приложение не дожидаясь завершения работы.
        /// </summary>
        public new void Close()
        {
            if (State == Status.Off || State == Status.Error)
            {
                return;
            }

            if (State == Status.Reboot)
            {
                ServerOff -= RunOffServer;
            }

            State = Status.Off;
            _process.Kill();
        }

        /// <summary>
        /// Отключает все сервисы.
        /// </summary>
        public void CloseAllServices()
        {
            _services.ForEach(x => x.Close());
        }

        /// <summary>
        /// Выводит сообщение от серверного приложения.
        /// </summary>
        /// <param name="message">Текст сообщения.</param>
        protected override void GetAppMessage(string message = "")
        {
            base.GetAppMessage(message);

            DetectingCompletionStartupServer(message);
            DetectingUser(message);
        }

        /// <summary>
        /// Отправляет команду в серверное приложение.
        /// </summary>
        /// <param name="message">Команда для серверного приложения.</param>
        public override void SendAppMessage(string message = "")
        {
            if (State != Status.Run)
            {
                return;
            }

            base.SendAppMessage(message);
        }

        /// <summary>
        /// Проверяет данные серверного приложения.
        /// </summary>
        /// <param name="data">Информация о серверном приложении.</param>
        public void CheckServerData(Server data)
        {
            CheckApplicationData(data);

            if (data.Port != null)
            {
                if (data.Port <= 1023 || data.Port >= 65535)
                {
                    throw new ArgumentOutOfRangeException(nameof(data.Port), "Значения порта задано вне допустимого диапазона 1024 - 65535");
                }
            }
        }

        private void AutoStartBackgroundService()
        {
            _services.ForEach(service =>
            {
                if (service.AutoStart &&
                    service.State == Actions.Application.Status.Off)
                {
                    service.Start();
                }
            });
        }

        private void CloseBackgroundService()
        {
            _services.ForEach(service =>
            {
                if (service.AutoClose &&
                    service.State == Actions.Application.Status.Run)
                {
                    service.Close();
                }
            });
        }

        /// <summary>
        /// Определение полной загрузки сервера.
        /// </summary>
        /// <param name="message">Текст сообщения от сервера.</param>
        private void DetectingCompletionStartupServer(string message)
        {
            if (string.IsNullOrEmpty(message))
            {
                return;
            }

            if (State != Status.Launch && State != Status.Reboot)
            {
                return;
            }

            var MessageServerStarted = "Done";

            if (message.Contains(MessageServerStarted))
            {
                State = Status.Run;
                base.State = Actions.Application.Status.Run;
                Started?.Invoke(ServerId);
            }
        }

        /// <summary>
        /// Определение подключения/отключения пользователя.
        /// </summary>
        /// <param name="message">Текст сообщения от сервера.</param>
        private void DetectingUser(string message)
        {
            if (string.IsNullOrEmpty(message))
            {
                return;
            }

            if (State == Status.Off || State == Status.Error || State == Status.Launch)
            {
                return;
            }

            //Регулярное выражение для определения подключения пользователя.
            const string patternUserConnected = @"\[.*\]:\s([^\<\>\[\]\s]*)\sjoined\sthe\sgame$";

            //Регулярное выражение для определения отключения пользователя.
            const string patternUserDisconnected = @"\[.*\]:\s([^\<\>\[\]\s]*)\sleft\sthe\sgame$";


            int groupLogin = 1; // Расположение логина в группе.

            // Определение подключения пользователя к серверу.
            if (Regex.Match(message, patternUserConnected).Success)
            {
                _userList.Add(Regex.Match(message, patternUserConnected).Groups[groupLogin].Value);
            }

            // Определение отключения пользователя от сервера.
            if (Regex.Match(message, patternUserDisconnected).Success)
            {
                _userList.Remove(Regex.Match(message, patternUserDisconnected).Groups[groupLogin].Value);
            }
        }
    }
}
