using System.ComponentModel.DataAnnotations;
using System.ComponentModel;
using MCServerManager.Library.Data.Models;

namespace MCServerManager.Models
{
	/// <summary>
	/// Данные о сервере
	/// </summary>
	public class GameServerDetail : ApplicationDetail
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

		public GameServerData GetGameServerData(Guid id)
		{
			return new GameServerData
			{
				Id = id,
				Name = Name,
				AutoStart = AutoStart,
				WorkDirectory = WorkDirectory,
				Programm = Programm,
				Arguments = Arguments,
				Address = Address,
				Port = Port
			};
		}
	}
}
