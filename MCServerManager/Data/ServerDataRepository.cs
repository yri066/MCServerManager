using MCServerManager.Library.Data.Interface;
using MCServerManager.Library.Data.Models;
using Microsoft.EntityFrameworkCore;

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
        public async Task CreateServerAsycn(Server gameServerData)
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
        public async Task CreateServiceAsycn(Library.Data.Models.Service serviceData)
        {
            using (IServiceScope scope = _provider.CreateScope())
            {
                var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
                //var server = await context.Servers.FindAsync(serviceData.ServerId);

                //if (server == null) throw new Exception("Сервер не найден.");

                context.Services.Add(serviceData);
                await context.SaveChangesAsync();
            }
        }

        /// <summary>
		/// Удалить сервер.
		/// </summary>
		/// <param name="id">Идентификатор сервера.</param>
        public async Task DeleteServerAsycn(Guid id)
        {
            using (IServiceScope scope = _provider.CreateScope())
            {
                var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
                var server = await context.Servers
                    .Include(server => server.Services)
                    .Where(server => server.ServerId == id)
                    .FirstOrDefaultAsync();

                if (server == null) throw new Exception("Сервер не найден.");

                context.Servers.Remove(server);
                await context.SaveChangesAsync();
            }
        }

        /// <summary>
		/// Удалить сервис.
		/// </summary>
		/// <param name="id">Идентификатор сервис.</param>
        public async Task DeleteServiceAsycn(Guid id)
        {
            using (IServiceScope scope = _provider.CreateScope())
            {
                var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
                var item = await context.Services.FindAsync(id);
                //var element = await context.Servers
                //    .Select(server => server.Services
                //        .Where(service => service.ServiceId == id)
                //        .FirstOrDefault())
                //    .FirstOrDefaultAsync();

                if (item == null) throw new Exception("Сервис не найден.");

                context.Remove(item);
                await context.SaveChangesAsync();
            }
        }

        /// <summary>
		/// Загружает информацию о серверах.
		/// </summary>
		/// <returns>Список данных о серверах.</returns>
        public async Task<List<Server>> LoadServerDataAsycn()
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
        public async Task UpdateServerAsycn(Server serverData)
        {
            using (IServiceScope scope = _provider.CreateScope())
            {
                var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
                var server = await context.Servers.FindAsync(serverData.ServerId);

                if (server == null) throw new Exception("Сервер не найден.");

                serverData.UpdateServerData(server);
                await context.SaveChangesAsync();
            }
        }

        /// <summary>
		/// Обновить информацию о сервисе.
		/// </summary>
		/// <param name="serverData">Информация о сервисе.</param>
        public async Task UpdateServiceAsycn(Library.Data.Models.Service serviceData)
        {
            using (IServiceScope scope = _provider.CreateScope())
            {
                var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

                var servise = await context.Services.FindAsync(serviceData.ServerId);

                //var element = await context.Servers
                //    .Select(server => server.Services
                //        .Where(service => service.ServiceId == serviceData.ServiceId)
                //        .FirstOrDefault())
                //    .FirstOrDefaultAsync();

                if (servise == null) throw new Exception("Сервис не найден.");

                serviceData.UpdateServiceData(servise);
                await context.SaveChangesAsync();
            }
        }
    }
}
