using MCServerManager.Library.Data.Interface;
using MCServerManager.Library.Data.Models;
using MCServerManager.Library.Data.Tools;
using Microsoft.Extensions.Configuration;

namespace MCServerManager.Library.Data.Data
{
	/// <summary>
	/// Хранение данных в файловой системе.
	/// </summary>
	public class GameServerDataFileRepository : IGameServerDataContext
	{
		/// <summary>
		/// Ключ названия файла настроек.
		/// </summary>
		private const string _keyGetFileSettings = "GameServers";

		/// <summary>
		/// Путь к файлу с информацией настроек серверов.
		/// </summary>
		private readonly string _pathFileSettings;

		/// <summary>
		/// Конфигурация.
		/// </summary>
		private readonly IConfiguration _configuration;

		public GameServerDataFileRepository(IConfiguration configuration)
		{
			_configuration = configuration;
			_pathFileSettings = _configuration[_keyGetFileSettings];
		}

		public async Task CreateServerAsycn(Server server)
		{
			var list = await LoadServerDataAsycn();

			if(GetServer(list, server.Id) is null)
			{
				list.Add(server);
				await SaveServerDataAsycn(list);
			}
		}

		public async Task CreateServiceAsycn(Service service)
		{
			var list = await LoadServerDataAsycn();
			var item = GetServer(list, service.ServerId);

			if (item is not null)
			{
				if (GetService(list, service.Id) is null)
				{
					item.Services.Add(service);
					await SaveServerDataAsycn(list);
				}
			}
		}

		public async Task DeleteServerAsycn(Guid serverId)
		{
			var list = await LoadServerDataAsycn();
			var item = GetServer(list, serverId);

			if (item is not null)
			{
				list.Remove(item);
				await SaveServerDataAsycn(list);
			}
		}

		public async Task DeleteServiceAsycn(Guid serviceId)
		{
			var list = await LoadServerDataAsycn();
			var item = GetService(list, serviceId);

			if (item is not null)
			{
				GetServer(list, item.ServerId).Services.Remove(item);
				await SaveServerDataAsycn(list);
			}
		}

		public async Task<List<Server>> LoadServerDataAsycn()
		{
			return await JsonTool.LoadJsonDataFromFile<List<Server>>(_pathFileSettings);
		}

		public async Task SaveServerDataAsycn(List<Server> gameServers)
		{
			await JsonTool.SaveJsonDataToFile(_pathFileSettings, gameServers);
		}

		public async Task UpdateServerAsycn(Server server)
		{
			var list = await LoadServerDataAsycn();
			var item = GetServer(list, server.Id);

			if (item is not null)
			{
				server.UpdateServerData(item);
				await SaveServerDataAsycn(list);
			}
		}

		public async Task UpdateServiceAsycn(Service service)
		{
			var list = await LoadServerDataAsycn();
			var item = GetService(list, service.Id);

			if (service is not null)
			{
				var server = GetServer(list, item.ServerId);
				server.Services.Remove(item);
				server.Services.Add(item);
				await SaveServerDataAsycn(list);
			}
		}

		private Server GetServer(List<Server> list, Guid? serverId)
		{
			return list.FirstOrDefault(x => x.Id == serverId);
		}

		private Service GetService(List<Server> list, Guid serviceId)
		{
			return (from server in list
					from service in server.Services
					where service.Id == serviceId
					select service).FirstOrDefault();
		}
	}
}
