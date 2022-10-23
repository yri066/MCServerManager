using MCServerManager.Library.Actions;
using MCServerManager.Library.Data.Model;
using MCServerManager.Library.Data.Tools;

namespace MCServerManager.Service
{
	/// <summary>
	/// Сервис управляния работой серверов.
	/// </summary>
	public class GameServerService
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

		public GameServerService(IConfiguration configuration)
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
			AddServer(new GameServerData()
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
		public Guid CreateService(Guid id, string name, bool autoStart, string workDirectory, string programm,
			string arguments, string addres, int? port)
		{
			var serviceId = Guid.NewGuid();
			AddService(new BackgroundServiceData()
			{
				Id = serviceId,
				GameServerId = id,
				Name = name,
				AutoStart = autoStart,
				WorkDirectory = workDirectory,
				Programm = programm,
				Arguments = arguments,
				Addres = addres,
				Port = port
			});

			SaveServerData();

			return serviceId;
		}

		/// <summary>
		/// Добавить новый сервер.
		/// </summary>
		/// <param name="serverData">Информация о сервере.</param>
		/// <exception cref="Exception">Директория или порт используются другим сервером.</exception>
		private void AddServer(GameServerData serverData)
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
		/// Добавить новый сервис.
		/// </summary>
		/// <param name="serviceData">Информация о сервисе.</param>
		/// <exception cref="Exception">Директория или порт используются другим сервером или сервисом.</exception>
		private void AddService(BackgroundServiceData serviceData)
		{
			/// TODO Сделать проверку во всех сервисах, а не только в серверах.
			
			if (Servers.Where(x => x.Port == serviceData.Port).Any())
			{
				throw new Exception($"Порт {serviceData.Port} занят другим сервером");
			}

			if (Servers.Where(x => x.WorkDirectory == serviceData.WorkDirectory).Any())
			{
				throw new Exception($"Указанная директория используется другим сервером");
			}

			var exemplar = GetServer(serviceData.GameServerId);
			exemplar.Services.Add(new Library.Actions.BackgroundService(serviceData));
		}

		/// <summary>
		/// Удалить указанный сервер.
		/// </summary>
		/// <param name="id">Идентификатор сервера.</param>
		public void DeleteServer(Guid id)
		{
			var exemplar = GetServer(id);
			
			if(exemplar.State != GameServerStatus.Status.Off && exemplar.State != GameServerStatus.Status.Error)
			{
				exemplar.Close();
			}

			Servers.Remove(exemplar);
			SaveServerData();
		}

		/// <summary>
		/// Удалить указанный сервис.
		/// </summary>
		/// <param name="id">Идентификатор сервера.</param>
		/// <param name="seriveId">Идентификатор сервиса.</param>
		public void DeleteService(Guid id, Guid seriveId)
		{
			GetServer(id).DeleteService(seriveId);
			SaveServerData();
		}

		/// <summary>
		/// Обновить информацию указанного сервера.
		/// </summary>
		/// <param name="id">Идентификатор сервера.</param>
		/// <param name="serverData">Информация о сервере.</param>
		/// <exception cref="ArgumentException">Идентификаторы id и serverData.Id не совпадают.</exception>
		public void UpdateServer(Guid id, GameServerData serverData)
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
		/// Обновить информацию указанного сервера.
		/// </summary>
		/// <param name="id">Идентификатор сервера.</param>
		/// <param name="serverData">Информация о сервере.</param>
		/// <exception cref="ArgumentException">Идентификаторы id и serverData.Id не совпадают.</exception>
		public void UpdateService(Guid id, BackgroundServiceData serviceData)
		{
			var exemplar = GetService(id, serviceData.Id);

			if (id != serviceData.GameServerId)
			{
				throw new ArgumentException("Ошибка, идентификатор сервиса не совпадает с идентификатором сервера.");
			}

			exemplar.UpdateData(serviceData);
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
		private List<GameServerData> LoadServerData()
		{
			return JsonTool.LoadJsonDataFromFile<List<GameServerData>>(_pathFileSettings);
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
			var exemplar = Servers.FirstOrDefault(x => x.Id == id);

			if (exemplar == null)
			{
				throw new Exception("Указанный сервер не найден");
			}

			return exemplar;
		}

		public Library.Actions.BackgroundService GetService(Guid id, Guid serviceId)
		{
			var service = GetServer(id).Services.FirstOrDefault(x =>x.Id == serviceId);

			if(service == null)
			{
				throw new Exception("Указанный сервис не найден");
			}

			return service;
		}

		public BackgroundServiceData GetServiceData(Guid id, Guid serviceId)
		{
			return GetService(id, serviceId).Data;
		}

		/// <summary>
		/// Получить настройки серверного приложения по идентификатору.
		/// </summary>
		/// <param name="id">Идентификатор сервера.</param>
		/// <returns>Настройки серверного приложения.</returns>
		public GameServerData GetServerData(Guid id)
		{
			return GetServer(id).Data;
		}

		/// <summary>
		/// Отправляет сообщение в серверное приложение
		/// </summary>
		/// <param name="id">Идентификатор сервера.</param>
		/// <param name="text">Сообщение.</param>
		public void SendServerCommand(Guid id, string message)
		{
			GetServer(id).SendServerCommand(message);
		}
	}
}
