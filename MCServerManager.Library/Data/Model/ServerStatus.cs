namespace MCServerManager.Library.Data.Model
{
	/// <summary>
	/// Состояние сервера
	/// </summary>
	public class ServerStatus
	{
		public enum Status
		{
			Launch,
			Run,
			Stopping,
			Off,
			Error
		}
	}
}
