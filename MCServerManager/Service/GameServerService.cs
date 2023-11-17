using MCServerManager.Library.Actions;
using MCServerManager.Library.Data.Interface;
using MCServerManager.Library.Data.Models;
using static MCServerManager.Library.Actions.Application;
using MCBackgroundService = MCServerManager.Library.Actions.BackgroundService;
using MCService = MCServerManager.Library.Data.Models.Service;


namespace MCServerManager.Service
{
	/// <summary>
	/// Сервис управления работой серверов.
	/// </summary>
	public class GameServerService
	{
		/// <summary>
		/// Список серверов.
		/// </summary>
		private List<GameServer> _servers = new();

		/// <summary>
		/// Список серверов.
		/// </summary>
		public IEnumerable<GameServer> Servers { get { return _servers; } }

		/// <summary>
		/// Конфигурация.
		/// </summary>
		private readonly IConfiguration _configuration;
		private readonly IGameServerDataContext _context;

		public GameServerService(IConfiguration configuration, IGameServerDataContext context)
		{
			_configuration = configuration;
			_context = context;

            LoadServers();
			AutoRun();
		}

		/// <summary>
		/// Заполняет список серверов
		/// </summary>
		private void LoadServers()
		{
			var list = _context.LoadServerDataAsycn().Result;

			foreach (var server in list)
			{
				try
				{
					AddServer(server);
				}
				catch (Exception ex)
				{
					Console.WriteLine(ex.ToString());
				}
			}
		}

		/// <summary>
		/// Запускает серверные приложения
		/// </summary>
		private async void AutoRun()
		{
			foreach (var server in _servers)
			{
                if (server.AutoStart)
                {
                    try
                    {
                        await RunAsync(server);
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine(ex.ToString());
                    }
                }
			}
		}

        private Task RunAsync(GameServer server)
        {
            var tcs = new TaskCompletionSource();
            server.FullServiceStarted += (id) =>
            {
                tcs.TrySetResult();
            };
            server.Closed += (id) =>
            {
                tcs.TrySetCanceled();
            };

            server.Start();

            return tcs.Task;
        }

		/// <summary>
		/// Создает новый сервер.
		/// </summary>
		/// <param name="name">Название.</param>
		/// <param name="autoStart">Автозапуск.</param>
		/// <param name="workDirectory">Расположение сервера.</param>
		/// <param name="program">Программа для запуска.</param>
		/// <param name="arguments">Аргументы запуска.</param>
		/// <param name="address">Адрес сервера.</param>
		/// <param name="port">Используемый порт.</param>
		/// <returns>Идентификатор сервера.</returns>
		public async Task<Guid> CreateServer(string name, bool autoStart, string workDirectory, string program,
			string? arguments, string? address, int? port)
		{
			var id = Guid.NewGuid();
			var server = new Server()
			{
				ServerId = id,
				Name = name,
				AutoStart = autoStart,
				WorkDirectory = workDirectory,
				StartProgram = program,
				Arguments = arguments,
				Address = address,
				Port = port,
				Services = new List<MCService>()
			};

            AddServer(server);
            await _context.CreateServerAsycn(server);

            return id;
		}

		/// <summary>
		/// Создает новый сервер.
		/// </summary>
		/// <param name="name">Название.</param>
		/// <param name="autoStart">Автозапуск.</param>
		/// <param name="workDirectory">Расположение сервера.</param>
		/// <param name="program">Программа для запуска.</param>
		/// <param name="arguments">Аргументы запуска.</param>
		/// <param name="address">Адрес сервера.</param>
		/// <param name="port">Используемый порт.</param>
		/// <returns>Идентификатор сервера.</returns>
		public async Task<Guid> CreateServiceAsync(Guid id, string name, bool autoStart, string workDirectory, string program,
			string? arguments, string? address, int? port)
		{
			var serviceId = Guid.NewGuid();
			var servise = new MCService()
			{
				ServiceId = serviceId,
				ServerId = id,
				Name = name,
				AutoStart = autoStart,
				WorkDirectory = workDirectory,
				StartProgram = program,
				Arguments = arguments,
				Address = address,
				Port = port
			};

            AddService(servise);
            await _context.CreateServiceAsycn(servise);

            return serviceId;
		}

