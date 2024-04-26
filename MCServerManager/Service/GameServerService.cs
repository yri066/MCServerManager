using MCServerManager.Library.Actions;
using MCServerManager.Library.Data.Interface;
using MCServerManager.Library.Data.Models;
using static MCServerManager.Library.Actions.Application;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;
using MCBackgroundService = MCServerManager.Library.Actions.BackgroundService;
using MCService = MCServerManager.Library.Data.Models.Service;


namespace MCServerManager.Service
{
    /// <summary>
    /// Сервис управления работой серверов.
    /// </summary>
    public class GameServerService
    {
        /// <summary>
        /// Ключ времени ожидания запуска приложения.
        /// </summary>
        private const string _keyGetStartupWaitTime = "StartupWaitTime";

        /// <summary>
        /// Список серверов.
        /// </summary>
        private List<GameServer> _servers = new();

        /// <summary>
        /// Список серверов.
        /// </summary>
        public IEnumerable<GameServer> Servers { get { return _servers; } }

        /// <summary>
        /// Конфигурация.
        /// </summary>
        private readonly IConfiguration _configuration;
        private readonly IGameServerDataContext _context;

        public GameServerService(IConfiguration configuration, IGameServerDataContext context)
        {
            _configuration = configuration;
            _context = context;

            LoadServers();
            AutoRun();
        }

        /// <summary>
        /// Заполняет список серверов
        /// </summary>
        private void LoadServers()
        {
            var list = _context.LoadServerDataAsync().Result;

            foreach (var server in list)
            {
                try
                {
                    AddServer(server);
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.ToString());
                }
            }
        }

        /// <summary>
        /// Поочередно запускает серверные приложения.
        /// </summary>
        private void AutoRun()
        {
            var startupWaitTime = 600;

            if (!string.IsNullOrEmpty(_configuration[_keyGetStartupWaitTime]))
            {
                var count = _configuration.GetValue<int>(_keyGetStartupWaitTime);
                var minWaitTime = 0;
                var maxWaitTime = 7200;

                if (count < minWaitTime || count > maxWaitTime)
                {
                    Console.WriteLine($"Значение {_keyGetStartupWaitTime} задано вне допустимого диапазона {minWaitTime} - {maxWaitTime} в Settings.json, используется значение по умолчанию: {startupWaitTime}");
                }
                else
                {
                    startupWaitTime = count;
                }
            }

            var task = Task.Run(() =>
            {
                foreach (var server in _servers)
                {
                    if (!server.AutoStart || server.State != Status.Off)
                    {
                        continue;
                    }

                    try
                    {
                        if (!RunServerAsync(server).Wait(TimeSpan.FromSeconds(startupWaitTime)))
                        {
                            Console.WriteLine($"Превышено время ожидания запуска сервера: {server.ServerId} - {server.Name}");
                        }
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine(ex.ToString());
                    }
                }
            });
        }

        /// <summary>
        /// Создает задачу, ожидающую запуск серверного приложения и его сервисов.
        /// </summary>
        private Task RunServerAsync(GameServer server)
        {
            var tcs = new TaskCompletionSource();
            server.FullStarted += (id) =>
            {
                tcs.TrySetResult();
            };
            server.Closed += (id) =>
            {
                tcs.TrySetCanceled();
            };

            server.Start();
            return tcs.Task;
        }

        /// <summary>
        /// Создает новый сервер.
        /// </summary>
        /// <param name="server">Информация о сервере.</param>
        /// <returns>Идентификатор сервера.</returns>
        public async Task<Guid> CreateServerAsync(Server server)
        {
            if (string.IsNullOrEmpty(server.UserId))
            {
                throw new ArgumentNullException(nameof(server), "Идентификатор пользователя не задан.");
            }

            CheckData(server, server.Port, server.Address);

            var id = Guid.NewGuid();
            server.ServerId = id;
            server.Services = new List<MCService>();

            try
            {
                await _context.CreateServerAsync(server);
                _servers.Add(new GameServer(server, _configuration));
            }
            catch (Exception)
            {
                throw new Exception("Невозможно сохранить сервер. Попробуйте еще раз, и если проблема не исчезнет, обратитесь к системному администратору.");
            }

            return id;
        }

