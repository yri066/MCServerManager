﻿using System.ComponentModel.DataAnnotations;
using System.ComponentModel;
using MCServerManager.Library.Data.Model;

namespace MCServerManager.Models
{
	/// <summary>
	/// Данные о сервисе
	/// </summary>
	public class BackgroundServiceDetail : ApplicationDetail
	{
		/// <summary>
		/// Адрес сервера(ip)
		/// </summary>
		[Required, StringLength(100), DisplayName("Адрес сервера")]
		public string Addres { get; set; }

		/// <summary>
		/// Используемый порт
		/// </summary>
		[Range(1024, 65535), DisplayName("Используемый порт")]
		public int? Port { get; set; }

		/// <summary>
		/// Автовыключение вместе с сервером.
		/// </summary>
		[Required, DisplayName("Автовыключение вместе с сервером")]
		public bool AutoClose { get; set; }

		public BackgroundServiceData GetBackgroundServiceData(Guid id, Guid serverId)
		{
			return new BackgroundServiceData
			{
				Id = id,
				GameServerId = serverId,
				Name = Name,
				AutoStart = AutoStart,
				AutoClose = AutoClose,
				WorkDirectory = WorkDirectory,
				Programm = Programm,
				Arguments = Arguments,
				Addres = Addres,
				Port = Port
			};
		}
	}
}
