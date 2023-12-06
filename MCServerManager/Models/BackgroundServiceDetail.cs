using System.ComponentModel.DataAnnotations;
using System.ComponentModel;

namespace MCServerManager.Models
{
	/// <summary>
	/// Данные о сервисе
	/// </summary>
	public class BackgroundServiceDetail : ApplicationDetail
	{
		/// <summary>
		/// Адрес сервера(ip)
		/// </summary>
		[StringLength(100), DisplayName("Адрес сервера:")]
		public string? Address { get; set; }

        /// <summary>
        /// Задержка до полного запуска
        /// </summary>
        [Required, Range(0, 7200), DisplayName("Задержка до полного запуска:")]
        public int Delay { get; set; } = 10;

        /// <summary>
        /// Используемый порт
        /// </summary>
        [DisplayName("Используемый порт:")]
		public int? Port { get; set; }

		/// <summary>
		/// Автовыключение вместе с сервером.
		/// </summary>
		[Required, DisplayName("Автовыключение вместе с сервером:")]
		public bool AutoClose { get; set; }

		public Library.Data.Models.Service GetBackgroundServiceData(Guid id, Guid serverId)
		{
			return new Library.Data.Models.Service
			{
				ServiceId = id,
				ServerId = serverId,
				Name = Name,
				AutoStart = AutoStart,
				AutoClose = AutoClose,
                Delay = Delay,
				WorkDirectory = WorkDirectory,
                StartProgram = StartProgram,
				Arguments = Arguments,
				Address = Address,
				Port = Port
			};
		}
	}
}
