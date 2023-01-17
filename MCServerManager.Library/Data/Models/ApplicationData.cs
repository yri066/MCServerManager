namespace MCServerManager.Library.Data.Models
{
	/// <summary>
	/// Данные о приложении
	/// </summary>
	public class ApplicationData
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
		public string Program { get; set; }

		/// <summary>
		/// Аргументы запуска
		/// </summary>
		public string Arguments { get; set; }
	}
}
