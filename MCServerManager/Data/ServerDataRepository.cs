using MCServerManager.Library.Data.Interface;
using MCServerManager.Library.Data.Models;
using Microsoft.EntityFrameworkCore;
using MCService = MCServerManager.Library.Data.Models.Service;

namespace MCServerManager.Data
{
    /// <summary>
	/// Взаимодействие с данными хранящимся на сервере.
	/// </summary>
    public class ServerDataRepository : IGameServerDataContext
    {
        private readonly IServiceProvider _provider;

        public ServerDataRepository(IServiceProvider provider)
        {
            _provider = provider;
        }

        /// <summary>
		/// Добавить новый сервер.
		/// </summary>
		/// <param name="serverData">Информация о сервере.</param>
        public async Task CreateServerAsync(Server gameServerData)
        {
            using (IServiceScope scope = _provider.CreateScope())
            {
                var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

                await context.AddAsync(gameServerData);
                await context.SaveChangesAsync();
            }
        }

        /// <summary>
		/// Добавить новый сервис.
		/// </summary>
		/// <param name="serverData">Информация о сервисе.</param>
        public async Task CreateServiceAsync(MCService serviceData)
        {
            using (IServiceScope scope = _provider.CreateScope())
            {
                var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

                context.Services.Add(serviceData);
                await context.SaveChangesAsync();
            }
        }

        /// <summary>
		/// Удалить сервер.
		/// </summary>
		/// <param name="serverID">Идентификатор сервера.</param>
        public async Task DeleteServerAsync(Guid serverID)
        {
            using (IServiceScope scope = _provider.CreateScope())
            {
                var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
                var server = await context.Servers
                    .Include(server => server.Services)
                    .Where(server => server.ServerId == serverID)
                    .FirstOrDefaultAsync();

                if (server == null)
                {
                    return;
                }

                context.Servers.Remove(server);
                await context.SaveChangesAsync();
            }
        }

        /// <summary>
		/// Удалить сервис.
		/// </summary>
		/// <param name="serviceId">Идентификатор сервис.</param>
        public async Task DeleteServiceAsync(Guid serviceId)
        {
            using (IServiceScope scope = _provider.CreateScope())
            {
                var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
                var item = await context.Services.FindAsync(serviceId);

                if (item == null)
                {
                    return;
                }

                context.Remove(item);
                await context.SaveChangesAsync();
            }
        }

        /// <summary>
		/// Загружает информацию о серверах.
		/// </summary>
		/// <returns>Список данных о серверах.</returns>
        public async Task<List<Server>> LoadServerDataAsync()
        {
            using (IServiceScope scope = _provider.CreateScope())
            {
                var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

                return await context.Servers.AsNoTracking().Include(server => server.Services).ToListAsync();
            }
        }

        /// <summary>
		/// Обновить информацию о сервере.
		/// </summary>
		/// <param name="serverData">Информация о сервере.</param>
        public async Task UpdateServerAsync(Server serverData)
        {
            using (IServiceScope scope = _provider.CreateScope())
            {
                var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
                var server = await context.Servers.FindAsync(serverData.ServerId);

                if (server == null) throw new Exception("Сервер не найден.");

                server.UpdateData(serverData);
                await context.SaveChangesAsync();
            }
        }

        /// <summary>
		/// Обновить информацию о сервисе.
		/// </summary>
		/// <param name="serverData">Информация о сервисе.</param>
        public async Task UpdateServiceAsync(MCService serviceData)
        {
            using (IServiceScope scope = _provider.CreateScope())
            {
                var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
                var servise = await context.Services.FindAsync(serviceData.ServiceId);

                if (servise == null) throw new Exception("Сервис не найден.");

                servise.UpdateData(serviceData);
                await context.SaveChangesAsync();
            }
        }
    }
}
