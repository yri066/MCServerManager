using MCServerManager.Library.Data.Interface;
using Newtonsoft.Json;

namespace MCServerManager.Library.Data.Models
{
	/// <summary>
	/// Данные о сервере
	/// </summary>
	public class Server : IApplication
	{
		/// <summary>
		/// Идентификатор приложения
		/// </summary>
		public Guid ServerId { get; set; }

        /// <summary>
        /// Идентификатор приложения
        /// </summary>
        [JsonIgnore]
        public Guid Id { get { return ServerId; } }

        /// <summary>
        /// Название приложения
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Автозапуск
        /// </summary>
        public bool AutoStart { get; set; }

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

		/// <summary>
		/// Список сервисов.
		/// </summary>
		public ICollection<Service> Services { get; set; }

        public void UpdateData(Server server)
        {
            Name = server.Name;
            AutoStart = server.AutoStart;
            WorkDirectory = server.WorkDirectory;
            StartProgram = server.StartProgram;
            Arguments = server.Arguments;
            Address = server.Address;
            Port = server.Port;
        }
    }
}
