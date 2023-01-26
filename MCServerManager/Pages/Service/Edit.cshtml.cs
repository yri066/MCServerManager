using MCServerManager.Models;
using MCServerManager.Service;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace MCServerManager.Pages.Service
{
    [Authorize]
    public class EditServiceModel : PageModel
	{
		[BindProperty]
		public BackgroundServiceDetail Input { get; set; }
		public Guid Id { get; private set; }
		private readonly GameServerService _service;
        private readonly UserService _userService;

        public EditServiceModel(GameServerService serverService, UserService userService)
		{
			_service = serverService;
            _userService = userService;
        }

		public IActionResult OnGet(Guid id)
		{
			Id = id;
			try
			{
				var service = _service.GetServiceData(id);
                var userId = _userService.UserId;

                if (userId != service.UserId) return Forbid();

                Input = new BackgroundServiceDetail
				{
					Name = service.Name,
					AutoStart = service.AutoStart,
					AutoClose = service.AutoClose,
					WorkDirectory = service.WorkDirectory,
					StartProgram = service.StartProgram,
					Arguments = service.Arguments,
					Address = service.Address,
					Port = service.Port
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
		public async Task<IActionResult> OnPostAsync(Guid id)
		{
			Id = id;
			try
			{
				if (ModelState.IsValid)
				{
					var service = _service.GetServiceData(id);
                    var userId = _userService.UserId;

                    if (userId != service.UserId) return Forbid();

					service.UpdateData(Input.GetBackgroundServiceData(id, service.ServiceId));
                    await _service.UpdateServiceAsync(id, service);
					return RedirectToPage("/Service/Service", new { id });
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
