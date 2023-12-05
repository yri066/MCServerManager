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

        /// <summary>
        /// Создает запись о новом сервере.
        /// </summary>
        /// <param name="server">Сервер.</param>
        /// <returns></returns>
        public async Task CreateServerAsync(Server server)
        {
            var list = await LoadServerDataAsync();

            if (GetServer(list, server.Id) is null)
            {
                list.Add(server);
                await SaveServerDataAsync(list);
            }
        }

        /// <summary>
        /// Создает запись о новом сервисе.
        /// </summary>
        /// <param name="service">Сервис.</param>
        /// <returns></returns>
        public async Task CreateServiceAsync(Service service)
        {
            var list = await LoadServerDataAsync();
            var item = GetServer(list, service.ServerId);

            if (item is not null)
            {
                if (GetService(list, service.Id) is null)
                {
                    item.Services.Add(service);
                    await SaveServerDataAsync(list);
                }
            }
        }

        /// <summary>
        /// Удаляет запись о сервере.
        /// </summary>
        /// <param name="serverId">Идентификатор сервера.</param>
        /// <returns></returns>
        public async Task DeleteServerAsync(Guid serverId)
        {
            var list = await LoadServerDataAsync();
            var item = GetServer(list, serverId);

            if (item is not null)
            {
                list.Remove(item);
                await SaveServerDataAsync(list);
            }
        }

        /// <summary>
        /// Удаляет запись о сервисе.
        /// </summary>
        /// <param name="serviceId">Идентификатор сервиса.</param>
        /// <returns></returns>
        public async Task DeleteServiceAsync(Guid serviceId)
        {
            var list = await LoadServerDataAsync();
            var item = GetService(list, serviceId);

            if (item is not null)
            {
                GetServer(list, item.ServerId).Services.Remove(item);
                await SaveServerDataAsync(list);
            }
        }

        /// <summary>
        /// Загружает список серверов.
        /// </summary>
        /// <returns>Список серверов.</returns>
        public async Task<List<Server>> LoadServerDataAsync()
        {
            return await JsonTool.LoadJsonDataFromFile<List<Server>>(_pathFileSettings);
        }

        /// <summary>
        /// Сохраняет список серверов.
        /// </summary>
        /// <param name="gameServers">Список серверов.</param>
        /// <returns></returns>
        public async Task SaveServerDataAsync(List<Server> gameServers)
        {
            await JsonTool.SaveJsonDataToFile(_pathFileSettings, gameServers);
        }

        /// <summary>
        /// Обновляет информацию о сервере.
        /// </summary>
        /// <param name="server">Сервер.</param>
        /// <returns></returns>
        public async Task UpdateServerAsync(Server server)
        {
            var list = await LoadServerDataAsync();
            var item = GetServer(list, server.Id);

            if (item is not null)
            {
                item.UpdateData(server);
                await SaveServerDataAsync(list);
            }
        }

        /// <summary>
        /// Обновляет информацию о сервисе.
        /// </summary>
        /// <param name="service">Сервис.</param>
        /// <returns></returns>
        public async Task UpdateServiceAsync(Service service)
        {
            var list = await LoadServerDataAsync();
            var item = GetService(list, service.Id);

            if (item is not null)
            {
                item.UpdateData(service);
                await SaveServerDataAsync(list);
            }
        }

        /// <summary>
        /// Получить сервер из списка.
        /// </summary>
        /// <param name="list">Список серверов.</param>
        /// <param name="serverId">Идентификатор сервера.</param>
        /// <returns>Сервер.</returns>
        private Server GetServer(List<Server> list, Guid? serverId)
        {
            return list.FirstOrDefault(x => x.Id == serverId);
        }

        /// <summary>
        /// Получить сервис из списка.
        /// </summary>
        /// <param name="list">Список серверов.</param>
        /// <param name="serviceId">Идентификатор сервиса.</param>
        /// <returns>Сервис.</returns>
        private Service GetService(List<Server> list, Guid serviceId)
        {
            return (from server in list
                    from service in server.Services
                    where service.Id == serviceId
                    select service).FirstOrDefault();
        }
    }
}
