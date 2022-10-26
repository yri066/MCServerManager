﻿using System.ComponentModel.DataAnnotations;
using System.ComponentModel;
using MCServerManager.Library.Data.Model;

namespace MCServerManager.Models
{
	/// <summary>
	/// Данные о сервере
	/// </summary>
	public class GameServerDetail : ApplicationDetail
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

		public GameServerData GetGameServerData(Guid id)
		{
			return new GameServerData
			{
				Id = id,
				Name = Name,
				AutoStart = AutoStart,
				WorkDirectory = WorkDirectory,
				Programm = Programm,
				Arguments = Arguments,
				Addres = Addres,
				Port = Port
			};
		}
	}
}