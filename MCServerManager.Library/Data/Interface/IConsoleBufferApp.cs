namespace MCServerManager.Library.Data.Interface
{
    /// <summary>
    /// Буфер вывода консольного приложения.
    /// </summary>
    public interface IConsoleBufferApp
    {
        /// <summary>
        /// Версия буфера.
        /// </summary>
        public Guid Version { get; }
        /// <summary>
        /// Буфер вывода консольного приложения.
        /// </summary>
        public IEnumerable<string> ConsoleBuffer { get; }

        /// <summary>
        /// Получить новые сообщения из буфера.
        /// </summary>
        /// <param name="version">Версия буфера.</param>
        /// <returns>Новые сообщения из буфера консольного приложения.</returns>
        public IEnumerable<string> GetConsoleBuffer(Guid version);
    }
}
