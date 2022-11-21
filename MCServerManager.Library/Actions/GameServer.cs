using MCServerManager.Library.Data.Model;
using Newtonsoft.Json;
using System.Text.RegularExpressions;

namespace MCServerManager.Library.Actions
{
	/// <summary>
	/// Работа с сервером.
	/// </summary>
	public class GameServer : Application
	{
		/// <summary>
		/// Состояния сервера.
		/// </summary>
		public new enum Status
		{
			Launch,
			Run,
			Shutdown,
			Off,
			Reboot,
			Error
		}

		/// <summary>
		/// Информация о серверном приложении.
		/// </summary>
		[JsonIgnore]
		public new GameServerData Data { get; private set; }

		/// <summary>
		/// Адрес сервера/ip.
		/// </summary>
		public string Address { get { return Data.Address; } }

		/// <summary>
		/// Используемый порт.
		/// </summary>
		public int? Port { get { return Data.Port; } }

		/// <summary>
		/// Список сервисов.
		/// </summary>
		private List<BackgroundService> _services = new();

		public IEnumerable<BackgroundService> Services { get { return _services; } }

		/// <summary>
		/// Состояние сервера.
		/// </summary>
		[JsonIgnore]
		public new Status State { get; private set; }

		/// <summary>
		/// Список игроков на сервере.
		/// </summary>
		[JsonIgnore]
		public UsersListServer<string> UserList { get; private set; } = new();


		/// <summary>
		/// Делегат события завершения работы серверного приложения.
		/// </summary>
		/// <param name="id">Идентификатор сервера.</param>
		public delegate void ServerClocedEventHandler(Guid id);

		/// <summary>
		/// Cобытие завершения работы серверного приложения.
		/// </summary>
		public event ServerClocedEventHandler ServerCloced;

		/// <summary>
		/// Делегат события начала работы серверного приложения.
		/// </summary>
		/// <param name="id">Идентификатор сервера.</param>
		public delegate void ServerStartedEventHandler(Guid id);

		/// <summary>
		/// Cобытие начала работы серверного приложения.
		/// </summary>
		public event ServerStartedEventHandler ServerStarted;

		/// <summary>
		/// Делегат события завершения работы серверного приложения при перезагрузке.
		/// </summary>
		delegate void ServerOffEventHandler();

		/// <summary>
		/// Cобытие завершения работы серверного приложения при перезагрузке.
		/// </summary>
		event ServerOffEventHandler ServerOff;

		/// <summary>
		/// Конструктор с параметром
		/// </summary>
		/// <param name="data">Информания о серверном приложении.</param>
		public GameServer(GameServerData data) : base(data)
		{
			CheckServerData(data);

			Data = data;
			State = Status.Off;

			if (Data.Services != null)
			{
				foreach (var service in Data.Services)
				{
					_services.Add(new BackgroundService(service));
				}
			}

			ServerStarted += (id) => AutoStartBackgroundService();
			ServerOff += () => CloseBackgroundService();
		}

		/// <summary>
		/// Обновляет настройки серверного приложения.
		/// </summary>
		/// <param name="data">Информания о серверном приложении.</param>
		public void UpdateData(GameServerData data)
		{
			base.UpdateData(data);

			if (Id != data.Id)
			{
				throw new Exception("Идентификаторы не совпадают");
			}

			CheckServerData(data);
			Data = data;
		}

		/// <summary>
		/// Обновляет информацию о сервисе.
		/// </summary>
		/// <param name="serviceData">Информания о сервисе.</param>
		public void UpdateServiceData(BackgroundServiceData serviceData)
		{
			var service = GetService(serviceData.Id);
			service.UpdateData(Data);

			var item = Data.Services.FirstOrDefault(x => x.Id == serviceData.Id);

			if (item != null)
			{
				Data.Services.Remove(item);
			}

			Data.Services.Add(serviceData);
		}

		public void AddService(BackgroundService service)
		{
			if (Id != service.GameServerId)
			{
				throw new Exception("Сервис предназначен для использования с другим сервером.");
			}

			if(_services.FirstOrDefault(x=> x.Id == service.Id) != null)
			{
				throw new Exception("Сервис с таким же id уже существует.");
			}

			_services.Add(service);
		}

		public BackgroundService GetService(Guid serviceId)
		{
			var service = _services.FirstOrDefault(x => x.Id == serviceId);

			if (service == null)
			{
				throw new Exception("Сервис не найден.");
			}

			return service;
		}

		public void DeleteService(Guid serviceId)
		{
			var service = _services.FirstOrDefault(x => x.Id == serviceId);

			if (service == null)
			{
				throw new Exception("Сервис с таким id отсутствует.");
			}

			if(service.State == Application.Status.Run)
			{
				service.Close();
			}

			_services.Remove(service);
		}

		/// <summary>
		/// Запускает серверное приложение.
		/// </summary>
		public new void Start()
		{
			if (State != Status.Off && State != Status.Error && State != Status.Reboot)
			{
				return;
			}

			if (State != Status.Reboot)
			{
				State = Status.Launch;
			}

			StartServer(new EventHandler((sender, e) =>
			{
				ProcessClosed();
				ServerOff?.Invoke();
			})
			);
		}