        /// <summary>
        /// Создает новый сервис.
        /// </summary>
        /// <param name="service">Информация о сервисе.</param>
        /// <returns>Идентификатор сервера.</returns>
        public async Task<Guid> CreateServiceAsync(MCService service)
        {
            if (string.IsNullOrEmpty(service.UserId))
            {
                throw new ArgumentNullException(nameof(service), "Идентификатор пользователя не задан.");
            }

            CheckData(service, service.Port, service.Address);

            var serviceId = Guid.NewGuid();
            service.ServiceId = serviceId;
            service.RatingNumber = GetMaxServiceRatingNumber(service.ServerId) + 1;

            try
            {
                await _context.CreateServiceAsync(service);
                GetServer(service.ServerId).AddService(new MCBackgroundService(service, _configuration));
            }
            catch (Exception)
            {
                throw new Exception("Невозможно сохранить сервис. Попробуйте еще раз, и если проблема не исчезнет, обратитесь к системному администратору.");
            }
            

            return serviceId;
        }

        /// <summary>
        /// Добавить новый сервер.
        /// </summary>
        /// <param name="server">Информация о сервере.</param>
        /// <exception cref="Exception">Директория или порт используются другим сервером.</exception>
        private void AddServer(Server server)
        {
            CheckFreeDirectory(server.WorkDirectory);
            CheckFreePort(server.Port, server.Address, server.Id);

            _servers.Add(new GameServer(server, _configuration));
        }

        /// <summary>
        /// Удалить указанный сервер.
        /// </summary>
        /// <param name="serverId">Идентификатор сервера.</param>
        public async Task DeleteServerAsync(Guid serverId)
        {
            var exemplar = GetServer(serverId);

            if (exemplar.State != Status.Off && exemplar.State != Status.Error)
            {
                exemplar.Close();
                exemplar.CloseAllServices();
            }

            try
            {
                await _context.DeleteServerAsync(serverId);
                _servers.Remove(exemplar);
            }
            catch (Exception ex)
            {
                throw new Exception("Невозможно удалить. Попробуйте еще раз, и если проблема не исчезнет, обратитесь к системному администратору.");
            }
        }

        /// <summary>
        /// Удалить указанный сервис.
        /// </summary>
        /// <param name="serviceId">Идентификатор сервиса.</param>
        public async Task DeleteServiceAsync(Guid serviceId)
        {
            var exemplar = GetService(serviceId);

            try
            {
                await _context.DeleteServiceAsync(serviceId);
                GetServer(exemplar.GameServerId).DeleteService(serviceId);
            }
            catch (Exception ex)
            {
                throw new Exception("Невозможно удалить. Попробуйте еще раз, и если проблема не исчезнет, обратитесь к системному администратору.");
            }
        }

        /// <summary>
        /// Обновить информацию указанного сервера.
        /// </summary>
        /// <param name="serverData">Информация о сервере.</param>
        public async Task UpdateServerAsync(Server serverData)
        {
            CheckData(serverData, serverData.Port, serverData.Address);

            var exemplar = GetServer(serverData.ServerId);
            try
            {
                await _context.UpdateServerAsync(serverData);
                exemplar.UpdateData(serverData);
            }
            catch (Exception ex)
            {
                throw new Exception("Невозможно сохранить изменения. Попробуйте еще раз, и если проблема не исчезнет, обратитесь к системному администратору.");
            }
        }

