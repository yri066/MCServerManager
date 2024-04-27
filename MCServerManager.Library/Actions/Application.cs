using MCServerManager.Library.Data.Interface;
using MCServerManager.Library.Data.Models;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using System.Diagnostics;

namespace MCServerManager.Library.Actions
{
	/// <summary>
	/// Базовый класс для запуска приложения.
	/// </summary>
	public abstract class Application
	{
        public readonly object DataLock;
		/// <summary>
		/// Состояния приложения.
		/// </summary>
		public enum Status
		{
            Off,
            Launch,
            Run,
            Shutdown,
            Reboot,
            Error,
            Upgrade,
            Deleting
        }

		/// <summary>
		/// Информация о серверном приложении.
		/// </summary>
		[JsonIgnore]
		public IApplication Data { get; private set; }

		/// <summary>
		/// Автозапуск.
		/// </summary>
		public bool AutoStart { get { return Data.AutoStart; } }

		/// <summary>
		/// Название приложения.
		/// </summary>
		public string Name { get { return Data.Name; } }

		/// <summary>
		/// Расположение приложения.
		/// </summary>
		public string WorkDirectory { get { return Data.WorkDirectory; } }

		/// <summary>
		/// Программа для запуска.
		/// </summary>
		public string StartProgram { get { return Data.StartProgram; } }

		/// <summary>
		/// Аргументы запуска.
		/// </summary>
		public string Arguments { get { return Data.Arguments; } }

        /// <summary>
        /// Позиция в рейтинге.
        /// </summary>
        public int RatingNumber { get { return Data.RatingNumber; } }

        /// <summary>
        /// Процесс, управляющий серверным приложением.
        /// </summary>
        protected Process _process;

		/// <summary>
		/// Состояние сервера.
		/// </summary>
		[JsonIgnore]
		public Status State { get; protected set; }

		/// <summary>
		/// Буфер вывода консольного приложения.
		/// </summary>
		private ConsoleBufferApp _consoleBuffer { get; set; }

		/// <summary>
		/// Буфер вывода консольного приложения.
		/// </summary>
		[JsonIgnore]
		public IConsoleBufferApp ConsoleBuffer { get { return _consoleBuffer; } }

        /// <summary>
        /// Делегат события завершения работы серверного приложения.
        /// </summary>
        /// <param name="id">Идентификатор сервера.</param>
        public delegate void ServerClosedEventHandler(Guid id);

        /// <summary>
        /// Событие завершения работы серверного приложения.
        /// </summary>
        public event ServerClosedEventHandler Closed;

        /// <summary>
        /// Делегат события начала работы серверного приложения.
        /// </summary>
        /// <param name="id">Идентификатор сервера.</param>
        public delegate void ServerStartedEventHandler(Guid id);

        /// <summary>
        /// Событие начала работы серверного приложения.
        /// </summary>
        public event ServerStartedEventHandler Started;

        public Application(IApplication data, IConfiguration configuration)
		{
			CheckApplicationData(data);

            DataLock = this;
            _consoleBuffer = new(configuration);
			Data = data;
			State = Status.Off;
		}

		/// <summary>
		/// Обновляет настройки серверного приложения.
		/// </summary>
		/// <param name="data">Информация о серверном приложении.</param>
		public void UpdateData(IApplication data)
		{
			if (Data.Id != data.Id)
			{
				throw new Exception("Идентификаторы не совпадают");
			}

			CheckApplicationData(data);
			Data = data;
		}

        /// <summary>
        /// Запускает серверное приложение.
        /// </summary>
        /// <exception cref="InvalidOperationException"></exception>
        /// <exception cref="System.ComponentModel.Win32Exception"></exception>
        /// <exception cref="ObjectDisposedException"></exception>
        /// <exception cref="PlatformNotSupportedException"></exception>
        public void Start()
		{
			if (State != Status.Off)
			{
				return;
			}

			StartServer(new EventHandler((sender, e) =>
			{
				ProcessClosed();
			}),
            new DataReceivedEventHandler((sender, e) =>
            {
                GetAppMessage(e.Data);
            })
			);

			State = Status.Run;
            Started?.Invoke(Data.Id);
        }

        /// <summary>
        /// Запускает процесс серверного приложения.
        /// </summary>
        /// <param name="eventExited">Обработчик событий, обрабатывающий завершение процесса.</param>
        /// <param name="eventOutputDataReceived">Обработчик событий, обрабатывающий текстовый вывод из приложения.</param>
        /// <exception cref="InvalidOperationException"></exception>
        /// <exception cref="System.ComponentModel.Win32Exception"></exception>
        /// <exception cref="ObjectDisposedException"></exception>
        /// <exception cref="PlatformNotSupportedException"></exception>
        protected void StartServer(EventHandler eventExited = null, DataReceivedEventHandler eventOutputDataReceived = null)
		{
            _process = new Process();
			_process.StartInfo.WorkingDirectory = WorkDirectory;
            _process.StartInfo.FileName = StartProgram;
			_process.StartInfo.Arguments = Arguments;
			_process.StartInfo.UseShellExecute = false;
			_process.StartInfo.RedirectStandardInput = true;
			_process.StartInfo.RedirectStandardOutput = true;
			_process.EnableRaisingEvents = true;

            _process.OutputDataReceived += eventOutputDataReceived;
			_process.Exited += eventExited;

			_process.Start();
			_process.BeginOutputReadLine();
		}

		/// <summary>
		/// Очищает ресурсы процесса после завершения работы.
		/// </summary>
		protected void ProcessClosed()
		{
			_process.Dispose();

			State = Status.Off;
            Closed?.Invoke(Data.Id);
        }

		/// <summary>
		/// Отключает серверное приложение не дожидаясь завершения работы.
		/// </summary>
		public void Close()
		{
			if (State == Status.Off || State == Status.Upgrade || State == Status.Deleting)
			{
				return;
			}

			_process.Kill();
		}

		/// <summary>
		/// Выводит сообщение от серверного приложения.
		/// </summary>
		/// <param name="message">Текст сообщения.</param>
		protected virtual void GetAppMessage(string message = "")
		{
			_consoleBuffer.Add(message);
		}

		/// <summary>
		/// Отправляет команду в серверное приложение.
		/// </summary>
		/// <param name="message">Команда для серверного приложения.</param>
		public virtual void SendAppMessage(string message = "")
		{
			if (State != Status.Run)
			{
				return;
			}

			_consoleBuffer.Add(message);
			_process.StandardInput.WriteLine(message);
		}

        /// <summary>
        /// Изменение номера рейтинга.
        /// </summary>
        /// <param name="rating">Рейтинг.</param>
        public void UpdateRateNumber(int rating)
        {
            Data.RatingNumber = rating;
        }

        /// <summary>
        /// Проверяет данные серверного приложения.
        /// </summary>
        /// <param name="data">Информация о серверном приложении.</param>
        public void CheckApplicationData(IApplication data)
		{
			if (string.IsNullOrEmpty(data.StartProgram))
			{
				throw new ArgumentNullException(nameof(data.StartProgram), "Программа для запуска не задана");
			}

			if (string.IsNullOrEmpty(data.WorkDirectory))
			{
				throw new ArgumentNullException(nameof(data.WorkDirectory), "Директория не задана.");
			}

			if (!Directory.Exists(data.WorkDirectory))
			{
				throw new DirectoryNotFoundException($"Указанная директория не найдена: {data.WorkDirectory}");
			}
		}
	}
}
