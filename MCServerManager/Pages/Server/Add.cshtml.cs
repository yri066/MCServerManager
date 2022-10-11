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
		private readonly ServerService _service;
		
		public AddModel(ServerService service)
		{
			_service = service;
		}
		public void OnGet()
        {
            Input = new();
        }

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
				// TODO: Log error
				// Add a model-level error by using an empty string key
				ModelState.AddModelError(
					string.Empty,
					ex.Message
					);
			}

			return Page();
		}
    }
}