        /// <summary>
        /// Обновить информацию указанного сервиса.
        /// </summary>
        /// <param name="serviceData">Информация о сервисе.</param>
        /// <exception cref="ArgumentException">Идентификаторы не совпадают.</exception>
        public async Task UpdateServiceAsync(MCService serviceData)
        {
            CheckData(serviceData, serviceData.Port, serviceData.Address);

            var exemplar = GetService(serviceData.ServiceId);
            try
            {
                await _context.UpdateServiceAsync(exemplar.Data);
                exemplar.UpdateData(serviceData);
            }
            catch (Exception ex)
            {
                throw new Exception("Невозможно сохранить изменения. Попробуйте еще раз, и если проблема не исчезнет, обратитесь к системному администратору.");
            }
        }

        
        public void UpdateRateServices(Guid serverId, Dictionary<string, int> serviceRate)
        {
            if(serviceRate == null || serviceRate.Count == 0)
            {
                return;
            }

            lock (GetServer(serverId).DataLock)
            {
                var serviceCollection = GetServer(serverId).Services;
                var countItems = serviceCollection.Count() >= serviceRate.Count ? serviceCollection.Count : serviceRate.Count;
                var serviceRateSorted = serviceRate.OrderBy(x => x.Value).ToList();
                var serviceListSorted = new List<MCBackgroundService>(serviceCollection.OrderBy(x => x.RatingNumber));
                var count = 0;

                void Change(MCBackgroundService service, int raiting)
                {
                    serviceListSorted.Remove(service);
                    try
                    {
                        _context.UpdateServiceAsync(service.Data).Wait();
                        service.UpdateRateNumber(raiting);
                    }
                    catch(Exception ex)
                    {
                        Console.WriteLine("Не удалось обновить.");
                    }
                }

                for (int i = 0; i < countItems; i++)
                {
                    if (serviceListSorted.Count() == 0)
                    {
                        break;
                    }

                    if (serviceRateSorted.Count() == 0)
                    {
                        var temp = serviceListSorted.First();
                        Change(temp, count++);
                        continue;
                    }

                    var item = serviceRateSorted.First();
                    serviceRateSorted.Remove(item);

                    var exemplar = (from service in serviceListSorted
                                    where service.ServiceId == Guid.Parse(item.Key)
                                    select service).FirstOrDefault();

                    if (exemplar == null)
                    {
                        continue;
                    }

                    Change(exemplar, count++);
                }
            }
        }

        /// <summary>
        /// Запустить указанный сервер.
        /// </summary>
        /// <param name="serverId">Идентификатор сервера.</param>
        public void StartServer(Guid serverId)
        {
            GetServer(serverId).Start();
        }

        /// <summary>
        /// Остановить указанный сервер.
        /// </summary>
        /// <param name="serverId">Идентификатор сервера.</param>
        public void StopServer(Guid serverId)
        {
            GetServer(serverId).Stop();
        }

        /// <summary>
        /// Выключает указанный сервер, не дожидаясь сохранения данных.
        /// </summary>
        /// <param name="serverId">Идентификатор сервера.</param>
        public void CloseServer(Guid serverId)
        {
            GetServer(serverId).Close();
        }

        /// <summary>
        /// Перезагружает указанный сервер.
        /// </summary>
        /// <param name="serverId">Идентификатор сервера.</param>
        public void RestartServer(Guid serverId)
        {
            GetServer(serverId).Restart();
        }

        /// <summary>
        /// Запустить указанный сервис.
        /// </summary>
        /// <param name="serviceId">Идентификатор сервиса.</param>
        public void StartService(Guid serviceId)
        {
            GetService(serviceId).Start();
        }

        /// <summary>
        /// Выключает указанный сервис.
        /// </summary>
        /// <param name="serviceId">Идентификатор сервиса.</param>
        public void CloseService(Guid serviceId)
        {
            GetService(serviceId).Close();
        }

        /// <summary>
        /// Получить экземпляр класса сервера по идентификатору.
        /// </summary>
        /// <param name="serverId">Идентификатор сервера.</param>
        /// <returns>Экземпляр класса.</returns>
        /// <exception cref="Exception">Указанный сервер не найден.</exception>
        public GameServer GetServer(Guid serverId)
        {
            var exemplar = _servers.FirstOrDefault(x => x.ServerId == serverId);

            if (exemplar == null)
            {
                throw new Exception("Указанный сервер не найден");
            }

            return exemplar;
        }

