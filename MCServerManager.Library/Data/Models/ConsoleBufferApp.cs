using Microsoft.Extensions.Configuration;

namespace MCServerManager.Library.Data.Models
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

	/// <summary>
	/// Хранение буфера вывода консольного приложения.
	/// </summary>
	public class ConsoleBufferApp : IConsoleBufferApp
	{
		/// <summary>
		/// Ключ количества элементов в буфере.
		/// </summary>
		private string _keyGetConsoleCountItems = "ConsoleCapacity";

		/// <summary>
		/// Количество элементов в буфере.
		/// </summary>
		private int _consoleCountItems { get; set; } = 100;

		/// <summary>
		/// Список идентификаторов сообщений буфера.
		/// </summary>
		private List<Guid> _consoleMap = new();

		/// <summary>
		/// Буфер вывода консольного приложения.
		/// </summary>
		private List<string> _console = new();

		/// <summary>
		/// Версия буфера.
		/// </summary>
		public Guid Version { get; private set; } = Guid.NewGuid();

		/// <summary>
		/// Буфер вывода консольного приложения.
		/// </summary>
		public IEnumerable<string> ConsoleBuffer { get { return _console; } }

		public ConsoleBufferApp(IConfiguration configuration)
		{
			if (!string.IsNullOrEmpty(configuration[_keyGetConsoleCountItems]))
			{
				var count = configuration.GetValue<int>(_keyGetConsoleCountItems);

				_consoleCountItems = count <= 0
									? 100
									: count;
			}

			_console.Capacity = _consoleCountItems;
			_consoleMap.Capacity = _consoleCountItems;
		}

		/// <summary>
		/// Добавляет текст в буфер.
		/// </summary>
		/// <param name="text">Текст.</param>
		public void Add(string text)
		{
			if (_console.Count == _consoleCountItems)
			{
				const int indexFirstItem = 0;
				_consoleMap.RemoveAt(indexFirstItem);
				_console.RemoveAt(indexFirstItem);
			}

			Version = Guid.NewGuid();
			_consoleMap.Add(Version);
			_console.Add(text);
		}

		/// <summary>
		/// Получить новые сообщения из буфера.
		/// </summary>
		/// <param name="version">Версия буфера.</param>
		/// <returns>Новые сообщения из буфера консольного приложения.</returns>
		public IEnumerable<string> GetConsoleBuffer(Guid version)
		{
			if (!_consoleMap.Contains(version))
			{
				return _console;
			}

			var index = _consoleMap.IndexOf(version) + 1;

			if (index > _console.Count)
			{
				return new List<string>();
			}

			return _console.GetRange(index, _console.Count - index);
		}

		/// <summary>
		/// Очищает буфер.
		/// </summary>
		public void Clear()
		{
			_consoleMap.Clear();
			_console.Clear();
			Version = Guid.NewGuid();
		}
	}
}
