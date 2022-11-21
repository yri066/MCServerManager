using MCServerManager.Service;
using Microsoft.AspNetCore.Mvc;

namespace MCServerManager.Pages.Server
{
	/// <summary>
	/// Взаимодействие с серверным приложением.
	/// </summary>
	[Route("/Server/{id:guid}/[action]")]
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
		/// <param name="id">Идентификатор сервера.</param>
		/// <returns>Информация о сервере.</returns>
		public object GetStatus(Guid id)
		{
			try
			{
				var server = _serverService.GetServer(id);

				return new {
					Status = server.State.ToString(),
					UserListVersion = server.UserList.Version
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
		/// <param name="id">Идентификатор сервера.</param>
		/// <returns>Информация о сервере.</returns>
		public object Start(Guid id)
		{
			try
			{
				_serverService.StartServer(id);
			}
			catch (Exception ex)
			{
				HttpContext.Response.StatusCode = 404;
				return new { errorText = ex.Message };
			}

			return GetStatus(id);
		}

		/// <summary>
		/// Перезапустить сервер.
		/// </summary>
		/// <param name="id">Идентификатор сервера.</param>
		/// <returns>Информация о сервере.</returns>
		public object Restart(Guid id)
		{
			try
			{
				_serverService.Restart(id);
			}
			catch (Exception ex)
			{
				HttpContext.Response.StatusCode = 404;
				return new { errorText = ex.Message };
			}

			return GetStatus(id);
		}

		/// <summary>
		/// Остановить сервер.
		/// </summary>
		/// <param name="id">Идентификатор сервера.</param>
		/// <returns>Информация о сервере.</returns>
		public object Stop(Guid id)
		{
			try
			{
				_serverService.StopServer(id);
			}
			catch (Exception ex)
			{
				HttpContext.Response.StatusCode = 404;
				return new { errorText = ex.Message };
			}

			return GetStatus(id);
		}

		/// <summary>
		/// Выключить сервер.
		/// </summary>
		/// <param name="id">Идентификатор сервера.</param>
		/// <returns>Информация о сервере.</returns>
		public object Close(Guid id)
		{
			try
			{
				_serverService.CloseServer(id);
			}
			catch (Exception ex)
			{
				HttpContext.Response.StatusCode = 404;
				return new { errorText = ex.Message };
			}

			return GetStatus(id);
		}

		/// <summary>
		/// Отправить сообщение на сервер.
		/// </summary>
		/// <param name="id">Идентификатор сервера.</param>
		/// <param name="message">Сообщение.</param>
		/// <returns>Информация о сервере.</returns>
		public object SendMessage(Guid id, string? message)
		{
			try
			{
				_serverService.SendServerCommand(id, message);
			}
			catch (Exception ex)
			{
				HttpContext.Response.StatusCode = 404;
				return new { errorText = ex.Message };
			}

			return GetStatus(id);
		}

		/// <summary>
		/// Получить список пользователей.
		/// </summary>
		/// <param name="id">Идентификатор сервера.</param>
		/// <returns>Список пользователей.</returns>
		public object GetUserList(Guid id)
		{
			try
			{
				return _serverService.GetServer(id).UserList;
			}
			catch (Exception ex)
			{
				HttpContext.Response.StatusCode = 404;
				return new { errorText = ex.Message };
			}
		}

		public IActionResult Console(Guid id)
		{
			return View("/Pages/Application/Console.cshtml", "Server");
		}
	}
}
