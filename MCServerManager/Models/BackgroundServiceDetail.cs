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
		[Required, StringLength(100), DisplayName("Адрес сервера")]
		public string Addres { get; set; }

		/// <summary>
		/// Используемый порт
		/// </summary>
		[Range(1024, 65535), DisplayName("Используемый порт")]
		public int? Port { get; set; }
	}
}
