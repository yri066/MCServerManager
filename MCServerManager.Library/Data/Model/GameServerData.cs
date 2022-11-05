namespace MCServerManager.Library.Data.Model
{
	/// <summary>
	/// Данные о сервере
	/// </summary>
	public class GameServerData : ApplicationData
	{
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
		public ICollection<BackgroundServiceData> Services { get; set; }
	}
}
