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
		public ServerDetail Input { get; set; }
		public Guid Id { get; private set; }
		private readonly GameServerService _service;

		public EditModel(GameServerService serverService)
		{
			_service = serverService;
		}

        /// <summary>
        /// Обрабатывает Get запрос.
        /// </summary>
        /// <param name="serverId">Идентификатор сервера.</param>
        /// <returns>Перенаправление на страницу.</returns>
        public IActionResult OnGet(Guid serverId)
		{
			Id = serverId;
			try
			{
				var server = _service.GetServerData(serverId);

                Input = new ServerDetail();
                Input.UpdateData(server);
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
		public async Task<IActionResult> OnPostAsync(Guid serverId)
		{
			Id = serverId;
			try
			{
				if (ModelState.IsValid)
				{
                    await _service.UpdateServerAsync(Input.GetServerData(serverId));
					return RedirectToPage("Server", new { serverId });
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
