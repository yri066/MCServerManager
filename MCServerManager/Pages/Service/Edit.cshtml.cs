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

		public IActionResult OnGet(Guid serviceId)
		{
			Id = serviceId;
			try
			{
				var service = _service.GetServiceData(serviceId);
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
        /// Обрабатывает Post запрос на изменение информации о сервера.
        /// </summary>
        /// <param name="serviceId">Идентификатор сервера.</param>
        /// <returns>Перенаправление на страницу.</returns>
        public async Task<IActionResult> OnPostAsync(Guid serviceId)
		{
			Id = serviceId;
			try
			{
				if (ModelState.IsValid)
				{
					var service = _service.GetServiceData(serviceId);

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