        /// <summary>
        /// Добавить новый сервер.
        /// </summary>
        /// <param name="server">Информация о сервере.</param>
        /// <exception cref="Exception">Директория или порт используются другим сервером.</exception>
        private void AddServer(Server server)
        {
            CheckFreeDirectory(server.WorkDirectory);
            CheckFreePort(server.Port, server.Address, server.Id);

            _servers.Add(new GameServer(server, _configuration));
        }

        /// <summary>
        /// Добавить новый сервис.
        /// </summary>
        /// <param name="service">Информация о сервисе.</param>
        /// <exception cref="Exception">Директория или порт используются другим сервером или сервисом.</exception>
        private void AddService(MCService service)
        {
            CheckFreeDirectory(service.WorkDirectory);
            CheckFreePort(service.Port, service.Address, service.Id);

            var exemplar = GetServer(service.ServerId);
            exemplar.AddService(new MCBackgroundService(service, _configuration));
        }

        /// <summary>
        /// Удалить указанный сервер.
        /// </summary>
        /// <param name="id">Идентификатор сервера.</param>
        public async Task DeleteServerAsync(Guid id)
		{
			var exemplar = GetServer(id);
			
			if(exemplar.State != Status.Off && exemplar.State != Status.Error)
			{
				exemplar.Close();
				exemplar.CloseAllServices();
			}

			_servers.Remove(exemplar);
            await _context.DeleteServerAsycn(id);
		}

		/// <summary>
		/// Удалить указанный сервис.
		/// </summary>
		/// <param name="id">Идентификатор сервера.</param>
		/// <param name="serviceId">Идентификатор сервиса.</param>
		public async Task DeleteServiceAsync(Guid id, Guid serviceId)
		{
			GetServer(id).DeleteService(serviceId);
            await _context.DeleteServiceAsycn(serviceId);
		}

		/// <summary>
		/// Обновить информацию указанного сервера.
		/// </summary>
		/// <param name="id">Идентификатор сервера.</param>
		/// <param name="serverData">Информация о сервере.</param>
		public async Task UpdateServerAsync(Guid id, Server serverData)
		{
			CheckData(id, serverData, serverData.Port, serverData.Address);

			var exemplar = GetServer(id);
			exemplar.UpdateData(serverData);
            await _context.UpdateServerAsycn(serverData);
		}

