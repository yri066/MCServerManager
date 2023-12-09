using MCServerManager.Library.Data.Models;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;

namespace MCServerManager.Library.Actions
{
    /// <summary>
    /// Фоновый сервис.
    /// </summary>
    public class BackgroundService : Application
    {
        /// <summary>
        /// Информация о сервисном приложении.
        /// </summary>
        [JsonIgnore]
        public new Service Data { get; private set; }

        /// <summary>
        /// Идентификатор сервиса.
        /// </summary>
        public Guid ServiceId { get { return Data.Id; } }

        /// <summary>
        /// Идентификатор сервера.
        /// </summary>
        public Guid GameServerId { get { return Data.ServerId; } }

        /// <summary>
        /// Задержка до полного запуска.
        /// </summary>
        public int Delay { get { return Data.Delay; } }

        /// <summary>
        /// Адрес сервера/ip.
        /// </summary>
        public string Address { get { return Data.Address; } }

        /// <summary>
        /// Используемый порт.
        /// </summary>
        public int? Port { get { return Data.Port; } }

        /// <summary>
        /// Автовыключение вместе с сервером.
        /// </summary>
        public bool AutoClose { get { return Data.AutoClose; } }

        /// <summary>
        /// Событие начала работы серверного приложения.
        /// </summary>
        public new event ServerStartedEventHandler Started;

        /// <summary>
        /// Таймер для задержки изменения состояния.
        /// </summary>
        protected Timer Timer;


        public BackgroundService(Service data, IConfiguration configuration) : base(data, configuration)
        {
            CheckServiceData(data);
            Data = data;

            Closed += (id) => { TimerDispose(); };
        }

        /// <summary>
        /// Обновляет настройки серверного приложения.
        /// </summary>
        /// <param name="data">Информация о серверном приложении.</param>
        public void UpdateData(Service data)
        {
            if (ServiceId != data.Id)
            {
                throw new Exception("Идентификаторы не совпадают");
            }

            if (Data.UserId != data.UserId)
            {
                throw new Exception("Идентификаторы не совпадают");
            }

            CheckServiceData(data);

            base.UpdateData(data);
            Data.UpdateData(data);
        }

        /// <summary>
        /// Запускает серверное приложение.
        /// </summary>
        /// <exception cref="InvalidOperationException"></exception>
        /// <exception cref="System.ComponentModel.Win32Exception"></exception>
        /// <exception cref="ObjectDisposedException"></exception>
        /// <exception cref="PlatformNotSupportedException"></exception>
        public new void Start()
        {
            if (State != Status.Off)
            {
                return;
            }

            base.Start();
            State = Status.Launch;
            Timer = new Timer((state) => 
                {
                    State = Status.Run;
                    Started?.Invoke(ServiceId);
                    TimerDispose();
                },
                null,
                TimeSpan.FromSeconds(Delay),
                Timeout.InfiniteTimeSpan);
        }

        protected void TimerDispose()
        {
            if (Timer != null)
            {
                Timer.Dispose();
                Timer = null;
            }
        }

        /// <summary>
        /// Проверяет данные сервиса.
        /// </summary>
        /// <param name="data">Информация о сервисе.</param>
        public void CheckServiceData(Service data)
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
    }
}
