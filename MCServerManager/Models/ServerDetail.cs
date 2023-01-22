using System.ComponentModel.DataAnnotations;
using System.ComponentModel;
using MCServerManager.Library.Data.Models;

namespace MCServerManager.Models
{
	/// <summary>
	/// Данные о сервере
	/// </summary>
	public class ServerDetail : ApplicationDetail
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

		public Server GetServerData(Guid id)
		{
			return new Server
			{
                ServerId = id,
				Name = Name,
				AutoStart = AutoStart,
				WorkDirectory = WorkDirectory,
                StartProgram = StartProgram,
				Arguments = Arguments,
				Address = Address,
				Port = Port
			};
		}
	}
}
