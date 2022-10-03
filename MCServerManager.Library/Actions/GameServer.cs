using MCServerManager.Library.Data.Model;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using static MCServerManager.Library.Data.Model.ServerStatus;

namespace MCServerManager.Library.Actions
{
	/// <summary>
	/// Работа с сервером
	/// </summary>
	public class GameServer
	{
		/// <summary>
		/// Информация о серверном приложении.
		/// </summary>
		public ServerData ServerData { get; private set; }

		/// <summary>
		/// Идентификатор приложения.
		/// </summary>
		public Guid Id { get { return ServerData.Id; } }

		/// <summary>
		/// Автозапуск.
		/// </summary>
		public bool AutoStart { get { return ServerData.AutoStart; } }

		/// <summary>
		/// Название приложения.
		/// </summary>
		public string Name { get { return ServerData.Name; } }

		/// <summary>
		/// Расположение приложения.
		/// </summary>
		public string WorkDirectory { get { return ServerData.WorkDirectory; } }

		/// <summary>
		/// Программа для запуска.
		/// </summary>
		public string Programm { get { return ServerData.Programm; } }

		/// <summary>
		/// Аргументы запуска.
		/// </summary>
		public string Arguments { get { return ServerData.Arguments; } }

		/// <summary>
		/// Адрес сервера/ip.
		/// </summary>
		public string Addres { get { return ServerData.Addres; } }

		/// <summary>
		/// Используемый порт.
		/// </summary>
		public int? Port { get { return ServerData.Port; } }

		/// <summary>
		/// Состояние сервера.
		/// </summary>
		public Status State { get; private set; }

		/// <summary>
		/// Конструктор с параметром
		/// </summary>
		/// <param name="data">Информания о экземпляре серверного приложения.</param>
		public GameServer(ServerData data)
		{
			CheckServerData(data);

			this.ServerData = data;

			State = Status.Off;
		}

		/// <summary>
		/// Запускает серверное приложение.
		/// </summary>
		public void Start()
		{

		}

		/// <summary>
		/// Завершает работу серверого приложения.
		/// </summary>
		public void Stop()
		{

		}

		/// <summary>
		/// Перезапускает серверное приложение.
		/// </summary>
		public void Restart()
		{

		}

		/// <summary>
		/// Отключает серверное приложение не дожидаясь завершения работы.
		/// </summary>
		public void Close()
		{

		}

		/// <summary>
		/// Обновляет настройки серверного приложения.
		/// </summary>
		/// <param name="data">Информания о серверном приложении.</param>
		public void UpdateData(ServerData data)
		{

		}

		/// <summary>
		/// Проверяет данные серверного приложения.
		/// </summary>
		/// <param name="data">Информания о серверном приложении.</param>
		public void CheckServerData(ServerData data)
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
				throw new DirectoryNotFoundException(nameof(data.WorkDirectory));
			}

			if (data.Port <= 1023 || data.Port >= 65535)
			{
				throw new ArgumentOutOfRangeException(nameof(data.Port), "Значения порта задано вне диапазона 1024 - 65535");
			}
		}
	}
}
