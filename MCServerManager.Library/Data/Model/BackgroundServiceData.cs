using System;
using System.Collections.Generic;
using System.Linq;
namespace MCServerManager.Library.Data.Model
{
	/// <summary>
	/// Данные о сервисе
	/// </summary>
	public class BackgroundServiceData : ApplicationData
	{
		/// <summary>
		/// Идентификатор сервера
		/// </summary>
		public Guid GameServerId { get; set; }

		/// <summary>
		/// Адрес сервера(ip)
		/// </summary>
		public string Addres { get; set; }

		/// <summary>
		/// Используемый порт
		/// </summary>
		public int? Port { get; set; }
	}
}
