﻿using MCServerManager.Library.Data.Model;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static MCServerManager.Library.Data.Model.ApplicationStatus;

namespace MCServerManager.Library.Actions
{
	public class BackgroundService : Application
	{
		[JsonIgnore]
		public new BackgroundServiceData Data { get; private set; }

		/// <summary>
		/// Адрес сервера/ip.
		/// </summary>
		public string Addres { get { return Data.Addres; } }

		/// <summary>
		/// Используемый порт.
		/// </summary>
		public int? Port { get { return Data.Port; } }

		public Guid GameServerId { get { return Data.GameServerId; } }

		public BackgroundService(BackgroundServiceData data) : base(data)
		{
			CheckServiceData(data);
			Data = data;
		}

		/// <summary>
		/// Обновляет настройки серверного приложения.
		/// </summary>
		/// <param name="data">Информания о серверном приложении.</param>
		public void UpdateData(BackgroundServiceData data)
		{
			base.UpdateData(data);

			if (Id != data.Id)
			{
				throw new Exception("Идентификаторы не совпадают");
			}

			CheckServiceData(data);
			Data = data;
		}

		/// <summary>
		/// Проверяет данные сервиса.
		/// </summary>
		/// <param name="data">Информания о сервисе.</param>
		public void CheckServiceData(BackgroundServiceData data)
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
	}
}