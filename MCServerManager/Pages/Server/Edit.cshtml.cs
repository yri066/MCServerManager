using MCServerManager.Models;
using MCServerManager.Service;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace MCServerManager.Pages.Server
{
    [Authorize]
    public class EditModel : PageModel
	{
		[BindProperty]
		public ServerDetail Input { get; set; }
		public Guid Id { get; private set; }
		private readonly GameServerService _service;
        private readonly UserService _userService;

        public EditModel(GameServerService serverService, UserService userService)
		{
			_service = serverService;
            _userService = userService;
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
                var userId = _userService.UserId;

                if (userId != server.UserId) return Forbid();

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
		/// Обрабатывает Post запрос на изменение информации о сервере.
		/// </summary>
		/// <param name="serverId">Идентификатор сервера.</param>
		/// <returns>Перенаправление на страницу.</returns>
		public async Task<IActionResult> OnPostAsync(Guid serverId)
		{
			Id = serverId;
			try
			{
				if (ModelState.IsValid)
				{
                    var server = _service.GetServerData(serverId);
                    var userId = _userService.UserId;

                    if (userId != server.UserId) return Forbid();

                    await _service.UpdateServerAsync(Input.GetServerData(server));
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
