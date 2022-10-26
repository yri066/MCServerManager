using MCServerManager.Models;
using MCServerManager.Service;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace MCServerManager.Pages.Application
{
	public class AddServiceModel : PageModel
	{
		[BindProperty]
		public BackgroundServiceDetail Input { get; set; }
		private readonly GameServerService _service;

		public AddServiceModel(GameServerService service)
		{
			_service = service;
		}


		public void OnGet(Guid id)
		{
			Input = new();

		}

		public IActionResult OnPost(Guid id)
		{
			try
			{
				if (ModelState.IsValid)
				{
					var serviceId = _service.CreateService(id, Input.Name, Input.AutoStart, Input.WorkDirectory, Input.Programm,
						Input.Arguments, Input.Addres, Input.Port);
					return RedirectToPage("/Application/Index", new { serviceId = serviceId });
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
