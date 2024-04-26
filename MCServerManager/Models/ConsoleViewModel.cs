namespace MCServerManager.Models
{
    public class ConsoleViewModel
    {
        // <summary>
        /// Версия буфера.
        /// </summary>
        public Guid Version { get; }
        /// <summary>
        /// Буфер вывода консольного приложения.
        /// </summary>
        public IEnumerable<string> ConsoleText { get; }

        public ConsoleViewModel(Guid version, IEnumerable<string> consoleText)
        {
            Version = version;
            ConsoleText = consoleText;
        }
    }
}
