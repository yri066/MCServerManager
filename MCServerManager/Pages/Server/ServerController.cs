using MCServerManager.Service;
using Microsoft.AspNetCore.Mvc;

namespace MCServerManager.Pages.Server
{
	/// <summary>
	/// Взаимодействие с серверным приложением.
	/// </summary>
	[Route("/Server/{serverId:guid}/[action]")]
	public class ServerController : Controller
	{
		private readonly GameServerService _serverService;

		public ServerController(GameServerService serverService)
		{
			_serverService = serverService;
		}

		/// <summary>
		/// Получить информацию о сервере.
		/// </summary>
		/// <param name="serverId">Идентификатор сервера.</param>
		/// <returns>Информация о сервере.</returns>
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
				HttpContext.Response.StatusCode = 404;
				return new { errorText = ex.Message };
			}
		}

		/// <summary>
		/// Запустить сервер.
		/// </summary>
		/// <param name="serverId">Идентификатор сервера.</param>
		/// <returns>Информация о сервере.</returns>
		public object Start(Guid serverId)
		{
			try
			{
				_serverService.StartServer(serverId);
			}
			catch (Exception ex)
			{
				HttpContext.Response.StatusCode = 404;
				return new { errorText = ex.Message };
			}

			return GetStatus(serverId);
		}

		/// <summary>
		/// Перезапустить сервер.
		/// </summary>
		/// <param name="serverId">Идентификатор сервера.</param>
		/// <returns>Информация о сервере.</returns>
		public object Restart(Guid serverId)
		{
			try
			{
				_serverService.RestartServer(serverId);
			}
			catch (Exception ex)
			{
				HttpContext.Response.StatusCode = 404;
				return new { errorText = ex.Message };
			}

			return GetStatus(serverId);
		}

		/// <summary>
		/// Остановить сервер.
		/// </summary>
		/// <param name="serverId">Идентификатор сервера.</param>
		/// <returns>Информация о сервере.</returns>
		public object Stop(Guid serverId)
		{
			try
			{
				_serverService.StopServer(serverId);
			}
			catch (Exception ex)
			{
				HttpContext.Response.StatusCode = 404;
				return new { errorText = ex.Message };
			}

			return GetStatus(serverId);
		}

		/// <summary>
		/// Выключить сервер.
		/// </summary>
		/// <param name="serverId">Идентификатор сервера.</param>
		/// <returns>Информация о сервере.</returns>
		public object Close(Guid serverId)
		{
			try
			{
				_serverService.CloseServer(serverId);
			}
			catch (Exception ex)
			{
				HttpContext.Response.StatusCode = 404;
				return new { errorText = ex.Message };
			}

			return GetStatus(serverId);
		}

		/// <summary>
		/// Получить список пользователей.
		/// </summary>
		/// <param name="serverId">Идентификатор сервера.</param>
		/// <returns>Список пользователей.</returns>
		public object GetUserList(Guid serverId)
		{
			try
			{
				return _serverService.GetServer(serverId).UserList;
			}
			catch (Exception ex)
			{
				HttpContext.Response.StatusCode = 404;
				return new { errorText = ex.Message };
			}
		}

		/// <summary>
		/// Открыть страницу консоли приложения.
		/// </summary>
		/// <param name="serverId">Идентификатор сервера.</param>
		/// <returns>Страница консоли.</returns>
		public IActionResult Console(Guid serverId)
		{
			try
			{
				ViewData["Name"] = _serverService.GetServer(serverId).Name;
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
		/// <param name="serverId">Идентификатор сервера.</param>
		/// <param name="bufferId">Версия буфера.</param>
		/// <returns>Буфер вывода приложения.</returns>
		[Route("/Server/{serverId:guid}/[action]/{version:guid}")]
		public object Console(Guid serverId, Guid version)
		{
			try
			{
				var server = _serverService.GetServer(serverId);

				return new {
					Console = server.ConsoleBuffer.GetConsoleBuffer(version),
					server.ConsoleBuffer.Version
				};
			}
			catch (Exception ex)
			{
				HttpContext.Response.StatusCode = 404;
				return new { errorText = ex.Message };
			}
		}

		/// <summary>
		/// Отправить сообщение на сервер.
		/// </summary>
		/// <param name="serverId">Идентификатор сервера.</param>
		/// <param name="message">Сообщение.</param>
		/// <returns>Информация о сервере.</returns>
		[HttpPost]
		public object Console(Guid serverId, string message = "")
		{
			try
			{
				_serverService.SendServerAppMessage(serverId, message);
			}
			catch (Exception ex)
			{
				HttpContext.Response.StatusCode = 404;
				return new { errorText = ex.Message };
			}

			return GetStatus(serverId);
		}
	}
}
