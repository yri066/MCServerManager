using MCServerManager.Library.Actions;
using MCServerManager.Service;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace MCServerManager.Pages.Server
{
	public class ServerModel : PageModel
	{
		/// <summary>
		/// Конфигурация.
		/// </summary>
		public readonly IConfiguration ButtonStyle;

		private readonly GameServerService _serverService;
		public GameServer Exemplar { get; private set; }

		public ServerModel(GameServerService serverService, IConfiguration configuration)
		{
			_serverService = serverService;
			ButtonStyle = configuration.GetSection("Action:GameServer");
		}

		public IActionResult OnGet(Guid id)
		{
			try
			{
				Exemplar = _serverService.GetServer(id);
			}
			catch
			{
				return NotFound();
			}

			return Page();
		}

		public IActionResult OnGetDelete(Guid id)
		{
			try
			{
				_serverService.DeleteServer(id);
			}
			catch
			{}
			
			return RedirectToPage("List");
		}
	}
}
