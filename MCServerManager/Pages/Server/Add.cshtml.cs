using MCServerManager.Library.Data.Models;
using MCServerManager.Models;
using MCServerManager.Service;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace MCServerManager.Pages.Server
{
    [Authorize]
    public class AddModel : PageModel
	{
		[BindProperty]
		public ServerDetail Input { get; set; }
		private readonly GameServerService _service;
        private readonly UserService _userService;

        public AddModel(GameServerService service, UserService userService)
		{
			_service = service;
			_userService = userService;
		}

		public void OnGet()
		{
			Input = new();
		}

		public async Task<IActionResult> OnPost()
		{
			try
			{
				if (ModelState.IsValid)
				{
                    var userId = _userService.UserId!;

                    var serverId = await _service.CreateServerAsync(Input.GetServerData(userId));
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