        /// <summary>
        /// Получить экземпляр класса сервиса по идентификатору.
        /// </summary>
        /// <param name="serviceId">Идентификатор сервиса.</param>
        /// <returns>Экземпляр класса.</returns>
        /// <exception cref="Exception">Указанный сервис не найден.</exception>
        public MCBackgroundService GetService(Guid serviceId)
        {
            var exemplar = (from server in _servers
                            from service in server.Services
                            where service.ServiceId == serviceId
                            select service).FirstOrDefault();

            if (exemplar == null)
            {
                throw new Exception("Указанный сервис не найден");
            }

            return exemplar;
        }

        /// <summary>
        /// Получить настройки серверного приложения по идентификатору.
        /// </summary>
        /// <param name="serverId">Идентификатор сервера.</param>
        /// <returns>Настройки серверного приложения.</returns>
        public Server GetServerData(Guid serverId)
        {
            return GetServer(serverId).Data;
        }

        /// <summary>
        /// Получить настройки сервисного приложения по идентификатору.
        /// </summary>
        /// <param name="serviceId">Идентификатор сервиса.</param>
        /// <returns>Настройки сервисного приложения.</returns>
        public MCService GetServiceData(Guid serviceId)
        {
            return GetService(serviceId).Data;
        }

        private int GetMaxServiceRatingNumber(Guid serverId)
        {
            var services = GetServer(serverId).Services;
            return services.Count() > 0 ? services.Max(x => x.RatingNumber) : 0;
        }

        /// <summary>
        /// Отправляет сообщение в серверное приложение
        /// </summary>
        /// <param name="serverId">Идентификатор сервера.</param>
        /// <param name="message">Сообщение.</param>
        public void SendServerAppMessage(Guid serverId, string message = "")
        {
            GetServer(serverId).SendAppMessage(message);
        }

        /// <summary>
        /// Отправляет сообщение в сервис.
        /// </summary>
        /// <param name="serviceId">Идентификатор сервиса.</param>
        /// <param name="message">Сообщение.</param>
        public void SendServiceAppMessage(Guid serviceId, string message = "")
        {
            GetService(serviceId).SendAppMessage(message);
        }

        /// <summary>
        /// Проверка данных приложения.
        /// </summary>
        private void CheckData(IApplication data, int? Port, string Address)
        {
            CheckFreeDirectory(data.WorkDirectory, data.Id);
            CheckFreePort(Port, Address, data.Id);
        }


        /// <summary>
        /// Проверяет порта на использование другими приложениями.
        /// </summary>
        /// <param name="port">Порт.</param>
        /// <param name="id">Идентификатор приложения при обновлении данных.</param>
        /// <exception cref="Exception">Данный порт используется другим приложением.</exception>
        private void CheckFreePort(int? port, string address, Guid? id = null)
        {
            if (port != null)
            {
                if (_servers.Where(x => x.Port == port &&
                                        x.Address == address &&
                                        x.ServerId != id)
                            .Any())
                {
                    throw new Exception($"Порт {port} занят другим сервером");
                }

                if (_servers.Where(x => x.Services.Where(y => y.Port == port &&
                                                              y.Address == address &&
                                                              y.ServiceId != id
                                                         )
                                                  .Any())
                            .Any())
                {
                    throw new Exception($"Порт {port} занят другим сервисом");
                }
            }
        }

        /// <summary>
        /// Проверяет директорию на использование другими приложениями.
        /// </summary>
        /// <param name="directory">Директория.</param>
        /// <param name="id">Идентификатор приложения при обновлении данных.</param>
        /// <exception cref="Exception">Данная директория используется другим приложением.</exception>
        private void CheckFreeDirectory(string directory, Guid? id = null)
        {
            if (_servers.Where(x => x.WorkDirectory == directory && x.ServerId != id).Any())
            {
                throw new Exception($"Указанная директория используется другим сервером");
            }

            if (_servers.Where(x => x.Services.Where(y => y.WorkDirectory == directory && y.ServiceId != id).Any()).Any())
            {
                throw new Exception($"Указанная директория используется другим сервисом");
            }
        }
    }
}
