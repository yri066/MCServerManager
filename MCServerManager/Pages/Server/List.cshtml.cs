using MCServerManager.Library.Actions;
using MCServerManager.Service;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace MCServerManager.Pages.Server
{
    public class ListModel : PageModel
    {
        private readonly ServerService _service;
        public readonly List<GameServer> Servers;

        public ListModel(ServerService service)
        {
            _service = service;
			Servers = _service.Servers;

		}

        public void OnGet()
        {
        }
    }
}
