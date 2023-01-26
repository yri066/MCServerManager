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

                    var id = await _service.CreateServer(Input.Name, Input.AutoStart, Input.WorkDirectory, Input.StartProgram,
						Input.Arguments, Input.Address, Input.Port, userId);
					return RedirectToPage("Server", new { id });
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
