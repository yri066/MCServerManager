namespace MCServerManager.Library.Data.Models
{
	/// <summary>
	/// Данные о сервере
	/// </summary>
	public class GameServerData : ServerData
	{
		/// <summary>
		/// Список сервисов.
		/// </summary>
		public ICollection<BackgroundServiceData> Services { get; set; }
	}
}