		/// <summary>
		/// Обновить информацию указанного сервиса.
		/// </summary>
		/// <param name="id">Идентификатор сервиса.</param>
		/// <param name="serverData">Информация о сервисе.</param>
		/// <exception cref="ArgumentException">Идентификаторы не совпадают.</exception>
		public async Task UpdateServiceAsync(Guid id, MCService serviceData)
		{
			CheckData(id, serviceData, serviceData.Port, serviceData.Address);

			var exemplar = GetService(id);

			exemplar.UpdateData(serviceData);
            await _context.UpdateServiceAsycn(exemplar.Data);
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
		public void RestartServer(Guid id)
		{
			GetServer(id).Restart();
		}

		/// <summary>
		/// Запустить указанный сервис.
		/// </summary>
		/// <param name="id">Идентификатор сервиса.</param>
		public void StartService(Guid id)
		{
			GetService(id).Start();
		}

		/// <summary>
		/// Выключает указанный сервис.
		/// </summary>
		/// <param name="id">Идентификатор сервиса.</param>
		public void CloseService(Guid id)
		{
			GetService(id).Close();
		}

		/// <summary>
		/// Получить экземпляр класса сервера по идентификатору.
		/// </summary>
		/// <param name="id">Идентификатор сервера.</param>
		/// <returns>Экземпляр класса.</returns>
		/// <exception cref="Exception">Указанный сервер не найден.</exception>
		public GameServer GetServer(Guid id)
		{
			var exemplar = _servers.FirstOrDefault(x => x.ServerId == id);

			if (exemplar == null)
			{
				throw new Exception("Указанный сервер не найден");
			}

			return exemplar;
		}

		/// <summary>
		/// Получить экземпляр класса сервиса по идентификатору.
		/// </summary>
		/// <param name="serviceId">Идентификатор сервиса.</param>
		/// <returns>Экземпляр класса.</returns>
		/// <exception cref="Exception">Указанный сервис не найден.</exception>
		public MCBackgroundService GetService(Guid serviceId)
		{
			var exemplar = (from server in _servers
							from service in server.Services
							where service.ServiceId == serviceId
							select service).FirstOrDefault();

			if (exemplar == null)
			{
				throw new Exception("Указанный сервис не найден");
			}

			return exemplar;
		}

		/// <summary>
		/// Получить настройки серверного приложения по идентификатору.
		/// </summary>
		/// <param name="id">Идентификатор сервера.</param>
		/// <returns>Настройки серверного приложения.</returns>
		public Server GetServerData(Guid id)
		{
			return GetServer(id).Data;
		}

		/// <summary>
		/// Получить настройки сервисного приложения по идентификатору.
		/// </summary>
		/// <param name="serviceId">Идентификатор сервиса.</param>
		/// <returns>Настройки сервисного приложения.</returns>
		public MCService GetServiceData(Guid serviceId)
		{
			return GetService(serviceId).Data;
		}

		/// <summary>
		/// Отправляет сообщение в серверное приложение
		/// </summary>
		/// <param name="id">Идентификатор сервера.</param>
		/// <param name="text">Сообщение.</param>
		public void SendServerAppMessage(Guid id, string message = "")
		{
			GetServer(id).SendAppMessage(message);
		}

		/// <summary>
		/// Отправляет сообщение в сервис.
		/// </summary>
		/// <param name="id">Идентификатор сервиса.</param>
		/// <param name="text">Сообщение.</param>
		public void SendServiceAppMessage(Guid id, string message = "")
		{
			GetService(id).SendAppMessage(message);
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="id"></param>
		/// <param name="data"></param>
		/// <param name="Port"></param>
		/// <param name="Address"></param>
		/// <exception cref="ArgumentException">Идентификаторы не совпадают.</exception>
		private void CheckData(Guid id, IApplication data, int? Port, string Address)
		{
			if (id != data.Id)
			{
				throw new ArgumentException("Ошибка, идентификаторы не совпадают.");
			}

			CheckFreeDirectory(data.WorkDirectory, data.Id);
			CheckFreePort(Port, Address, data.Id);
		}


		/// <summary>
		/// Проверяет порта на использование другими приложениями.
		/// </summary>
		/// <param name="port">Порт.</param>
		/// <param name="id">Идентификатор приложения при обновлении данных.</param>
		/// <exception cref="Exception">Данный порт используется другим приложением.</exception>
		private void CheckFreePort(int? port, string address, Guid? id = null)
		{
			if (port != null)
			{
				if (_servers.Where(x => x.Port == port &&
										x.Address == address &&
										x.ServerId != id).Any())
				{
					throw new Exception($"Порт {port} занят другим сервером");
				}

				if (_servers.Where(x => x.Services.Where(y => y.Port == port &&
																y.Address == address &&
																y.ServiceId != id).Any()).Any())
				{
					throw new Exception($"Порт {port} занят другим сервисом");
				}
			}
		}

		/// <summary>
		/// Проверяет директорию на использование другими приложениями.
		/// </summary>
		/// <param name="directory">Директория.</param>
		/// <param name="id">Идентификатор приложения при обновлении данных.</param>
		/// <exception cref="Exception">Данная директория используется другим приложением.</exception>
		private void CheckFreeDirectory(string directory, Guid? id = null)
		{
			if (_servers.Where(x => x.WorkDirectory == directory && x.ServerId != id).Any())
			{
				throw new Exception($"Указанная директория используется другим сервером");
			}

			if (_servers.Where(x => x.Services.Where(y => y.WorkDirectory == directory && y.ServiceId != id).Any()).Any())
			{
				throw new Exception($"Указанная директория используется другим сервисом");
			}
		}
	}
}
