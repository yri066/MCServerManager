namespace MCServerManager.Library.Data.Interface
{
	public interface IApplication
	{
		/// <summary>
		/// Идентификатор приложения
		/// </summary>
		public Guid Id { get; }

		/// <summary>
		/// Название приложения
		/// </summary>
		public string Name { get; }

		/// <summary>
		/// Расположение приложения
		/// </summary>
		public string WorkDirectory { get; }

		/// <summary>
		/// Программа для запуска
		/// </summary>
		public string StartProgram { get; }

		/// <summary>
		/// Аргументы запуска
		/// </summary>
		public string Arguments { get; }

        /// <summary>
        /// Название приложения
        /// </summary>
        public bool AutoStart { get; }
    }
}
