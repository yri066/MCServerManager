namespace MCServerManager.Library.Data.Models
{
	/// <summary>
	/// Данные о сервисе
	/// </summary>
	public class BackgroundServiceData : ApplicationData
	{
		/// <summary>
		/// Идентификатор сервера
		/// </summary>
		public Guid GameServerId { get; set; }

		/// <summary>
		/// Адрес сервера(ip)
		/// </summary>
		public string Address { get; set; }

		/// <summary>
		/// Используемый порт
		/// </summary>
		public int? Port { get; set; }

		/// <summary>
		/// Автовыключение вместе с сервером
		/// </summary>
		public bool AutoClose { get; set; }
	}
}
