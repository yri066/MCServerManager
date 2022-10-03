namespace MCServerManager.Library.Data.Model
{
	/// <summary>
	/// Данные о приложении
	/// </summary>
	public class ServerData
	{
		/// <summary>
		/// Идентификатор приложения
		/// </summary>
		public Guid Id { get; set; }

		/// <summary>
		/// Название приложения
		/// </summary>
		public string Name { get; set; }

		/// <summary>
		/// Автозапуск
		/// </summary>
		public bool AutoStart { get; set; }

		/// <summary>
		/// Расположение приложения
		/// </summary>
		public string WorkDirectory { get; set; }

		/// <summary>
		/// Программа для запуска
		/// </summary>
		public string Programm { get; set; }

		/// <summary>
		/// Аргументы запуска
		/// </summary>
		public string Arguments { get; set; }

		/// <summary>
		/// Адрес сервера(ip)
		/// </summary>
		public string Addres { get; set; }

		/// <summary>
		/// Используемый порт
		/// </summary>
		public int? Port { get; set; }
	}
}
