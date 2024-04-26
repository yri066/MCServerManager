using MCServerManager.Data.FilterAttributes;
using MCServerManager.Models;
using MCServerManager.Service;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace MCServerManager.Pages.Server
{
    /// <summary>
    /// Взаимодействие с серверным приложением.
    /// </summary>
	[Authorize]
    [Route("/Server/{serverId:guid}/[action]")]
    public class ServerController : Controller
    {
        private readonly GameServerService _serverService;
        private readonly UserService _userService;

        public ServerController(GameServerService serverService, UserService userService)
        {
            _serverService = serverService;
            _userService = userService;

        }

        /// <summary>
        /// Получить информацию о сервере.
        /// </summary>
        /// <param name="serverId">Идентификатор сервера.</param>
        /// <returns>Информация о сервере.</returns>
        [ServiceFilter(typeof(UserServerAccessFilter))]
        public object GetStatus(Guid serverId)
        {
            try
            {
                var server = _serverService.GetServer(serverId);

                return new
                {
                    Status = server.State.ToString(),
                    UserListVersion = server.UserList.Version,
                    ConsoleVersion = server.ConsoleBuffer.Version
                };
            }
            catch (Exception ex)
            {
                return SetErrorMessage(404, ex.Message);
            }
        }

        /// <summary>
        /// Запустить сервер.
        /// </summary>
        /// <param name="serverId">Идентификатор сервера.</param>
        /// <returns>Информация о сервере.</returns>
        [ServiceFilter(typeof(UserServerAccessFilter))]
        public object Start(Guid serverId)
        {
            try
            {
                _serverService.StartServer(serverId);
            }
            catch (Exception ex)
            {
                return SetErrorMessage(404, ex.Message);
            }

            return GetStatus(serverId);
        }

        /// <summary>
        /// Перезапустить сервер.
        /// </summary>
        /// <param name="serverId">Идентификатор сервера.</param>
        /// <returns>Информация о сервере.</returns>
        [ServiceFilter(typeof(UserServerAccessFilter))]
        public object Restart(Guid serverId)
        {
            try
            {
                _serverService.RestartServer(serverId);
            }
            catch (Exception ex)
            {
                return SetErrorMessage(404, ex.Message);
            }

            return GetStatus(serverId);
        }

        /// <summary>
        /// Остановить сервер.
        /// </summary>
        /// <param name="serverId">Идентификатор сервера.</param>
        /// <returns>Информация о сервере.</returns>
        [ServiceFilter(typeof(UserServerAccessFilter))]
        public object Stop(Guid serverId)
        {
            try
            {
                _serverService.StopServer(serverId);
            }
            catch (Exception ex)
            {
                return SetErrorMessage(404, ex.Message);
            }

            return GetStatus(serverId);
        }

        /// <summary>
        /// Выключить сервер.
        /// </summary>
        /// <param name="serverId">Идентификатор сервера.</param>
        /// <returns>Информация о сервере.</returns>
        [ServiceFilter(typeof(UserServerAccessFilter))]
        public object Close(Guid serverId)
        {
            try
            {
                _serverService.CloseServer(serverId);
            }
            catch (Exception ex)
            {
                return SetErrorMessage(404, ex.Message);
            }

            return GetStatus(serverId);
        }

        /// <summary>
        /// Получить список пользователей.
        /// </summary>
        /// <param name="serverId">Идентификатор сервера.</param>
        /// <returns>Список пользователей.</returns>
        [ServiceFilter(typeof(UserServerAccessFilter))]
        public object GetUserList(Guid serverId)
        {
            try
            {
                return _serverService.GetServer(serverId).UserList;
            }
            catch (Exception ex)
            {
                return SetErrorMessage(404, ex.Message);
            }
        }

        /// <summary>
        /// Открыть страницу консоли приложения.
        /// </summary>
        /// <param name="serverId">Идентификатор сервера.</param>
        /// <returns>Страница консоли.</returns>
        [ServiceFilter(typeof(UserServerAccessFilter))]
        public IActionResult Console(Guid serverId)
        {
            try
            {
                var server = _serverService.GetServer(serverId);

                if (_userService.UserId is null || _userService.UserId != server.Data.UserId)
                    return Forbid();

                ViewData["Name"] = server.Name;
                return View("/Pages/Application/Console.cshtml", server.ConsoleBuffer);
            }
            catch (Exception)
            {
                return Redirect("/List");
            }
        }

        /// <summary>
        /// Получить буфер вывода приложения.
        /// </summary>
        /// <param name="serverId">Идентификатор сервера.</param>
        /// <param name="bufferId">Версия буфера.</param>
        /// <returns>Буфер вывода приложения.</returns>
        [Route("/Server/{serverId:guid}/[action]/{version:guid}")]
        [ServiceFilter(typeof(UserServerAccessFilter))]
        public object Console(Guid serverId, Guid version)
        {
            try
            {
                var server = _serverService.GetServer(serverId);

                return new
                {
                    Console = server.ConsoleBuffer.GetConsoleBuffer(version),
                    server.ConsoleBuffer.Version
                };
            }
            catch (Exception ex)
            {
                return SetErrorMessage(404, ex.Message);
            }
        }

        /// <summary>
        /// Отправить сообщение на сервер.
        /// </summary>
        /// <param name="serverId">Идентификатор сервера.</param>
        /// <param name="message">Сообщение.</param>
        /// <returns>Информация о сервере.</returns>
        [HttpPost]
        [ServiceFilter(typeof(UserServerAccessFilter))]
        public object Console(Guid serverId, string message = "")
        {
            try
            {
                _serverService.SendServerAppMessage(serverId, message);
            }
            catch (Exception ex)
            {
                return SetErrorMessage(404, ex.Message);
            }

            return GetStatus(serverId);
        }

        private object SetErrorMessage(int statusCode, string message)
        {
            HttpContext.Response.StatusCode = statusCode;
            return new { errorText = message };
        }

        [HttpPost]
        [ServiceFilter(typeof(UserServerAccessFilter))]
        public void UpdateRateServices(Guid serverId, string content)
        {
            try
            {
                var dict = System.Text.Json.JsonSerializer.Deserialize<Dictionary<string, int>>(content);
                _serverService.UpdateRateServices(serverId, dict);
            }
            catch (Exception ex)
            {
                HttpContext.Response.StatusCode = 500;
                HttpContext.Response.WriteAsync(ex.ToString()).Wait();
            }
        }
    }
}
