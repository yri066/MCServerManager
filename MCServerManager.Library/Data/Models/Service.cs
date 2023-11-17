using MCServerManager.Library.Data.Interface;
using Newtonsoft.Json;

namespace MCServerManager.Library.Data.Models
{
	/// <summary>
	/// Данные о сервисе
	/// </summary>
	public class Service : IApplication
	{
        /// <summary>
        /// Идентификатор сервиса
        /// </summary>
        public Guid ServiceId { get; set; }

        /// <summary>
        /// Идентификатор сервиса
        /// </summary>
        [JsonIgnore]
        public Guid Id { get { return ServiceId; } }

        /// <summary>
        /// Идентификатор сервера
        /// </summary>
        public Guid ServerId { get; set; }

		/// <summary>
		/// Идентификатор приложения
		/// </summary>
		//public Guid Id { get { return ServiceId; } }

        /// <summary>
        /// Название приложения
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Автозапуск
        /// </summary>
        public bool AutoStart { get; set; }

		/// <summary>
		/// Выключение вместе с сервером
		/// </summary>
		public bool AutoClose { get; set; }

        /// <summary>
        /// Расположение приложения
        /// </summary>
        public string WorkDirectory { get; set; }

        /// <summary>
        /// Программа для запуска
        /// </summary>
        public string StartProgram { get; set; }

        /// <summary>
        /// Аргументы запуска
        /// </summary>
        public string Arguments { get; set; }

        /// <summary>
        /// Адрес сервера(ip)
        /// </summary>
        public string Address { get; set; }

		/// <summary>
		/// Используемый порт
		/// </summary>
		public int? Port { get; set; }

        public void UpdateServiceData(Service service)
        {
            service.Name = Name;
            service.AutoStart = AutoStart;
            service.AutoClose = AutoClose;
            service.WorkDirectory = WorkDirectory;
            service.StartProgram = StartProgram;
            service.Arguments = Arguments;
            service.Address = Address;
            service.Port = Port;
        }

        public void UpdateData(Service service)
        {
            Name = service.Name;
            AutoStart = service.AutoStart;
            AutoClose = service.AutoClose;
            WorkDirectory = service.WorkDirectory;
            StartProgram = service.StartProgram;
            Arguments = service.Arguments;
            Address = service.Address;
            Port = service.Port;
        }
    }
}
