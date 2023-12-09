using MCServerManager.Models;
using MCServerManager.Service;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace MCServerManager.Pages.Service
{
	public class AddServiceModel : PageModel
	{
		[BindProperty]
		public BackgroundServiceDetail Input { get; set; }
		public Guid Id { get; private set; }
		private readonly GameServerService _service;

		public AddServiceModel(GameServerService service)
		{
			_service = service;
		}


		public void OnGet(Guid serverId)
		{
			Id = serverId;
			Input = new();

		}

		public async Task<IActionResult> OnPost(Guid serverId)
		{
			Id = serverId;
			try
			{
				if (ModelState.IsValid)
				{
                    var serviceId = await _service.CreateServiceAsync(Input.GetBackgroundServiceData(serverId));
					return RedirectToPage("Service", new { serviceId });
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