		/// <summary>
		/// Завершает работу серверого приложения.
		/// </summary>
		public void Stop()
		{
			if (State != Status.Run && State != Status.Reboot)
			{
				return;
			}

			if (State != Status.Reboot)
			{
				State = Status.Shutdown;
			}

			var stopCommand = "stop";
			_process.StandardInput.WriteLine(stopCommand);
		}

		/// <summary>
		/// Очищает ресурсы процесса после завершения работы.
		/// </summary>
		private new void ProcessClosed()
		{
			_process.Dispose();

			if (State == Status.Launch)
			{
				State = Status.Error;
				return;
			}

			if (State != Status.Reboot)
			{
				State = Status.Off;
				// Вызывается событие отключения серверного приложения
				ServerCloced?.Invoke(Id);
			}
		}

		/// <summary>
		/// Перезапускает серверное приложение.
		/// </summary>
		public void Restart()
		{
			if (State != Status.Run)
			{
				return;
			}

			State = Status.Reboot;
			ServerOff += RunOffServer;
			Stop();
		}

		/// <summary>
		/// Запускает серверное приложение после завершения работы при перезапуске.
		/// </summary>
		private void RunOffServer()
		{
			ServerOff -= RunOffServer;
			Start();
		}

		/// <summary>
		/// Отключает серверное приложение не дожидаясь завершения работы.
		/// </summary>
		public new void Close()
		{
			if (State == Status.Off || State == Status.Error)
			{
				return;
			}

			if (State == Status.Reboot)
			{
				ServerOff -= RunOffServer;
			}

			_process.Kill();
		}

		/// <summary>
		/// Отключает все сервисы.
		/// </summary>
		public void CloseAllServices()
		{
			_services.ForEach(x => x.Close());
		}

		/// <summary>
		/// Выводит сообщение от серверного приложения.
		/// </summary>
		/// <param name="message">Текст сообщения.</param>
		protected override void GetServerMessage(string message)
		{
			base.GetServerMessage(message);

			DetectingCompletionStartupServer(message);
			DetectingnUser(message);
		}

		/// <summary>
		/// Отправляет команду в серверное приложение.
		/// </summary>
		/// <param name="message">Команда для серверного приложения.</param>
		public override void SendServerCommand(string message)
		{
			base.SendServerCommand(message);
		}

		/// <summary>
		/// Проверяет данные серверного приложения.
		/// </summary>
		/// <param name="data">Информания о серверном приложении.</param>
		public void CheckServerData(GameServerData data)
		{
			CheckApplicationData(data);

			if (data.Port != null)
			{
				if (data.Port <= 1023 || data.Port >= 65535)
				{
					throw new ArgumentOutOfRangeException(nameof(data.Port), "Значения порта задано вне допустимого диапазона 1024 - 65535");
				}
			}
		}

		private void AutoStartBackgroundService()
		{
			_services.ForEach(service => {
				if (service.AutoStart &&
					service.State == Application.Status.Off)
				{
					service.Start();
				}
			});
		}

		private void CloseBackgroundService()
		{
			_services.ForEach(service => {
				if (service.AutoClose &&
					service.State == Application.Status.Run)
				{
					service.Close();
				}
			});
		}

		/// <summary>
		/// Определение полной загрузки сервера.
		/// </summary>
		/// <param name="message">Текст сообщения от сервера.</param>
		private void DetectingCompletionStartupServer(string message)
		{
			if (State != Status.Launch && State != Status.Reboot)
			{
				return;
			}

			var MessageServerStarted = "Done";

			if (message.Contains(MessageServerStarted))
			{
				State = Status.Run;
				ServerStarted?.Invoke(Id);
			}
		}

		/// <summary>
		/// Определение подключения/отключения пользователя.
		/// </summary>
		/// <param name="message">Текст сообщения от сервера.</param>
		private void DetectingnUser(string message)
		{
			if(string.IsNullOrEmpty(message))
			{
				return;
			}

			if (State == Status.Off || State == Status.Error || State == Status.Launch)
			{
				return;
			}

			//Регулярное выражение для определения подключения пользователя.
			const string pattertUserConnected = @"\[.*\]:\s([^\<\>\[\]\s]*)\sjoined\sthe\sgame$";

			//Регулярное выражение для определения отключения пользователя.
			const string pattertUserDisconnected = @"\[.*\]:\s([^\<\>\[\]\s]*)\sleft\sthe\sgame$";


			int groupLogin = 1; // Расположение логина в группе.

			// Определение подключения пользователя к серверу.
			if (Regex.Match(message, pattertUserConnected).Success)
			{
				UserList.Add(Regex.Match(message, pattertUserConnected).Groups[groupLogin].Value);
			}

			// Определение отключения пользователя от сервера.
			if (Regex.Match(message, pattertUserDisconnected).Success)
			{
				UserList.Remove(Regex.Match(message, pattertUserDisconnected).Groups[groupLogin].Value);
			}
		}
	}
}
