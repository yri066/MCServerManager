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

		public IActionResult OnGet(Guid serviceId)
		{
			Id = serviceId;
			try
			{
				var service = _service.GetServiceData(serviceId);
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
		/// Обрабатывает Post запрос на изменение информации о сервисе.
		/// </summary>
		/// <param name="serviceId">Идентификатор сервиса.</param>
		/// <returns>Перенаправление на страницу.</returns>
		public async Task<IActionResult> OnPostAsync(Guid serviceId)
		{
			Id = serviceId;
			try
			{
				if (ModelState.IsValid)
				{
					var service = _service.GetServiceData(serviceId);
                    var userId = _userService.UserId;

                    if (userId != service.UserId) return Forbid();

					service.UpdateData(this.Input.GetBackgroundServiceData(serviceId, service.ServerId));
                    await _service.UpdateServiceAsync(serviceId, service);
					return RedirectToPage("/Service/Service", new { serviceId });
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
