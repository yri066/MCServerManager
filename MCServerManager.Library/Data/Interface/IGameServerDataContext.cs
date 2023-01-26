using MCServerManager.Library.Data.Models;

namespace MCServerManager.Library.Data.Interface
{
	public interface IGameServerDataContext
	{
		/// <summary>
		/// Загружает информацию о серверах.
		/// </summary>
		/// <returns>Список данных о серверах.</returns>
		public Task<List<Server>> LoadServerDataAsycn();

		/// <summary>
		/// Добавить новый сервер.
		/// </summary>
		/// <param name="serverData">Информация о сервере.</param>
		public Task CreateServerAsycn(Server gameServerData);

		/// <summary>
		/// Добавить новый сервис.
		/// </summary>
		/// <param name="serverData">Информация о сервисе.</param>
		public Task CreateServiceAsycn(Service serviceData);

		/// <summary>
		/// Обновить информацию о сервере.
		/// </summary>
		/// <param name="serverData">Информация о сервере.</param>
		public Task UpdateServerAsycn(Server serverData);

		/// <summary>
		/// Обновить информацию о сервисе.
		/// </summary>
		/// <param name="serverData">Информация о сервисе.</param>
		public Task UpdateServiceAsycn(Service serviceData);

		/// <summary>
		/// Удалить сервер.
		/// </summary>
		/// <param name="id">Идентификатор сервера.</param>
		public Task DeleteServerAsycn(Guid id);

		/// <summary>
		/// Удалить сервис.
		/// </summary>
		/// <param name="id">Идентификатор сервис.</param>
		public Task DeleteServiceAsycn(Guid id);
	}
}
