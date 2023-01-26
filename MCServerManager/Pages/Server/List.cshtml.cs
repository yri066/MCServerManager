using MCServerManager.Library.Actions;
using MCServerManager.Service;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace MCServerManager.Pages.Server
{
	public class ListModel : PageModel
	{
		private readonly GameServerService _service;
		public readonly List<GameServer> Servers;
        public readonly string? UserId;

        public ListModel(GameServerService service, UserService userService)
		{
			_service = service;
			Servers = _service.Servers.ToList();
            UserId = userService.UserId;
		}

		public void OnGet()
		{
		}
	}
}
