using System.ComponentModel.DataAnnotations;
using System.ComponentModel;
using MCServerManager.Library.Data.Models;

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
		[StringLength(100), DisplayName("Адрес сервера.")]
		public string? Address { get; set; }

		/// <summary>
		/// Используемый порт
		/// </summary>
		[DisplayName("Используемый порт.")]
		public int? Port { get; set; }

		/// <summary>
		/// Автовыключение вместе с сервером.
		/// </summary>
		[Required, DisplayName("Автовыключение вместе с сервером.")]
		public bool AutoClose { get; set; }

		public BackgroundServiceData GetBackgroundServiceData(Guid id, Guid serverId)
		{
			return new BackgroundServiceData
			{
				Id = id,
				GameServerId = serverId,
				Name = Name,
				AutoStart = AutoStart,
				AutoClose = AutoClose,
				WorkDirectory = WorkDirectory,
				Programm = Programm,
				Arguments = Arguments,
				Address = Address,
				Port = Port
			};
		}
	}
}
