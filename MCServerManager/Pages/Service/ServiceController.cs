using MCServerManager.Data.FilterAttributes;
using MCServerManager.Library.Data.Models;
using MCServerManager.Service;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace MCServerManager.Pages.Service
{
    /// <summary>
    /// Взаимодействие с серверным приложением.
    /// </summary>
    [Authorize]
    [Route("/Service/{id:guid}/[action]")]
	public class ServiceController : Controller
	{
		private readonly GameServerService _serverService;
        private readonly UserService _userService;

        public ServiceController(GameServerService serverService, UserService userService)
		{
			_serverService = serverService;
            _userService = userService;
        }

        /// <summary>
        /// Получить информацию о сервисе.
        /// </summary>
        /// <param name="id">Идентификатор сервиса.</param>
        /// <returns>Информация о сервисе.</returns>
        [ServiceFilter(typeof(UserServiceAccessFilter))]
        public object GetStatus(Guid id)
		{
			try
			{
                var service = _serverService.GetService(id);

				return new
				{
					Status = service.State.ToString()
				};
			}
			catch (Exception ex)
			{
				HttpContext.Response.StatusCode = 404;
				return new { errorText = ex.Message };
			}
		}

        /// <summary>
        /// Запустить сервис.
        /// </summary>
        /// <param name="id">Идентификатор сервиса.</param>
        /// <returns>Информация о сервисе.</returns>
        [ServiceFilter(typeof(UserServiceAccessFilter))]
        public object Start(Guid id)
		{
			try
			{
                _serverService.StartService(id);
			}
			catch (Exception ex)
			{
				HttpContext.Response.StatusCode = 404;
				return new { errorText = ex.Message };
			}

			return GetStatus(id);
		}

        /// <summary>
        /// Выключить сервис.
        /// </summary>
        /// <param name="id">Идентификатор сервиса.</param>
        /// <returns>Информация о сервисе.</returns>
        [ServiceFilter(typeof(UserServiceAccessFilter))]
        public object Close(Guid id)
		{
			try
			{
				_serverService.CloseService(id);
			}
			catch (Exception ex)
			{
				HttpContext.Response.StatusCode = 404;
				return new { errorText = ex.Message };
			}

			return GetStatus(id);
		}

        /// <summary>
        /// Открыть страницу консоли приложения.
        /// </summary>
        /// <param name="id">Идентификатор сервера.</param>
        /// <returns>Страница консоли.</returns>
        public IActionResult Console(Guid id)
		{
			try
			{
                var service = _serverService.GetService(id);

                if (_userService.UserId is null || _userService.UserId != service.Data.UserId)
					return Forbid();

                ViewData["Name"] = service.Name;
				return View("/Pages/Application/Console.cshtml");
			}
			catch (Exception)
			{
				return Redirect("/List");
			}
		}

		/// <summary>
		/// Получить буфер вывода приложения.
		/// </summary>
		/// <param name="id">Идентификатор сервиса.</param>
		/// <param name="bufferId">Версия буфера.</param>
		/// <returns>Буфер вывода приложения.</returns>
		[Route("/Service/{id:guid}/[action]/{bufferId:guid}")]
        [ServiceFilter(typeof(UserServiceAccessFilter))]
        public object Console(Guid id, Guid bufferId)
		{
			try
			{
				var service = _serverService.GetService(id);

				return new
				{
					Console = service.ConsoleBuffer.GetConsoleBuffer(bufferId),
					service.ConsoleBuffer.Version
				};
			}
			catch (Exception ex)
			{
				HttpContext.Response.StatusCode = 404;
				return new { errorText = ex.Message };
			}
		}

		/// <summary>
		/// Отправить сообщение в сервис.
		/// </summary>
		/// <param name="id">Идентификатор сервиса.</param>
		/// <param name="message">Сообщение.</param>
		/// <returns>Информация о сервисе.</returns>
		[HttpPost]
        [ServiceFilter(typeof(UserServiceAccessFilter))]
        public object Console(Guid id, string message = "")
		{
			try
			{
				_serverService.SendServiceAppMessage(id, message);
			}
			catch (Exception ex)
			{
				HttpContext.Response.StatusCode = 404;
				return new { errorText = ex.Message };
			}

			return GetStatus(id);
        }
    }
}
