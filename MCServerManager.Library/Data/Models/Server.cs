using MCServerManager.Library.Data.Interface;
using Microsoft.AspNetCore.Identity;
using Newtonsoft.Json;

namespace MCServerManager.Library.Data.Models
{
	/// <summary>
	/// Данные о сервере.
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

        /// <summary>
        /// Идентификатор пользователя.
        /// </summary>
        public string UserId { get; set; }
        public IdentityUser User { get; set; }

        public void UpdateServerData(Server server)
		{
			server.Name = Name;
			server.AutoStart = AutoStart;
			server.WorkDirectory= WorkDirectory;
			server.StartProgram = StartProgram;
			server.Arguments= Arguments;
			server.Address = Address;
			server.Port = Port;
		}
	}
}
