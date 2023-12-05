using MCServerManager.Library.Data.Models;

namespace MCServerManager.Library.Data.Interface
{
	public interface IGameServerDataContext
	{
		/// <summary>
		/// Загружает информацию о серверах.
		/// </summary>
		/// <returns>Список данных о серверах.</returns>
		public Task<List<Server>> LoadServerDataAsync();

		/// <summary>
		/// Добавить новый сервер.
		/// </summary>
		/// <param name="serverData">Информация о сервере.</param>
		public Task CreateServerAsync(Server gameServerData);

		/// <summary>
		/// Добавить новый сервис.
		/// </summary>
		/// <param name="serverData">Информация о сервисе.</param>
		public Task CreateServiceAsync(Service serviceData);

		/// <summary>
		/// Обновить информацию о сервере.
		/// </summary>
		/// <param name="serverData">Информация о сервере.</param>
		public Task UpdateServerAsync(Server serverData);

		/// <summary>
		/// Обновить информацию о сервисе.
		/// </summary>
		/// <param name="serverData">Информация о сервисе.</param>
		public Task UpdateServiceAsync(Service serviceData);

		/// <summary>
		/// Удалить сервер.
		/// </summary>
		/// <param name="id">Идентификатор сервера.</param>
		public Task DeleteServerAsync(Guid id);

		/// <summary>
		/// Удалить сервис.
		/// </summary>
		/// <param name="id">Идентификатор сервис.</param>
		public Task DeleteServiceAsync(Guid id);
	}
}
