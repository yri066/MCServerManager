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

		public ListModel(GameServerService service)
        {
            _service = service;
			Servers = _service.Servers;

		}

        public void OnGet()
        {
        }
    }
}
