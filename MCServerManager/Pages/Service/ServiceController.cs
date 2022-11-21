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
				var server = _serverService.GetService(id);

				return new
				{
					Status = server.State.ToString()
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

		public IActionResult Console(Guid id)
		{
			return View("/Pages/Application/Console.cshtml", "Service");
		}
	}
}
