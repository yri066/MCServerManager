using MCServerManager.Library.Actions;
using MCServerManager.Library.Data.Model;
using MCServerManager.Library.Data.Tools;

namespace MCServerManager.Service
{
	/// <summary>
	/// Сервис управляния работой серверов.
	/// </summary>
	public class ServerService
	{
		/// <summary>
		/// Название файла настроек.
		/// </summary>
		private const string _keyGetFileSettings = "GameServers";

		/// <summary>
		/// Путь к файлу с информацией о настройках серверов.
		/// </summary>
		private readonly string _pathFileSettings;

		/// <summary>
		/// Список серверов.
		/// </summary>
		public List<GameServer> Servers { get; private set; } = new();

		/// <summary>
		/// Конфигурация.
		/// </summary>
		private readonly IConfiguration _configuration;

		public ServerService(IConfiguration configuration)
		{
			_configuration = configuration;
			_pathFileSettings = _configuration.GetValue<string>(_keyGetFileSettings);

			LoadServers();
			AutoRun();
		}

		/// <summary>
		/// Заполняет список серверов
		/// </summary>
		private void LoadServers()
		{
			var list = LoadServerData();

			foreach (var server in list)
			{
				try
				{
					AddServer(server);
				}
				catch (Exception ex)
				{
					Console.WriteLine(ex.Message);
				}
			}
		}

		/// <summary>
		/// Запускает серверные приложения
		/// </summary>
		private void AutoRun()
		{
			foreach (var server in Servers)
			{
				if (server.AutoStart)
				{
					server.Start();
				}
			}
		}

		/// <summary>
		/// Создает новый сервер.
		/// </summary>
		/// <param name="name">Название.</param>
		/// <param name="autoStart">Автозапуск.</param>
		/// <param name="workDirectory">Расположение сервера.</param>
		/// <param name="programm">Программа для запуска.</param>
		/// <param name="arguments">Аргументы запуска.</param>
		/// <param name="addres">Адрес сервера.</param>
		/// <param name="port">Используемый порт.</param>
		/// <returns>Идентификатор сервера.</returns>
		public Guid CreateServer(string name, bool autoStart, string workDirectory, string programm,
			string arguments, string addres, int? port)
		{
			var id = Guid.NewGuid();
			AddServer(new ServerData()
			{
				Id = id,
				Name = name,
				AutoStart = autoStart,
				WorkDirectory = workDirectory,
				Programm = programm,
				Arguments = arguments,
				Addres = addres,
				Port = port
			});

			SaveServerData();

			return id;
		}

		/// <summary>
		/// Добавить новый сервер.
		/// </summary>
		/// <param name="serverData">Информация о сервере.</param>
		/// <exception cref="Exception">Директория или порт используются другим сервером.</exception>
		private void AddServer(ServerData serverData)
		{
			if (Servers.Where(x => x.Port == serverData.Port).Any())
			{
				throw new Exception($"Порт {serverData.Port} занят другим сервером");
			}

			if (Servers.Where(x => x.WorkDirectory == serverData.WorkDirectory).Any())
			{
				throw new Exception($"Указанная директория используется другим сервером");
			}

			Servers.Add(new GameServer(serverData));
		}

		/// <summary>
		/// Удалить указанный сервер.
		/// </summary>
		/// <param name="id">Идентификатор сервера.</param>
		public void DeleteServer(Guid id)
		{
			var exemplar = GetServer(id);
			
			if(exemplar.State != ServerStatus.Status.Off && exemplar.State != ServerStatus.Status.Error)
			{
				exemplar.Close();
			}

			Servers.Remove(exemplar);
			SaveServerData();
		}

		/// <summary>
		/// Обновить информацию указанного сервера.
		/// </summary>
		/// <param name="id">Идентификатор сервера.</param>
		/// <param name="serverData">Информация о сервере.</param>
		/// <exception cref="ArgumentException">Идентификаторы id и serverData.Id не совпадают.</exception>
		public void UpdateServer(Guid id, ServerData serverData)
		{
			var exemplar = GetServer(id);

			if (id != serverData.Id)
			{
				throw new ArgumentException("Ошибка, идентификатор настроек не совпадает с идентификатором изменяемого сервера.");
			}

			exemplar.UpdateData(serverData);
			SaveServerData();
		}

		/// <summary>
		/// Запустить указанный сервер.
		/// </summary>
		/// <param name="id">Идентификатор сервера.</param>
		public void StartServer(Guid id)
		{
			GetServer(id).Start();
		}

		/// <summary>
		/// Остановить указанный сервер.
		/// </summary>
		/// <param name="id">Идентификатор сервера.</param>
		public void StopServer(Guid id)
		{
			GetServer(id).Stop();
		}

		/// <summary>
		/// Выключает указанный сервер, не дожидаясь сохранения данных.
		/// </summary>
		/// <param name="id">Идентификатор сервера.</param>
		public void CloseServer(Guid id)
		{
			GetServer(id).Close();
		}

		/// <summary>
		/// Перезагружает указанный сервер.
		/// </summary>
		/// <param name="id">Идентификатор сервера.</param>
		public void Restart(Guid id)
		{
			GetServer(id).Restart();
		}

		/// <summary>
		/// Загружает информацию о серверах.
		/// </summary>
		/// <returns>Список данных о серверах.</returns>
		private List<ServerData> LoadServerData()
		{
			return JsonTool.LoadJsonDataFromFile<List<ServerData>>(_pathFileSettings);
		}

		/// <summary>
		/// Сохраняет информацию о серверах.
		/// </summary>
		private void SaveServerData()
		{
			JsonTool.SaveJsonDataToFile(_pathFileSettings, Servers);
		}

		/// <summary>
		/// Получить экземпляе класса сервера по идентификатору.
		/// </summary>
		/// <param name="id">Идентификатор сервера.</param>
		/// <returns>Экземпляе класса</returns>
		/// <exception cref="Exception">Указанный сервер не найден.</exception>
		public GameServer GetServer(Guid id)
		{
			var exemplar = Servers.Where(x => x.Id == id).FirstOrDefault();

			if (exemplar == null)
			{
				throw new Exception("Указанный сервер не найден");
			}

			return exemplar;
		}

		/// <summary>
		/// Получить настройки серверного приложения по идентификатору.
		/// </summary>
		/// <param name="id">Идентификатор сервера.</param>
		/// <returns>Настройки серверного приложения.</returns>
		public ServerData GetServerData(Guid id)
		{
			return GetServer(id).ServerData;
		}

		/// <summary>
		/// Отправляет сообщение в серверное приложение
		/// </summary>
		/// <param name="id">Идентификатор сервера.</param>
		/// <param name="text">Сообщение.</param>
		public void SetServerCommant(Guid id, string message)
		{
			GetServer(id).SendServerCommand(message);
		}
	}
}
