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
                    var userId = _userService.UserId!;

                    var serviceId = await _service.CreateServiceAsync(Input.GetBackgroundServiceData(serverId, userId));
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
