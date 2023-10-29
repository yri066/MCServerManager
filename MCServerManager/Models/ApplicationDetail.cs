using MCServerManager.Data.ValidationAttributes;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel;

namespace MCServerManager.Models
{
	/// <summary>
	/// Данные о приложении
	/// </summary>
	public class ApplicationDetail
	{
		/// <summary>
		/// Название приложения
		/// </summary>
		[Required, StringLength(100), DisplayName("Название приложения:")]
		public string Name { get; set; }

		/// <summary>
		/// Автозапуск
		/// </summary>
		[Required, DisplayName("Автозапуск.")]
		public bool AutoStart { get; set; }

		/// <summary>
		/// Расположение приложения
		/// </summary>
		[Required, DirectoryExists, DisplayName("Директория расположения сервера:")]
		public string WorkDirectory { get; set; }

		/// <summary>
		/// Программа для запуска
		/// </summary>
		[Required, StringLength(100), DisplayName("Программа для запуска:")]
		public string StartProgram { get; set; }

		/// <summary>
		/// Аргументы запуска
		/// </summary>
		[StringLength(200), DisplayName("Аргументы для запуска.")]
		public string? Arguments { get; set; }
	}
}
