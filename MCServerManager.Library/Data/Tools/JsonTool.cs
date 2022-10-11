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
		/// Сериализация обьекта в JSON.
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
		public static T LoadJsonDataFromFile<T>(string path)
		{
			if (string.IsNullOrEmpty(path))
			{
				throw new ArgumentException("Указанный путь к файлу пустой", nameof(path));
			}

			if (!File.Exists(path))
			{
				throw new FileNotFoundException("Указанный файл не найден", nameof(path));
			}

			return JsonConvert.DeserializeObject<T>(File.ReadAllText(path));
		}

		/// <summary>
		/// Сохраняет данные в файл.
		/// </summary>
		/// <typeparam name="T">Тип объекта.</typeparam>
		/// <param name="path">Путь до файла.</param>
		/// <param name="data">Объект для сериализации.</param>
		/// <returns>true - данные успешно сохранены, false - не удалось сохранить.</returns>
		public static bool SaveJsonDataToFile<T>(string path, T data)
		{
			if (!File.Exists(path))
			{
				throw new FileNotFoundException("Указанный файл не найден", nameof(path));
			}

			if (data == null)
			{
				throw new ArgumentNullException(nameof(data));
			}

			var json = JsonConvert.SerializeObject(data);

			try
			{
				File.WriteAllText(path, json);
			}
			catch (Exception ex)
			{
				Console.WriteLine(ex.ToString());
				return false;
			}

			return true;
		}
	}
}
