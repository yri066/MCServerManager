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


		public void OnGet(Guid id)
		{
			Id = id;
			Input = new();

		}

		public async Task<IActionResult> OnPost(Guid id)
		{
			Id = id;
			try
			{
				if (ModelState.IsValid)
				{
					var serviceId = await _service.CreateServiceAsync(id, Input.Name, Input.AutoStart, Input.WorkDirectory, Input.StartProgram,
						Input.Arguments, Input.Address, Input.Port);
					return RedirectToPage("/Service/Service", new { id = serviceId });
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
