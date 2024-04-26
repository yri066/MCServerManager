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

                Input = new BackgroundServiceDetail();
                Input.UpdateData(service);
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

                    await _service.UpdateServiceAsync(Input.GetBackgroundServiceData(service));
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
