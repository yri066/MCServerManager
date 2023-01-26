using MCServerManager.Models;
using MCServerManager.Service;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace MCServerManager.Pages.Service
{
    [Authorize]
    public class AddServiceModel : PageModel
	{
		[BindProperty]
		public BackgroundServiceDetail Input { get; set; }
		public Guid Id { get; private set; }
		private readonly GameServerService _service;
        private readonly UserService _userService;

        public AddServiceModel(GameServerService service, UserService userService)
		{
			_service = service;
            _userService = userService;
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
                    var userId = _userService.UserId!;

                    var serviceId = await _service.CreateServiceAsync(id, Input.Name, Input.AutoStart, Input.WorkDirectory, Input.StartProgram,
						Input.Arguments, Input.Address, Input.Port, userId);
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
