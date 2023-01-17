namespace MCServerManager.Library.Data.Models
{
	/// <summary>
	/// Данные о сервере
	/// </summary>
	public class ServerData : ApplicationData
	{
		/// <summary>
		/// Адрес сервера(ip)
		/// </summary>
		public string Address { get; set; }

		/// <summary>
		/// Используемый порт
		/// </summary>
		public int? Port { get; set; }

		public void UpdateServerData(GameServerData server)
		{
			server.Name = Name;
			server.AutoStart = AutoStart;
			server.WorkDirectory= WorkDirectory;
			server.Program = Program;
			server.Arguments= Arguments;
			server.Address = Address;
			server.Port = Port;
		}
	}
}
