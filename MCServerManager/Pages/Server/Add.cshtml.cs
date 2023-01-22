using MCServerManager.Models;
using MCServerManager.Service;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace MCServerManager.Pages.Server
{
	public class AddModel : PageModel
	{
		[BindProperty]
		public ServerDetail Input { get; set; }
		private readonly GameServerService _service;

		public AddModel(GameServerService service)
		{
			_service = service;
		}

		/// <summary>
		/// ������������ Get ������.
		/// </summary>
		public void OnGet()
		{
			Input = new();
		}

		/// <summary>
		/// ������������ Post ������ �� ���������� ������ �������.
		/// </summary>
		/// <returns>��������������� �� ��������.</returns>
		public async Task<IActionResult> OnPost()
		{
			try
			{
				if (ModelState.IsValid)
				{
					var id = await _service.CreateServer(Input.Name, Input.AutoStart, Input.WorkDirectory, Input.StartProgram,
						Input.Arguments, Input.Address, Input.Port);
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
