using MCServerManager.Service;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System;

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

		/// <summary>
		/// Открыть страницу консоли приложения.
		/// </summary>
		/// <param name="id">Идентификатор сервера.</param>
		/// <returns>Страница консоли.</returns>
		public IActionResult Console(Guid id)
		{
			try
			{
				ViewData["Name"] = _serverService.GetServer(id).Name;
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
		/// <param name="id">Идентификатор сервера.</param>
		/// <param name="bufferId">Версия буфера.</param>
		/// <returns>Буфер вывода приложения.</returns>
		[Route("/Server/{id:guid}/[action]/{version:guid}")]
		public object Console(Guid id, Guid version)
		{
			try
			{
				var server = _serverService.GetServer(id);

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
		/// <param name="id">Идентификатор сервера.</param>
		/// <param name="message">Сообщение.</param>
		/// <returns>Информация о сервере.</returns>
		[HttpPost]
		public object Console(Guid id, string message = "")
		{
			try
			{
				_serverService.SendServerAppMessage(id, message);
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
