using MCServerManager.Library.Data.Model;
using Newtonsoft.Json;
using System.Diagnostics;

namespace MCServerManager.Library.Actions
{
	/// <summary>
	/// Работа с приложением.
	/// </summary>
	public class Application
	{
		/// <summary>
		/// Состояния приложения.
		/// </summary>
		public enum Status
		{
			Run,
			Off
		}

		/// <summary>
		/// Информация о серверном приложении.
		/// </summary>
		[JsonIgnore]
		public ApplicationData Data { get; private set; }

		/// <summary>
		/// Идентификатор приложения.
		/// </summary>
		public Guid Id { get { return Data.Id; } }

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
		public string Programm { get { return Data.Programm; } }

		/// <summary>
		/// Аргументы запуска.
		/// </summary>
		public string Arguments { get { return Data.Arguments; } }

		/// <summary>
		/// Процесс, управляющий серверным приложением.
		/// </summary>
		protected Process _process;

		/// <summary>
		/// Состояние сервера.
		/// </summary>
		[JsonIgnore]
		public virtual Status State { get; private set; }

		public Application(ApplicationData data)
		{
			CheckApplicationData(data);

			Data = data;
			State = Status.Off;
		}

		/// <summary>
		/// Обновляет настройки серверного приложения.
		/// </summary>
		/// <param name="data">Информания о серверном приложении.</param>
		public void UpdateData(ApplicationData data)
		{
			if (Id != data.Id)
			{
				throw new Exception("Идентификаторы не совпадают");
			}

			CheckApplicationData(data);
			Data = data;
		}

		/// <summary>
		/// Запускает серверное приложение.
		/// </summary>
		public void Start()
		{
			if (State != Status.Off)
			{
				return;
			}

			StartServer(new EventHandler((sender, e) =>
			{
				ProcessClosed();
			})
			);

			State = Status.Run;
		}

		/// <summary>
		/// Запускает процесс, управляющий серверным приложением.
		/// </summary>
		protected void StartServer(EventHandler @event = null)
		{
			_process = new Process();
			_process.StartInfo.WorkingDirectory = WorkDirectory;
			_process.StartInfo.FileName = Programm;
			_process.StartInfo.Arguments = Arguments;
			_process.StartInfo.UseShellExecute = false;
			_process.StartInfo.RedirectStandardInput = true;
			_process.StartInfo.RedirectStandardOutput = true;
			_process.EnableRaisingEvents = true;

			_process.OutputDataReceived += new DataReceivedEventHandler((sender, e) =>
			{
				GetServerMessage(e.Data);
			});

			_process.Exited += @event;

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
		}

		/// <summary>
		/// Отключает серверное приложение не дожидаясь завершения работы.
		/// </summary>
		public void Close()
		{
			if (State == Status.Off)
			{
				return;
			}

			_process.Kill();
		}

		/// <summary>
		/// Выводит сообщение от серверного приложения.
		/// </summary>
		/// <param name="message">Текст сообщения.</param>
		protected virtual void GetServerMessage(string message)
		{
			if (string.IsNullOrEmpty(message))
			{
				message = "";
			}

			Console.WriteLine(message);
		}

		/// <summary>
		/// Отправляет команду в серверное приложение.
		/// </summary>
		/// <param name="message">Команда для серверного приложения.</param>
		public virtual void SendServerCommand(string message)
		{
			if (string.IsNullOrEmpty(message))
			{
				message = "";
			}

			Console.WriteLine(message);
			_process.StandardInput.WriteLine(message);
		}

		/// <summary>
		/// Проверяет данные серверного приложения.
		/// </summary>
		/// <param name="data">Информания о серверном приложении.</param>
		public void CheckApplicationData(ApplicationData data)
		{
			if (string.IsNullOrEmpty(data.Programm))
			{
				throw new ArgumentNullException(nameof(data.Programm), "Программа для запуска не задана");
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
