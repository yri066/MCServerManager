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
        public Guid? GameServerId { get { return Data.ServerId; } }

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


        public BackgroundService(Service data, IConfiguration configuration) : base(data, configuration)
        {
            CheckServiceData(data);
            Data = data;
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
