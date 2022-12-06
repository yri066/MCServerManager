using MCServerManager.Service;
using Microsoft.AspNetCore.Mvc;

namespace MCServerManager.Pages.Service
{
	[Route("/Service/{id:guid}/[action]")]
	public class ServiceController : Controller
	{
		private readonly GameServerService _serverService;

		public ServiceController(GameServerService serverService)
		{
			_serverService = serverService;
		}

		/// <summary>
		/// Получить информацию о сервисе.
		/// </summary>
		/// <param name="id">Идентификатор сервиса.</param>
		/// <returns>Информация о сервисе.</returns>
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
				ViewData["Name"] = _serverService.GetService(id).Name;
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
		public object Console(Guid id, string? message)
		{
			try
			{
				_serverService.SendServiceCommand(id, message);
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
