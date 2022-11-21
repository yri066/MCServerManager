using MCServerManager.Models;
using MCServerManager.Service;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace MCServerManager.Pages.Server
{
	public class EditModel : PageModel
	{
		[BindProperty]
		public GameServerDetail Input { get; set; }
		public Guid Id { get; private set; }
		private readonly GameServerService _service;

		public EditModel(GameServerService serverService)
		{
			_service = serverService;
		}

		/// <summary>
		/// Обрабатывает Get запрос.
		/// </summary>
		/// <param name="id">Идентификатор сервера.</param>
		/// <returns>Перенаправление на страницу.</returns>
		public IActionResult OnGet(Guid id)
		{
			Id = id;
			try
			{
				var server = _service.GetServerData(id);
				Input = new GameServerDetail
				{
					Name = server.Name,
					AutoStart = server.AutoStart,
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
			try
			{
				if (ModelState.IsValid)
				{
					_service.UpdateServer(id, Input.GetGameServerData(id));
					return RedirectToPage("Server", new { id });
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
