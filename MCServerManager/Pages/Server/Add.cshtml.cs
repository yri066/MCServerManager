using MCServerManager.Models;
using MCServerManager.Service;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace MCServerManager.Pages.Server
{
	public class AddModel : PageModel
	{
		[BindProperty]
		public GameServerDetail Input { get; set; }
		private readonly GameServerService _service;

		public AddModel(GameServerService service)
		{
			_service = service;
		}

		/// <summary>
		/// Обрабатывает Get запрос.
		/// </summary>
		public void OnGet()
		{
			Input = new();
		}

		/// <summary>
		/// Обрабатывает Post запрос на добавление нового сервера.
		/// </summary>
		/// <returns>Перенаправление на страницу.</returns>
		public IActionResult OnPost()
		{
			try
			{
				if (ModelState.IsValid)
				{
					var id = _service.CreateServer(Input.Name, Input.AutoStart, Input.WorkDirectory, Input.Programm,
						Input.Arguments, Input.Addres, Input.Port);
					return RedirectToPage("Index", new { id });
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
