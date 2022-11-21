using MCServerManager.Models;
using MCServerManager.Service;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace MCServerManager.Pages.Service
{
	public class EditServiceModel : PageModel
	{
		[BindProperty]
		public BackgroundServiceDetail Input { get; set; }
		public Guid Id { get; private set; }
		private readonly GameServerService _service;

		public EditServiceModel(GameServerService serverService)
		{
			_service = serverService;
		}
		public IActionResult OnGet(Guid id)
		{
			Id = id;
			try
			{
				var server = _service.GetServiceData(id);
				Input = new BackgroundServiceDetail
				{
					Name = server.Name,
					AutoStart = server.AutoStart,
					AutoClose = server.AutoClose,
					WorkDirectory = server.WorkDirectory,
					Programm = server.Programm,
					Arguments = server.Arguments,
					Address = server.Address,
					Port = server.Port
				};
			}
			catch
			{
				return NotFound();
			}

			return Page();
		}

		/// <summary>
		/// Обрабатывает Post запрос на изменение информации о сервера.
		/// </summary>
		/// <param name="id">Идентификатор сервера.</param>
		/// <returns>Перенаправление на страницу.</returns>
		public IActionResult OnPost(Guid id)
		{
			Id = id;
			try
			{
				if (ModelState.IsValid)
				{
					var serverId = _service.GetServiceData(id).GameServerId;
					_service.UpdateService(id, Input.GetBackgroundServiceData(id, serverId));
					return RedirectToPage("/Service/Service", new { id = id });
				}
			}
			catch (Exception ex)
			{
				ModelState.AddModelError(
					string.Empty,
					ex.Message
				);
			}

			return Page();
		}
	}
}
