using Microsoft.AspNetCore.Identity;

namespace MCServerManager.Library.Data.Interface
{
	public interface IApplication
	{
		/// <summary>
		/// Идентификатор приложения.
		/// </summary>
		public Guid Id { get; }

		/// <summary>
		/// Название приложения
		/// </summary>
		public string Name { get; set; }

        /// <summary>
        /// Расположение приложения
        /// </summary>
        public string WorkDirectory { get; set; }

        /// <summary>
        /// Программа для запуска
        /// </summary>
        public string StartProgram { get; set; }

        /// <summary>
        /// Аргументы запуска
        /// </summary>
        public string Arguments { get; set; }

        /// <summary>
        /// Название приложения
        /// </summary>
        public bool AutoStart { get; set; }

        /// <summary>
        /// Идентификатор пользователя.
        /// </summary>
        public string UserId { get; set; }

        /// <summary>
        /// Позиция в рейтинге.
        /// </summary>
        public int RatingNumber { get; set; }
    }
}
