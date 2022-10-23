namespace MCServerManager.Library.Data.Model
{
	/// <summary>
	/// Состояние сервера
	/// </summary>
	public class GameServerStatus
	{
		public enum Status
		{
			Launch,
			Run,
			Shutdown,
			Off,
			Reboot,
			Error
		}
	}
}
