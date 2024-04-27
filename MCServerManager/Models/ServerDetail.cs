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
		[StringLength(100), DisplayName("Адрес сервера:")]
		public string? Address { get; set; }

		/// <summary>
		/// Используемый порт
		/// </summary>
		[DisplayName("Используемый порт:")]
		public int? Port { get; set; }

		public Server GetServerData()
		{
			return new Server
			{
				Name = Name,
				AutoStart = AutoStart,
				WorkDirectory = WorkDirectory,
                StartProgram = StartProgram,
				Arguments = Arguments,
				Address = Address,
				Port = Port
			};
		}

        public Server GetServerData(Server server)
        {
            var exemplar = GetServerData();
            exemplar.ServerId = server.ServerId;
            exemplar.RatingNumber = server.RatingNumber;
            return exemplar;
        }

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
