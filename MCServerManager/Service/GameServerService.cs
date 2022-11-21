﻿using MCServerManager.Library.Actions;
using MCServerManager.Library.Data.Model;
using MCServerManager.Library.Data.Tools;
using System.Net;

namespace MCServerManager.Service
{
	/// <summary>
	/// Сервис управляния работой серверов.
	/// </summary>
	public class GameServerService
	{
		/// <summary>
		/// Ключ названия файла настроек.
		/// </summary>
		private const string _keyGetFileSettings = "GameServers";

		/// <summary>
		/// Путь к файлу с информацией о настройках серверов.
		/// </summary>
		private readonly string _pathFileSettings;

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
			foreach (var server in _servers)
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
				Address = addres,
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
				Address = addres,
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
			CheckFreeDirectory(serverData.WorkDirectory);
			CheckFreePort(serverData.Port, serverData.Address, serverData.Id);

			_servers.Add(new GameServer(serverData));
		}

		/// <summary>
		/// Добавить новый сервис.
		/// </summary>
		/// <param name="serviceData">Информация о сервисе.</param>
		/// <exception cref="Exception">Директория или порт используются другим сервером или сервисом.</exception>
		private void AddService(BackgroundServiceData serviceData)
		{
			/// TODO Сделать проверку во всех сервисах, а не только в серверах.

			CheckFreeDirectory(serviceData.WorkDirectory);
			CheckFreePort(serviceData.Port, serviceData.Address, serviceData.Id);

			var exemplar = GetServer(serviceData.GameServerId);
			exemplar.AddService(new Library.Actions.BackgroundService(serviceData));
		}

		/// <summary>
		/// Удалить указанный сервер.
		/// </summary>
		/// <param name="id">Идентификатор сервера.</param>
		public void DeleteServer(Guid id)
		{
			var exemplar = GetServer(id);
			
			if(exemplar.State != GameServer.Status.Off && exemplar.State != GameServer.Status.Error)
			{
				exemplar.Close();
				exemplar.CloseAllServices();
			}

			_servers.Remove(exemplar);
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
		/// <exception cref="ArgumentException">Идентификаторы не совпадают.</exception>
		public void UpdateServer(Guid id, GameServerData serverData)
		{
			if (id != serverData.Id)
			{
				throw new ArgumentException("Ошибка, идентификатор настроек не совпадает с идентификатором изменяемого сервера.");
			}

			CheckFreeDirectory(serverData.WorkDirectory, serverData.Id);
			CheckFreePort(serverData.Port, serverData.Address, serverData.Id);

			var exemplar = GetServer(id);
			exemplar.UpdateData(serverData);
			SaveServerData();
		}

		/// <summary>
		/// Обновить информацию указанного сервиса.
		/// </summary>
		/// <param name="id">Идентификатор сервиса.</param>
		/// <param name="serverData">Информация о сервисе.</param>
		/// <exception cref="ArgumentException">Идентификаторы не совпадают.</exception>
		public void UpdateService(Guid id, BackgroundServiceData serviceData)
		{
			if (id != serviceData.Id)
			{
				throw new ArgumentException("Ошибка, идентификатор настроек не совпадает с идентификатором изменяемого сервиса.");
			}

			CheckFreeDirectory(serviceData.WorkDirectory, serviceData.Id);
			CheckFreePort(serviceData.Port, serviceData.Address, serviceData.Id);

			var exemplar = GetService(id);

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
			JsonTool.SaveJsonDataToFile(_pathFileSettings, _servers);
		}

		/// <summary>
		/// Получить экземпляе класса сервера по идентификатору.
		/// </summary>
		/// <param name="id">Идентификатор сервера.</param>
		/// <returns>Экземпляе класса.</returns>
		/// <exception cref="Exception">Указанный сервер не найден.</exception>
		public GameServer GetServer(Guid id)
		{
			var exemplar = _servers.FirstOrDefault(x => x.Id == id);

			if (exemplar == null)
			{
				throw new Exception("Указанный сервер не найден");
			}

			return exemplar;
		}

		/// <summary>
		/// Получить экземпляе класса сервиса по идентификатору.
		/// </summary>
		/// <param name="serviceId">Идентификатор сервиса.</param>
		/// <returns>Экземпляе класса.</returns>
		/// <exception cref="Exception">Указанный сервис не найден.</exception>
		public Library.Actions.BackgroundService GetService(Guid serviceId)
		{
			var exemplar = (from server in _servers
							from service in server.Services
							where service.Id == serviceId
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
		public GameServerData GetServerData(Guid id)
		{
			return GetServer(id).Data;
		}

		/// <summary>
		/// Получить настройки сервисного приложения по идентификатору.
		/// </summary>
		/// <param name="serviceId">Идентификатор сервиса.</param>
		/// <returns>Настройки сервисного приложения.</returns>
		public BackgroundServiceData GetServiceData(Guid serviceId)
		{
			return GetService(serviceId).Data;
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

		/// <summary>
		/// Проверяет порта на использование другими приложениями.
		/// </summary>
		/// <param name="port">Порт.</param>
		/// <param name="id">Идентификатор приложения при обнорлении данных.</param>
		/// <exception cref="Exception">Данный порт используется другим приложением.</exception>
		public void CheckFreePort(int? port, string address, Guid? id = null)
		{
			if (port != null)
			{
				if (_servers.Where(x => x.Port == port &&
										x.Address == address &&
										x.Id != id).Any())
				{
					throw new Exception($"Порт {port} занят другим сервером");
				}

				if (_servers.Where(x => x.Services.Where(y => y.Port == port &&
																y.Address == address &&
																y.Id != id).Any()).Any())
				{
					throw new Exception($"Порт {port} занят другим сервисом");
				}
			}
		}

		/// <summary>
		/// Проверяет директорию на использование другими приложениями.
		/// </summary>
		/// <param name="directory">Директория.</param>
		/// <param name="id">Идентификатор приложения при обнорлении данных.</param>
		/// <exception cref="Exception">Данная директория используется другим приложением.</exception>
		public void CheckFreeDirectory(string directory, Guid? id = null)
		{
			if (_servers.Where(x => x.WorkDirectory == directory && x.Id != id).Any())
			{
				throw new Exception($"Указанная директория используется другим сервером");
			}

			if (_servers.Where(x => x.Services.Where(y => y.WorkDirectory == directory && y.Id != id).Any()).Any())
			{
				throw new Exception($"Указанная директория используется другим сервисом");
			}
		}
	}
}
