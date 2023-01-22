using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MCServerManager.Library.Data.Tools
{
	/// <summary>
	/// Работа с Json
	/// </summary>
	public static class JsonTool
	{
		/// <summary>
		/// Десериализует JSON в объект.
		/// </summary>
		/// <typeparam name="T">Тип объекта.</typeparam>
		/// <param name="json">Данные в формате Json.</param>
		/// <returns>Экземпляр объекта.</returns>
		public static T Deserialize<T>(string json)
		{
			if (string.IsNullOrEmpty(json))
			{
				throw new ArgumentNullException(nameof(json));
			}

			return JsonConvert.DeserializeObject<T>(json);
		}

		/// <summary>
		/// Сериализация объекта в JSON.
		/// </summary>
		/// <typeparam name="T">Тип объекта.</typeparam>
		/// <param name="data">Объект для сериализации.</param>
		/// <returns>Данные в формате Json.</returns>
		public static string Serialize<T>(T data)
		{
			return JsonConvert.SerializeObject(data);
		}

		/// <summary>
		/// Загружает данные из файла.
		/// </summary>
		/// <typeparam name="T">Тип объекта.</typeparam>
		/// <param name="path">Путь до файла.</param>
		/// <returns>Экземпляр объекта.</returns>
		public static async Task<T> LoadJsonDataFromFile<T>(string path)
		{
			if (string.IsNullOrEmpty(path))
			{
				throw new ArgumentException("Не указан путь к файлу загрузки данных", nameof(path));
			}

			if (!File.Exists(path))
			{
				throw new FileNotFoundException("Указанный файл загрузки данных не найден", nameof(path));
			}

			return JsonConvert.DeserializeObject<T>(await File.ReadAllTextAsync(path));
		}

		/// <summary>
		/// Сохраняет данные в файл.
		/// </summary>
		/// <typeparam name="T">Тип объекта.</typeparam>
		/// <param name="path">Путь до файла.</param>
		/// <param name="data">Объект для сериализации.</param>
		/// <returns>true - данные успешно сохранены, false - не удалось сохранить.</returns>
		public static async Task SaveJsonDataToFile<T>(string path, T data)
		{
			if (string.IsNullOrEmpty(path))
			{
				throw new ArgumentException("Не указан путь к файлу сохранения данных", nameof(path));
			}

			if (!File.Exists(path))
			{
				throw new FileNotFoundException("Указанный файл сохранения данных не найден", nameof(path));
			}

			if (data == null)
			{
				throw new ArgumentNullException(nameof(data));
			}

			var json = JsonConvert.SerializeObject(data);

			try
			{
				await File.WriteAllTextAsync(path, json);
			}
			catch (Exception)
			{ }
		}
	}
}
